using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class CreditCards{
		#region Get Methods
		///<summary>Returns a list of all credit cards associated to the passed in PayPlanNum.</summary>
		public static List<CreditCard> GetForPayPlan(long payPlanNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),payPlanNum);
			}
			string command=$"SELECT * FROM creditcard WHERE PayPlanNum={POut.Long(payPlanNum)}";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Returns the max itemOrder in CreditCards for a patient. If the patient has no credit cards, -1 is returned.</summary>
		public static int GetMaxItemOrderForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<int>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$"SELECT Max(ItemOrder) FROM creditcard WHERE PatNum="+POut.Long(patNum)+" ";
			string MaxItemOrder=Db.GetScalar(command);
			if(MaxItemOrder.IsNullOrEmpty()) {
				return -1;
			}
			return PIn.Int(MaxItemOrder);
		}

		///<summary>Gets one CreditCard from the db using the PayConnectToken field.</summary>
		public static CreditCard GetOneWithPayConenctToken(string payConnectTokenNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<CreditCard>(MethodBase.GetCurrentMethod(),payConnectTokenNum);
			}
			string command=$"SELECT * FROM creditcard WHERE PayConnectToken='{POut.String(payConnectTokenNum)}'";
			return Crud.CreditCardCrud.SelectOne(command);
		}

		#endregion

		///<summary>If patNum==0 then does not filter on PatNum; otherwise filters on PatNum. Only includes credit cards whose source is Open Dental
		///proper.</summary>
		public static List<CreditCard> Refresh(long patNum){
			//No need to check MiddleTierRole; no call to db.
			List<CreditCardSource> listCreditCardSources=Enum.GetValues(typeof(CreditCardSource)).Cast<CreditCardSource>()
				.Where(x => !x.In(CreditCards.GetCreditCardSourcesForOnlinePayments().ToArray())).ToList();
			//This CCSource is used in ODProper and Payment Portal/Patient Portal. Adding so OD Proper can handle this CCSource correctly.
			listCreditCardSources.Add(CreditCardSource.EdgeExpressCNP);
			return RefreshBySource(patNum,listCreditCardSources);
		}

		///<summary>If patNum==0 then does not filter on PatNum; otherwise filters on PatNum. Includes all credit cards sources.</summary>
		public static List<CreditCard> RefreshAll(long patNum){
			//No need to check MiddleTierRole; no call to db.
			List<CreditCardSource> listCreditCardSources=Enum.GetValues(typeof(CreditCardSource)).Cast<CreditCardSource>().ToList();
			return RefreshBySource(patNum,listCreditCardSources);
		}

		///<summary>Get all credit cards by a given list of CreditCardSource(s). Optionally filter by a given patNum.</summary>
		public static List<CreditCard> RefreshBySource(long patNum,List<CreditCardSource> listCreditCardSources) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),patNum,listCreditCardSources);
			}
			if(listCreditCardSources.Count==0) {
				return new List<CreditCard>();
			}
			string command="SELECT * FROM creditcard WHERE CCSource IN ("+string.Join(",",listCreditCardSources.Select(x=>POut.Int((int)x)))+") ";
			if(patNum!=0) { //Add the PatNum criteria.
				command+="AND PatNum="+POut.Long(patNum)+" ";
			}
			command+="ORDER BY ItemOrder DESC";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Gets one CreditCard from the db.</summary>
		public static CreditCard GetOne(long creditCardNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<CreditCard>(MethodBase.GetCurrentMethod(),creditCardNum);
			}
			return Crud.CreditCardCrud.SelectOne(creditCardNum);
		}

		///<summary></summary>
		public static long Insert(CreditCard creditCard){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				creditCard.CreditCardNum=Meth.GetLong(MethodBase.GetCurrentMethod(),creditCard);
				return creditCard.CreditCardNum;
			}
			return Crud.CreditCardCrud.Insert(creditCard);
		}

		///<summary>Validate payConnectResponseWeb and create a new credit card from the PayConnectResponseWeb.</summary>
		public static long InsertFromPayConnect(PayConnectResponseWeb payConnectResponseWeb) {
			//No need to check MiddleTierRole; no call to db.
			List<CreditCardSource> listCreditCardSources=new List<CreditCardSource>();
			listCreditCardSources.Add(CreditCardSource.PayConnect);
			listCreditCardSources.Add(CreditCardSource.PayConnectPortal);
			listCreditCardSources.Add(CreditCardSource.PayConnectPortalLogin);
			if(GetTokenCount(payConnectResponseWeb.PaymentToken,listCreditCardSources)>0) { //Prevent duplicates.
				throw new Exception("PayConnect token already exists: "+payConnectResponseWeb.PaymentToken);
			}
			SecurityLogs.MakeLogEntry(EnumPermType.CreditCardEdit,payConnectResponseWeb.PatNum,"Credit Card Added");
			return Insert(payConnectResponseWeb.ToCreditCard());
		}

		///<summary>Creates a new credit card from the XWebResponse if it doesn't exist. Returns CreditCardNum of new or existing card.</summary>
		public static void InsertFromXWeb(XWebResponse xWebResponse) {
			//No need to check MiddleTierRole; no call to db.
			List<CreditCardSource> listCreditCardSources=new List<CreditCardSource>();
			listCreditCardSources.Add(CreditCardSource.XWeb);
			listCreditCardSources.Add(CreditCardSource.XWebPortalLogin);
			listCreditCardSources.Add(CreditCardSource.EdgeExpressCNP);
			listCreditCardSources.Add(CreditCardSource.EdgeExpressRCM);
			listCreditCardSources.Add(CreditCardSource.XServer);
			listCreditCardSources.Add(CreditCardSource.XServerPayConnect);
			listCreditCardSources.Add(CreditCardSource.EdgeExpressPaymentPortal);
			listCreditCardSources.Add(CreditCardSource.EdgeExpressPaymentPortalGuest);
			listCreditCardSources.Add(CreditCardSource.XWebPaymentPortal);
			listCreditCardSources.Add(CreditCardSource.XWebPaymentPortalGuest);
			//Prevent duplicates.
			if(GetTokenCount(xWebResponse.Alias,listCreditCardSources)>0) {
				GetCardsByToken(xWebResponse.Alias,listCreditCardSources);
				return;
			}
			//EdgeExpress doesn't mask a couple of the beginning numbers, so we'll go through and mask it appropriately. Doesn't hurt to do this for other card sources too.
			int idxLast4Digits=(xWebResponse.MaskedAcctNum.Length-4);
			string ccNumberMasked=(new string('X',idxLast4Digits))+xWebResponse.MaskedAcctNum.Substring(idxLast4Digits);//Replace the first all but last 4 with X's
			CreditCard creditCard=new CreditCard();
			creditCard.PatNum=xWebResponse.PatNum;
			creditCard.XChargeToken=xWebResponse.Alias;
			creditCard.CCNumberMasked=ccNumberMasked;
			creditCard.CCExpiration=xWebResponse.AccountExpirationDate;
			creditCard.CCSource=xWebResponse.CCSource;
			creditCard.ClinicNum=xWebResponse.ClinicNum;
			creditCard.ItemOrder=RefreshAll(xWebResponse.PatNum).Count;
			creditCard.Address="";
			creditCard.Zip="";
			creditCard.ChargeAmt=0;
			creditCard.DateStart=DateTime.MinValue;
			creditCard.DateStop=DateTime.MinValue;
			creditCard.Note="";
			creditCard.PayPlanNum=0;
			creditCard.PayConnectToken="";
			creditCard.PayConnectTokenExp=DateTime.MinValue;
			creditCard.PaySimpleToken="";
			creditCard.Procedures="";
			Insert(creditCard);
			SecurityLogs.MakeLogEntry(EnumPermType.CreditCardEdit,xWebResponse.PatNum,"Credit Card Added");
		}

		///<summary>Inserts entry into audit trail if any credit card field has been edited</summary>
		public static void InsertAuditTrailEntry(CreditCard creditCardNew,CreditCard creditCardOld) {
			//No need to check MiddleTierRole; no call to db.
			StringBuilder stringBuilder=new StringBuilder();
			if(creditCardNew.Address != creditCardOld.Address) {
				stringBuilder.AppendLine($"Address changed from: {creditCardOld.Address} to {creditCardNew.Address}.");
			}
			if(creditCardNew.Zip != creditCardOld.Zip) {
				stringBuilder.AppendLine($"Zip changed from: {creditCardOld.Zip} to {creditCardNew.Zip}.");
			}
			if(creditCardNew.XChargeToken != creditCardOld.XChargeToken) {
				stringBuilder.AppendLine($"XChargeToken changed from: {creditCardOld.XChargeToken} to {creditCardNew.XChargeToken}.");
			}
			if(creditCardNew.CCNumberMasked != creditCardOld.CCNumberMasked) {
				stringBuilder.AppendLine($"CCNumberMasked changed from: {creditCardOld.CCNumberMasked} to {creditCardNew.CCNumberMasked}.");
			}
			if((creditCardNew.CCExpiration.Date.Month != creditCardOld.CCExpiration.Date.Month) || (creditCardNew.CCExpiration.Date.Year != creditCardOld.CCExpiration.Date.Year)) {
				stringBuilder.AppendLine($"CCExpiration changed from: {creditCardOld.CCExpiration.ToString("MMyy")} to {creditCardNew.CCExpiration.ToString("MMyy")}.");
			}
			if(creditCardNew.ItemOrder != creditCardOld.ItemOrder) {
				stringBuilder.AppendLine($"ItemOrder changed from: {creditCardOld.ItemOrder} to {creditCardNew.ItemOrder}.");
			}
			if(creditCardNew.ChargeAmt != creditCardOld.ChargeAmt) {
				stringBuilder.AppendLine($"ChargeAmt changed from: {creditCardOld.ChargeAmt} to {creditCardNew.ChargeAmt}.");
			}
			if(creditCardNew.DateStart.Date != creditCardOld.DateStart.Date) {
				stringBuilder.AppendLine($"DateStart changed from: {creditCardOld.DateStart.Date} to {creditCardNew.DateStart.Date}.");
			}
			if(creditCardNew.DateStop.Date != creditCardOld.DateStop.Date) {
				stringBuilder.AppendLine($"DateStop changed from: {creditCardOld.DateStop.Date} to {creditCardNew.DateStop.Date}.");
			}
			if(creditCardNew.Note != creditCardOld.Note) {
				stringBuilder.AppendLine($"Note changed from: {creditCardOld.Note} to {creditCardNew.Note}.");
			}
			if(creditCardNew.PayPlanNum != creditCardOld.PayPlanNum) {
				stringBuilder.AppendLine($"PayPlanNum changed from: {creditCardOld.PayPlanNum} to {creditCardNew.PayPlanNum}.");
			}
			if(creditCardNew.PayConnectToken != creditCardOld.PayConnectToken) {
				stringBuilder.AppendLine($"PayConnectToken changed from: {creditCardOld.PayConnectToken} to {creditCardNew.PayConnectToken}.");
			}
			if(creditCardNew.PayConnectTokenExp.Date != creditCardOld.PayConnectTokenExp.Date) {
				stringBuilder.AppendLine($"PayConnectTokenExp changed from: {creditCardOld.PayConnectTokenExp.Date} to {creditCardNew.PayConnectTokenExp.Date}");
			}
			if(creditCardNew.Procedures != creditCardOld.Procedures) {
				stringBuilder.AppendLine($"Procedures changed from: {creditCardOld.Procedures} to {creditCardNew.Procedures}.");
			}
			if(creditCardNew.CCSource != creditCardOld.CCSource) {
				stringBuilder.AppendLine($"CCSource changed from: {creditCardOld.CCSource} to {creditCardNew.CCSource}.");
			}
			if(creditCardNew.ClinicNum != creditCardOld.ClinicNum) {
				stringBuilder.AppendLine($"ClinicNum changed from: {creditCardOld.ClinicNum} to {creditCardNew.ClinicNum}.");
			}
			if(creditCardNew.ExcludeProcSync != creditCardOld.ExcludeProcSync) {
				stringBuilder.AppendLine($"ExcludeProcSync changed from: {creditCardOld.ExcludeProcSync} to {creditCardNew.ExcludeProcSync}.");
			}
			if(creditCardNew.PaySimpleToken != creditCardOld.PaySimpleToken) { 
				stringBuilder.AppendLine($"PaySimpleToken changed from: {creditCardOld.PaySimpleToken} to {creditCardNew.PaySimpleToken}.");
			}
			if(creditCardNew.ChargeFrequency != creditCardOld.ChargeFrequency) {
				string oldFrequency=GetHumanReadableFrequency(creditCardOld.ChargeFrequency);
				string newFrequency=GetHumanReadableFrequency(creditCardNew.ChargeFrequency);
				if(string.IsNullOrWhiteSpace(oldFrequency)) {
					oldFrequency="None";
				}
				if(string.IsNullOrWhiteSpace(newFrequency)) {
					newFrequency="None";
				}
				stringBuilder.AppendLine($"ChargeFrequency changed from: {oldFrequency} to {newFrequency}.");
			}
			if(creditCardNew.CanChargeWhenNoBal != creditCardOld.CanChargeWhenNoBal) {
				stringBuilder.AppendLine($"CanChargeWhenNoBal changed from: {creditCardOld.CanChargeWhenNoBal} to {creditCardNew.CanChargeWhenNoBal}.");
			}
			if(creditCardNew.PaymentType != creditCardOld.PaymentType) {
				stringBuilder.AppendLine($"PaymentType changed from: {creditCardOld.PaymentType} to {creditCardNew.PaymentType}.");
			}
			if(creditCardNew.IsRecurringActive != creditCardOld.IsRecurringActive) {
				stringBuilder.AppendLine($"IsRecurringActive changed from: {creditCardOld.IsRecurringActive} to {creditCardNew.IsRecurringActive}.");
			}
			string logText=stringBuilder.ToString();
			if(!String.IsNullOrEmpty(logText)) { 
				SecurityLogs.MakeLogEntry(EnumPermType.CreditCardEdit,creditCardOld.PatNum,logText,creditCardOld.CreditCardNum,DateTime.Now);
			}
		}

		///<summary></summary>
		public static void Update(CreditCard creditCard){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),creditCard);
				return;
			}
			Crud.CreditCardCrud.Update(creditCard);
		}

		///<summary></summary>
		public static void Delete(long creditCardNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),creditCardNum);
				return;
			}
			string command= "DELETE FROM creditcard WHERE CreditCardNum = "+POut.Long(creditCardNum);
			Db.NonQ(command);
		}

		///<summary>Gets the masked CC# and exp date for all cards setup for monthly charges for the specified patient.  Only used for filling [CreditCardsOnFile] variable when emailing statements.</summary>
		public static string GetMonthlyCardsOnFile(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetString(MethodBase.GetCurrentMethod(),patNum);
			}
			string result="";
			string command="SELECT * FROM creditcard WHERE PatNum="+POut.Long(patNum)
				+" AND ("+DbHelper.Year("DateStop")+"<1880 OR DateStop>"+DbHelper.Now()+") "//Recurring card is active.
				+" AND ChargeAmt>0"
				+" AND CCSource NOT IN ("+POut.Enum(CreditCardSource.XWeb)+","+POut.Enum(CreditCardSource.XWebPortalLogin)+") ";//Not created from the Patient Portal
			List<CreditCard> listCreditCardsMonthly=Crud.CreditCardCrud.SelectMany(command);
			for(int i=0;i<listCreditCardsMonthly.Count;i++) {
				if(i>0) {
					result+=", ";
				}
				result+=listCreditCardsMonthly[i].CCNumberMasked+" exp:"+listCreditCardsMonthly[i].CCExpiration.ToString("MM/yy");
			}
			return result;
		}

		///<summary>Returns list of active credit cards.</summary>
		public static List<CreditCard> GetActiveCards(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM creditcard WHERE PatNum="+POut.Long(patNum)
				+" AND ("+DbHelper.Year("DateStop")+"<1880 OR DateStop>="+DbHelper.Curdate()+") "
				+" AND ("+DbHelper.Year("DateStart")+">1880 AND DateStart<="+DbHelper.Curdate()+") "//Recurring card is active.
				+" AND CCSource NOT IN ("+POut.Enum(CreditCardSource.XWeb)+","+POut.Enum(CreditCardSource.XWebPortalLogin)+") ";//Not created from the Patient Portal
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Updates the Procedures column on all cards that have not stopped that are not marked to exclude from sync.  Only used at HQ.</summary>
		public static void SyncDefaultProcs(List<string> listProcCodes) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listProcCodes);
				return;
			}
			string command="UPDATE creditcard SET Procedures='"+string.Join(",",listProcCodes.Select(x => POut.String(x)))+"'"
				+" WHERE ("+DbHelper.Year("DateStop")+"<1880 OR DateStop>="+DbHelper.Curdate()+") "//Stop date has not past
				+" AND ExcludeProcSync=0"
				+" AND CCSource NOT IN ("+POut.Enum(CreditCardSource.XWeb)+","+POut.Enum(CreditCardSource.XWebPortalLogin)+") ";//Not created from the Patient Portal
			Db.NonQ(command);
		}

		///<summary>Returns list of credit cards that are ready for a recurring charge.  Filters by ClinicNums in list if provided.  List of ClinicNums
		///should contain all clinics the current user is authorized to access.  Further filtering by selected clinics is done at the UI level to save
		///DB calls.</summary>
		public static List<RecurringChargeData> GetRecurringChargeList(List<long> listClinicNums,DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<RecurringChargeData>>(MethodBase.GetCurrentMethod(),listClinicNums,date);
			}
			List<RecurringChargeData> listRecurringChargeDatas=new List<RecurringChargeData>();
			//This query will return patient information and the latest recurring payment whom:
			//	-have recurring charges setup and today's date falls within the start and stop range.
			//NOTE: Query will return patients with or without payments regardless of when that payment occurred, filtering is done below.
			string command="SELECT CreditCardNum,PatNum,PatName,FamBalTotal,PayPlanDue,"+POut.Date(DateTime.MinValue)+" AS LatestPayment,DateStart,Address,"
				+"AddressPat,Zip,ZipPat,XChargeToken,CCNumberMasked,CCExpiration,ChargeAmt,PayPlanNum,ProvNum,PayPlanPatnum,ClinicNum,Procedures,BillingCycleDay,Guarantor,"
				+"PayConnectToken,PayConnectTokenExp,PaySimpleToken,CCSource,ChargeFrequency,CanChargeWhenNoBal,PaymentType,IsRecurringActive "
				+"FROM (";
			#region Payments
			//The PayOrder is used to differentiate rows attached to payment plans
			command+="(SELECT 1 AS PayOrder,cc.CreditCardNum,cc.PatNum,"+DbHelper.Concat("pat.LName","', '","pat.FName")+" PatName,"
				+"guar.LName GuarLName,guar.FName GuarFName,guar.BalTotal-guar.InsEst FamBalTotal,0 AS PayPlanDue,"
				+"cc.DateStart,cc.Address,pat.Address AddressPat,cc.Zip,pat.Zip ZipPat,cc.XChargeToken,cc.CCNumberMasked,cc.CCExpiration,cc.ChargeAmt,"
				+"cc.PayPlanNum,cc.DateStop,0 ProvNum,0 PayPlanPatNum,cc.ClinicNum ClinicNum,cc.Procedures,pat.BillingCycleDay,pat.Guarantor,cc.PayConnectToken,"
				+"cc.PayConnectTokenExp,cc.PaySimpleToken,cc.CCSource,cc.ChargeFrequency,cc.CanChargeWhenNoBal,cc.PaymentType,cc.IsRecurringActive "
				+"FROM creditcard cc "
				+"INNER JOIN patient pat ON pat.PatNum=cc.PatNum "
				+"INNER JOIN patient guar ON guar.PatNum=pat.Guarantor "
				+"WHERE cc.PayPlanNum=0 "//Keeps card from showing up in case they have a balance AND is setup for payment plan. 
				+"AND CCSource NOT IN ("+POut.Int((int)CreditCardSource.XWeb)+","+POut.Int((int)CreditCardSource.XWebPortalLogin)+") ";//Not created from the Patient Portal
			if(listClinicNums!=null && listClinicNums.Count>0) {
				command+="AND cc.ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY cc.CreditCardNum) ";
			}
			else {//Oracle
				command+="GROUP BY cc.CreditCardNum,cc.PatNum,"+DbHelper.Concat("pat.LName","', '","pat.FName")+",PatName,guar.BalTotal-guar.InsEst,"
					+"cc.Address,pat.Address,cc.Zip,pat.Zip,cc.XChargeToken,cc.CCNumberMasked,cc.CCExpiration,cc.ChargeAmt,cc.PayPlanNum,cc.DateStop,PayPlanPatNum,"
					+"ClinicNum,cc.Procedures,pat.BillingCycleDay,pat.Guarantor,cc.PayConnectToken,cc.PayConnectTokenExp,cc.PaySimpleToken) ";
			}
			#endregion
			command+="UNION ALL ";
			#region Payment Plans
			command+="(SELECT 2 AS PayOrder,cc.CreditCardNum,cc.PatNum,"+DbHelper.Concat("pat.LName","', '","pat.FName")+" PatName,"
				+"guar.LName GuarLName,guar.FName GuarFName,guar.BalTotal-guar.InsEst FamBalTotal,"
				+"ROUND(COALESCE(ppc.pastCharges,0)-COALESCE(SUM(ps.SplitAmt),0),2) PayPlanDueCalc,"//payplancharges-paysplits attached to pp is PayPlanDueCalc
				+"cc.DateStart,cc.Address,pat.Address AddressPat,cc.Zip,pat.Zip ZipPat,cc.XChargeToken,cc.CCNumberMasked,cc.CCExpiration,cc.ChargeAmt,"
				+"cc.PayPlanNum,cc.DateStop,COALESCE(ppc.maxProvNum,0) ProvNum,COALESCE(ppc.maxPatNum,0) PayPlanPatNum,cc.ClinicNum ClinicNum,cc.Procedures,"
				+"pat.BillingCycleDay,pat.Guarantor,cc.PayConnectToken,cc.PayConnectTokenExp,cc.PaySimpleToken,cc.CCSource,cc.ChargeFrequency,"
				+"cc.CanChargeWhenNoBal,cc.PaymentType,cc.IsRecurringActive "
				+"FROM creditcard cc "
				+"INNER JOIN patient pat ON pat.PatNum=cc.PatNum "
				+"INNER JOIN patient guar ON guar.PatNum=pat.Guarantor "
				+"LEFT JOIN paysplit ps ON ps.PayPlanNum=cc.PayPlanNum AND ps.PayPlanNum<>0 "
				+"LEFT JOIN ("
					+"SELECT PayPlanNum,MAX(ProvNum) maxProvNum,MAX(PatNum) maxPatNum,"
					+"SUM(CASE WHEN ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "
						+"AND ChargeDate <= "+DbHelper.Curdate()+" THEN Principal+Interest ELSE 0 END) pastCharges "
					+"FROM payplancharge "
					+"GROUP BY PayPlanNum"
				+") ppc ON ppc.PayPlanNum=cc.PayPlanNum "
				+"WHERE cc.PayPlanNum>0 "
				+"AND CCSource NOT IN ("+POut.Int((int)CreditCardSource.XWeb)+","+POut.Int((int)CreditCardSource.XWebPortalLogin)+") ";//Not created from the Patient Portal
			if(listClinicNums!=null && listClinicNums.Count>0) {
				command+="AND cc.ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY cc.CreditCardNum ";
			}
			else {//Oracle
				command+="GROUP BY cc.CreditCardNum,cc.PatNum,"+DbHelper.Concat("pat.LName","', '","pat.FName")+",PatName,guar.BalTotal-guar.InsEst,"
					+"cc.Address,pat.Address,cc.Zip,pat.Zip,cc.XChargeToken,cc.CCNumberMasked,cc.CCExpiration,cc.ChargeAmt,cc.PayPlanNum,cc.DateStop,PayPlanPatNum,"
					+"ClinicNum,cc.Procedues,pat.BillingCycleDay,pat.Guarantor,cc.PayConnectToken,cc.PayConnectTokenExp,cc.PaySimpleToken ";
			}
			command+="HAVING PayPlanDueCalc>0)";//don't show cc's attached to payplans when the payplan has nothing due
			#endregion
			//Now we have all the results for payments and payment plans, so do an obvious filter. A more thorough filter happens later.
			command+=") due "
				+"WHERE DateStart<="+DbHelper.Curdate()+" AND "+DbHelper.Year("DateStart")+">1880 "
				+"AND (DateStop>="+DbHelper.Curdate()+" OR "+DbHelper.Year("DateStop")+"<1880) "
				//We want to exclude any cards that are currently being processed by Open Dental Service or a different human.
				+"AND due.CreditCardNum NOT IN (SELECT recurringcharge.CreditCardNum FROM recurringcharge "
					+"WHERE recurringcharge.ChargeStatus="+POut.Int((int)RecurringChargeStatus.NotYetCharged)+" "
					+"AND "+DbHelper.DtimeToDate("recurringcharge.DateTimeCharge")+"="+DbHelper.DtimeToDate(DbHelper.Now())+") "
				+"ORDER BY GuarLName,GuarFName,PatName,PayOrder DESC";
			DataTable table=Db.GetTable(command);
			//Query for latest payments seperately because this takes a very long time when run as a sub select
			if(table.Rows.Count<1) {
				return listRecurringChargeDatas;
			}
			List<string> listStrCreditCardNums=table.Rows.AsEnumerable<DataRow>().Select(x=>POut.String(x["CreditCardNum"].ToString())).ToList();
			command="SELECT cc.PatNum,cc.CreditCardNum,MAX(CASE WHEN "+DbHelper.Year("p.RecurringChargeDate")+" > 1880 "
				+"THEN p.RecurringChargeDate ELSE p.PayDate END) RecurringChargeDate " +
				"FROM creditcard cc " +
				"INNER JOIN recurringcharge rc ON cc.creditcardnum=rc.creditcardnum "+
				"INNER JOIN payment p ON p.paynum=rc.paynum AND p.IsRecurringCC=1 AND p.PayAmt > 0 " +
				"WHERE cc.CreditCardNum IN ("+string.Join(",", listStrCreditCardNums)+") " +
				"GROUP BY cc.CreditCardNum";
			List<DataRow> listDataRowLatestPayments=Db.GetTable(command).Rows.AsEnumerable<DataRow>().ToList();
			for(int i=0;i<table.Rows.Count;i++) {
				string strCreditCardNum=table.Rows[i]["CreditCardNum"].ToString();
				DataRow dataRowCreditCard=listDataRowLatestPayments.Find(x => x["CreditCardNum"].ToString()==strCreditCardNum);
				if(dataRowCreditCard==null) {
					continue;
				}
				table.Rows[i]["LatestPayment"]=dataRowCreditCard["RecurringChargeDate"].ToString();
			}
			table.Columns.Add("RecurringChargeDate",typeof(DateTime));//Will get set in FilterRecurringChargeList
			FilterRecurringChargeList(table,date);
			for(int i=0;i<table.Rows.Count;i++){
				RecurringChargeData recurringChargeData=new RecurringChargeData();
				recurringChargeData.Address=table.Rows[i]["Address"].ToString();
				recurringChargeData.AddressPat=table.Rows[i]["AddressPat"].ToString();
				recurringChargeData.BillingCycleDay=PIn.Int(table.Rows[i]["BillingCycleDay"].ToString());
				recurringChargeData.CCExpiration=PIn.DateT(table.Rows[i]["CCExpiration"].ToString());
				recurringChargeData.CCNumberMasked=table.Rows[i]["CCNumberMasked"].ToString();
				recurringChargeData.CCSource=PIn.Enum<CreditCardSource>(table.Rows[i]["CCSource"].ToString());
				recurringChargeData.DateStart=PIn.DateT(table.Rows[i]["DateStart"].ToString());
				recurringChargeData.Guarantor=PIn.Long(table.Rows[i]["Guarantor"].ToString());
				recurringChargeData.LatestPayment=PIn.DateT(table.Rows[i]["LatestPayment"].ToString());
				recurringChargeData.PatName=table.Rows[i]["PatName"].ToString();
				recurringChargeData.PayConnectToken=table.Rows[i]["PayConnectToken"].ToString();
				recurringChargeData.PayConnectTokenExp=PIn.Date(table.Rows[i]["PayConnectTokenExp"].ToString());
				recurringChargeData.PayPlanNum=PIn.Long(table.Rows[i]["PayPlanNum"].ToString());
				recurringChargeData.PayPlanPatNum=PIn.Long(table.Rows[i]["PayPlanPatNum"].ToString());
				recurringChargeData.PaySimpleToken=table.Rows[i]["PaySimpleToken"].ToString();
				recurringChargeData.Procedures=table.Rows[i]["Procedures"].ToString();
				recurringChargeData.ProvNum=PIn.Long(table.Rows[i]["ProvNum"].ToString());
				RecurringCharge recurringCharge=new RecurringCharge();
				recurringCharge.ChargeAmt=PIn.Double(table.Rows[i]["ChargeAmt"].ToString());
				recurringCharge.ChargeStatus=RecurringChargeStatus.NotYetCharged;
				recurringCharge.ClinicNum=0;
				if(PrefC.HasClinicsEnabled){
					recurringCharge.ClinicNum=PIn.Long(table.Rows[i]["ClinicNum"].ToString());
				}
				recurringCharge.CreditCardNum=PIn.Long(table.Rows[i]["CreditCardNum"].ToString());
				recurringCharge.FamBal=PIn.Double(table.Rows[i]["FamBalTotal"].ToString());
				recurringCharge.PatNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				recurringCharge.PayPlanDue=PIn.Double(table.Rows[i]["PayPlanDue"].ToString());
				recurringCharge.RepeatAmt=PIn.Double(table.Rows[i]["ChargeAmt"].ToString());
				recurringCharge.UserNum=Security.CurUser.UserNum;
				recurringChargeData.RecurringCharge=recurringCharge;
				recurringChargeData.RecurringChargeDate=PIn.DateT(table.Rows[i]["RecurringChargeDate"].ToString());
				recurringChargeData.XChargeToken=table.Rows[i]["XChargeToken"].ToString();
				recurringChargeData.Zip=table.Rows[i]["Zip"].ToString();
				recurringChargeData.ZipPat=table.Rows[i]["ZipPat"].ToString();
				recurringChargeData.CanChargeWhenNoBal=PIn.Bool(table.Rows[i]["CanChargeWhenNoBal"].ToString());
				recurringChargeData.PaymentType=PIn.Long(table.Rows[i]["PaymentType"].ToString());
				recurringChargeData.CCIsRecurringActive=PIn.Bool(table.Rows[i]["IsRecurringActive"].ToString());
				//negative family balance does not subtract from payplan amount due and negative payplan amount due does not subtract from family balance due
				double totalBal=Math.Max(recurringChargeData.RecurringCharge.FamBal,0);
				if(CompareDouble.IsZero(recurringChargeData.RecurringCharge.PayPlanDue)) {
					recurringChargeData.RecurringCharge.TotalDue=totalBal;
					listRecurringChargeDatas.Add(recurringChargeData);
					continue;
				}
				if(PrefC.GetInt(PrefName.PayPlansVersion)==1) {//in PP v2, the PP amt due is included in the pat balance
					totalBal+=Math.Max(recurringChargeData.RecurringCharge.PayPlanDue,0);
				}
				else if(PrefC.GetInt(PrefName.PayPlansVersion)==2) {
					totalBal=Math.Max(totalBal,recurringChargeData.RecurringCharge.PayPlanDue);//At minimum, the Total Due should be the Pay Plan Due amount.
				}
				recurringChargeData.RecurringCharge.TotalDue=totalBal;
				listRecurringChargeDatas.Add(recurringChargeData);
			}
			return listRecurringChargeDatas;
		}
		
		/// <summary>Adds up the total fees for the procedures passed in that have been completed since the last billing day.</summary>
		public static double TotalRecurringCharges(long patNum,string procedures,int billingDay) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod(),patNum,procedures,billingDay);
			}
			//Find the beginning of the current billing cycle, use that date to total charges between now and then for this cycle only.
			//Include that date only when we are not on the first day of the current billing cycle.
			DateTime dateStartBillingCycle;
			if(DateTime.Today.Day>billingDay) {//if today is 7/13/2015 and billingDay is 26, startBillingCycle will be 6/26/2015
				dateStartBillingCycle=new DateTime(DateTime.Today.Year,DateTime.Today.Month,billingDay);
			}
			else {
				//DateTime.Today.AddMonths handles the number of days in the month and leap years
				//Examples: if today was 12/31/2015, AddMonths(-1) would yield 11/30/2015; if today was 3/31/2016, AddMonths(-1) would yield 2/29/2016
				dateStartBillingCycle=DateTime.Today.AddMonths(-1);
				if(billingDay<=DateTime.DaysInMonth(dateStartBillingCycle.Year,dateStartBillingCycle.Month)) {
					//This corrects the issue of a billing cycle day after today but this month doesn't have enough days when last month does
					//Example: if today was 11/30/2015 and the pat's billing cycle day was the 31st, startBillingCycle=Today.AddMonths(-1) would be 10/30/2015.
					//But this pat's billing cycle day is the 31st and the December has 31 days.  This adjusts the start of the billing cycle to 10/31/2015.
					//Example 2: if today was 2/29/2016 (leap year) and the pat's billing cycle day was the 30th, startBillingCycle should be 1/30/2016.
					//Today.AddMonths(-1) would be 1/29/2016, so this adjusts startBillingCycle to 1/30/2016.
					dateStartBillingCycle=new DateTime(dateStartBillingCycle.Year,dateStartBillingCycle.Month,billingDay);
				}
			}
			string procStr="'"+POut.String(procedures).Replace(",","','")+"'";
			string command="SELECT SUM(pl.ProcFee) + COALESCE(SUM(adj.AdjAmt),0) "
				+"FROM procedurelog pl "
				+"INNER JOIN procedurecode pc ON pl.CodeNum=pc.CodeNum "
				+"LEFT JOIN ( "
					+"SELECT SUM(adjustment.AdjAmt) AdjAmt,adjustment.ProcNum "
					+"FROM adjustment "
					+"WHERE adjustment.PatNum="+POut.Long(patNum)+" "
					+"AND adjustment.ProcNum!=0 "
					+"GROUP BY adjustment.ProcNum "
				+") adj ON adj.ProcNum=pl.ProcNum "
				+"WHERE pl.ProcStatus=2 "
				+"AND pc.ProcCode IN ("+procStr+") "
				+"AND pl.PatNum="+POut.Long(patNum)+" "
				+"AND pl.ProcDate<="+DbHelper.Curdate()+" ";
			//If today is the billingDay or today is the last day of the current month and the billingDay is greater than today
			//i.e. billingDay=31 and today is the 30th which is the last day of the current month, only count procs with date after the 31st of last month
			if(billingDay==DateTime.Today.Day
				|| (billingDay>DateTime.Today.Day
				&& DateTime.Today.Day==DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)))
			{
				command+="AND pl.ProcDate>"+POut.Date(dateStartBillingCycle);
			}
			else {
				command+="AND pl.ProcDate>="+POut.Date(dateStartBillingCycle);
			}
			return PIn.Double(Db.GetScalar(command));
		}

		/// <summary>Returns true if the procedure passed in is linked to any other active card on the patient's account.</summary>
		public static bool ProcLinkedToCard(long patNum,string procCode,long cardNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum,procCode,cardNum);
			}
			string command="SELECT CreditCardNum,Procedures "
				+"FROM creditcard "
				+"WHERE PatNum="+POut.Long(patNum)+" "
				+"AND DateStart<="+DbHelper.Curdate()+" AND "+DbHelper.Year("DateStart")+">1880 "
				+"AND (DateStop>="+DbHelper.Curdate()+" OR "+DbHelper.Year("DateStop")+"<1880) "
				+"AND CreditCardNum!="+POut.Long(cardNum);
			DataTable table=Db.GetTable(command);
			return table.Rows.OfType<DataRow>().SelectMany(x => x["Procedures"].ToString().Split(',')).Any(x => x==procCode);
		}

		///<summary>Filters out cards that do not to be charged for their recurring payment.
		///Table must include columns labeled LatestPayment, DateStart, ChargeAmt, RecurringChargeDate, and ChargeFrequency.</summary>
		public static void FilterRecurringChargeList(DataTable table,DateTime date) {
			//No need to check MiddleTierRole; no call to db.
			//Loop through table and remove patients that do not need to be charged yet.
			//Modify the charge amount if necessary to account for the number of missed charges in the last month.
			for(int i=0;i<table.Rows.Count;i++) {
				DateTime dateStart=PIn.DateT(table.Rows[i]["DateStart"].ToString());
				DateTime dateLatestPayment=PIn.Date(table.Rows[i]["LatestPayment"].ToString());
				string chargeFrequency=table.Rows[i]["ChargeFrequency"].ToString();
				DateTime dateRecurringCharge=new DateTime();
				int chargeCount=0;
				if(GetFrequencyType(chargeFrequency)==ChargeFrequencyType.FixedDayOfMonth) {
					chargeCount=GetCountChargesFixedDay(date,dateLatestPayment,dateStart,chargeFrequency,out dateRecurringCharge);
				}
				else if(GetFrequencyType(chargeFrequency)==ChargeFrequencyType.FixedWeekDay) {
					chargeCount=GetCountChargesFixedWeekday(date,dateLatestPayment,dateStart,chargeFrequency,out dateRecurringCharge);
				}
				table.Rows[i]["RecurringChargeDate"]=dateRecurringCharge;
				if(chargeCount > 0) {
					//Charge them for the number of charges they haven't paid in the last month. This mimics old logic that would charge them once
					//regardless of how far back their last payment was. In this case, if they are charged twice a month, charge them two times the
					//amount they pay for each charge.
					table.Rows[i]["ChargeAmt"]=(PIn.Double(table.Rows[i]["ChargeAmt"].ToString())*chargeCount).ToString();
					continue;
				}
				//Patient doesn't need to be charged yet. Remove from table.
				table.Rows.RemoveAt(i);
				i--;
			}
		}

		///<summary>Returns true if the patient should be charged for the date being checked. This logic should be used when dealing with a 
		///payment that should be charged only one specific day per month (FixedDayOfMonth or nth weekday of every month).</summary>
		public static DateTime GetDateCharge(DateTime dateThisMonthDayToBeCharged,DateTime dateLastMonthDayToBeCharged,DateTime date,DateTime dateLatestPayment,DateTime dateStart) 
		{
			//No need to check MiddleTierRole; no call to db.
			if(dateThisMonthDayToBeCharged.Date<=date.Date) {//If it is past their time to be charged for this month
				if(dateStart.Date<=dateThisMonthDayToBeCharged.Date) {
					//if their last payment was before their day to be charged
					if(dateLatestPayment.Date < dateThisMonthDayToBeCharged.Date) {
						return dateThisMonthDayToBeCharged;
					}
				}
				return new DateTime();
			}
			//It's not past their date for this month
			//This next check is to make sure last month they should have been charged based on the dateStart.
			//An example where this will be false is when a recurring charge is created on February 2nd and set to every 16th of every month.
			//When they run the tool on February 3rd, the card should not be charged as January 16th is before the plan was created.
			if(dateStart.Date<=dateLastMonthDayToBeCharged.Date) {
				//Check to see if the payment happened for last month's date
				if(dateLatestPayment.Date < dateLastMonthDayToBeCharged.Date) {//if not charge them
					return dateLastMonthDayToBeCharged;
				}
			}
			return new DateTime();
		}

		///<summary>Gets the number of charges that should be charged if the recurring charge is for fixed day(s) of the month.</summary>
		private static int GetCountChargesFixedDay(DateTime date,DateTime dateLatestPayment,DateTime dateStart,string chargeFrequency,
			out DateTime dateRecurringCharge) 
		{
			//No need to check MiddleTierRole; no call to db.
			dateRecurringCharge=new DateTime();
			//Get days to be charged on.
			int chargeCount=0;
			List<int> listDays=GetDaysOfMonthForChargeFrequency(chargeFrequency).Split(',').Select(x => PIn.Int(x)).ToList();
			int daysInThisMonth=DateTime.DaysInMonth(date.Year,date.Month);
			//Only time not accurate is curDate.Month=1. Dec has 31 days regardless of year, however.
			int daysInLastMonth=DateTime.DaysInMonth(date.AddMonths(-1).Year,date.AddMonths(-1).Month);
			for(int i=0;i<listDays.Count;i++){
				//this will get the day it should be charged. e.g. the card gets charged on the 31st of every month. If the month is february,
				//Day to be charged is Feb 28th.
				DateTime dateThisMonthDayToBeCharged=new DateTime(date.Year,date.Month,Math.Min(daysInThisMonth,listDays[i]));
				//Calculates the last months charge date. In the example above on the 31st of every month, while february's charge date
				//is the 28th, January's was the 31st. 
				DateTime dateLastMonthDayToBeCharged=new DateTime(date.AddMonths(-1).Year,date.AddMonths(-1).Month,
					Math.Min(daysInLastMonth,listDays[i]));
				DateTime dateThisCharge=GetDateCharge(dateThisMonthDayToBeCharged,dateLastMonthDayToBeCharged,date,dateLatestPayment,dateStart);
				if(dateThisCharge.Year<1880){
					continue;
				}
				chargeCount++;
				dateRecurringCharge=ODMathLib.Max(dateRecurringCharge,dateThisCharge);
			}
			return chargeCount;
		}

		///<summary>Gets the number of charges that should be charged if the recurring charge is for fixed weekday(s) of the month.</summary>
		private static int GetCountChargesFixedWeekday(DateTime date,DateTime dateLatestPayment,DateTime dateStart,string chargeFrequency,
			out DateTime dateRecurringCharge)
		{
			//No need to check MiddleTierRole; no call to db.
			int chargeCount=0;
			DayOfWeekFrequency dayOfWeekFrequency=GetDayOfWeekFrequency(chargeFrequency);
			DayOfWeek dayOfWeek=GetDayOfWeek(chargeFrequency);
			if(dayOfWeekFrequency==DayOfWeekFrequency.Every) {
				//Due to the limitations of pulling one latest payment and the expectation that people will run this frequently, in the case
				//of many missed charges, we only charge up to a month worth of charges. E.g. If someone is charged every friday, 
				//only the previous four fridays will be checked and charged if their latest payment is far back.
				//Get dateTime for the most recent day that is the day of the week that is <= today
				DateTime recentDateToCharge=MiscUtils.GetMostRecentDayOfWeek(date,dayOfWeek);
				dateRecurringCharge=recentDateToCharge;
				for(int j=0;j<4;j++) {
					//Make sure the day being looked at was after the start date. This prevents cards from being charged for weeks that 
					//were before the recurring charge was created
					if(recentDateToCharge.AddDays(-j*7)<dateStart) {
						continue;
					}
					if(dateLatestPayment.Date < recentDateToCharge.Date.AddDays(-j*7)) {
						chargeCount++;
						//keep checking to see when the latestpayment was. For practices running their report frequently, this will
						//almost never make it past the first loop.
					}
				}
				return chargeCount;
			}
			if(dayOfWeekFrequency==DayOfWeekFrequency.EveryOther) {
				//Similar to DayOfWeekFrequency.Every, in the case of many missed payments, the last 2 instances of the weekday will be 
				//checked. The every other cycle will be reset if it is not clear what week rotation is being charge on based 
				//on the latestpayment
				//Get the most recent date for the day of the week in the every other cycle
				DateTime dateRecentToCharge=dateLatestPayment;
				if(dateLatestPayment.Year > 1880) {//if a payment has occurred
					//if it was last paid on a day that is not the designated day of the week, find the closest.
					if(dateLatestPayment.DayOfWeek!=dayOfWeek) {
						dateRecentToCharge=MiscUtils.GetMostRecentDayOfWeek(dateLatestPayment,dayOfWeek);
					}
					//Continue the every other cycle until finding the most recent day of the week for this two week cycle
					while(dateRecentToCharge.AddDays(14)<=date) {
						dateRecentToCharge=dateRecentToCharge.AddDays(14);
					}
				}
				else {//if no payment, find the most recent day of the week that occurred in the past to begin the every other cycle.
					dateRecentToCharge=MiscUtils.GetMostRecentDayOfWeek(date,dayOfWeek);
				}
				dateRecurringCharge=dateRecentToCharge;
				for(int j=0;j<2;j++) {
					//Make sure the day being looked at was after the start date. This prevents cards from being charged for weeks that 
					//were before the recurring charge was created
					if(dateRecentToCharge.AddDays(-j*14)>=dateStart) {
						if(dateRecentToCharge.AddDays(-j*14) > dateLatestPayment) {
							chargeCount++;
						}
					}
				}
				return chargeCount;
			}
			//handles all nth day of the month.
			//Get day to charge on this month and for last month.
			//(int)frequency-1 -- see DayOfWeekFrequencyEnum
			DateTime thisMonthDayToBeCharged=GetNthWeekdayofMonth(date,(int)dayOfWeekFrequency-1,dayOfWeek);
			DateTime lastMonthDayToBeCharged=GetNthWeekdayofMonth(date.AddMonths(-1),(int)dayOfWeekFrequency-1,dayOfWeek);
			dateRecurringCharge=GetDateCharge(thisMonthDayToBeCharged,lastMonthDayToBeCharged,date,dateLatestPayment,dateStart);
			if(dateRecurringCharge.Year>1880){
				chargeCount++;
			}
			return chargeCount;
		}

		///<summary>Gets the nth day of the week of a month. If the nth day does not exist, it will return the last day of the month passed in.
		///date should be the year and month that the nth day will be found in.</summary>
		public static DateTime GetNthWeekdayofMonth(DateTime date,int nthWeek,DayOfWeek dayOfWeek) {
			//No need to check MiddleTierRole; no call to db.
			DateTime dateNth=DateTools.ToBeginningOfMonth(date);
			dateNth=MiscUtils.GetUpcomingDayOfWeek(dateNth,dayOfWeek);
			dateNth=dateNth.AddDays((nthWeek-1)*7);
			if(dateNth.Month!=date.Month) {
				dateNth=DateTools.ToEndOfMonth(date);
			}
			return dateNth;
		}

		///<summary>Returns number of times token is in use. Token was duplicated once and caused the wrong card to be charged.</summary>
		public static int GetXChargeTokenCount(string token,bool includeXWeb) {
			if(string.IsNullOrEmpty(token)) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),token,includeXWeb);
			}
			string command=$"SELECT * FROM creditcard WHERE XChargeToken='{POut.String(token)}' ";
			if(!includeXWeb) {
				command+=$"AND CCSource NOT IN({POut.Int((int)CreditCardSource.XWeb)},{POut.Int((int)CreditCardSource.XWebPortalLogin)},{POut.Int((int)CreditCardSource.XWebPaymentPortal)},{POut.Int((int)CreditCardSource.XWebPaymentPortalGuest)}) ";
			}
			return CountSameCard(Crud.CreditCardCrud.SelectMany(command));
		}

		///<summary>Returns number of times token is in use.</summary>
		public static int GetPayConnectTokenCount(string token) {
			if(string.IsNullOrEmpty(token)) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),token);
			}
			string command=$"SELECT * FROM creditcard WHERE PayConnectToken='{POut.String(token)}' ";
			return CountSameCard(Crud.CreditCardCrud.SelectMany(command));
		}

		///<summary>Returns number of times token is in use.</summary>
		public static int GetPaySimpleTokenCount(string token,bool isAch) {
			if(string.IsNullOrEmpty(token)) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),token,isAch);
			}
			string command=$"SELECT * FROM creditcard WHERE PaySimpleToken='{POut.String(token)}' ";
			if(!isAch) {
				command+=$"AND CCSource!={POut.Int((int)CreditCardSource.PaySimpleACH)} ";
				command+=$"AND CCSource!={POut.Int((int)CreditCardSource.PaySimplePaymentPortalACH)} ";
			}
			return CountSameCard(Crud.CreditCardCrud.SelectMany(command));
		}

		///<summary>Returns number of times token is in use.  Token was duplicated once and caused the wrong card to be charged.</summary>
		public static int GetTokenCount(string token,List<CreditCardSource> listCreditCardSources) {
			//No need to check MiddleTierRole; no call to db.
			return CountSameCard(GetCardsByToken(token,listCreditCardSources));
		}

		///<summary>Counts how many cards are probably the same.</summary>
		private static int CountSameCard(List<CreditCard> listCreditCards) {
			//No need to check MiddleTierRole; no call to db.
			//There may be duplicate tokens if the office adds the same CC multiple times (on the same patient or on multiple patients).
			//This is considered valid and we only want to catch duplicates for different CC numbers
			//CCNumberMasked only stores 4 digits, so adding the extra data to the select "grouping" allows a more reliable count.
			int countSameCard=listCreditCards
				.Select(x => x.CCNumberMasked.Substring(x.CCNumberMasked.Length-4)+x.CCExpiration.ToString("MMYYYYY")+x.CCSource.ToString())
				.Distinct()
				.Count();
			return countSameCard;
		}

		public static List<CreditCard> GetCardsByToken(string token,List<CreditCardSource> listCreditCardSources) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),token,listCreditCardSources);
			}
			if(listCreditCardSources.Count==0) {
				return new List<CreditCard>();
			}
			string command="SELECT * FROM creditcard WHERE CCSource IN ("+string.Join(",",listCreditCardSources.Select(x=>POut.Int((int)x)))+") AND (0 ";
			if(listCreditCardSources.Any(x=>x.In(CreditCardSource.PayConnect,
				CreditCardSource.XServerPayConnect,
				CreditCardSource.PayConnectPaymentPortal,
				CreditCardSource.PayConnectPaymentPortalGuest)))
			{
				command+="OR PayConnectToken='"+POut.String(token)+"' ";
			}
			if(listCreditCardSources.Any(x=>x.In(CreditCardSource.XServer,
				CreditCardSource.XWeb,
				CreditCardSource.XWebPaymentPortal,
				CreditCardSource.XWebPaymentPortalGuest,
				CreditCardSource.XServerPayConnect,
				CreditCardSource.XWebPortalLogin,
				CreditCardSource.EdgeExpressCNP,
				CreditCardSource.EdgeExpressRCM,
				CreditCardSource.EdgeExpressPaymentPortal,
				CreditCardSource.EdgeExpressPaymentPortalGuest)))
			{
				command+="OR XChargeToken='"+POut.String(token)+"' ";
			}
			if(listCreditCardSources.Any(x=>x.In(CreditCardSource.PaySimple,
				CreditCardSource.PaySimpleACH,
				CreditCardSource.PaySimplePaymentPortal,
				CreditCardSource.PaySimplePaymentPortalACH,
				CreditCardSource.PaySimplePaymentPortalGuest)))
			{
				command+="OR PaySimpleToken='"+POut.String(token)+"' ";
			}
			command+=")";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		public static List<CreditCard> GetCardsWithTokenBySource(List<CreditCardSource> listCreditCardSources) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),listCreditCardSources);
			}
			if(listCreditCardSources.Count==0) {
				return new List<CreditCard>();
			}
			string command="SELECT * FROM creditcard WHERE CCSource IN ("+string.Join(",",listCreditCardSources.Select(x=>POut.Int((int)x)))+") AND (0 ";
			if(listCreditCardSources.Contains(CreditCardSource.PayConnect) || listCreditCardSources.Contains(CreditCardSource.XServerPayConnect)) {
				command+="OR PayConnectToken!='' ";
			}
			if(listCreditCardSources.Exists(x=>x.In(CreditCardSource.XServer, CreditCardSource.XWeb, CreditCardSource.XServerPayConnect, CreditCardSource.XWebPortalLogin,CreditCardSource.XWebPaymentPortal,CreditCardSource.XWebPaymentPortalGuest)))
			{
				command+="OR XChargeToken!='' ";
			}
			if(listCreditCardSources.Contains(CreditCardSource.PaySimple)) {
				command+="OR PaySimpleToken!='' ";
			}
			command+=")";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		public static List<CreditCardSource> GetCreditCardSourcesForOnlinePayments() {
			//No need to check MiddleTierRole; no call to db.
			List<CreditCardSource> listCreditCardSources=typeof(CreditCardSource).GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(x => x.IsDefined(typeof(OnlinePaymentMethod),false)).Select(x => (CreditCardSource)x.GetValue(null)).ToList();
			return listCreditCardSources;
		}

		///<summary>Gets every credit card in the db with an X-Charge token that was created from the specified source.</summary>
		public static List<CreditCard> GetCardsWithXChargeTokens(CreditCardSource creditCardSource=CreditCardSource.XServer) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),creditCardSource);
			}
			string command="SELECT * FROM creditcard WHERE XChargeToken!=\"\" "
				+"AND CCSource="+POut.Int((int)creditCardSource);
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Gets every credit card in the db with a PayConnect token.</summary>
		public static List<CreditCard> GetCardsWithPayConnectTokens() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod());
			}
			string command = "SELECT * FROM creditcard WHERE PayConnectToken!=\"\"";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Gets a token that can be used by XWeb. A token that is created by the XCharge client program can be used for XWeb after stripping
		///off the beginning 3 characters which are always "XAW".</summary>
		public static string GetXWebToken(CreditCard creditCard) {
			//No need to check MiddleTierRole; no call to db.
			//CM 07/05/2018 - I don't have any documentation about removing "XAW" from the token in order to use it for XWeb, but I think I got that
			//information from an email from someone at OpenEdge.
			if(!creditCard.IsXWeb() && creditCard.XChargeToken.StartsWith("XAW")) {
				return creditCard.XChargeToken.Substring(3);
			}
			return creditCard.XChargeToken;
		}

		///<summary>Gets the chargefrequencytype for a given credit card. See enum for types.</summary>
		public static ChargeFrequencyType GetFrequencyType(string chargeFrequency) {
			//No need to check MiddleTierRole; no call to db.
			string chargeFrequencyType=chargeFrequency.Substring(0,chargeFrequency.IndexOf('|'));
			return (ChargeFrequencyType)PIn.Int(chargeFrequencyType);		
		}

		///<summary>Gets the DayOfWeekFrequency for a given credit card. This should only be used when the freqeuency type is FixedWeekDay.</summary>
		public static DayOfWeekFrequency GetDayOfWeekFrequency(string chargeFrequency) {
			//No need to check MiddleTierRole; no call to db.
			string dayOfWeekFrequency=chargeFrequency.Substring(chargeFrequency.IndexOf('|')+1,
				chargeFrequency.LastIndexOf('|')-chargeFrequency.IndexOf('|')-1);
			return (DayOfWeekFrequency)PIn.Int(dayOfWeekFrequency);
		}

		///<summary>Gets the DayOfWeek when the frequency type is FixedWeekDay.</summary>
		public static DayOfWeek GetDayOfWeek(string chargeFrequency) {
			//No need to check MiddleTierRole; no call to db.
			string dayOfWeek=chargeFrequency.Substring(chargeFrequency.LastIndexOf('|')+1);
			return (DayOfWeek)PIn.Int(dayOfWeek);
		}

		///<summary>Gets the days of the month the credit card will be charged on when the frequency type is FixedDayOfMonth.</summary>
		public static string GetDaysOfMonthForChargeFrequency(string chargeFrequency) {
			//No need to check MiddleTierRole; no call to db.
			return chargeFrequency.Substring(chargeFrequency.LastIndexOf('|')+1);
		}
		
		/// <summary>Takaes in a CreditCard.ChargeFrequency string and parses it into a human readable form to be displayed to users</summary>
		public static string GetHumanReadableFrequency(string chargeFrequency) {
			//No need to check MiddleTierRole; no call to db.
			string humanReadableFrequencyString="";
			if(String.IsNullOrEmpty(chargeFrequency) || !chargeFrequency.Contains("|")) {
				return humanReadableFrequencyString;
			}
			ChargeFrequencyType chargeFrequencyType=GetFrequencyType(chargeFrequency);
			switch(chargeFrequencyType) {
				case ChargeFrequencyType.FixedDayOfMonth:
					humanReadableFrequencyString=GetHumanReadableFrequencyFixedMonthly(chargeFrequency);
					break;
				case ChargeFrequencyType.FixedWeekDay:
					humanReadableFrequencyString=GetHumanReadableFrequencyFixedWeekly(chargeFrequency);
					break;
			}
			return humanReadableFrequencyString;
		}

		private static string GetHumanReadableFrequencyFixedWeekly(string chargeFrequency) {
			//No need to check MiddleTierRole; no call to db.
			string humanReadableFrequencyString="";
			DayOfWeekFrequency dayOfWeekFrequency=GetDayOfWeekFrequency(chargeFrequency);
			DayOfWeek dayOfWeek=GetDayOfWeek(chargeFrequency);
			if(dayOfWeekFrequency==DayOfWeekFrequency.EveryOther) {
				humanReadableFrequencyString+=Lans.g("CreditCardFrequency","Every Other")+" ";
			}
			else {
				humanReadableFrequencyString+=Lans.g("CreditCardFrequency",Enum.GetName(typeof(DayOfWeekFrequency),dayOfWeekFrequency)+" ");
			}
			humanReadableFrequencyString+=Lans.g("CreditCardFrequency",Enum.GetName(typeof(DayOfWeek),dayOfWeek));
			humanReadableFrequencyString+=" "+Lans.g("CreditCardFrequency","of the month");
			return humanReadableFrequencyString;
		}

		private static string GetHumanReadableFrequencyFixedMonthly(string chargeFrequency) {
			//No need to check MiddleTierRole; no call to db.
			List<string> listOfDates=GetDaysOfMonthForChargeFrequency(chargeFrequency).Split(',').ToList();
			string strDates=Lans.g("CreditCardFrequency","Days of the month:")+" ";
			if(listOfDates.Count==1) {
				strDates+=listOfDates[0];
				return strDates;
			}
			for(int i=0;i<listOfDates.Count;i++) {
				if(i==listOfDates.Count-1) {
					strDates+=" "+Lans.g("All","and")+listOfDates[i];
				}
				else {
					strDates+=listOfDates[i]+",";
				}
			}
			return strDates;
		}

		public static bool HasDuplicateXChargeToken(CreditCard creditCard) {
			//No need to check MiddleTierRole; no call to db.
			int count=GetCardsByToken(creditCard.XChargeToken,ListTools.FromSingle(creditCard.CCSource)).Count;
			return count>1;
		}

		public static bool HasDuplicatePaySimpleToken(CreditCard creditCard) {
			//No need to check MiddleTierRole; no call to db.
			int count=GetCardsByToken(creditCard.PaySimpleToken,ListTools.FromSingle(creditCard.CCSource)).Count;
			return count>1;
		}

		public static bool HasDuplicatePayConnectToken(CreditCard creditCard) {
			//No need to check MiddleTierRole; no call to db.
			int count=GetCardsByToken(creditCard.PayConnectToken,ListTools.FromSingle(creditCard.CCSource)).Count;
			return count>1;
		}

		///<summary>Checks if a credit card has a recurring charge associated with it. Returns true if it does, false if not.</summary>
		public static bool IsRecurring(CreditCard creditCard) {
			//No need to check MiddleTierRole; no call to db.
			if(creditCard.DateStart.Year<1880){
				return false;
			}
			if(CompareDouble.IsZero(creditCard.ChargeAmt)){
				return false;
			}
			if(creditCard.ChargeFrequency.IsNullOrEmpty()){
				return false;
			}
			return true;
		}

		///<summary>Inserts a CreditCard for EdgeExpress or X-Charge.</summary>
		public static CreditCard CreateNewOpenEdgeCard(long patNum,long clinicNum,string token,string expMonth,string expYear,string ccNumberMasked,
			CreditCardSource creditCardSource) 
		{
			//No need to check MiddleTierRole; no call to db.
			CreditCard creditCard=new CreditCard();
			List<CreditCard> listCreditCardItemOrderCount=RefreshAll(patNum);
			creditCard.ItemOrder=listCreditCardItemOrderCount.Count;
			creditCard.PatNum=patNum;
			creditCard.CCExpiration=new DateTime(Convert.ToInt32("20"+expYear),Convert.ToInt32(expMonth),1);
			creditCard.XChargeToken=token;
			creditCard.CCNumberMasked=ccNumberMasked;
			creditCard.Procedures=PrefC.GetString(PrefName.DefaultCCProcs);
			creditCard.CCSource=creditCardSource;
			creditCard.ClinicNum=clinicNum;
			Insert(creditCard);
			SecurityLogs.MakeLogEntry(EnumPermType.CreditCardEdit,patNum,"Credit Card Added");
			return creditCard;
		}

		public static void RemoveRecurringCharges(long payPlanNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payPlanNum);
				return;
			}
			string command=$"UPDATE creditcard "+
				$"SET PayPlanNum=0,ChargeAmt=0,DateStart={POut.Date(DateTime.MinValue)},DateStop={POut.Date(DateTime.MinValue)},ChargeFrequency='' "+
				$"WHERE PayPlanNum={POut.Long(payPlanNum)}";
			Db.NonQ(command);
		}

	}
}