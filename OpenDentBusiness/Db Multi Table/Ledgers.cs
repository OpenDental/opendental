using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using DataConnectionBase;
using MySql.Data.MySqlClient;

namespace OpenDentBusiness{

	///<summary>This does not correspond to any table in the database.  It works with a variety of tables to calculate aging.</summary>
	public class Ledgers {

		///<summary>Returns a rough guess on how long RunAging() will take in milliseconds based on the amount of data within certain tables that are used to compute aging.</summary>
		public static double GetAgingComputationTime() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod());
			}
			//Factor of 0.0042680625638876 was discovered by timing aging on a large database.  It proved to be very accurate when tested on other databases.
			//A large database with 6091757 rows in the following tables took on average 26 seconds (26000 ms) to run aging.  26000(ms) / 6091757(rows) = 0.0042680625638876
			string command=@"SELECT ((SELECT COUNT(*) FROM patient)
				+ (SELECT COUNT(*) FROM procedurelog)
				+ (SELECT COUNT(*) FROM paysplit)
				+ (SELECT COUNT(*) FROM adjustment)
				+ (SELECT COUNT(*) FROM claimproc)
				+ (SELECT COUNT(*) FROM payplan)
				+ (SELECT COUNT(*) FROM payplancharge)) * 0.0042680625638876 AgingInMilliseconds";
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				command+=" FROM dual";//Oracle requires a FROM clause be present.
			}
			return PIn.Double(Db.GetScalar(command));
		}

		///<summary>This runs aging for all patients.  If using monthly aging, it always just runs the aging as of the last date again.  If using daily
		///aging, it runs it as of today.  This logic used to be in FormAging, but is now centralized.
		///<para>IMPORTANT: If AgingIsEnterprise the calling method MUST check the AgingBeginDateTime pref to determine whether or not another aging
		///calculation has already started.  If a calculation is running and this is called again the famaging table could be truncated and one or both
		///processes could update the family bals incorrectly.</para></summary>
		public static void RunAging() {
			//No need to check RemotingRole; no call to db.
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {
				ComputeAging(0,PrefC.GetDate(PrefName.DateLastAging));
			}
			else {
				ComputeAging(0,DateTime.Today);
				if(PrefC.GetDate(PrefName.DateLastAging) != DateTime.Today) {
					Prefs.UpdateString(PrefName.DateLastAging,POut.Date(DateTime.Today,false));
					//Since this is always called from UI, the above line works fine to keep the prefs cache current.
				}
			}
		}

		///<summary>Computes aging for the family specified. Specify guarantor=0 in order to calculate aging for all families. Gets all info from db.
		///<para>IMPORTANT: If AgingIsEnterprise and guarantorNum is 0 (i.e. run for all patients), the calling method MUST check the AgingBeginDateTime
		///pref to determine whether or not another aging calculation has already started.  If a calculation is running and this is called again the
		///famaging table could be truncated and one or both processes could update the family bals incorrectly.</para>
		///<para>The aging calculation will use the following rules within each family:</para>
		///<para>1) The aging "buckets" (0 to 30, 31 to 60, 61 to 90 and Over 90) ONLY include account activity on or before AsOfDate.</para>
		///<para>2) BalTotal includes all account activity, even future entries. If historical, BalTotal excludes entries after AsOfDate.</para>
		///<para>3) InsEst includes all insurance estimates, even future estimates. If historical, InsEst excludes ins est after AsOfDate.</para>
		///<para>4) PayPlanDue includes all payplan charges minus credits. If historical, PayPlanDue excludes charges and credits after AsOfDate.</para></summary>
		public static void ComputeAging(long guarantorNum,DateTime asOfDate) {
			if(guarantorNum>0) {
				ComputeAging(new List<long>() { guarantorNum },asOfDate);
			}
			else {
				ComputeAging(new List<long>(),asOfDate);
			}
		}

		///<summary>Computes aging for the family specified. Specify guarantor=0 in order to calculate aging for all families. Gets all info from db.
		///<para>IMPORTANT: If AgingIsEnterprise and listGuarantorNums has more than one guarantor in it, the calling method MUST check the
		///AgingBeginDateTime pref to determine whether or not another aging calculation has already started.  If a calculation is running and this is
		///called again the famaging table could be truncated and one or both processes could update the family bals incorrectly.</para>
		///<para>The aging calculation will use the following rules within each family:</para>
		///<para>1) The aging "buckets" (0 to 30, 31 to 60, 61 to 90 and Over 90) ONLY include account activity on or before AsOfDate.</para>
		///<para>2) BalTotal includes all account activity, even future entries. If historical, BalTotal excludes entries after AsOfDate.</para>
		///<para>3) InsEst includes all insurance estimates, even future estimates. If historical, InsEst excludes ins est after AsOfDate.</para>
		///<para>4) PayPlanDue includes all payplan charges minus credits. If historical, PayPlanDue excludes charges and credits after AsOfDate.</para></summary>
		public static void ComputeAging(List<long> listGuarantorNums,DateTime asOfDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listGuarantorNums,asOfDate);
				return;
			}
			bool isMySqlDb=(DataConnection.DBtype==DatabaseType.MySql);
			string command="";
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				#region Using FamAging Table
				if(listGuarantorNums.Count==1) {
					FamAging famAgingCur=Crud.FamAgingCrud.SelectOne(GetAgingQueryString(asOfDate,listGuarantorNums));
					command="UPDATE patient p SET "
						+"p.BalOver90 =CASE WHEN p.Guarantor!=p.PatNum THEN 0 ELSE "+famAgingCur.BalOver90 +" END,"
						+"p.Bal_61_90 =CASE WHEN p.Guarantor!=p.PatNum THEN 0 ELSE "+famAgingCur.Bal_61_90 +" END,"
						+"p.Bal_31_60 =CASE WHEN p.Guarantor!=p.PatNum THEN 0 ELSE "+famAgingCur.Bal_31_60 +" END,"
						+"p.Bal_0_30  =CASE WHEN p.Guarantor!=p.PatNum THEN 0 ELSE "+famAgingCur.Bal_0_30  +" END,"
						+"p.BalTotal  =CASE WHEN p.Guarantor!=p.PatNum THEN 0 ELSE "+famAgingCur.BalTotal  +" END,"
						+"p.InsEst    =CASE WHEN p.Guarantor!=p.PatNum THEN 0 ELSE "+famAgingCur.InsEst    +" END,"
						+"p.PayPlanDue=CASE WHEN p.Guarantor!=p.PatNum THEN 0 ELSE "+famAgingCur.PayPlanDue+" END "
						+"WHERE p.Guarantor="+listGuarantorNums[0];
				}
				else {
					List<FamAging> listFamAgings=Crud.FamAgingCrud.SelectMany(GetAgingQueryString(asOfDate,listGuarantorNums)
						+" HAVING BalOver90!=0 OR Bal_61_90!=0 OR Bal_31_60!=0 OR Bal_0_30!=0 OR BalTotal!=0 OR PayPlanDue!=0 OR InsEst!=0");
					command="TRUNCATE TABLE famaging";
					Db.NonQ(command);
					SecurityLogs.MakeLogEntry(Permissions.FamAgingTruncate,0,"Family aging table truncated");
					FamAgings.InsertMany(listFamAgings);//use InsertMany so inserts are broken into statements no larger than max allowed packet
					command=@"UPDATE patient p
						LEFT JOIN famaging f ON p.PatNum=f.PatNum
						SET
						p.BalOver90  = COALESCE(f.BalOver90,0),
						p.Bal_61_90  = COALESCE(f.Bal_61_90,0),
						p.Bal_31_60  = COALESCE(f.Bal_31_60,0),
						p.Bal_0_30   = COALESCE(f.Bal_0_30,0),
						p.BalTotal   = COALESCE(f.BalTotal,0),
						p.InsEst     = COALESCE(f.InsEst,0),
						p.PayPlanDue = COALESCE(f.PayPlanDue,0)
						"+(listGuarantorNums.Count>0?(@" WHERE p.Guarantor IN ("+string.Join(",",listGuarantorNums)+@")"):"")+@";
						TRUNCATE TABLE famaging;";
				}
				#endregion Using FamAging Table
			}
			else {
				#region Not Using FamAging Table
				//If is for all patients, not single family, zero out all aged bals in order to catch former guarantors.  Zeroing out for a single family is
				//handled in the query below. (see the region "Get All Family PatNums")  Unioning is too slow for all patients, so run this statement first.
				//Added to the same query string to force Galera Cluster to process both queries on the same node to prevent a deadlock error.
				if(listGuarantorNums.Count==0) {
					command="UPDATE patient SET "
					+"Bal_0_30   = 0,"
					+"Bal_31_60  = 0,"
					+"Bal_61_90  = 0,"
					+"BalOver90  = 0,"
					+"InsEst     = 0,"
					+"BalTotal   = 0,"
					+"PayPlanDue = 0;";
				}
				command+=(isMySqlDb?"UPDATE patient p,":"MERGE INTO patient p USING ")
					+"("+GetAgingGuarTransQuery(asOfDate,listGuarantorNums)+") famSums "
					+(isMySqlDb?"":"ON (p.Guarantor=famSums.PatNum) WHEN MATCHED THEN UPDATE ")
					//Update the patient table based on the family amounts summed from 'famSums', and distribute the payments into the oldest balances first.
					+"SET p.BalOver90=(CASE WHEN p.Guarantor != p.PatNum THEN 0 "//zero out non-guarantors
						+"ELSE ROUND(CASE WHEN famSums.TotalCredits >= famSums.ChargesOver90 THEN 0 "//over 90 day bal paid in full
						+"ELSE famSums.ChargesOver90-famSums.TotalCredits END,3) END),"//over 90 day bal partially paid or unpaid.
					+"p.Bal_61_90=(CASE WHEN p.Guarantor != p.PatNum THEN 0 "//zero out non-guarantors
						+"ELSE ROUND(CASE WHEN famSums.TotalCredits <= famSums.ChargesOver90 THEN famSums.Charges_61_90 "//61-90 day bal unpaid
						+"WHEN famSums.ChargesOver90+famSums.Charges_61_90 <= famSums.TotalCredits THEN 0 "//61-90 day bal paid in full
						+"ELSE famSums.ChargesOver90+famSums.Charges_61_90-famSums.TotalCredits END,3) END),"//61-90 day bal partially paid
					+"p.Bal_31_60=(CASE WHEN p.Guarantor != p.PatNum THEN 0 "//zero out non-guarantors
						+"ELSE ROUND(CASE WHEN famSums.TotalCredits < famSums.ChargesOver90+famSums.Charges_61_90 "
						+"THEN famSums.Charges_31_60 "//31-60 day bal unpaid
						+"WHEN famSums.ChargesOver90+famSums.Charges_61_90+famSums.Charges_31_60 <= famSums.TotalCredits THEN 0 "//31-60 day bal paid in full
						+"ELSE famSums.ChargesOver90+famSums.Charges_61_90+famSums.Charges_31_60-famSums.TotalCredits END,3) END),"//31-60 day bal partially paid
					+"p.Bal_0_30=(CASE WHEN p.Guarantor != p.PatNum THEN 0 "//zero out non-guarantors
						+"ELSE ROUND(CASE WHEN famSums.TotalCredits < famSums.ChargesOver90+famSums.Charges_61_90+famSums.Charges_31_60 "
						+"THEN famSums.Charges_0_30 "//0-30 day bal unpaid
						+"WHEN famSums.ChargesOver90+famSums.Charges_61_90+famSums.Charges_31_60+famSums.Charges_0_30 <= famSums.TotalCredits "
						+"THEN 0 "//0-30 day bal paid in full
						+"ELSE famSums.ChargesOver90+famSums.Charges_61_90+famSums.Charges_31_60+famSums.Charges_0_30-famSums.TotalCredits "
						+"END,3) END),"//0-30 day bal partially paid
					+"p.BalTotal=(CASE WHEN p.Guarantor != p.PatNum THEN 0 "//zero out non-guarantors
						+"ELSE ROUND(famSums.BalTotal,3) END),"
					+"p.InsEst=(CASE WHEN p.Guarantor != p.PatNum THEN 0 "//zero out non-guarantors
						+"ELSE ROUND(famSums.InsPayEst+famSums.InsWoEst,3) END),"
					+"p.PayPlanDue=(CASE WHEN p.Guarantor != p.PatNum THEN 0 "//zero out non-guarantors
						+"ELSE ROUND(famSums.PayPlanDue,3) END)"
					+(isMySqlDb?" WHERE p.Guarantor=famSums.PatNum":"");//Aging calculations only apply to guarantors, zero out non-guarantor bals
				#endregion Not Using FamAging Table
			}
			Db.NonQ(command);
		}

		///<summary>Computes aging for PaySplits not associated to the patient passed in. Does nothing if all PaySplits are for the passed in PatNum.
		///Returns error message if AgingIsEnterprise preference is on and someone is currently computing aging. Otherwise returns empty string.</summary>
		public static string ComputeAgingForPaysplitsAllocatedToDiffPats(long patNum,List<PaySplit> listPaySplits) {
			//No need to check RemotingRole; no call to db.;
			List<long> listFamPatNums=Patients.GetFamily(patNum).ListPats.Select(x => x.PatNum).ToList();
			//Get all PatNums not in the patient's family
			List<long> listPatNumsAssociatedToDiffFam=listPaySplits.Where(x => !ListTools.In(x.PatNum,listFamPatNums)).Select(y => y.PatNum).Distinct().ToList();
			if(listPatNumsAssociatedToDiffFam.Count==0) {
				return "";//all PaySplits are for the patient's family passed in. 
			}
			string strErrorMsg="";
			List<long> listGuarantorNums=Patients.GetGuarantorsForPatNums(listPatNumsAssociatedToDiffFam);
			DateTime dateAsOf=DateTime.Today.Date;
			DateTime dateTAgingBeganPref=DateTime.MinValue;
			DateTime dtNow=MiscData.GetNowDateTime();
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {
				dateAsOf=PrefC.GetDate(PrefName.DateLastAging);
			}
			bool isFamaging=(PrefC.GetBool(PrefName.AgingIsEnterprise) && listGuarantorNums.Count>1);//will only use the famaging table if more than 1 guar
			if(isFamaging) {//if this will utilize the famaging table we need to check and set the pref to block others from starting aging
				Prefs.RefreshCache();
				dateTAgingBeganPref=PrefC.GetDateT(PrefName.AgingBeginDateTime);
				if(dateTAgingBeganPref>DateTime.MinValue) {//pref has been set by another process, don't run aging and notify user
					strErrorMsg=Lans.g("Ledgers","Aging failed to run for patients who had paysplits created outside of the current family. This is due to "
						+"the currently running aging calculations which began on")+" "+dateTAgingBeganPref.ToString()+".  "+Lans.g("Ledgers","If you "
						+"believe the current aging process has finished, a user with SecurityAdmin permission can manually clear the date and time by going "
						+"to Setup | Miscellaneous and pressing the 'Clear' button.  You will need to run aging manually once the current aging process has "
						+"finished or date and time is cleared.");
				}
				else {
					Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dtNow,false));//get lock on pref to block others
					Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
					try {
						Ledgers.ComputeAging(listGuarantorNums,dateAsOf);
					}
					finally {
						Prefs.UpdateString(PrefName.AgingBeginDateTime,"");//clear lock on pref whether aging was successful or not
						Signalods.SetInvalid(InvalidType.Prefs);
					}
				}
			}
			else {//not enterprise aging or only 1 guar so not using the famaging table, just run aging as usual
				Ledgers.ComputeAging(listGuarantorNums,dateAsOf);
			}
			return strErrorMsg;
		}

		///<summary>Generates a dictionary where the Key:PatNum and Val:FamilyBalance for passed-in guarantors.</summary>
		public static Dictionary<long,double> GetBalancesForFamilies(List<long> listGuarantorNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,double>(MethodBase.GetCurrentMethod(),listGuarantorNums);
			}
			string command = GetAgingQueryString(DateTime.Today,listGuarantorNums);
			return Db.GetTable(command).Rows.OfType<DataRow>().ToDictionary(x => PIn.Long(x["PatNum"].ToString()),y => PIn.Double(y["BalTotal"].ToString()));
		}

		///<summary>Returns a query string for selecting the guarantor and aged bals with InsEst and PayPlanDue.
		///<para>For enterprise aging: isAllPats=>the results are used to populate the famaging table; !isAllPats=>returns 1 row used to update family.</para>
		///If !isWOAged, query results will exactly match what the patient table would be updated to if aging was run with the same settings and writeoff
		///estimates will be in a separate column, optionally combined with InsPayEst.
		///<para>If isWOAged then the writeoff estimates are aged and retrieved based on the value of woAgedType as follows:
		///WriteoffNotAged: Writeoffs not aged, reported in separate column 'InsWoEst';
		///WriteoffLive: Writeoff estimates from claimprocs;
		///WriteoffOrig: Writeoff estimates from claimsnapshot if exists, otherwise writeoff estimates not aged;
		///WriteoffPreferOrig: Writeoff estimates from claimsnapshot if exists, otherwise from claimproc.</para></summary>
		public static string GetAgingQueryString(DateTime asOfDate,List<long> listGuarantors=null,bool isHistoric=false,bool isInsPayWoCombined=true,
			bool hasDateLastPay=false,bool isGroupByGuar=true,bool isWoAged=false,bool doAgePatPayPlanPayments=false, bool doExcludeIncomeTransfers=false)
		{
			//No need to check RemotingRole; no call to db.
			//Returns family amounts summed from 'tSums', with payments distributed into the oldest balances first.
			string command="SELECT tSums.PatNum,"//if grouped by guar, this is the guar's PatNum; if grouped by patient.PatNum rows are individual pats
				+"ROUND(CASE WHEN tSums.TotalCredits >= tSums.ChargesOver90 THEN 0 "//over 90 day paid in full
					+"ELSE tSums.ChargesOver90 - tSums.TotalCredits END,3) BalOver90,"//over 90 day partially paid or unpaid.
				+"ROUND(CASE WHEN tSums.TotalCredits <= tSums.ChargesOver90 THEN tSums.Charges_61_90 "//61-90 day unpaid
					+"WHEN tSums.ChargesOver90 + tSums.Charges_61_90 <= tSums.TotalCredits THEN 0 "//61-90 day paid in full
					+"ELSE tSums.ChargesOver90 + tSums.Charges_61_90 - tSums.TotalCredits END,3) Bal_61_90,"//61-90 day partially paid
				+"ROUND(CASE WHEN tSums.TotalCredits < tSums.ChargesOver90 + tSums.Charges_61_90 THEN tSums.Charges_31_60 "//31-60 day unpaid
					+"WHEN tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 <= tSums.TotalCredits THEN 0 "//31-60 day paid in full
					+"ELSE tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 - tSums.TotalCredits END,3) Bal_31_60,"//31-60 day partially paid
				+"ROUND(CASE WHEN tSums.TotalCredits < tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 THEN tSums.Charges_0_30 "//0-30 day unpaid
					+"WHEN tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 + tSums.Charges_0_30 <= tSums.TotalCredits THEN 0 "//0-30 day paid in full
					+"ELSE tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 + tSums.Charges_0_30 - tSums.TotalCredits END,3) Bal_0_30,"//0-30 day partially paid
				+"ROUND(tSums.BalTotal,3) BalTotal,";
			if(isInsPayWoCombined) {
				command+="ROUND(tSums.InsPayEst+tSums.InsWoEst,3) InsEst,";
			}
			else {
				command+="ROUND(tSums.InsWoEst,3) InsWoEst,"
					+"ROUND(tSums.InsPayEst,3) InsPayEst,";
			}
			command+="ROUND(tSums.PayPlanDue,3) PayPlanDue"//PayPlanDue included for enterprise aging use
				+(hasDateLastPay?",tSums.DateLastPay ":" ")
				+"FROM ("+GetAgingGuarTransQuery(asOfDate,listGuarantors,hasDateLastPay,isHistoric,isGroupByGuar,isWoAged,
					doAgePatPayPlanPayments, doExcludeIncomeTransfers)+") tSums";
			return command;
		}

		///<summary>For unit tests.</summary>
		public static Dictionary<long,DataRow> GetAgingGuarTransTable(DateTime asOfDate,List<long> listGuarantors,bool hasDateLastPay=false,bool isHistoric=false,
			bool isGroupByGuar=true,bool isWoAged=false,bool doAgePatPayPlanPayments=false)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,DataRow>(MethodBase.GetCurrentMethod(),asOfDate,listGuarantors,hasDateLastPay,isHistoric,
					isGroupByGuar,isWoAged,doAgePatPayPlanPayments);
			}
			string command=GetAgingGuarTransQuery(asOfDate,listGuarantors,hasDateLastPay,isHistoric,isGroupByGuar,
				isWoAged,doAgePatPayPlanPayments);
			return Db.GetTable(command).Rows.OfType<DataRow>().ToDictionary(x => PIn.Long(x["PatNum"].ToString()),y => y);
		}

		///<summary>Returns a query string.</summary>
		private static string GetAgingGuarTransQuery(DateTime asOfDate,List<long> listGuarantors,bool hasDateLastPay=false,bool isHistoric=false,
			bool isGroupByGuar=true,bool isWoAged=false,bool doAgePatPayPlanPayments=false, bool doExcludeIncomeTransfers=false)
		{
			//No need to check RemotingRole; no call to db.
			if(asOfDate.Year<1880) {
				asOfDate=DateTime.Today;
			}
			string asOfDateStr=POut.Date(asOfDate);
			string thirtyDaysAgo=POut.Date(asOfDate.AddDays(-30));
			string sixtyDaysAgo=POut.Date(asOfDate.AddDays(-60));
			string ninetyDaysAgo=POut.Date(asOfDate.AddDays(-90));
			string familyPatNums="";
			string command="";
			if(listGuarantors!=null && listGuarantors.Any(x => x>0)) {
				familyPatNums=string.Join(",",Patients.GetAllFamilyPatNumsForGuars(listGuarantors.FindAll(x => x>0)));//list is not null with at least 1 non-zero guarNum in it
			}
			command="";
			bool isAllPats=string.IsNullOrWhiteSpace(familyPatNums);//true if guarantor==0 or invalid, meaning for all patients not just one family
			//Negative adjustments can optionally be overridden in order to ignore the global preference.
			bool isAgedByProc=PrefC.GetYN(PrefName.AgingProcLifo);
			if(isWoAged || isAgedByProc) {
				//WriteoffOrig and/or negative Adjs are included in the charges buckets.  Since that could reduce a bucket to less than 0 we need to move any
				//excess to the TotalCredits bucket to be applied to the oldest charge first
				command+="SELECT transSums.PatNum,"
					+"GREATEST(transSums.ChargesOver90,0) ChargesOver90,"//if credit reduced an aged column to <0, add the neg amount to the total credits
					+"GREATEST(transSums.Charges_61_90,0) Charges_61_90,"
					+"GREATEST(transSums.Charges_31_60,0) Charges_31_60,"
					+"GREATEST(transSums.Charges_0_30,0) Charges_0_30,"
					+"transSums.TotalCredits "
						+"- LEAST(transSums.ChargesOver90,0) "//minus because TotalCredits is positive and we want to increase it (i.e. minus a negative = +)
						+"- LEAST(transSums.Charges_61_90,0) "
						+"- LEAST(transSums.Charges_31_60,0) "
						+"- LEAST(transSums.Charges_0_30,0) TotalCredits,"
					+"transSums.BalTotal,"
					+"transSums.InsWoEst,"
					+"transSums.InsPayEst,"
					+"transSums.PayPlanDue"
					+(hasDateLastPay?",transSums.DateLastPay ":" ")
					+"FROM (";
			}
			List <string> listInstantTranTypes=new List<string>();
			listInstantTranTypes.Add("'WriteoffOrig'");
			if(isAgedByProc) {
				listInstantTranTypes.Add("'SumByProcAndDate'");
			}
			string instantAdd="trans.TranType IN ("+string.Join(",",listInstantTranTypes)+")";
			command+="SELECT "+(isGroupByGuar?"p.Guarantor PatNum,":"trans.PatNum,")
				+"SUM(CASE WHEN (trans.TranAmount > 0 OR "+instantAdd+") "
					+"AND trans.TranDate < "+ninetyDaysAgo+" THEN trans.TranAmount ELSE 0 END) ChargesOver90,"
				+"SUM(CASE WHEN (trans.TranAmount > 0 OR "+instantAdd+") "
					+"AND trans.TranDate < "+sixtyDaysAgo+" AND trans.TranDate >= "+ninetyDaysAgo+" THEN trans.TranAmount ELSE 0 END) Charges_61_90,"
				+"SUM(CASE WHEN (trans.TranAmount > 0 OR "+instantAdd+") "
					+"AND trans.TranDate < "+thirtyDaysAgo+" AND trans.TranDate >= "+sixtyDaysAgo+" THEN trans.TranAmount ELSE 0 END) Charges_31_60,"
				+"SUM(CASE WHEN (trans.TranAmount > 0 OR "+instantAdd+") "
					+"AND trans.TranDate <= "+asOfDateStr+" AND trans.TranDate >= "+thirtyDaysAgo+" THEN trans.TranAmount ELSE 0 END) Charges_0_30,"
				+"-SUM(CASE WHEN trans.TranAmount < 0 AND NOT("+instantAdd+") "
					+"AND trans.TranDate <= "+asOfDateStr+" THEN trans.TranAmount ELSE 0 END) TotalCredits,"
				+"SUM(CASE WHEN trans.TranAmount != 0"+(isHistoric?(" AND trans.TranDate <= "+asOfDateStr):"")+" THEN trans.TranAmount ELSE 0 END) BalTotal,"
				+"SUM(trans.InsWoEst) InsWoEst,"
				+"SUM(trans.InsPayEst) InsPayEst,"
				+"SUM(trans.PayPlanAmount) PayPlanDue"
				+(hasDateLastPay?",MAX(CASE WHEN trans.TranType='PatPay' THEN trans.TranDate ELSE '0001-01-01' END) DateLastPay ":" ")
				+"FROM ("
					+GetTransQueryStringAgeByProc(asOfDate,familyPatNums,isWoAged,isHistoric,doAgePatPayPlanPayments,isAgedByProc,doExcludeIncomeTransfers)
				+") trans ";
			if(isGroupByGuar) {
				command+="INNER JOIN patient p ON p.PatNum=trans.PatNum "
					+"GROUP BY p.Guarantor";
				if(!isAllPats || !PrefC.GetBool(PrefName.AgingIsEnterprise)) {//only if for one fam or if not using famaging table
					command+=" ORDER BY NULL";//without this, the explain for this query lists 'using filesort' since there is a group by
				}
			}
			else {
				command+="GROUP BY trans.PatNum";
			}
			if(isWoAged || isAgedByProc) {
				command+=") transSums";
			}
			return command;
		}

		///<summary>Returns the transaction query string used in calculating aging.  string familyPatNums is usually a comma delimited list of PatNums for
		///a family, but can be a comma delimited list of patients from many families or null/empty.  Returns the query string used to select the trans
		///for calculating aging for the pats in the familyPatNums string.  If familyPatNums is null/empty the query string will be for all pats.</summary>
		public static string GetTransQueryStringAgeByProc(DateTime asOfDate,string familyPatNums,bool isWoAged=false,bool isHistoric=false,
			bool doAgePatPayPlanPayments=false,bool isAgedByProc=false, bool doExcludeIncomeTransfers=false)
		{
			if(!isAgedByProc) {
				return GetTransQueryString(asOfDate,familyPatNums,isWoAged,isHistoric,doAgePatPayPlanPayments,false,false,doExcludeIncomeTransfers);
			}
			string tranType=
				"(CASE "
					+"WHEN tranbyproc.AgedProcNum=0 THEN tranbyproc.TranType "
					+"ELSE 'SumByProcAndDate' "
				+"END)";
			string command="SELECT "+tranType+" TranType,0 PriKey,tranbyproc.PatNum,"
				+"(CASE "
					+"WHEN tranbyproc.AgedProcNum!=0 AND SUM(tranbyproc.TranAmount) < 0 THEN tranbyproc.AgedProcDate "
					+"ELSE tranbyproc.TranDate "
				+"END) TranDate,"
				+"SUM(tranbyproc.TranAmount) TranAmount,"
				+"SUM(tranbyproc.PayPlanAmount) PayPlanAmount,SUM(tranbyproc.InsWoEst) InsWoEst,SUM(tranbyproc.InsPayEst) InsPayEst "
				+"FROM ("
					+GetTransQueryString(asOfDate,familyPatNums,isWoAged,isHistoric,doAgePatPayPlanPayments,false,true,doExcludeIncomeTransfers)
				+") tranbyproc "
				+"GROUP BY tranbyproc.PatNum,tranbyproc.AgedProcNum,tranbyproc.TranDate,"+tranType+","
					+"(CASE "
						+"WHEN tranbyproc.AgedProcNum=0 AND tranbyproc.TranAmount >= 0 THEN 'credit' "
						+"WHEN tranbyproc.AgedProcNum=0 AND tranbyproc.TranAmount < 0 THEN 'charge' "
						+"ELSE 'SumByProcAndDate' "//When aging by proc, group flagged transactions together on the same day, regardless of credit/charge.
					+"END)";
			return command;
		}

		///<summary>Returns the transaction query string used in calculating aging.  string familyPatNums is usually a comma delimited list of PatNums for
		///a family, but can be a comma delimited list of patients from many families or null/empty.  Returns the query string used to select the trans
		///for calculating aging for the pats in the familyPatNums string.  If familyPatNums is null/empty the query string will be for all pats.
		///doIncludeProcNum is only used for Transworld and will cause the ProcNum and PayNum columns to be returned by this query.</summary>
		public static string GetTransQueryString(DateTime asOfDate,string familyPatNums,bool isWoAged=false,bool isHistoric=false,
			bool doAgePatPayPlanPayments=false,bool doIncludeProcNum=false,bool isAgedByProc=false, bool doExcludeIncomeTransfers=false)
		{
			//No need to check RemotingRole; no call to db.
			string billInAdvanceDate;
			if(isHistoric) {
				//This if statement never really does anything.  The only places that call this function with historic=true don't look at the
				//patient.payplandue amount, and patient aging gets reset after the reports are generated.  In the future if we start looking at payment plan
				//due amounts when historic=true we may need to revaluate this if statement.
				billInAdvanceDate=POut.Date(DateTime.Today.AddDays(PrefC.GetLong(PrefName.PayPlansBillInAdvanceDays)));
			}
			else {
				billInAdvanceDate=POut.Date(asOfDate.AddDays(PrefC.GetLong(PrefName.PayPlansBillInAdvanceDays)));
			}
			string asOfDateStr=POut.Date(asOfDate);
			PayPlanVersions payPlanVersionCur=(PayPlanVersions)PrefC.GetInt(PrefName.PayPlansVersion);
			bool isAllPats=string.IsNullOrWhiteSpace(familyPatNums);
			string command="";
			#region Completed Procs
			command+="SELECT 'Proc' TranType,pl.ProcNum PriKey,pl.PatNum,pl.ProcDate TranDate,pl.ProcFee*(pl.UnitQty+pl.BaseUnits) TranAmount,"
				+"0 PayPlanAmount,0 InsWoEst,0 InsPayEst"
				+(doIncludeProcNum?",pl.ProcNum,0 PayNum":"")
				+(isAgedByProc?",0 AgedProcNum,'0001-01-01' AgedProcDate":"")+" "
				+"FROM procedurelog pl "
				+"WHERE pl.ProcStatus=2 "
				+"AND pl.ProcFee != 0 "
				+(isAllPats?"":("AND pl.PatNum IN ("+familyPatNums+") "))
			#endregion Completed Procs
				+"UNION ALL "
			#region Insurance Payments and WriteOffs, PayPlan Ins Payments, and InsPayEst
			#region Regular Claimproc By DateCP
				+"SELECT 'Claimproc' TranType,cp.ClaimProcNum PriKey,cp.PatNum,cp.DateCP TranDate,"
				+"(CASE WHEN cp.Status != 0 THEN (CASE WHEN cp.PayPlanNum = 0 THEN -cp.InsPayAmt ELSE 0 END)"
					+(isWoAged?"":" - cp.WriteOff")+" ELSE 0 END) TranAmount,0 PayPlanAmount,"
				+(isWoAged?"0":("(CASE WHEN "+(isHistoric?("cp.ProcDate <= "+asOfDateStr+" "//historic=NotRcvd OR Rcvd and DateCp>asOfDate
					+"AND (cp.Status = 0 OR (cp.Status = 1 AND cp.DateCP > "+asOfDateStr+"))"):"cp.Status = 0")+" "//not historic=NotReceived
					+"THEN cp.WriteOff ELSE 0 END)"))+" InsWoEst,"//writeoff
				+"(CASE WHEN "+(isHistoric?("cp.ProcDate <= "+asOfDateStr+" "//historic=NotRcvd OR Rcvd and DateCp>asOfDate
					+"AND (cp.Status = 0 OR (cp.Status = 1 AND cp.DateCP > "+asOfDateStr+"))"):"cp.Status = 0")+" "//not historic=NotReceived
					+"THEN cp.InsPayEst ELSE 0 END) InsPayEst"//inspayest
				+(doIncludeProcNum?",cp.ProcNum,0 PayNum":"")
				+(isAgedByProc?",0 AgedProcNum,'0001-01-01' AgedProcDate":"")+" "
				+"FROM claimproc cp "
				+(payPlanVersionCur==PayPlanVersions.AgeCreditsAndDebits?"LEFT JOIN payplan pp ON cp.PayPlanNum=pp.PayPlanNum ":"")
				+"WHERE cp.Status IN (0,1,4,5,7) "//NotReceived,Received,Supplemental,CapClaim,CapComplete
				+(isAllPats?"":("AND cp.PatNum IN ("+familyPatNums+") "))
				//efficiency improvement for MySQL only.
				+(DataConnection.DBtype==DatabaseType.MySql?"HAVING TranAmount != 0 OR InsWoEst != 0 OR InsPayEst != 0 ":"");
			#endregion Regular Claimproc By DateCP
			#region Original and Current Writeoff/Delta
			//Only included if writeoffs are aged.  Requires joining to the claimsnapshot table.
			if(isWoAged) {
				command+="UNION ALL "
					//This union is for aging the snapshot w/o if one exists, otherwise the claimproc w/o, using ProcDate.
					+"SELECT 'WriteoffOrig' TranType,cp.ClaimProcNum PriKey,cp.PatNum,cp.ProcDate TranDate,"//use ProcDate
					+"COALESCE(CASE WHEN css.Writeoff = -1 THEN 0 ELSE -css.Writeoff END,"//Rcvd and NotRcvd, age snapshot w/o if snapshot exists and w/o != -1
						+"CASE WHEN cp.Status!=0 THEN -cp.WriteOff ELSE 0 END) TranAmount,"//if Rcvd and no snapshot exists, age claimproc w/o
					+"0 PayPlanAmount,"
					//Include in InsWoEst column either claimproc w/o if no snapshot or claimproc w/o - snapshot w/o (delta) if snapshot exists and either
						//1. not historic and NotRcvd or
						//2. historic and ProcDate<=asOfDate and either NotRcvd or Rcvd with DateCp>asOfDate (i.e. Rcvd after the asOfDate)
					+"(CASE WHEN "+(isHistoric?("cp.ProcDate <= "+asOfDateStr+" "//historic=ProcDate<=asOfDate and either NotRcvd OR Rcvd with DateCp>asOfDate
						+"AND (cp.Status = 0 OR (cp.Status = 1 AND cp.DateCP > "+asOfDateStr+")) "):"cp.Status = 0 ")//not historic=NotReceived
						+"THEN cp.Writeoff - COALESCE(CASE WHEN css.Writeoff=-1 THEN 0 ELSE css.Writeoff END,0) ELSE 0 END) InsWoEst,"
					+"0 InsPayEst"
					+(doIncludeProcNum?",cp.ProcNum,0 PayNum":"")
					+(isAgedByProc?",0 AgedProcNum,'0001-01-01' AgedProcDate":"")+" "
					+"FROM claimproc cp "
					+"LEFT JOIN claimsnapshot css ON cp.ClaimProcNum=css.ClaimProcNum "
					+"WHERE cp.Status IN (0,1,4,5,7) "//NotReceived,Received,Supplemental,CapClaim,CapComplete
					+(isAllPats?"":("AND cp.PatNum IN ("+familyPatNums+") "))
					+(DataConnection.DBtype==DatabaseType.MySql?"HAVING TranAmount != 0 OR InsWoEst != 0 ":"")//efficiency improvement for MySQL only.
					+"UNION ALL "
					//This union is for Rcvd claims with snapshots only and is the claimproc w/o's - snapshot w/o's (delta) using DateCp
					+"SELECT 'Writeoff' TranType,cp.ClaimProcNum PriKey,cp.PatNum,cp.DateCP TranDate,"//use DateCP
					//If Rcvd and snapshot exists, age claimproc w/o - snapshot w/o (delta)
					+"-(cp.Writeoff - (CASE WHEN css.Writeoff = -1 THEN 0 ELSE css.Writeoff END)) TranAmount,"
					+"0 PayPlanAmount,0 InsWoEst,0 InsPayEst"
					+(doIncludeProcNum?",cp.ProcNum,0 PayNum":"")
					+(isAgedByProc?",0 AgedProcNum,'0001-01-01' AgedProcDate":"")+" "
					+"FROM claimproc cp "
					+"INNER JOIN claimsnapshot css ON cp.ClaimProcNum=css.ClaimProcNum "
					+"WHERE cp.Status IN (1,4,5,7) "//Received,Supplemental,CapClaim,CapComplete
					+(isAllPats?"":("AND cp.PatNum IN ("+familyPatNums+") "))
					+(DataConnection.DBtype==DatabaseType.MySql?"HAVING TranAmount != 0 ":"");//efficiency improvement for MySQL only.
			}
			#endregion Original and Current Writeoff/Delta
			#endregion Insurance Payments and WriteOffs, PayPlan Ins Payments, and InsPayEst
			command+="UNION ALL "
			#region Adjustments
				+"SELECT 'Adj' TranType,a.AdjNum PriKey,a.PatNum,a.AdjDate TranDate,a.AdjAmt TranAmount,0 PayPlanAmount,0 InsWoEst,0 InsPayEst"
				+(doIncludeProcNum?",a.ProcNum,0 PayNum":"")
				+(isAgedByProc?",a.ProcNum AgedProcNum,a.ProcDate AgedProcDate":"")+" "
				+"FROM adjustment a "
				+"WHERE a.AdjAmt != 0 "
				+(isAllPats?"":("AND a.PatNum IN ("+familyPatNums+") "))
			#endregion Adjustments
				+"UNION ALL "
			#region Paysplits and PayPlan Paysplits
				+"SELECT 'PatPay' TranType,ps.SplitNum PriKey,ps.PatNum,ps.DatePay TranDate,";
			//v1 and v3: splits not attached to payment plans, v2 or doAgePatPayPlanPayments: all splits for pat/fam
			if(ListTools.In(payPlanVersionCur,PayPlanVersions.DoNotAge,PayPlanVersions.AgeCreditsOnly) && !doAgePatPayPlanPayments) {
				command+="(CASE WHEN ps.PayPlanNum=0 THEN -ps.SplitAmt ELSE 0 END) ";
			}
			else {
				command+="-ps.SplitAmt ";
			}
			List<long> listHiddenUnearnedDefNums=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue))
					.Select(x => x.DefNum).ToList();
			command+="TranAmount,"
				//We cannot exclude payments made outside the specified family, since payment plan guarantors can be in another family.
				+"(CASE WHEN ps.PayPlanNum != 0 "
					+(payPlanVersionCur==PayPlanVersions.AgeCreditsAndDebits?"AND COALESCE(pp.IsClosed,0)=0 ":"") //ignore closed payment plans on v2
					+"THEN -ps.SplitAmt ELSE 0 END) PayPlanAmount,"//Paysplits attached to payment plans
				+"0 InsWoEst,0 InsPayEst"
				+(doIncludeProcNum?",ps.ProcNum,ps.PayNum":"")
				+(isAgedByProc?",0 AgedProcNum,'0001-01-01' AgedProcDate":"")+" "
				+"FROM paysplit ps ";
			if(doExcludeIncomeTransfers) {
				command+="INNER JOIN payment ON payment.PayNum=ps.PayNum ";
			}
			command+=(payPlanVersionCur==PayPlanVersions.AgeCreditsAndDebits?"LEFT JOIN payplan pp ON ps.PayPlanNum=pp.PayPlanNum ":"")
				+"WHERE ps.SplitAmt != 0 ";
			if(doExcludeIncomeTransfers) {
				command+="AND ("
						+"payment.PayType>0 "//not income transfer
						+"OR ps.PayNum IN ( "
							+"SELECT payment.PayNum "
							+"FROM payment "
							+"INNER JOIN paysplit ON payment.PayNum=paysplit.PayNum "
							+"INNER JOIN patient ON patient.PatNum=paysplit.PatNum "
							+"WHERE payment.PayType=0 "//is income transfer
							+"GROUP BY payment.PayNum "
							+"HAVING COUNT(DISTINCT patient.Guarantor)>1"//splits to more than one family
						+")"
					+") ";
			}
			if(listHiddenUnearnedDefNums.Count > 0) {
				command+="AND ps.UnearnedType NOT IN ("+string.Join(",",listHiddenUnearnedDefNums)+") ";
			}
			command+=(isHistoric?("AND ps.DatePay <= "+asOfDateStr+" "):"")
				+(isAllPats?"":("AND ps.PatNum IN ("+familyPatNums+") "))
			#endregion Paysplits and PayPlan Paysplits
				+"UNION ALL "
			#region PayPlan Charges
				//Calculate the payment plan charges for each payment plan guarantor on or before date considering the PayPlansBillInAdvanceDays setting.
				//Ignore pay plan charges for a different family, since payment plan guarantors might be in another family.
				+"SELECT 'PPCharge' TranType,ppc.PayPlanChargeNum PriKey,ppc.Guarantor PatNum,ppc.ChargeDate TranDate,0 TranAmount,"
				+"ppc.Principal+ppc.Interest PayPlanAmount,0 InsWoEst,0 InsPayEst"
				+(doIncludeProcNum?",0 ProcNum,0 PayNum":"")
				+(isAgedByProc?",0 AgedProcNum,'0001-01-01' AgedProcDate":"")+" "
				+"FROM payplancharge ppc "
				+"INNER JOIN payplan pp ON ppc.PayPlanNum=pp.PayPlanNum "
				+"WHERE ppc.ChargeDate <= "+billInAdvanceDate+" "//accounts for historic vs current because of how it's set above
				+"AND ppc.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "
				+(payPlanVersionCur==PayPlanVersions.AgeCreditsAndDebits?"AND !pp.IsClosed ":"")
				+(isAllPats?"":("AND ppc.Guarantor IN ("+familyPatNums+") "))
				+"AND ppc.Principal+ppc.Interest != 0 "
				+"AND pp.PlanNum=0 ";//exclude payplans used to track insurance payments
			#endregion PayPlan Charges
			#region PayPlan Principal and Interest/CompletedAmt
			#region PayPlan Version 1
			if(payPlanVersionCur==PayPlanVersions.DoNotAge && !doAgePatPayPlanPayments) {
				//v1 and NOT doAgePatPayPlanPayments: aging the entire payment plan, not the payPlanCharges
				//if aging patient payplan payments, don't age the CompletedAmt or it will duplicate the credits aged
				command+="UNION ALL "
					+"SELECT 'PPComplete' TranType,pp.PayPlanNum PriKey,pp.PatNum,pp.PayPlanDate TranDate,-pp.CompletedAmt TranAmount,"
					+"0 PayPlanAmount,0 InsWoEst,0 InsPayEst"
					+(doIncludeProcNum?",0 ProcNum,0 PayNum":"")
					+(isAgedByProc?",0 AgedProcNum,'0001-01-01' AgedProcDate":"")+" "
					+"FROM payplan pp "
					+"WHERE pp.CompletedAmt != 0 "
					+(isAllPats?"":("AND pp.PatNum IN ("+familyPatNums+") "));
			}
			#endregion PayPlan Version 1
			#region PayPlan Version 2
			else if(payPlanVersionCur==PayPlanVersions.AgeCreditsAndDebits) {//v2, we should be looking for payplancharges and aging those as patient debits/credits accordingly
				//For credits, use the patient on the payment plan (because they need to have their account balance reduced).
				//For debits, use the guarantor on the payplancharge (because that is the person that needs to be paying on the payment plan).
				command+="UNION ALL "
					+"SELECT 'PPCComplete' TranType,ppc.PayPlanChargeNum PriKey,"
					+"(CASE WHEN ppc.ChargeType = "+POut.Int((int)PayPlanChargeType.Debit)+" THEN ppc.Guarantor ELSE pp.PatNum END) PatNum,"
					+"ppc.ChargeDate TranDate,"
					+"(CASE WHEN ppc.ChargeType != "+POut.Int((int)PayPlanChargeType.Debit)+" THEN -ppc.Principal "
						+"WHEN pp.PlanNum=0 THEN ppc.Principal+ppc.Interest ELSE 0 END) TranAmount,0 PayPlanAmount,0 InsWoEst,0 InsPayEst"
					+(doIncludeProcNum?",0 ProcNum,0 PayNum":"")
					+(isAgedByProc?",(CASE WHEN ppc.ChargeType="+POut.Int((int)PayPlanChargeType.Credit)+" THEN ppc.ProcNum ELSE 0 END) AgedProcNum":"")
					+(isAgedByProc?",pl.ProcDate AgedProcDate":"")+" "
					+"FROM payplancharge ppc "
					+"LEFT JOIN payplan pp ON pp.PayPlanNum=ppc.PayPlanNum "
					+(isAgedByProc?"LEFT JOIN procedurelog pl ON pl.ProcNum=ppc.ProcNum ":"")
					+"WHERE ppc.ChargeDate <= "+asOfDateStr+" "
					+(isAllPats?"":("AND (CASE WHEN ppc.ChargeType = "+POut.Int((int)PayPlanChargeType.Debit)+" THEN ppc.Guarantor ELSE ppc.PatNum end) IN ("+familyPatNums+") "));
			}
			#endregion PayPlan Version 2
			#region PayPlan Version 3
			else if(payPlanVersionCur==PayPlanVersions.AgeCreditsOnly) {//v3, we should only be aging payplancharge credits
				//Use the patient on the payplan because that patient needs to have their account balance reduced.
				command+="UNION ALL "
					+"SELECT 'PPCComplete' TranType,ppc.PayPlanChargeNum PriKey,ppc.PatNum,ppc.ChargeDate TranDate,"
					+"-ppc.Principal TranAmount,0 PayPlanAmount,0 InsWoEst,0 InsPayEst"
					+(doIncludeProcNum?",0 ProcNum,0 PayNum":"")
					+(isAgedByProc?",ppc.ProcNum AgedProcNum,pl.ProcDate AgedProcDate":"")+" "
					+"FROM payplancharge ppc "
					+"LEFT JOIN payplan pp ON pp.PayPlanNum=ppc.PayPlanNum "
					+(isAgedByProc?"LEFT JOIN procedurelog pl ON pl.ProcNum=ppc.ProcNum ":"")
					+"WHERE ppc.ChargeDate <= "+asOfDateStr+" "
					+"AND ppc.ChargeType = "+POut.Int((int)PayPlanChargeType.Credit)+" "
					+(isAllPats?"":("AND ppc.PatNum IN ("+familyPatNums+") "));
			}
			#endregion PayPlan Version 3
			#region Payment Plans Version 4 - No Charges
			else if(payPlanVersionCur==PayPlanVersions.NoCharges) {
				//For No Charges payment plans, payment plan charges DO NOT affect account balances.  This is intentional.
			}
			#endregion
			#region PayPlanLinks (Dynamic Payment Plan) for Versions 2 & 3
			if(ListTools.In(payPlanVersionCur,PayPlanVersions.AgeCreditsAndDebits,PayPlanVersions.AgeCreditsOnly)) {
				command+=@"UNION ALL
					SELECT 'PayPlanLink' TranType,ppl.PayPlanLinkNum PriKey,prodlink.PatNum PatNum,DATE(ppl.SecDateTEntry) TranDate,
					(CASE WHEN ppl.AmountOverride=0 THEN -prodlink.Fee ELSE -ppl.AmountOverride END) TranAmount,0 PayPlanAmount,0 InsWoEst,0 InsPayEst";
				if(doIncludeProcNum) {
					command+=",prodlink.ProcNum ProcNum,0 PayNum ";
				}
				if(isAgedByProc) {
					command+=",prodlink.ProcNum AgedProcNum,prodlink.AgeDate AgedProcDate ";
				}
				command+=$@"
					FROM payplanlink ppl
					LEFT JOIN (
						SELECT procedurelog.PatNum,(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)
								+COALESCE(procAdj.AdjAmt,0)+COALESCE(procClaimProc.InsPay,0)
								+COALESCE(procClaimProc.WriteOff,0)
								+COALESCE(procSplit.SplitAmt,0)
							) Fee,
							payplanlink.PayPlanLinkNum LinkNum,procedurelog.ProcNum,procedurelog.ProcDate AgeDate 
						FROM payplanlink
						INNER JOIN payplan ON payplanlink.PayPlanNum=payplan.PayPlanNum
						INNER JOIN procedurelog ON procedurelog.ProcNum=payplanlink.FKey
							AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Procedure)}
							AND !(payplan.dynamicPayPlanTPOption={POut.Int((int)DynamicPayPlanTPOptions.AwaitComplete)} AND procedurelog.ProcStatus!={POut.Int((int)ProcStat.C)})
						LEFT JOIN (
							SELECT SUM(adjustment.AdjAmt) AdjAmt,adjustment.ProcNum,adjustment.PatNum,adjustment.ProvNum,adjustment.ClinicNum
							FROM adjustment ";
							command+=isAllPats ? "" : $"WHERE adjustment.PatNum IN ({familyPatNums}) ";
							command+=$@"GROUP BY adjustment.ProcNum,adjustment.PatNum,adjustment.ProvNum,adjustment.ClinicNum
						)procAdj ON procAdj.ProcNum=procedurelog.ProcNum
							AND procAdj.PatNum=procedurelog.PatNum
							AND procAdj.ProvNum=procedurelog.ProvNum
							AND procAdj.ClinicNum=procedurelog.ClinicNum
						LEFT JOIN (
							SELECT SUM(COALESCE((CASE WHEN claimproc.Status IN (
								{POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapComplete)}
							) THEN claimproc.InsPayAmt 
								WHEN claimproc.InsEstTotalOverride!=-1 THEN claimproc.InsEstTotalOverride ELSE claimproc.InsPayEst END),0)*-1) InsPay
							,SUM(COALESCE((CASE WHEN claimproc.Status IN (
								{POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapComplete)}
							)	THEN claimproc.WriteOff 
								WHEN claimproc.WriteOffEstOverride!=-1 THEN claimproc.WriteOffEstOverride 
								WHEN claimproc.WriteOffEst!=-1 THEN claimproc.WriteOffEst ELSE 0 END),0)*-1) WriteOff
							,claimproc.ProcNum
							FROM claimproc 
							WHERE claimproc.Status!={POut.Int((int)ClaimProcStatus.Preauth)} ";
							command+=isAllPats?"":$"AND claimproc.PatNum IN ({familyPatNums}) ";
							command+=$@"GROUP BY claimproc.ProcNum
						)procClaimProc ON procClaimProc.ProcNum=procedurelog.ProcNum 
						LEFT JOIN (
							SELECT SUM(paysplit.SplitAmt)*-1 SplitAmt,paysplit.ProcNum
							FROM paysplit
							WHERE paysplit.PayPlanNum=0 ";
							command+=isAllPats?"":$"AND paysplit.PatNum IN ({familyPatNums}) ";
							command+=$@"GROUP BY paysplit.ProcNum
						)procSplit ON procSplit.ProcNum=procedurelog.ProcNum
					UNION ALL 
						SELECT adjustment.PatNum,adjustment.AdjAmt + COALESCE(adjSplit.SplitAmt,0) Fee,payplanlink.PayPlanLinkNum LinkNum
							,0 ProcNum,DATE('0001-01-01') AgeDate 
						FROM payplanlink 
						INNER JOIN adjustment ON adjustment.AdjNum=payplanlink.FKey 
							AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Adjustment)} 
						LEFT JOIN (
								SELECT SUM(COALESCE(paysplit.SplitAmt,0))*-1 SplitAmt,paysplit.AdjNum
								FROM paysplit
								WHERE paysplit.PayPlanNum=0 ";
								command+=isAllPats?"":$"AND paysplit.PatNum IN ({familyPatNums}) ";
								command+=$@"GROUP BY paysplit.AdjNum
						)adjSplit ON adjSplit.AdjNum=adjustment.AdjNum
					) prodlink ON prodlink.LinkNum=ppl.PayPlanLinkNum ";
				command+=isAllPats?"":$"WHERE prodlink.PatNum IN ({familyPatNums}) AND prodlink.AgeDate <= {asOfDateStr} ";
			}
			#endregion
			#endregion PayPlan Principal and Interest/CompletedAmt
			#region Get All Family PatNums
			if(!isAllPats) {
				//get all family PatNums in case there are no transactions for the family in order to clear out the family balance
				command+="UNION ALL "
					+"SELECT 'FamPatNums' TranType,PatNum PriKey,PatNum,'0001-01-01' TranDate,0 TranAmount,0 PayPlanAmount,0 InsWoEst,0 InsPayEst"
					+(doIncludeProcNum?",0 ProcNum,0 PayNum":"")
					+(isAgedByProc?",0 AgedProcNum,'0001-01-01' AgedProcDate":"")+" "
					+"FROM patient "
					+"WHERE PatNum IN ("+familyPatNums+")";
			}
			#endregion Get All Family PatNums
			return command;
		}

		///<summary>Returns dictionary with key=Guarantor, value=dictionary with key=Tuple&lt;TsiFKeyType,FKey>, value=TsiTrans.  This links a guarantor
		///to all transactions that comprise the family's aging, grouped by guarantor and transaction type (i.e. procedures, paysplits, etc.).
		///Called by the OpenDentalService.TransworldThread sync code.</summary>
		public static Dictionary<long,Dictionary<Tuple<TsiFKeyType,long>,TsiTrans>> GetDictTransForGuars(List<long> listGuarNums) {
			//No need to check RemotingRole; no call to db.
			return GetTransForGuars(listGuarNums).GroupBy(x => x.Guarantor)
				.ToDictionary(x => x.Key,x => x.ToDictionary(y => Tuple.Create(y.KeyType,y.PriKey)));
		}

		public static List<TsiTrans> GetTransForGuars(List<long> listGuarNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TsiTrans>>(MethodBase.GetCurrentMethod(),listGuarNums);
			}
			string familyPatNums="";
			string command="";
			if(listGuarNums!=null && listGuarNums.Count>0) {
				command="SELECT p.PatNum FROM patient p WHERE p.Guarantor IN ("+string.Join(",",listGuarNums)+")";
				familyPatNums=string.Join(",",Db.GetListLong(command));//will contain at least one patnum (the guarantor)
			}
			command="SELECT patient.Guarantor,trans.TranType,trans.PriKey,trans.ProcNum,trans.PayNum,trans.PatNum,trans.TranDate,trans.TranAmount,"
				+"trans.InsWoEst+trans.InsPayEst InsEst "
				+"FROM ("+GetTransQueryString(DateTime.Today,familyPatNums,doIncludeProcNum:true)+") trans "
				+"INNER JOIN patient ON patient.PatNum=trans.PatNum";
			Dictionary<string,TsiFKeyType> dictTranTypes=new Dictionary<string,TsiFKeyType>() {
				{ "Proc",TsiFKeyType.Procedure },
				{ "Claimproc",TsiFKeyType.Claimproc },//will most likely be neg
				{ "Adj",TsiFKeyType.Adjustment },//could be pos or neg
				{ "PatPay",TsiFKeyType.PaySplit },//will be neg, v1 and v3: tranamount=splits not attached to payplans,v2: tranamount=all splits
				{ "PPComplete",TsiFKeyType.PayPlan },//v1: tranamount=-pp.CompletedAmt
				{ "PPCComplete",TsiFKeyType.PayPlanCharge }//if (v2 & debit) OR (v3 & credit) then tranamount=-ppc.Principal, else if v2 & plannum==0 then tranamount=ppc.Principal+ppc.Interest
			};
			return Db.GetTable(command).Select()
				.Where(x => dictTranTypes.ContainsKey(x["TranType"].ToString()))
				.Select(x => new TsiTrans(
					PIn.Long(x["PriKey"].ToString()),
					dictTranTypes[x["TranType"].ToString()],
					PIn.Long(x["ProcNum"].ToString()),
					PIn.Long(x["PayNum"].ToString()),
					PIn.Long(x["PatNum"].ToString()),
					PIn.Long(x["Guarantor"].ToString()),
					PIn.Date(x["TranDate"].ToString()),
					PIn.Double(x["TranAmount"].ToString())-PIn.Double(x["InsEst"].ToString())//have to subtract InsEst so that balance due will match the PatAging.AmountDue
				)).ToList();
		}

		///<summary>Gets the earliest date of any portion of the current balance for all guarantors in the database.  Used for A/R Manager (Transworld).
		///Returns SerializableDictionary&lt;long,DateTime> with Key=GuarNum, Value=earliest date of balance or MinValue if no balance.</summary>
		public static SerializableDictionary<long,DateTime> GetDateBalanceBegan(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,DateTime>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			List<long> listClinNums=null;
			if(PrefC.HasClinicsEnabled) {
				listClinNums=new List<long>() { clinicNum };
			}
			return GetGuarDateBals(DateTime.Today,listClinicNums:listClinNums)//Create a dictionary that tells a story about the transactions and their dates for each family.
				//find the earliest trans that uses up the account credits and is therefore the trans date for which the account balance is "first" positive
				.ToSerializableDictionary(x => x.Key,x => x.Value.Where(y => y.Bal>0.005).Select(y => y.TranDate).DefaultIfEmpty(DateTime.MinValue).Min());
		}
		
		///<summary>Returns a data table with columns: PatNum, DateAccountAge, DateZeroBal, and ClinicNum.  If listGuarantors is empty or null, gets for
		///all guars with a positive balance.  DateAcountAge=the earliest date of any portion of the current balance for the family.  DateZeroBal=the
		///first trans date after the most recent date the balance for the family was 0 or less.  Both dates will be DateTime.MinValue if the family has no
		///balance.  PatNum and ClinicNum are for the guarantor of the family or superhead.</summary>
		public static DataTable GetDateBalanceBegan(List<PatAging> listGuarantors,DateTime dateAsOf,bool isSuperBills,List<long> listClinicNums=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listGuarantors,dateAsOf,isSuperBills,listClinicNums);
			}
			DataTable retval=new DataTable();
			retval.Columns.Add("PatNum");
			retval.Columns.Add("DateAccountAge");
			retval.Columns.Add("DateZeroBal");
			retval.Columns.Add("ClinicNum");
			bool isAllPats=false;
			if(listGuarantors.IsNullOrEmpty()) {
				listGuarantors=Patients.GetAgingListSimple(null,null,isGuarsOnly:true,listClinicNums:listClinicNums,doIncludeSuperFamilyHeads:isSuperBills);
				isAllPats=true;//isAllPats==true means we don't set patNumStr and the table retrieved will include all pats (grouped by guar, so actually all guars)
			}
			//key=SuperFamily (PatNum); value=list of PatNums for the guars of each fam on a superbill, or if not a superbill, a list containing the GuarNum
			Dictionary<long,List<long>> dictSuperFamGNums=new Dictionary<long,List<long>>();
			foreach(PatAging patAgeCur in listGuarantors) {
				//if making superBills and this guarantor has super billing and is also the superhead for the super family,
				//fill dict with all guarnums and add all family members for all guars included in the superbill to the all patnums list
				if(isSuperBills && patAgeCur.HasSuperBilling && patAgeCur.SuperFamily==patAgeCur.PatNum) {
					dictSuperFamGNums[patAgeCur.SuperFamily]=Patients.GetSuperFamilyGuarantors(patAgeCur.SuperFamily)
						.FindAll(x => x.HasSuperBilling)
						.Select(x => x.PatNum).ToList();
				}
				else {//not a superBill, just add all family members for this guarantor
					dictSuperFamGNums[patAgeCur.PatNum]=new List<long>() { patAgeCur.PatNum };
				}
			}
			string patNumStr="";
			if(!isAllPats) {
				//list of all family member PatNums for each guarantor/superhead for all statements being generated
				List<long> listPatNumsAll=Patients.GetAllFamilyPatNums(dictSuperFamGNums.SelectMany(x => x.Value).ToList());
				if(listPatNumsAll.Count<1) {//should never happen
					return retval;
				}
				patNumStr=string.Join(",",listPatNumsAll);
			}
			//Create a dictionary that tells a story about the transactions and their dates for each family.
			Dictionary<long,List<GuarDateBals>> dictGuarDateBals=GetGuarDateBals(dateAsOf,patNumStr,listClinicNums);
			if(dictGuarDateBals.Count<1) {
				return retval;
			}
			DataRow row;
			List<DateTime> listDateBals;
			List<DateTime> listDateZeroBals;
			List<long> listGuarNums;
			//find the earliest trans that uses up the account credits and is therefore the trans date for which the account balance is "first" positive
			foreach(PatAging patAgeCur in listGuarantors) {
				if(isSuperBills && patAgeCur.HasSuperBilling && patAgeCur.PatNum!=patAgeCur.SuperFamily) {
					continue;
				}
				if(!isSuperBills || !patAgeCur.HasSuperBilling) {
					listGuarNums=new List<long>() { patAgeCur.PatNum };
				}
				else {//must be superbill and this is the superhead
					if(!dictSuperFamGNums.ContainsKey(patAgeCur.PatNum)) {
						continue;//should never happen
					}
					listGuarNums=dictSuperFamGNums[patAgeCur.PatNum];
				}
				listDateBals=new List<DateTime>();
				listDateZeroBals=new List<DateTime>();
				foreach(long guarNum in listGuarNums) {
					if(!dictGuarDateBals.ContainsKey(guarNum)) {//should never happen
						continue;
					}
					listDateBals.Add(dictGuarDateBals[guarNum].Where(x => x.Bal>0.005).Select(x => x.TranDate).DefaultIfEmpty(DateTime.MinValue).Min());
					//list of guars, or if not a super statement a list of one guar, and the date of the trans that used up the last of the acct credits
					listDateZeroBals.Add(dictGuarDateBals[guarNum]
						//Get dates greater than the most recent date the balance was 0
						.Where(x => x.TranDate>dictGuarDateBals[guarNum]
							.Where(y => y.BalZero<=0.005)//Per job 10783, we now want to include negative balances for consideration
							.Select(y => y.TranDate).DefaultIfEmpty(DateTime.MinValue).Max())
						//get the earliest date that was after the most recent 0 bal date. Defaults to DateTime.MinValue if patient has a zero balance.
						.Select(x => x.TranDate).DefaultIfEmpty(DateTime.MinValue).Min());
				}
				row=retval.NewRow();
				row["PatNum"]=POut.Long(patAgeCur.PatNum);
				row["ClinicNum"]=POut.Long(patAgeCur.ClinicNum);
				//set to the oldest balance date for all guarantors on this superbill, or if not a super bill, the oldest balance date for this guarantor
				//could be DateTime.MinValue if their credits pay for all of their charges
				row["DateAccountAge"]=listDateBals.Where(x => x>DateTime.MinValue).DefaultIfEmpty(DateTime.MinValue).Min();
				row["DateZeroBal"]=listDateZeroBals.Where(x => x>DateTime.MinValue).DefaultIfEmpty(DateTime.MinValue).Min();
				retval.Rows.Add(row);
			}
			return retval;
		}

		///<summary>Private method only called from within this file and behind a remoting role check, so this method doesn't need to have a remoting role
		///check.  Returns a dictionary of key=PatNum (guarNum), value=GuarDateBals object with TranDate, Bal, and BalZero fields.  List of clinic nums
		///filters by guar's clinic if provided, otherwise all guars.</summary>
		private static Dictionary<long,List<GuarDateBals>> GetGuarDateBals(DateTime dateAsOf,string patNumStr=null,List<long> listClinicNums=null) {
			//No remoting role check required, private method called from behind remoting role check already
			bool isHistoric=(dateAsOf.Date!=DateTime.Today);
			//Create dictionary that tells a story about the transactions and their dates for each family.
			Dictionary<long,List<GuarDateBals>> retval=new Dictionary<long,List<GuarDateBals>>();
			string clinicNumsJoin="";
			if(!listClinicNums.IsNullOrEmpty()) {
				clinicNumsJoin=$@"INNER JOIN patient guar ON patient.Guarantor=guar.PatNum AND guar.ClinicNum IN ({string.Join(",",listClinicNums)}) ";
			}
			string command=$@"SELECT patient.Guarantor PatNum,RawPatTrans.TranDate,
				SUM(CASE WHEN RawPatTrans.TranAmount>0 THEN RawPatTrans.TranAmount ELSE 0 END) AS ChargeForDate,
				SUM(CASE WHEN RawPatTrans.TranAmount<0 THEN RawPatTrans.TranAmount ELSE 0 END) AS PayForDate
				FROM ({GetTransQueryString(dateAsOf,patNumStr,isHistoric:isHistoric)}) RawPatTrans
				INNER JOIN patient ON patient.PatNum=RawPatTrans.PatNum
				{clinicNumsJoin}
				GROUP BY patient.Guarantor,RawPatTrans.TranDate
				ORDER BY patient.Guarantor,RawPatTrans.TranDate";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count<1) {
				return retval;
			}
			Dictionary<long,double> dictGuarCreditTotals=table.Select()
				.GroupBy(x => PIn.Long(x["PatNum"].ToString()),x => PIn.Double(x["PayForDate"].ToString()))
				.ToDictionary(x => x.Key,y => y.Sum());
			double runningChargesToDate=0;
			double runningCreditsToDate=0;
			long guarNumCur;
			foreach(DataRow rowCur in table.Rows) {
				guarNumCur=PIn.Long(rowCur["PatNum"].ToString());
				if(!retval.ContainsKey(guarNumCur)) {
					retval[guarNumCur]=new List<GuarDateBals>();
					runningChargesToDate=0;
					runningCreditsToDate=0;
				}
				runningChargesToDate+=PIn.Double(rowCur["ChargeForDate"].ToString());
				runningCreditsToDate+=PIn.Double(rowCur["PayForDate"].ToString());
				retval[guarNumCur].Add(new GuarDateBals() {
					TranDate=PIn.Date(rowCur["TranDate"].ToString()),
					Bal=runningChargesToDate+dictGuarCreditTotals[guarNumCur],
					BalZero=runningChargesToDate+runningCreditsToDate
				});
			}
			return retval;
		}

		///<summary>Exception handler for the aging threads. Handles all types of exceptions. Returns true if the exception was a deadlock
		///exception.</summary>
		public static void AgingExceptionHandler(Exception ex,Form currentForm,bool showDeadlockMessage=true) {
			MySqlException mySqlEx=null;
			ODException.SwallowAnyException(() => { mySqlEx=(MySqlException)ex; });
			if(mySqlEx==null || mySqlEx.Number!=1213) {//not a deadlock error, just throw
				throw ex;
			}
			currentForm.InvokeIfRequired(() => {
				currentForm.Cursor=Cursors.Default;
				if(showDeadlockMessage) {
					string msg=Lans.g("Ledgers","Deadlock error detected in aging transaction and rolled back. Try again later.");
					MessageBox.Show(currentForm,msg);
				}
			});
		}

		///<summary>Only used within this file and only behind remoting role checks.  Not serialized or publicly available.</summary>
		private struct GuarDateBals {
			public DateTime TranDate;
			public double Bal;
			public double BalZero;
		}
	}

}