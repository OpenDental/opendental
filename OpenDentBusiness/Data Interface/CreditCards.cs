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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),payPlanNum);
			}
			string command=$"SELECT * FROM creditcard WHERE PayPlanNum={POut.Long(payPlanNum)}";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Returns the max itemOrder in CreditCards for a patient. If the patient has no credit cards, -1 is returned.</summary>
		public static int GetMaxItemOrderForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<int>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$"SELECT Max(ItemOrder) FROM creditcard WHERE PatNum="+POut.Long(patNum)+" ";
			string MaxItemOrder=Db.GetScalar(command);
			if(MaxItemOrder.IsNullOrEmpty()) {
				return -1;
			}
			return PIn.Int(MaxItemOrder);
		}
		#endregion

		///<summary>If patNum==0 then does not filter on PatNum; otherwise filters on PatNum. Only includes credit cards whose source is Open Dental
		///proper.</summary>
		public static List<CreditCard> Refresh(long patNum){
			//No need to check RemotingRole; no call to db.
			List<CreditCardSource> listSources=Enum.GetValues(typeof(CreditCardSource)).Cast<CreditCardSource>()
				.Where(x => !ListTools.In(x,CreditCardSource.PayConnectPortalLogin,CreditCardSource.CareCredit)).ToList();
			return RefreshBySource(patNum,listSources);
		}

		///<summary>Get all credit cards by a given list of CreditCardSource(s). Optionally filter by a given patNum.</summary>
		public static List<CreditCard> RefreshBySource(long patNum,List<CreditCardSource> listSources) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),patNum,listSources);
			}
			if(listSources.Count==0) {
				return new List<CreditCard>();
			}
			string command="SELECT * FROM creditcard WHERE CCSource IN ("+string.Join(",",listSources.Select(x=>POut.Int((int)x)))+") ";
			if(patNum!=0) { //Add the PatNum criteria.
				command+="AND PatNum="+POut.Long(patNum)+" ";
			}
			command+="ORDER BY ItemOrder DESC";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Gets one CreditCard from the db.</summary>
		public static CreditCard GetOne(long creditCardNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<CreditCard>(MethodBase.GetCurrentMethod(),creditCardNum);
			}
			return Crud.CreditCardCrud.SelectOne(creditCardNum);
		}

		///<summary></summary>
		public static long Insert(CreditCard creditCard){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				creditCard.CreditCardNum=Meth.GetLong(MethodBase.GetCurrentMethod(),creditCard);
				return creditCard.CreditCardNum;
			}
			return Crud.CreditCardCrud.Insert(creditCard);
		}

		///<summary>Validate payConnectResponseWeb and create a new credit card from the PayConnectResponseWeb.</summary>
		public static long InsertFromPayConnect(PayConnectResponseWeb payConnectResponseWeb) {
			//No need to check RemotingRole;no call to db.
			if(GetTokenCount(payConnectResponseWeb.PaymentToken,
				new List<CreditCardSource> { CreditCardSource.PayConnect,CreditCardSource.PayConnectPortal,CreditCardSource.PayConnectPortalLogin })>0) { //Prevent duplicates.
				throw new Exception("PayConnect token already exists: "+payConnectResponseWeb.PaymentToken);
			}
			return Insert(payConnectResponseWeb.ToCreditCard());
		}

		///<summary>Creates a new credit card from the XWebResponse if it doesn't exist. Returns CreditCardNum of new or existing card.</summary>
		public static long InsertFromXWeb(XWebResponse xWebResponse) {
			//No need to check RemotingRole;no call to db.
			List<CreditCardSource> listSources=new List<CreditCardSource> { 
				CreditCardSource.XWeb,
				CreditCardSource.XWebPortalLogin,
				CreditCardSource.EdgeExpressCNP,
				CreditCardSource.EdgeExpressRCM 
			};
			//Prevent duplicates.
			if(GetTokenCount(xWebResponse.Alias,listSources)>0) {
				return GetCardsByToken(xWebResponse.Alias,listSources).First().CreditCardNum;
			}
			return Insert(new CreditCard() {
				PatNum=xWebResponse.PatNum,
				XChargeToken=xWebResponse.Alias,
				CCNumberMasked=xWebResponse.MaskedAcctNum,
				CCExpiration=xWebResponse.AccountExpirationDate,
				CCSource=xWebResponse.CCSource,
				ClinicNum=xWebResponse.ClinicNum,
				ItemOrder=Refresh(xWebResponse.PatNum).Count,
				Address="",
				Zip="",
				ChargeAmt=0,
				DateStart=DateTime.MinValue,
				DateStop=DateTime.MinValue,
				Note="",
				PayPlanNum=0,
				PayConnectToken="",
				PayConnectTokenExp=DateTime.MinValue,
				PaySimpleToken="",
				Procedures="",
			});
		}

		///<summary></summary>
		public static void Update(CreditCard creditCard){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),creditCard);
				return;
			}
			Crud.CreditCardCrud.Update(creditCard);
		}

		///<summary></summary>
		public static void Delete(long creditCardNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),creditCardNum);
				return;
			}
			string command= "DELETE FROM creditcard WHERE CreditCardNum = "+POut.Long(creditCardNum);
			Db.NonQ(command);
		}

		///<summary>Gets the masked CC# and exp date for all cards setup for monthly charges for the specified patient.  Only used for filling [CreditCardsOnFile] variable when emailing statements.</summary>
		public static string GetMonthlyCardsOnFile(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetString(MethodBase.GetCurrentMethod(),patNum);
			}
			string result="";
			string command="SELECT * FROM creditcard WHERE PatNum="+POut.Long(patNum)
				+" AND ("+DbHelper.Year("DateStop")+"<1880 OR DateStop>"+DbHelper.Now()+") "//Recurring card is active.
				+" AND ChargeAmt>0"
				+" AND CCSource NOT IN ("+(int)CreditCardSource.XWeb+","+(int)CreditCardSource.XWebPortalLogin+") ";//Not created from the Patient Portal
			List<CreditCard> monthlyCards=Crud.CreditCardCrud.SelectMany(command);
			for(int i=0;i<monthlyCards.Count;i++) {
				if(i>0) {
					result+=", ";
				}
				result+=monthlyCards[i].CCNumberMasked+" exp:"+monthlyCards[i].CCExpiration.ToString("MM/yy");
			}
			return result;
		}

		///<summary>Returns list of active credit cards.</summary>
		public static List<CreditCard> GetActiveCards(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM creditcard WHERE PatNum="+POut.Long(patNum)
				+" AND ("+DbHelper.Year("DateStop")+"<1880 OR DateStop>="+DbHelper.Curdate()+") "
				+" AND ("+DbHelper.Year("DateStart")+">1880 AND DateStart<="+DbHelper.Curdate()+") "//Recurring card is active.
				+" AND CCSource NOT IN ("+(int)CreditCardSource.XWeb+","+(int)CreditCardSource.XWebPortalLogin+") ";//Not created from the Patient Portal
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Updates the Procedures column on all cards that have not stopped that are not marked to exclude from sync.  Only used at HQ.</summary>
		public static void SyncDefaultProcs(List<string> listProcCodes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listProcCodes);
				return;
			}
			string command="UPDATE creditcard SET Procedures='"+string.Join(",",listProcCodes.Select(x => POut.String(x)))+"'"
				+" WHERE ("+DbHelper.Year("DateStop")+"<1880 OR DateStop>="+DbHelper.Curdate()+") "//Stop date has not past
				+" AND ExcludeProcSync=0"
				+" AND CCSource NOT IN ("+(int)CreditCardSource.XWeb+","+(int)CreditCardSource.XWebPortalLogin+") ";//Not created from the Patient Portal
			Db.NonQ(command);
		}

		///<summary>Returns list of credit cards that are ready for a recurring charge.  Filters by ClinicNums in list if provided.  List of ClinicNums
		///should contain all clinics the current user is authorized to access.  Further filtering by selected clinics is done at the UI level to save
		///DB calls.</summary>
		public static List<RecurringChargeData> GetRecurringChargeList(List<long> listClinicNums,DateTime curDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RecurringChargeData>>(MethodBase.GetCurrentMethod(),listClinicNums,curDate);
			}
			List<RecurringChargeData> listChargeData=new List<RecurringChargeData>();
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
				+"AND CCSource NOT IN ("+(int)CreditCardSource.XWeb+","+(int)CreditCardSource.XWebPortalLogin+") ";//Not created from the Patient Portal
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
				+"AND CCSource NOT IN ("+(int)CreditCardSource.XWeb+","+(int)CreditCardSource.XWebPortalLogin+") ";//Not created from the Patient Portal
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
				return listChargeData;
			}
			command="SELECT PatNum,MAX(CASE WHEN "+DbHelper.Year("RecurringChargeDate")+" > 1880 "
				+"THEN RecurringChargeDate ELSE PayDate END) RecurringChargeDate "
				+"FROM payment "
				+"WHERE IsRecurringCC=1 AND PayAmt > 0 "
				+"AND PatNum IN ("+string.Join(",",table.Select().Select(x => POut.String(x["PatNum"].ToString())))+") "//table has at least 1 row
				+"GROUP BY PatNum";
			//dictionary is key=PatNum, value=LatestPayment which will be the lastest date a recurring charge payment was made
			Dictionary<long,DateTime> dictPatNumDate=Db.GetTable(command).Select()
				.ToDictionary(x => PIn.Long(x["PatNum"].ToString()),x =>	PIn.Date(x["RecurringChargeDate"].ToString()));
			table.Select().Where(x => dictPatNumDate.ContainsKey(PIn.Long(x["PatNum"].ToString()))).ToList()
				.ForEach(x => x["LatestPayment"]=dictPatNumDate[PIn.Long(x["PatNum"].ToString())]);
			table.Columns.Add("RecurringChargeDate",typeof(DateTime));//Will get set in FilterRecurringChargeList
			FilterRecurringChargeList(table,curDate);
			foreach(DataRow row in table.Rows) {
				RecurringChargeData chargeData=new RecurringChargeData {
					Address=row["Address"].ToString(),
					AddressPat=row["AddressPat"].ToString(),
					BillingCycleDay=PIn.Int(row["BillingCycleDay"].ToString()),
					CCExpiration=PIn.DateT(row["CCExpiration"].ToString()),
					CCNumberMasked=row["CCNumberMasked"].ToString(),
					CCSource=PIn.Enum<CreditCardSource>(row["CCSource"].ToString()),
					DateStart=PIn.DateT(row["DateStart"].ToString()),
					Guarantor=PIn.Long(row["Guarantor"].ToString()),
					LatestPayment=PIn.DateT(row["LatestPayment"].ToString()),
					PatName=row["PatName"].ToString(),
					PayConnectToken=row["PayConnectToken"].ToString(),
					PayConnectTokenExp=PIn.Date(row["PayConnectTokenExp"].ToString()),
					PayPlanNum=PIn.Long(row["PayPlanNum"].ToString()),
					PayPlanPatNum=PIn.Long(row["PayPlanPatNum"].ToString()),
					PaySimpleToken=row["PaySimpleToken"].ToString(),
					Procedures=row["Procedures"].ToString(),
					ProvNum=PIn.Long(row["ProvNum"].ToString()),
					RecurringCharge=new RecurringCharge {
						ChargeAmt=PIn.Double(row["ChargeAmt"].ToString()),
						ChargeStatus=RecurringChargeStatus.NotYetCharged,
						ClinicNum=PrefC.HasClinicsEnabled ? PIn.Long(row["ClinicNum"].ToString()) : 0,
						CreditCardNum=PIn.Long(row["CreditCardNum"].ToString()),
						FamBal=PIn.Double(row["FamBalTotal"].ToString()),
						PatNum=PIn.Long(row["PatNum"].ToString()),
						PayPlanDue=PIn.Double(row["PayPlanDue"].ToString()),
						RepeatAmt=PIn.Double(row["ChargeAmt"].ToString()),
						UserNum=Security.CurUser.UserNum
					},
					RecurringChargeDate=PIn.DateT(row["RecurringChargeDate"].ToString()),
					XChargeToken=row["XChargeToken"].ToString(),
					Zip=row["Zip"].ToString(),
					ZipPat=row["ZipPat"].ToString(),
					CanChargeWhenNoBal=PIn.Bool(row["CanChargeWhenNoBal"].ToString()),
					PaymentType=PIn.Long(row["PaymentType"].ToString()),
					CCIsRecurringActive=PIn.Bool(row["IsRecurringActive"].ToString()),
				};
				//negative family balance does not subtract from payplan amount due and negative payplan amount due does not subtract from family balance due
				double totalBal=Math.Max(chargeData.RecurringCharge.FamBal,0);
				if(CompareDouble.IsZero(chargeData.RecurringCharge.PayPlanDue)) {
					chargeData.RecurringCharge.TotalDue=totalBal;
				}
				else {
					if(PrefC.GetInt(PrefName.PayPlansVersion)==1) {//in PP v2, the PP amt due is included in the pat balance
						totalBal+=Math.Max(chargeData.RecurringCharge.PayPlanDue,0);
					}
					else if(PrefC.GetInt(PrefName.PayPlansVersion)==2) {
						totalBal=Math.Max(totalBal,chargeData.RecurringCharge.PayPlanDue);//At minimum, the Total Due should be the Pay Plan Due amount.
					}
					chargeData.RecurringCharge.TotalDue=totalBal;
				}
				listChargeData.Add(chargeData);
			}
			return listChargeData;
		}
		
		/// <summary>Adds up the total fees for the procedures passed in that have been completed since the last billing day.</summary>
		public static double TotalRecurringCharges(long patNum,string procedures,int billingDay) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod(),patNum,procedures,billingDay);
			}
			//Find the beginning of the current billing cycle, use that date to total charges between now and then for this cycle only.
			//Include that date only when we are not on the first day of the current billing cycle.
			DateTime startBillingCycle;
			if(DateTime.Today.Day>billingDay) {//if today is 7/13/2015 and billingDay is 26, startBillingCycle will be 6/26/2015
				startBillingCycle=new DateTime(DateTime.Today.Year,DateTime.Today.Month,billingDay);
			}
			else {
				//DateTime.Today.AddMonths handles the number of days in the month and leap years
				//Examples: if today was 12/31/2015, AddMonths(-1) would yield 11/30/2015; if today was 3/31/2016, AddMonths(-1) would yield 2/29/2016
				startBillingCycle=DateTime.Today.AddMonths(-1);
				if(billingDay<=DateTime.DaysInMonth(startBillingCycle.Year,startBillingCycle.Month)) {
					//This corrects the issue of a billing cycle day after today but this month doesn't have enough days when last month does
					//Example: if today was 11/30/2015 and the pat's billing cycle day was the 31st, startBillingCycle=Today.AddMonths(-1) would be 10/30/2015.
					//But this pat's billing cycle day is the 31st and the December has 31 days.  This adjusts the start of the billing cycle to 10/31/2015.
					//Example 2: if today was 2/29/2016 (leap year) and the pat's billing cycle day was the 30th, startBillingCycle should be 1/30/2016.
					//Today.AddMonths(-1) would be 1/29/2016, so this adjusts startBillingCycle to 1/30/2016.
					startBillingCycle=new DateTime(startBillingCycle.Year,startBillingCycle.Month,billingDay);
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
				command+="AND pl.ProcDate>"+POut.Date(startBillingCycle);
			}
			else {
				command+="AND pl.ProcDate>="+POut.Date(startBillingCycle);
			}
			return PIn.Double(Db.GetScalar(command));
		}

		/// <summary>Returns true if the procedure passed in is linked to any other active card on the patient's account.</summary>
		public static bool ProcLinkedToCard(long patNum,string procCode,long cardNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
		public static void FilterRecurringChargeList(DataTable table,DateTime curDate) {
			//Loop through table and remove patients that do not need to be charged yet.
			//Modify the charge amount if necessary to account for the number of missed charges in the last month.
			for(int i=0;i<table.Rows.Count;i++) {
				DateTime dateStart=PIn.DateT(table.Rows[i]["DateStart"].ToString());
				DateTime latestPayment=PIn.Date(table.Rows[i]["LatestPayment"].ToString());
				string chargeFrequency=table.Rows[i]["ChargeFrequency"].ToString();
				DateTime recurringChargeDate=new DateTime();
				int chargeCount=0;
				if(GetFrequencyType(chargeFrequency)==ChargeFrequencyType.FixedDayOfMonth) {
					chargeCount=GetCountChargesFixedDay(curDate,latestPayment,dateStart,chargeFrequency,out recurringChargeDate);
				}
				else if(GetFrequencyType(chargeFrequency)==ChargeFrequencyType.FixedWeekDay) {
					chargeCount=GetCountChargesFixedWeekday(curDate,latestPayment,dateStart,chargeFrequency,out recurringChargeDate);
				}
				table.Rows[i]["RecurringChargeDate"]=recurringChargeDate;
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
		public static bool DoChargePatient(DateTime thisMonthDayToBeCharged,DateTime lastMonthDayToBeCharged,DateTime curDate,DateTime latestPayment,
			DateTime dateStart,out DateTime chargeDate) 
		{
			if(thisMonthDayToBeCharged.Date<=curDate.Date) {//If it is past their time to be charged for this month
				if(dateStart.Date<=thisMonthDayToBeCharged.Date) {
					//if their last payment was before their day to be charged
					if(latestPayment.Date < thisMonthDayToBeCharged.Date) {
						chargeDate=thisMonthDayToBeCharged;
						return true;
					}
				}
			}
			else {//It's not past their date for this month
				//This next check is to make sure last month they should have been charged based on the dateStart.
				//An example where this will be false is when a recurring charge is created on February 2nd and set to every 16th of every month.
				//When they run the tool on February 3rd, the card should not be charged as January 16th is before the plan was created.
				if(dateStart.Date<=lastMonthDayToBeCharged.Date) {
					//Check to see if the payment happened for last month's date
					if(latestPayment.Date < lastMonthDayToBeCharged.Date) {//if not charge them
						chargeDate=lastMonthDayToBeCharged;
						return true;
					}
				}
			}
			chargeDate=new DateTime();
			return false;
		}

		///<summary>Gets the number of charges that should be charged if the recurring charge is for fixed day(s) of the month.</summary>
		private static int GetCountChargesFixedDay(DateTime curDate,DateTime latestPayment,DateTime dateStart,string chargeFrequency,
			out DateTime recurringChargeDate) 
		{
			recurringChargeDate=new DateTime();
			//Get days to be charged on.
			int chargeCount=0;
			List<int> listDays=GetDaysOfMonthForChargeFrequency(chargeFrequency).Split(',').Select(x => PIn.Int(x)).ToList();
			int daysInThisMonth=DateTime.DaysInMonth(curDate.Year,curDate.Month);
			//Only time not accurate is curDate.Month=1. Dec has 31 days regardless of year, however.
			int daysInLastMonth=DateTime.DaysInMonth(curDate.AddMonths(-1).Year,curDate.AddMonths(-1).Month);
			foreach(int day in listDays) {//foreach day they should have been charged
				//this will get the day it should be charged. e.g. the card gets charged on the 31st of every month. If the month is february,
				//Day to be charged is Feb 28th.
				DateTime thisMonthDayToBeCharged=new DateTime(curDate.Year,curDate.Month,Math.Min(daysInThisMonth,day));
				//Calculates the last months charge date. In the example above on the 31st of every month, while february's charge date
				//is the 28th, January's was the 31st. 
				DateTime lastMonthDayToBeCharged=new DateTime(curDate.AddMonths(-1).Year,curDate.AddMonths(-1).Month,
					Math.Min(daysInLastMonth,day));
				DateTime chargeDateThisCharge;
				if(DoChargePatient(thisMonthDayToBeCharged,lastMonthDayToBeCharged,curDate,latestPayment,dateStart,out chargeDateThisCharge)) {
					chargeCount++;
					recurringChargeDate=ODMathLib.Max(recurringChargeDate,chargeDateThisCharge);
				}
			}
			return chargeCount;
		}

		///<summary>Gets the number of charges that should be charged if the recurring charge is for fixed weekday(s) of the month.</summary>
		private static int GetCountChargesFixedWeekday(DateTime curDate,DateTime latestPayment,DateTime dateStart,string chargeFrequency,
			out DateTime recurringChargeDate)
		{
			int chargeCount=0;
			DayOfWeekFrequency frequency=GetDayOfWeekFrequency(chargeFrequency);
			DayOfWeek day=GetDayOfWeek(chargeFrequency);
			if(frequency==DayOfWeekFrequency.Every) {
				//Due to the limitations of pulling one latest payment and the expectation that people will run this frequently, in the case
				//of many missed charges, we only charge up to a month worth of charges. E.g. If someone is charged every friday, 
				//only the previous four fridays will be checked and charged if their latest payment is far back.
				//Get dateTime for the most recent day that is the day of the week that is <= today
				DateTime recentDateToCharge=MiscUtils.GetMostRecentDayOfWeek(curDate,day);
				recurringChargeDate=recentDateToCharge;
				for(int j=0;j<4;j++) {
					//Make sure the day being looked at was after the start date. This prevents cards from being charged for weeks that 
					//were before the recurring charge was created
					if(recentDateToCharge.AddDays(-j*7)>=dateStart) {
						if(latestPayment.Date < recentDateToCharge.Date.AddDays(-j*7)) {
							chargeCount++;
							//keep checking to see when the latestpayment was. For practices running their report frequently, this will
							//almost never make it past the first loop.
						}
					}
				}
			}
			else if(frequency==DayOfWeekFrequency.EveryOther) {
				//Similar to DayOfWeekFrequency.Every, in the case of many missed payments, the last 2 instances of the weekday will be 
				//checked. The every other cycle will be reset if it is not clear what week rotation is being charge on based 
				//on the latestpayment
				//Get the most recent date for the day of the week in the every other cycle
				DateTime recentDateToCharge=latestPayment;
				if(latestPayment.Year > 1880) {//if a payment has occurred
					//if it was last paid on a day that is not the designated day of the week, find the closest.
					if(latestPayment.DayOfWeek!=day) {
						recentDateToCharge=MiscUtils.GetMostRecentDayOfWeek(latestPayment,day);
					}
					//Continue the every other cycle until finding the most recent day of the week for this two week cycle
					while(recentDateToCharge.AddDays(14)<=curDate) {
						recentDateToCharge=recentDateToCharge.AddDays(14);
					}
				}
				else {//if no payment, find the most recent day of the week that occurred in the past to begin the every other cycle.
					recentDateToCharge=MiscUtils.GetMostRecentDayOfWeek(curDate,day);
				}
				recurringChargeDate=recentDateToCharge;
				for(int j=0;j<2;j++) {
					//Make sure the day being looked at was after the start date. This prevents cards from being charged for weeks that 
					//were before the recurring charge was created
					if(recentDateToCharge.AddDays(-j*14)>=dateStart) {
						if(recentDateToCharge.AddDays(-j*14) > latestPayment) {
							chargeCount++;
						}
					}
				}
			}
			else {//handles all nth day of the month.
				//Get day to charge on this month and for last month.
				//(int)frequency-1 -- see DayOfWeekFrequencyEnum
				DateTime thisMonthDayToBeCharged=GetNthWeekdayofMonth(curDate,(int)frequency-1,day);
				DateTime lastMonthDayToBeCharged=GetNthWeekdayofMonth(curDate.AddMonths(-1),(int)frequency-1,day);
				if(DoChargePatient(thisMonthDayToBeCharged,lastMonthDayToBeCharged,curDate,latestPayment,dateStart,out recurringChargeDate)) {
					chargeCount++;
				}
			}
			return chargeCount;
		}

		///<summary>Gets the nth day of the week of a month. If the nth day does not exist, it will return the last day of the month passed in.
		///date should be the year and month that the nth day will be found in.</summary>
		public static DateTime GetNthWeekdayofMonth(DateTime date,int nthWeek,DayOfWeek dayOfWeek) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),token,includeXWeb);
			}
			string command=$"SELECT * FROM creditcard WHERE XChargeToken='{POut.String(token)}' ";
			if(!includeXWeb) {
				command+=$"AND CCSource NOT IN({POut.Int((int)CreditCardSource.XWeb)},{POut.Int((int)CreditCardSource.XWebPortalLogin)}) ";
			}
			return CountSameCard(Crud.CreditCardCrud.SelectMany(command));
		}

		///<summary>Returns number of times token is in use.</summary>
		public static int GetPayConnectTokenCount(string token) {
			if(string.IsNullOrEmpty(token)) {
				return 0;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),token,isAch);
			}
			string command=$"SELECT * FROM creditcard WHERE PaySimpleToken='{POut.String(token)}' ";
			if(!isAch) {
				command+=$"AND CCSource!={POut.Int((int)CreditCardSource.PaySimpleACH)} ";
			}
			return CountSameCard(Crud.CreditCardCrud.SelectMany(command));
		}

		///<summary>Returns number of times token is in use.  Token was duplicated once and caused the wrong card to be charged.</summary>
		public static int GetTokenCount(string token,List<CreditCardSource> listSources) {
			//No need to check RemotingRole; no call to db.
			return CountSameCard(GetCardsByToken(token,listSources));
		}

		///<summary>Counts how many cards are probably the same.</summary>
		private static int CountSameCard(List<CreditCard> listCards) {
			//There may be duplicate tokens if the office adds the same CC multiple times (on the same patient or on multiple patients).
			//This is considered valid and we only want to catch duplicates for different CC numbers
			//CCNumberMasked only stores 4 digits, so adding the extra data to the select "grouping" allows a more reliable count.
			return listCards
				.Select(x => x.CCNumberMasked+x.CCExpiration.ToString("MMYYYYY")+x.CCSource.ToString())
				.Distinct()
				.Count();
		}

		public static List<CreditCard> GetCardsByToken(string token,List<CreditCardSource> listSources) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),token,listSources);
			}
			if(listSources.Count==0) {
				return new List<CreditCard>();
			}
			string command="SELECT * FROM creditcard WHERE CCSource IN ("+string.Join(",",listSources.Select(x=>POut.Int((int)x)))+") AND (0 ";
			if(listSources.Contains(CreditCardSource.PayConnect) || listSources.Contains(CreditCardSource.XServerPayConnect)) {
				command+="OR PayConnectToken='"+POut.String(token)+"' ";
			}
			if(listSources.Contains(CreditCardSource.XServer)
				|| listSources.Contains(CreditCardSource.XWeb)
				|| listSources.Contains(CreditCardSource.XServerPayConnect)
				|| listSources.Contains(CreditCardSource.XWebPortalLogin)
				|| listSources.Contains(CreditCardSource.EdgeExpressCNP)
				|| listSources.Contains(CreditCardSource.EdgeExpressRCM))
			{
				command+="OR XChargeToken='"+POut.String(token)+"' ";
			}
			if(listSources.Contains(CreditCardSource.PaySimple) || listSources.Contains(CreditCardSource.PaySimpleACH)) {
				command+="OR PaySimpleToken='"+POut.String(token)+"' ";
			}
			command+=")";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		public static List<CreditCard> GetCardsWithTokenBySource(List<CreditCardSource> listSources) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),listSources);
			}
			if(listSources.Count==0) {
				return new List<CreditCard>();
			}
			string command="SELECT * FROM creditcard WHERE CCSource IN ("+string.Join(",",listSources.Select(x=>POut.Int((int)x)))+") AND (0 ";
			if(listSources.Contains(CreditCardSource.PayConnect) || listSources.Contains(CreditCardSource.XServerPayConnect)) {
				command+="OR PayConnectToken!='' ";
			}
			if(listSources.Contains(CreditCardSource.XServer)
				|| listSources.Contains(CreditCardSource.XWeb)
				|| listSources.Contains(CreditCardSource.XServerPayConnect)
				|| listSources.Contains(CreditCardSource.XWebPortalLogin))
			{
				command+="OR XChargeToken!='' ";
			}
			if(listSources.Contains(CreditCardSource.PaySimple)) {
				command+="OR PaySimpleToken!='' ";
			}
			command+=")";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Gets every credit card in the db with an X-Charge token that was created from the specified source.</summary>
		public static List<CreditCard> GetCardsWithXChargeTokens(CreditCardSource ccSource=CreditCardSource.XServer) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod(),ccSource);
			}
			string command="SELECT * FROM creditcard WHERE XChargeToken!=\"\" "
				+"AND CCSource="+POut.Int((int)ccSource);
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Gets every credit card in the db with a PayConnect token.</summary>
		public static List<CreditCard> GetCardsWithPayConnectTokens() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CreditCard>>(MethodBase.GetCurrentMethod());
			}
			string command = "SELECT * FROM creditcard WHERE PayConnectToken!=\"\"";
			return Crud.CreditCardCrud.SelectMany(command);
		}

		///<summary>Gets a token that can be used by XWeb. A token that is created by the XCharge client program can be used for XWeb after stripping
		///off the beginning 3 characters which are always "XAW".</summary>
		public static string GetXWebToken(CreditCard cc) {
			//CM 07/05/2018 - I don't have any documentation about removing "XAW" from the token in order to use it for XWeb, but I think I got that
			//information from an email from someone at OpenEdge.
			if(!cc.IsXWeb() && cc.XChargeToken.StartsWith("XAW")) {
				return cc.XChargeToken.Substring(3);
			}
			return cc.XChargeToken;
		}

		///<summary>Gets the chargefrequencytype for a given credit card. See enum for types.</summary>
		public static ChargeFrequencyType GetFrequencyType(string chargeFrequency) {
			//No need to check RemotingRole; no call to db.
			string chargeFrequencyType=chargeFrequency.Substring(0,chargeFrequency.IndexOf('|'));
			return (ChargeFrequencyType)PIn.Int(chargeFrequencyType);		
		}

		///<summary>Gets the DayOfWeekFrequency for a given credit card. This should only be used when the freqeuency type is FixedWeekDay.</summary>
		public static DayOfWeekFrequency GetDayOfWeekFrequency(string chargeFrequency) {
			//No need to check RemotingRole; no call to db.
			string dayOfWeekFrequency=chargeFrequency.Substring(chargeFrequency.IndexOf('|')+1,
				chargeFrequency.LastIndexOf('|')-chargeFrequency.IndexOf('|')-1);
			return (DayOfWeekFrequency)PIn.Int(dayOfWeekFrequency);
		}

		///<summary>Gets the DayOfWeek when the frequency type is FixedWeekDay.</summary>
		public static DayOfWeek GetDayOfWeek(string chargeFrequency) {
			//No need to check RemotingRole; no call to db.
			string dayOfWeek=chargeFrequency.Substring(chargeFrequency.LastIndexOf('|')+1);
			return (DayOfWeek)PIn.Int(dayOfWeek);
		}

		///<summary>Gets the days of the month the credit card will be charged on when the frequency type is FixedDayOfMonth.</summary>
		public static string GetDaysOfMonthForChargeFrequency(string chargeFrequency) {
			//No need to check RemotingRole; no call to db.
			return chargeFrequency.Substring(chargeFrequency.LastIndexOf('|')+1);
		}
		
		/// <summary>Takaes in a CreditCard.ChargeFrequency string and parses it into a human readable form to be displayed to users</summary>
		public static string GetHumanReadableFrequency(string chargeFrequency) {
			string humanReadableFrequencyString="";
			ChargeFrequencyType frequencyType=GetFrequencyType(chargeFrequency);
      switch(frequencyType) {
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
			string humanReadableFrequencyString="";
			DayOfWeekFrequency dayFrequency=GetDayOfWeekFrequency(chargeFrequency);
			DayOfWeek weekDay=GetDayOfWeek(chargeFrequency);
      if(dayFrequency==DayOfWeekFrequency.EveryOther) {
				humanReadableFrequencyString+=Lans.g("CreditCardFrequency","Every Other")+" ";
			}
			else {
				humanReadableFrequencyString+=Lans.g("CreditCardFrequency",Enum.GetName(typeof(DayOfWeekFrequency),dayFrequency)+" ");
			}
			humanReadableFrequencyString+=Lans.g("CreditCardFrequency",Enum.GetName(typeof(DayOfWeek),weekDay));
			humanReadableFrequencyString+=" "+Lans.g("CreditCardFrequency","of the month");
			return humanReadableFrequencyString;
		}

		private static string GetHumanReadableFrequencyFixedMonthly(string chargeFrequency) {
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
			//No need to check RemotingRole; no call to db.
			int count=GetCardsByToken(creditCard.XChargeToken,ListTools.FromSingle(creditCard.CCSource)).Count;
			return count>1;
		}

		public static bool HasDuplicatePaySimpleToken(CreditCard creditCard) {
			//No need to check RemotingRole; no call to db.
			int count=GetCardsByToken(creditCard.PaySimpleToken,ListTools.FromSingle(creditCard.CCSource)).Count;
			return count>1;
		}

		public static bool HasDuplicatePayConnectToken(CreditCard creditCard) {
			//No need to check RemotingRole; no call to db.
			int count=GetCardsByToken(creditCard.PayConnectToken,ListTools.FromSingle(creditCard.CCSource)).Count;
			return count>1;
		}

		///<summary>Checks if a credit card has a recurring charge associated with it. Returns true if it does, false if not.</summary>
		public static bool IsRecurring(CreditCard creditCard) {
			return (creditCard.DateStart.Year>1880 && !CompareDouble.IsZero(creditCard.ChargeAmt) && !creditCard.ChargeFrequency.IsNullOrEmpty());
		}

		///<summary>Inserts a CreditCard for EdgeExpress or X-Charge.</summary>
		public static CreditCard CreateNewOpenEdgeCard(long patNum,long clinicNum,string token,string expMonth,string expYear,string ccNumberMasked,
			CreditCardSource ccSource) 
		{
			CreditCard cc=new CreditCard();
			List<CreditCard> itemOrderCount=Refresh(patNum);
			cc.ItemOrder=itemOrderCount.Count;
			cc.PatNum=patNum;
			cc.CCExpiration=new DateTime(Convert.ToInt32("20"+expYear),Convert.ToInt32(expMonth),1);
			cc.XChargeToken=token;
			cc.CCNumberMasked=ccNumberMasked;
			cc.Procedures=PrefC.GetString(PrefName.DefaultCCProcs);
			cc.CCSource=ccSource;
			cc.ClinicNum=clinicNum;
			Insert(cc);
			return cc;
		}

		public static void RemoveRecurringCharges(long payPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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