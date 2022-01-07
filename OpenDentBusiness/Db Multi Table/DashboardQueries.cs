using System;
using System.Collections.Generic;
//using System.Windows.Controls;//need a reference for this dll, or get msgbox into UI layer.
using System.Data;
using System.Text;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness {
	public class DashboardQueries {
#if DEBUG
		///<summary>Set this boolean to true if you want to have message boxes pop up after each method is run when in debug mode.  Used to time long computations before loading the dashboard.</summary>
		private static bool _showElapsedTimesForDebug=false;
		private static string _elapsedTimeProdInc="";
		private static string _elapsedTimeProvList="";
		private static string _elapsedTimeProdProvs="";
		private static string _elapsedTimeAR="";
		private static string _elapsedTimeNewPatients="";
#endif

		public static DataTable GetProvList(DateTime dt) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dt);
			}
			System.Diagnostics.Stopwatch stopWatch=new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch stopWatchTotal=new System.Diagnostics.Stopwatch();
			#if DEBUG
				_elapsedTimeProvList="";
				_elapsedTimeProvList="Elapsed time for GetProvList:\r\n";
				stopWatch.Restart();
				stopWatchTotal.Restart();
			#endif
			Random rnd=new Random();
			string rndStr=rnd.Next(1000000).ToString();
			string command;
			command="DROP TABLE IF EXISTS tempdash"+rndStr+@";";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="DROP TABLE: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			command=@"CREATE TABLE tempdash"+rndStr+@" (
				ProvNum bigint NOT NULL PRIMARY KEY,
				production decimal NOT NULL,
				income decimal NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="CREATE TABLE: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//providers
			command=@"INSERT INTO tempdash"+rndStr+@" (ProvNum)
				SELECT ProvNum
				FROM provider WHERE IsHidden=0
				ORDER BY ItemOrder";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="providers: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//production--------------------------------------------------------------------
			//procs
			command=@"UPDATE tempdash"+rndStr+@" 
				SET production=(SELECT SUM(ProcFee*(UnitQty+BaseUnits)) FROM procedurelog 
				WHERE procedurelog.ProvNum=tempdash"+rndStr+@".ProvNum
				AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+@"
				AND ProcDate="+POut.Date(dt)+")";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="production - procs: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//capcomplete writeoffs were skipped
			//adjustments
			command=@"UPDATE tempdash"+rndStr+@" 
				SET production=production+(SELECT IFNULL(SUM(AdjAmt),0) FROM adjustment 
				WHERE adjustment.ProvNum=tempdash"+rndStr+@".ProvNum
				AND AdjDate="+POut.Date(dt)+")";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="production - adjustments: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//insurance writeoffs
			switch((PPOWriteoffDateCalc)PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate)) {//use procdate
				case PPOWriteoffDateCalc.ProcDate:
					command=@"UPDATE tempdash"+rndStr+@" 
					SET production=production-(SELECT IFNULL(SUM(WriteOff),0) FROM claimproc 
					WHERE claimproc.ProvNum=tempdash"+rndStr+@".ProvNum
					AND ProcDate="+POut.Date(dt)+@" 
					AND (claimproc.Status=1 OR claimproc.Status=4 OR claimproc.Status=0) )";//received or supplemental or notreceived
					break;
				case PPOWriteoffDateCalc.InsPayDate:
					command=@"UPDATE tempdash"+rndStr+@" 
					SET production=production-(SELECT IFNULL(SUM(WriteOff),0) FROM claimproc 
					WHERE claimproc.ProvNum=tempdash"+rndStr+@".ProvNum
					AND DateCP="+POut.Date(dt)+@" 
					AND (claimproc.Status=1 OR claimproc.Status=4) )";//received or supplemental 
					break;
				case PPOWriteoffDateCalc.ClaimPayDate:
					command=@"UPDATE tempdash"+rndStr+@" 
					SET production=production-(SELECT IFNULL(SUM(claimsnapshot.WriteOff),0) FROM claimproc cp
					INNER JOIN claimsnapshot ON cp.ClaimProcNum=claimsnapshot.ClaimProcNum
					WHERE claimproc.ProvNum=tempdash"+rndStr+@".ProvNum
					AND claimsnapshot.DateTEntry="+POut.Date(dt)+@" 
					AND (claimproc.Status=1 OR claimproc.Status=4) )";//received or supplemental 
					break;
			}
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="production - writeoffs: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//income------------------------------------------------------------------------
			//patient income
			command=@"UPDATE tempdash"+rndStr+@" 
				SET income=(SELECT SUM(SplitAmt) FROM paysplit 
				WHERE paysplit.ProvNum=tempdash"+rndStr+@".ProvNum
				AND DatePay="+POut.Date(dt)+")";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="income - patient: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//ins income
			command=@"UPDATE tempdash"+rndStr+@" 
				SET income=income+(SELECT IFNULL(SUM(InsPayAmt),0) FROM claimproc 
				WHERE claimproc.ProvNum=tempdash"+rndStr+@".ProvNum
				AND DateCP="+POut.Date(dt)+")";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="income - insurance: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//final queries
			command="SELECT * FROM tempdash"+rndStr+@"";
			DataTable table=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProvList+="SELECT * : "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			command="DROP TABLE IF EXISTS tempdash"+rndStr+@";";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				stopWatchTotal.Stop();
				_elapsedTimeProvList+="DROP TABLE: "+stopWatch.Elapsed.ToString()+"\r\n";
				_elapsedTimeProvList+="Total: "+stopWatchTotal.Elapsed.ToString();
				if(_showElapsedTimesForDebug) {
					System.Windows.Forms.MessageBox.Show(_elapsedTimeProvList);
				}
			#endif
			return table;
		}

		public static List<System.Windows.Media.Color> GetProdProvColors() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<System.Windows.Media.Color>>(MethodBase.GetCurrentMethod());
			}
			string command=@"SELECT ProvColor
				FROM provider WHERE IsHidden=0
				ORDER BY ItemOrder";
			DataTable table=Db.GetTable(command);
			List<System.Windows.Media.Color> retVal=new List<System.Windows.Media.Color>();
			for(int i=0;i<table.Rows.Count;i++){
				System.Drawing.Color dColor=System.Drawing.Color.FromArgb(PIn.Int(table.Rows[i]["ProvColor"].ToString()));
				System.Windows.Media.Color mColor=System.Windows.Media.Color.FromArgb(dColor.A,dColor.R,dColor.G,dColor.B);
				retVal.Add(mColor);
			}
			return retVal;
		}

		public static List<List<int>> GetProdProvs(DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<List<int>>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			Random rnd=new Random();
			string rndStr=rnd.Next(1000000).ToString();
			string command;
			System.Diagnostics.Stopwatch stopWatch=new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch stopWatchTotal=new System.Diagnostics.Stopwatch();
			#if DEBUG
				_elapsedTimeProdProvs="";
				_elapsedTimeProdProvs="Elapsed time for GetProdProvs:\r\n";
				stopWatch.Restart();
				stopWatchTotal.Restart();
			#endif
			command="DROP TABLE IF EXISTS tempdash"+rndStr+@";";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdProvs+="DROP TABLE: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//this table will contain approx 12x3xProv rows if there was production for each prov in each month.
			command=@"CREATE TABLE tempdash"+rndStr+@" (
				DatePeriod date NOT NULL,
				ProvNum bigint NOT NULL,
				production decimal NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdProvs+="CREATE TABLE: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//procs. Inserts approx 12xProv rows
			command=@"INSERT INTO tempdash"+rndStr+@"
				SELECT procedurelog.ProcDate,procedurelog.ProvNum,
				SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits))-IFNULL(SUM(claimproc.WriteOff),0)
				FROM procedurelog USE INDEX(indexPNPD)
				LEFT JOIN claimproc ON procedurelog.ProcNum=claimproc.ProcNum
				AND claimproc.Status='7' /*only CapComplete writeoffs are subtracted here*/
				WHERE procedurelog.ProcStatus = '2'
				AND procedurelog.ProcDate >= "+POut.Date(dateFrom)+@"
				AND procedurelog.ProcDate <= "+POut.Date(dateTo)+@"
				GROUP BY procedurelog.ProvNum,MONTH(procedurelog.ProcDate)";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdProvs+="INSERT INTO: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			
			//todo 2 more tables


			//get all the data as 12xProv rows
			command=@"SELECT DatePeriod,ProvNum,SUM(production) prod
				FROM tempdash"+rndStr+@" 
				GROUP BY ProvNum,MONTH(DatePeriod)";//this fails with date issue
			DataTable tableProd=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdProvs+="tableProd: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			command="DROP TABLE IF EXISTS tempdash"+rndStr+@";";
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdProvs+="DROP TABLE: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			Db.NonQ(command);
			command=@"SELECT ProvNum
				FROM provider WHERE IsHidden=0
				ORDER BY ItemOrder";
			DataTable tableProv=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				stopWatchTotal.Stop();
				_elapsedTimeProdProvs+="SELECT ProvNum FROM provider: "+stopWatch.Elapsed.ToString()+"\r\n";
				_elapsedTimeProdProvs+="Total: "+stopWatchTotal.Elapsed.ToString();
				if(_showElapsedTimesForDebug) {
					System.Windows.Forms.MessageBox.Show(_elapsedTimeProdProvs);
				}
			#endif
			List<List<int>> retVal=new List<List<int>>();
			for(int p=0;p<tableProv.Rows.Count;p++){//loop through each provider
				long provNum=PIn.Long(tableProv.Rows[p]["ProvNum"].ToString());
				List<int> listInt=new List<int>();//12 items
				for(int i=0;i<12;i++) {
					decimal prod=0;
					DateTime datePeriod=dateFrom.AddMonths(i);//only the month and year are important
					for(int j=0;j<tableProd.Rows.Count;j++)  {
						if(provNum==PIn.Long(tableProd.Rows[j]["ProvNum"].ToString())
							&& datePeriod.Month==PIn.Date(tableProd.Rows[j]["DatePeriod"].ToString()).Month
							&& datePeriod.Year==PIn.Date(tableProd.Rows[j]["DatePeriod"].ToString()).Year)
						{
		 					prod=PIn.Decimal(tableProd.Rows[j]["prod"].ToString());
							break;
						}
   				}
					listInt.Add((int)(prod));
				}
				retVal.Add(listInt);
			}
			return retVal;
		}

		///<summary>Returns all DashbaordAR(s) for the given time period. Caution, this will run aging and calculate a/r if a month within the given range is missing.
		///This can take several seconds per month missing.</summary>
		public static List<DashboardAR> GetAR(DateTime dateFrom,DateTime dateTo,List<DashboardAR> listDashAR) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DashboardAR>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listDashAR);
			}
			//assumes that dateFrom is the first of the month.
			string command;
			List<DashboardAR> listRet=new List<DashboardAR>();
			System.Diagnostics.Stopwatch stopWatch=new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch stopWatchTotal=new System.Diagnostics.Stopwatch();
			#if DEBUG
				_elapsedTimeAR="";
				_elapsedTimeAR="Elapsed time for GetAR:\r\n";
				stopWatchTotal.Start();
			#endif
			int months=0;
			while(dateTo>= dateFrom.AddMonths(months)) { //calculate the number of months between the two dates.
				months++;
			}
			for(int i = 0;i<months;i++) {
				DateTime dateLastOfMonth=dateFrom.AddMonths(i+1).AddDays(-1);
				DashboardAR dash=null;
				for(int d=0;d<listDashAR.Count;d++) {
					if(listDashAR[d].DateCalc!=dateLastOfMonth) {
						continue;
					}
					dash=listDashAR[d];
				}
				if(dash!=null) {//we found a DashboardAR object from the database for this month, so use it.
					listRet.Add(dash);
					continue;
				}
				if(ODBuild.IsDebug()) {
					stopWatch.Restart();
				}
				//run historical aging on all patients based on the date entered.
				command="SELECT SUM(Bal_0_30+Bal_31_60+Bal_61_90+BalOver90),SUM(InsEst) "
					+"FROM ("+Ledgers.GetAgingQueryString(dateLastOfMonth,isHistoric:true)+") guarBals";
				DataTable table=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
				#if DEBUG
					stopWatch.Stop();
					_elapsedTimeAR+="Aging using Ledgers.GetHistoricAgingQueryString() #"+i+" : "+stopWatch.Elapsed.ToString()+"\r\n";
				#endif
				dash=new DashboardAR();
				dash.DateCalc=dateLastOfMonth;
				dash.BalTotal=PIn.Double(table.Rows[0][0].ToString());
				dash.InsEst=PIn.Double(table.Rows[0][1].ToString());
				DashboardARs.Insert(dash);//save it to the db for later. 
				listRet.Add(dash); //and also use it now.
			}
			#if DEBUG
				stopWatchTotal.Stop();
				_elapsedTimeAR+="Total: "+stopWatchTotal.Elapsed.ToString();
				if(_showElapsedTimesForDebug) {
					System.Windows.Forms.MessageBox.Show(_elapsedTimeAR);
				}
			#endif
			return listRet;
		}

		public static List<List<int>> GetProdInc(DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<List<int>>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			System.Diagnostics.Stopwatch stopWatch=new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch stopWatchTotal=new System.Diagnostics.Stopwatch();
			#if DEBUG
				_elapsedTimeProdInc="";
				_elapsedTimeProdInc="Elapsed time for GetProdInc:\r\n";
				stopWatch.Restart();
				stopWatchTotal.Restart();
			#endif
			string command;
			command=@"SELECT procedurelog.ProcDate,
				SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits))-IFNULL(SUM(claimproc.WriteOff),0)
				FROM procedurelog
				LEFT JOIN claimproc ON procedurelog.ProcNum=claimproc.ProcNum
				AND claimproc.Status='7' /*only CapComplete writeoffs are subtracted here*/
				WHERE procedurelog.ProcStatus = '2'
				AND procedurelog.ProcDate >= "+POut.Date(dateFrom)+@"
				AND procedurelog.ProcDate <= "+POut.Date(dateTo)+@"
				GROUP BY MONTH(procedurelog.ProcDate)";
			DataTable tableProduction=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdInc+="tableProduction: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			command=@"SELECT AdjDate,
				SUM(AdjAmt)
				FROM adjustment
				WHERE AdjDate >= "+POut.Date(dateFrom)+@"
				AND AdjDate <= "+POut.Date(dateTo)+@"
				GROUP BY MONTH(AdjDate)";
			DataTable tableAdj=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdInc+="tableAdj: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			switch((PPOWriteoffDateCalc)PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate)) {
				case PPOWriteoffDateCalc.InsPayDate:
					command="SELECT "
						+"claimproc.DateCP," 
						+"SUM(claimproc.WriteOff) "
						+"FROM claimproc "
						+"WHERE claimproc.DateCP >= "+POut.Date(dateFrom)+" "
						+"AND claimproc.DateCP <= "+POut.Date(dateTo)+" "
						+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "//Received or supplemental
						+"GROUP BY MONTH(claimproc.DateCP)";
					break;
				case PPOWriteoffDateCalc.ProcDate:
					command="SELECT "
						+"claimproc.ProcDate," 
						+"SUM(claimproc.WriteOff) "
						+"FROM claimproc "
						+"WHERE claimproc.ProcDate >= "+POut.Date(dateFrom)+" "
						+"AND claimproc.ProcDate <= "+POut.Date(dateTo)+" "
						+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "
						+"GROUP BY MONTH(claimproc.ProcDate)";
					break;
				case PPOWriteoffDateCalc.ClaimPayDate:	//Means preference is PPOWriteoffDateCalc.InsDate, or PPOWriteoffDateCalc.ClaimPayDate.
					command="SELECT "
						+"claimsnaptshot.DateTEntry," 
						+"SUM(claimsnapshot.WriteOff) "
						+"FROM claimproc "
						+"INNER JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
						+"AND claimsnapshot.DateTEntry >= "+POut.Date(dateFrom)+" "
						+"AND claimsnapshot.DateTEntry <= "+POut.Date(dateTo)+" "
						+"WHERE claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "
						+"GROUP BY MONTH(claimsnaptshot.DateTEntry)";
					break;
			}	
			DataTable tableWriteoff=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdInc+="tableWriteoff: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			command="SELECT "
				+"paysplit.DatePay,"
				+"SUM(paysplit.SplitAmt) "
				+"FROM paysplit "
				+"WHERE paysplit.IsDiscount=0 "
				+"AND paysplit.DatePay >= "+POut.Date(dateFrom)+" "
				+"AND paysplit.DatePay <= "+POut.Date(dateTo)+" "
				+"GROUP BY MONTH(paysplit.DatePay)";
			DataTable tablePay=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeProdInc+="tablePay: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			command="SELECT claimpayment.CheckDate,SUM(claimproc.InsPayamt) "
				+"FROM claimpayment,claimproc WHERE "
				+"claimproc.ClaimPaymentNum = claimpayment.ClaimPaymentNum "
				+"AND claimpayment.CheckDate >= " + POut.Date(dateFrom)+" "
				+"AND claimpayment.CheckDate <= " + POut.Date(dateTo)+" "
				+" GROUP BY claimpayment.CheckDate ORDER BY checkdate";
			DataTable tableIns=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				stopWatchTotal.Stop();
				_elapsedTimeProdInc+="tableIns: "+stopWatch.Elapsed.ToString()+"\r\n";
				_elapsedTimeProdInc+="Total: "+stopWatchTotal.Elapsed.ToString();
				if(_showElapsedTimesForDebug) {
					System.Windows.Forms.MessageBox.Show(_elapsedTimeProdInc);
				}
			#endif
			//production--------------------------------------------------------------------
			List<int> listInt;
			listInt=new List<int>();
			for(int i=0;i<12;i++) {
				decimal prod=0;
				decimal adjust=0;
				decimal inswriteoff=0;
				DateTime datePeriod=dateFrom.AddMonths(i);//only the month and year are important
				for(int j=0;j<tableProduction.Rows.Count;j++)  {
				  if(datePeriod.Year==PIn.Date(tableProduction.Rows[j][0].ToString()).Year
						&& datePeriod.Month==PIn.Date(tableProduction.Rows[j][0].ToString()).Month)
					{
		 			  prod+=PIn.Decimal(tableProduction.Rows[j][1].ToString());
					}
   			}
				for(int j=0;j<tableAdj.Rows.Count; j++){
				  if(datePeriod.Year==PIn.Date(tableAdj.Rows[j][0].ToString()).Year
						&& datePeriod.Month==PIn.Date(tableAdj.Rows[j][0].ToString()).Month)
					{
						adjust+=PIn.Decimal(tableAdj.Rows[j][1].ToString());
					}
   			}
				for(int j=0;j<tableWriteoff.Rows.Count; j++){
					if(datePeriod.Year==PIn.Date(tableWriteoff.Rows[j][0].ToString()).Year
						&& datePeriod.Month==PIn.Date(tableWriteoff.Rows[j][0].ToString()).Month)
					{
						inswriteoff+=PIn.Decimal(tableWriteoff.Rows[j][1].ToString());
					}
				}
				listInt.Add((int)(prod+adjust-inswriteoff));
			}
			List<List<int>> retVal=new List<List<int>>();
			retVal.Add(listInt);
			//income----------------------------------------------------------------------
			listInt=new List<int>();
			for(int i=0;i<12;i++) {
				decimal ptincome=0;
				decimal insincome=0;
				DateTime datePeriod=dateFrom.AddMonths(i);//only the month and year are important
				for(int j=0;j<tablePay.Rows.Count;j++) {
					if(datePeriod.Year==PIn.Date(tablePay.Rows[j][0].ToString()).Year
						&& datePeriod.Month==PIn.Date(tablePay.Rows[j][0].ToString()).Month) 
					{
						ptincome+=PIn.Decimal(tablePay.Rows[j][1].ToString());
					}
				}
				for(int j=0;j<tableIns.Rows.Count;j++) {//
					if(datePeriod.Year==PIn.Date(tableIns.Rows[j][0].ToString()).Year
						&& datePeriod.Month==PIn.Date(tableIns.Rows[j][0].ToString()).Month) 
					{
						insincome+=PIn.Decimal(tableIns.Rows[j][1].ToString());
					}
				}
				listInt.Add((int)(ptincome+insincome));
			}
			retVal.Add(listInt);
			return retVal;
		}

		public static List<List<int>> GetNewPatients(DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<List<int>>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			Random rnd=new Random();
			string rndStr=rnd.Next(1000000).ToString();
			System.Diagnostics.Stopwatch stopWatch=new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch stopWatchTotal=new System.Diagnostics.Stopwatch();
			#if DEBUG
				_elapsedTimeNewPatients="";
				_elapsedTimeNewPatients="Elapsed time for GetNewPatients:\r\n";
				stopWatch.Restart();
				stopWatchTotal.Restart();
			#endif
			string command;
			command="DROP TABLE IF EXISTS tempdash"+rndStr+@";";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeNewPatients+="DROP TABLE: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			command=@"CREATE TABLE tempdash"+rndStr+@" (
				PatNum bigint NOT NULL PRIMARY KEY,
				dateFirstProc datetime NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeNewPatients+="CREATE TABLE: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			//table full of individual patients and their dateFirstProcs.
			command=@"INSERT INTO tempdash"+rndStr+@" 
				SELECT PatNum, MIN(ProcDate) dateFirstProc 
				FROM procedurelog USE INDEX(indexPatNum)
				WHERE ProcStatus=2 GROUP BY PatNum
				HAVING dateFirstProc >= "+POut.Date(dateFrom)+" "
				+"AND dateFirstProc <= "+POut.Date(dateTo);
			Db.NonQ(command);
			#if DEBUG
				stopWatch.Stop();
				_elapsedTimeNewPatients+="INSERT INTO: "+stopWatch.Elapsed.ToString()+"\r\n";
				stopWatch.Restart();
			#endif
			command="SELECT dateFirstProc,COUNT(*) "
				+"FROM tempdash"+rndStr+@" "
				+"GROUP BY MONTH(dateFirstProc)";
			DataTable tableCounts=Db.GetTable(command);
			#if DEBUG
				stopWatch.Stop();
				stopWatchTotal.Stop();
				_elapsedTimeNewPatients+="SELECT dateFirstProc,COUNT(*): "+stopWatch.Elapsed.ToString()+"\r\n";
				_elapsedTimeNewPatients+="Total: "+stopWatchTotal.Elapsed.ToString();
				if(_showElapsedTimesForDebug) {
					System.Windows.Forms.MessageBox.Show(_elapsedTimeNewPatients);
				}
			#endif
			List<int> listInt=new List<int>();
			for(int i=0;i<12;i++) {
				int ptcount=0;
				DateTime datePeriod=dateFrom.AddMonths(i);//only the month and year are important
				for(int j=0;j<tableCounts.Rows.Count;j++) {
					if(datePeriod.Year==PIn.Date(tableCounts.Rows[j][0].ToString()).Year
						&& datePeriod.Month==PIn.Date(tableCounts.Rows[j][0].ToString()).Month)
					{
						ptcount+=PIn.Int(tableCounts.Rows[j][1].ToString());
					}
				}
				listInt.Add(ptcount);
			}
			List<List<int>> retVal=new List<List<int>>();
			retVal.Add(listInt);
			return retVal;
		}

		#region OpenDentalGraph Queries
		public static DataTable GetTable(string command,bool doRunOnReportServer=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),command,doRunOnReportServer);
			}
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command),doRunOnReportServer);
		}
		#endregion

	}
}
