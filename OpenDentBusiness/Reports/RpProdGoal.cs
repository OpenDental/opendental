using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness {
	public class RpProdGoal {

		//IMPORTANT NOTE FOR ANYBODY WHO CODES IN HERE:  This is used in the CEMT so everything MUST be coded in such a way that they don't use the 
		//cache to look up information.  The CEMT does NOT keep copies of the remote database caches when this is used so things such as 
		//PrefC.GetBool or Clinics.GetDesc will return incorrect results.

		///<summary>If not using clinics then supply an empty list of clinics.</summary>
		public static DataSet GetData(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool hasAllProvs
			,bool hasAllClinics,PPOWriteoffDateCalc writeoffPayType,bool isCEMT=false) 
		{
			//No need to check RemotingRole; no call to db.
			DataSet dataSet=GetMonthlyGoalDataSet(dateFrom,dateTo,listProvs,listClinics,hasAllProvs,hasAllClinics,writeoffPayType,isCEMT);
			DataTable tableProduction=dataSet.Tables["tableProduction"];
			DataTable tableAdj=dataSet.Tables["tableAdj"];
			DataTable tableInsWriteoff=dataSet.Tables["tableInsWriteoff"];
			DataTable tableSched=dataSet.Tables["tableSched"];
			DataTable tableProdGoal=dataSet.Tables["tableProdGoal"];
			DataTable tableWriteoffAdj=dataSet.Tables["tableWriteOffAdjustments"];
			decimal scheduledForDay;
			decimal productionForDay;
			decimal adjustsForDay;
			decimal inswriteoffsForDay;	//spk 5/19/05
			decimal insWriteoffAdjsForDay;
			decimal totalproductionForDay;
			decimal prodGoalForDay;
			DataTable dt=new DataTable("Total");
			dt.Columns.Add(new DataColumn("Date"));
			dt.Columns.Add(new DataColumn("Weekday"));
			dt.Columns.Add(new DataColumn("Production",typeof(double)));
			dt.Columns.Add(new DataColumn("Prod Goal",typeof(double)));
			dt.Columns.Add(new DataColumn("Scheduled",typeof(double)));
			dt.Columns.Add(new DataColumn("Adjusts",typeof(double)));
			if(writeoffPayType==PPOWriteoffDateCalc.ClaimPayDate) {
				dt.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				dt.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				dt.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			dt.Columns.Add(new DataColumn("Tot Prod",typeof(double)));
			DataTable dtClinic=new DataTable("Clinic");
			dtClinic.Columns.Add(new DataColumn("Date"));
			dtClinic.Columns.Add(new DataColumn("Weekday"));
			dtClinic.Columns.Add(new DataColumn("Production",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Prod Goal",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Scheduled",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Adjusts",typeof(double)));
			if(writeoffPayType==PPOWriteoffDateCalc.ClaimPayDate) {
				dtClinic.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				dtClinic.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				dtClinic.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			dtClinic.Columns.Add(new DataColumn("Tot Prod",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Clinic"));
			//length of array is number of months between the two dates plus one.
			//The from date and to date will not be more than one year and must will be within the same year due to FormRpProdInc UI validation enforcement.
			DateTime[] dates=null;
			dates=new DateTime[dateTo.Subtract(dateFrom).Days+1];//Make a DateTime array with one position for each day in the report.
			//Preprocess schedules for provider so we can merge them together and not double count them.
			//For each schedule, find all others for this prov and clinic.  (in the case when they're overlapping and multiple clinics, we don't get this far)
			//Figure out a way to find all hours a provider worked in a single day across multiple schedules.  If they overlap (due to multiple operatories)
			//then we only count one.  
			//Sum up a schedule for the day. 
			Dictionary<Tuple<DateTime,long,long>,List<DataRow>> dictDates=new Dictionary<Tuple<DateTime,long,long>,List<DataRow>>();//We are grouping data rows by day, provnum, and clinicnum
			for(int j=0;j<tableProdGoal.Rows.Count;j++) {
				DateTime date=PIn.Date(tableProdGoal.Rows[j]["SchedDate"].ToString());
				long provNum=PIn.Long(tableProdGoal.Rows[j]["ProvNum"].ToString());
				long clinicNum=(!hasAllClinics && listClinics.Count==0) ? 0 : PIn.Long(tableProdGoal.Rows[j]["ClinicNum"].ToString());
				if(!dictDates.ContainsKey(Tuple.Create(date,provNum,clinicNum))) {
					dictDates.Add(Tuple.Create(date,provNum,clinicNum),new List<DataRow>() { tableProdGoal.Rows[j] });
					continue;//It's added, no need to do more.
				}
				//Date/prov/clinic combo exists in dictionary already, add row to the row collection.
				dictDates[Tuple.Create(date,provNum,clinicNum)].Add(tableProdGoal.Rows[j]);
			}
			List<ProvProdGoal> listProdGoal=new List<ProvProdGoal>();
			//Add all spans to a list of spans if they don't overlap.  If they do overlap, extend the start/end of an existing span.
			//Once all spans are added, compare spans in list to other spans and see if they overlap, expand as needed (removing the one merged).
			//If there is no movement, we are done.
			foreach(KeyValuePair<Tuple<DateTime,long,long>,List<DataRow>> kvp in dictDates) {//For each day (there are no multi-clinic overlaps, can't run report if there are)
				double hours=0;
				List<SchedRange> listRangeForDay=new List<SchedRange>();
				foreach(DataRow row in kvp.Value) {//Add all schedule ranges to the list
					TimeSpan stopTime=PIn.Time(row["StopTime"].ToString());
					TimeSpan startTime=PIn.Time(row["StartTime"].ToString());
					SchedRange range=new SchedRange() {StartTime=startTime,EndTime=stopTime};
					listRangeForDay.Add(range);
				}
				bool hasMovement=true;
				while(listRangeForDay.Count>1 && hasMovement) {//As they're added, attempt to merge ranges until there's no more movement.
					for(int i=listRangeForDay.Count-1;i>=0;i--) {
						SchedRange range1=listRangeForDay[i];
						for(int j=listRangeForDay.Count-1;j>=0;j--) {
							hasMovement=false;
							SchedRange range2=listRangeForDay[j];
							if(range1.PriKey==range2.PriKey) {
								continue;
							}
							if(range1.StartTime<=range2.StartTime && range1.EndTime>=range2.StartTime) {//range2 starts between range1's start and end.  Time to merge end time.
								if(range2.EndTime>=range1.EndTime) {
									range1.EndTime=range2.EndTime;
								}
								hasMovement=true;
							}
							if(range1.StartTime<=range2.EndTime && range1.EndTime>=range2.EndTime) {//range2 ends between range1's start and end.  Time to merge start time.
								if(range2.StartTime<=range1.StartTime) {
									range1.StartTime=range2.StartTime;
								}
								hasMovement=true;
							}
							if(hasMovement) {
								listRangeForDay.RemoveAt(j);
								--i;
							}
						}
					}
				}
				foreach(SchedRange sched in listRangeForDay) {						
					TimeSpan timeDiff=sched.EndTime.Subtract(sched.StartTime);
					hours+=timeDiff.TotalHours;
				}
				listProdGoal.Add(new ProvProdGoal() {ClinicNum=kvp.Key.Item3,ProvNum=kvp.Key.Item2,Date=kvp.Key.Item1,Hours=hours,ProdGoal=PIn.Double(kvp.Value[0]["ProvProdGoal"].ToString())});
			}			
			//Get a list of clinics so that we have access to their descriptions for the report.
			for(int it=0;it<listClinics.Count;it++) {//For each clinic
				for(int i=0;i<dates.Length;i++) {//usually 12 months in loop for annual.  Loop through the DateTime array, each position represents one date in the report.
					dates[i]=dateFrom.AddDays(i);//Monthly/Daily report, add a day
					DataRow row=dtClinic.NewRow();
					row["Date"]=dates[i].ToShortDateString();
					row["Weekday"]=dates[i].DayOfWeek.ToString();
					scheduledForDay=0;
					productionForDay=0;
					adjustsForDay=0;
					inswriteoffsForDay=0;	//spk 5/19/05
					insWriteoffAdjsForDay=0;
					prodGoalForDay=0;
					for(int j=0;j<tableProduction.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableProduction.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;//Only counting unassigned this time around.
						}
						else if(listClinics[it].ClinicNum!=0 && tableProduction.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Date==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Date) {
							productionForDay+=PIn.Decimal(tableProduction.Rows[j]["Production"].ToString());
						}
					}
					for(int j=0;j<tableAdj.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableAdj.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableAdj.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Date==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Date) {
							adjustsForDay+=PIn.Decimal(tableAdj.Rows[j]["Adjustment"].ToString());
						}
					}
					for(int j=0;j<tableInsWriteoff.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableInsWriteoff.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableInsWriteoff.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Date==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Date) {
							inswriteoffsForDay-=PIn.Decimal(tableInsWriteoff.Rows[j]["Writeoff"].ToString());					
						}			
					}
					foreach(DataRow rowCur in tableWriteoffAdj.Rows) {
						if(rowCur["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum) || dates[i].Date!=PIn.Date(rowCur["Date"].ToString()).Date) {
							continue;
						}
						insWriteoffAdjsForDay-=PIn.Decimal(rowCur["WriteOffEst"].ToString())+PIn.Decimal(rowCur["WriteOff"].ToString());
					}
					for(int j=0;j<tableSched.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableSched.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableSched.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Date==PIn.Date(tableSched.Rows[j]["SchedDate"].ToString()).Date) {
							scheduledForDay+=PIn.Decimal(tableSched.Rows[j]["Amount"].ToString());
						}
					}
					for(int j = 0;j<listProdGoal.Count;j++) {
						if(listClinics[it].ClinicNum==0 && listProdGoal[j].ClinicNum!=0) {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && listProdGoal[j].ClinicNum!=listClinics[it].ClinicNum) {
							continue;
						}
						if(dates[i].Date==listProdGoal[j].Date) {
							prodGoalForDay+=(decimal)(listProdGoal[j].Hours*listProdGoal[j].ProdGoal);//Multiply the hours for this schedule by the amount of production goal for this prov.
						}
					}
					totalproductionForDay=productionForDay+adjustsForDay+inswriteoffsForDay+insWriteoffAdjsForDay+scheduledForDay;
					string clinicDesc=listClinics[it].Description;
					row["Production"]=productionForDay.ToString("n");
					row["Prod Goal"]=prodGoalForDay.ToString("n");
					row["Scheduled"]=scheduledForDay.ToString("n");
					row["Adjusts"]=adjustsForDay.ToString("n");
					if(writeoffPayType==PPOWriteoffDateCalc.ClaimPayDate) {
						row["Writeoff Est"]=inswriteoffsForDay.ToString("n");
						row["Writeoff Adj"]=insWriteoffAdjsForDay.ToString("n");
					}
					else {
						row["Writeoff"]=inswriteoffsForDay.ToString("n");
					}
					row["Tot Prod"]=totalproductionForDay.ToString("n");
					row["Clinic"]=clinicDesc=="" ? Lans.g("FormRpProdInc","Unassigned"):clinicDesc;
					dtClinic.Rows.Add(row);
				}
			}
			for(int i=0;i<dates.Length;i++) {//usually 12 months in loop
				dates[i]=dateFrom.AddDays(i);
				DataRow row=dt.NewRow();
				row["Date"]=dates[i].ToShortDateString();
				row["Weekday"]=dates[i].DayOfWeek.ToString();
				scheduledForDay=0;
				productionForDay=0;
				adjustsForDay=0;
				inswriteoffsForDay=0;
				insWriteoffAdjsForDay=0;
				prodGoalForDay=0;
				for(int j=0;j<tableProduction.Rows.Count;j++) {
					if(dates[i].Date==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Date) {
						productionForDay+=PIn.Decimal(tableProduction.Rows[j]["Production"].ToString());
					}
				}
				for(int j=0;j<tableAdj.Rows.Count;j++) {
					if(dates[i].Date==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Date) {
						adjustsForDay+=PIn.Decimal(tableAdj.Rows[j]["Adjustment"].ToString());
					}
				}
				for(int j=0;j<tableInsWriteoff.Rows.Count;j++) {
					if(dates[i].Date==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Date) {
						inswriteoffsForDay-=PIn.Decimal(tableInsWriteoff.Rows[j]["Writeoff"].ToString());
					}
				}
				foreach(DataRow rowCur in tableWriteoffAdj.Rows) {
					if(dates[i].Date==PIn.Date(rowCur["Date"].ToString()).Date) {
						insWriteoffAdjsForDay-=PIn.Decimal(rowCur["WriteOffEst"].ToString())+PIn.Decimal(rowCur["WriteOff"].ToString());
					}
				}
				for(int j=0;j<tableSched.Rows.Count;j++) {
					if(dates[i].Date==PIn.Date(tableSched.Rows[j]["SchedDate"].ToString()).Date) {
						scheduledForDay+=PIn.Decimal(tableSched.Rows[j]["Amount"].ToString());
					}
				}
				for(int j = 0;j<listProdGoal.Count;j++) {
					if(dates[i].Date==listProdGoal[j].Date) {
						prodGoalForDay+=(decimal)(listProdGoal[j].Hours*listProdGoal[j].ProdGoal);//Multiply the hours for this schedule by the amount of production goal for this prov.
					}
				}
				totalproductionForDay=productionForDay+adjustsForDay+inswriteoffsForDay+insWriteoffAdjsForDay+scheduledForDay;
				row["Production"]=productionForDay.ToString("n");
				row["Prod Goal"]=prodGoalForDay.ToString("n");
				row["Scheduled"]=scheduledForDay.ToString("n");
				row["Adjusts"]=adjustsForDay.ToString("n");
				if(writeoffPayType==PPOWriteoffDateCalc.ClaimPayDate) {
					row["Writeoff Est"]=inswriteoffsForDay.ToString("n");
					row["Writeoff Adj"]=insWriteoffAdjsForDay.ToString("n");
				}
				else {
					row["Writeoff"]=inswriteoffsForDay.ToString("n");
				}
				row["Tot Prod"]=totalproductionForDay.ToString("n");
				dt.Rows.Add(row);
			}
			DataSet ds=null;
			ds=new DataSet("MonthlyData");
			ds.Tables.Add(dt);
			if(listClinics.Count!=0) {
				ds.Tables.Add(dtClinic);
			}
			return ds;
		}

		public static DataSet GetMonthlyGoalDataSet(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool hasAllProvs
			,bool hasAllClinics,PPOWriteoffDateCalc writeoffPayType,bool isCEMT=false) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvs,listClinics,hasAllProvs,hasAllClinics,writeoffPayType,isCEMT);
			}
			List<long> listClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
			List<long> listProvNums=listProvs.Select(x => x.ProvNum).ToList();
			#region Procedures
			string whereProv="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv="AND procedurelog.ProvNum IN ("+string.Join(",",listProvNums)+") ";
			}
			string whereClin="";
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND procedurelog.ClinicNum IN ("+string.Join(",",listClinicNums)+") ";
			}
			string command="SELECT "
				+"procedurelog.ProcDate,procedurelog.ClinicNum,"
				+"SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits))-IFNULL(SUM(cp.WriteOff),0) Production "
				+"FROM procedurelog "
				+"LEFT JOIN (SELECT SUM(claimproc.WriteOff) AS WriteOff, claimproc.ProcNum FROM claimproc "
				+"WHERE claimproc.Status=7 "//only CapComplete writeoffs are subtracted here.
				+"GROUP BY claimproc.ProcNum) cp ON procedurelog.ProcNum=cp.ProcNum "
				+"WHERE procedurelog.ProcStatus = 2 "
				+whereProv
				+whereClin
				+"AND procedurelog.ProcDate >= " +POut.Date(dateFrom)+" "
				+"AND procedurelog.ProcDate <= " +POut.Date(dateTo)+" "
				+"GROUP BY ClinicNum,YEAR(procedurelog.ProcDate),MONTH(procedurelog.ProcDate),DAY(procedurelog.ProcDate)";//Does not work for Oracle. Consider enhancing with DbHelper.Year(),DbHelper.Month()
			command+=" ORDER BY ClinicNum,ProcDate";
			DataTable tableProduction=new DataTable();
			if(isCEMT) {
				tableProduction=Db.GetTable(command);
			}
			else {
				tableProduction=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableProduction.TableName="tableProduction";
			#endregion
			#region Adjustments
			string whereProcProv="", whereProcClin="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv="AND adjustment.ProvNum IN ("+String.Join(",",listProvNums)+") ";
				whereProcProv="AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND adjustment.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
				whereProcClin="AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT "
				+"U.AdjDate,"
				+"U.ClinicNum,"
				+"SUM(U.Adjustment) Adjustment "
				+"FROM( "
				+"SELECT "
				+"adjustment.AdjDate,"
				+"adjustment.ClinicNum,"
				+"adjustment.AdjAmt Adjustment "
				+"FROM adjustment "
				+"WHERE AdjDate >= "+POut.Date(dateFrom)+" "
				+"AND AdjDate <= "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin
				+"UNION ALL "
				+"SELECT "
				+DbHelper.DtimeToDate("appointment.AptDateTime")+" AdjDate, "
				+"procedurelog.ClinicNum, "
				+"-(procedurelog.Discount + procedurelog.DiscountPlanAmt) Adjustment "
				+"FROM appointment "
				+"INNER JOIN procedurelog ON appointment.AptNum = procedurelog.AptNum AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP)+" "
				+"WHERE appointment.AptStatus = "+POut.Int((int)ApptStatus.Scheduled)+" "
				+"AND "+DbHelper.BetweenDates("appointment.AptDateTime",dateFrom,dateTo)+" "
				+whereProcProv
				+whereProcClin
				+") AS U "
				+"GROUP BY ClinicNum,YEAR(U.AdjDate),MONTH(U.AdjDate),DAY(U.AdjDate) "
				+"ORDER BY ClinicNum,AdjDate";
			DataTable tableAdj=new DataTable();
			if(isCEMT) {
				tableAdj=Db.GetTable(command);
			}
			else { 
				tableAdj=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableAdj.TableName="tableAdj";
			#endregion
			#region TableInsWriteoff
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv="AND claimproc.ProvNum IN ("+string.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND claimproc.ClinicNum IN ("+string.Join(",",listClinicNums)+") ";
			}			
			if(writeoffPayType==PPOWriteoffDateCalc.InsPayDate) {
				command="SELECT "
				+"claimproc.DateCP Date," 
				+"claimproc.ClinicNum,"
				+"SUM(claimproc.WriteOff) WriteOff "
				+"FROM claimproc "
				+"WHERE claimproc.DateCP >= "+POut.Date(dateFrom)+" "
				+"AND claimproc.DateCP <= "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin
				+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "//received or supplemental
				+"GROUP BY ClinicNum,DATE(claimproc.DateCP) "
				+"ORDER BY ClinicNum,DateCP";
			}
			else if(writeoffPayType==PPOWriteoffDateCalc.ProcDate) {
				command="SELECT "
				+"claimproc.ProcDate Date," 
				+"claimproc.ClinicNum,"
				+"SUM(claimproc.WriteOff) WriteOff "
				+"FROM claimproc "
				+"WHERE claimproc.ProcDate >= "+POut.Date(dateFrom)+" "
				+"AND claimproc.ProcDate <= "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin
				+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "//received or supplemental or notreceived
				+"GROUP BY ClinicNum,DATE(claimproc.ProcDate) "
				+"ORDER BY ClinicNum,ProcDate";
			}
			else { // writeoffPayType==PPOWriteoffDateCalc.ClaimPayDate
				command="SELECT "
				+"claimsnapshot.DateTEntry Date," 
				+"claimproc.ClinicNum,"
				+"SUM(CASE WHEN claimsnapshot.WriteOff=-1 THEN 0 ELSE claimsnapshot.WriteOff END) WriteOff "
				+"FROM claimproc "
				+"INNER JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimProc.ClaimProcNum "
				+"WHERE "+DbHelper.BetweenDates("claimsnapshot.DateTEntry",dateFrom,dateTo)+" "
				+whereProv
				+whereClin
				+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "//received or supplemental or notreceived
				+"GROUP BY ClinicNum,DATE(claimsnapshot.DateTEntry) "
				+"ORDER BY ClinicNum,claimsnapshot.DateTEntry";
			}
			DataTable tableInsWriteoff=new DataTable();
			if(isCEMT) {
				tableInsWriteoff=Db.GetTable(command);
			}
			else { 
				tableInsWriteoff=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableInsWriteoff.TableName="tableInsWriteoff";
			#endregion
			#region TableSched
			DataTable tableSched=new DataTable();
			//Reads from the procedurelog table instead of claimproc because we are looking for scheduled procedures.
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv="AND procedurelog.ProvNum IN ("+string.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND procedurelog.ClinicNum IN ("+string.Join(",",listClinicNums)+") ";
			}
			command= "SELECT "+DbHelper.DtimeToDate("t.AptDateTime")+" SchedDate,SUM(t.Fee-t.WriteoffEstimate) Amount,ClinicNum "
				+"FROM (SELECT appointment.AptDateTime,IFNULL(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits),0) Fee,appointment.ClinicNum,";
			if(ReportsComplex.RunFuncOnReportServer(() => Prefs.GetBoolNoCache(PrefName.ReportPandIschedProdSubtractsWO))) {
				//Subtract both PPO and capitation writeoffs
				command+="SUM(IFNULL(CASE WHEN WriteOffEstOverride != -1 THEN WriteOffEstOverride ELSE WriteOffEst END,0)) WriteoffEstimate ";
			}
			else {
				//Always subtract CapEstimate writeoffs from scheduled production. This is so that the scheduled production will match actual production
				//when the procedures are set complete. Nathan decided this 01/05/2017.
				command+="SUM(IFNULL((CASE WHEN claimproc.Status="+POut.Int((int)ClaimProcStatus.Estimate)+" THEN 0 "
					+"WHEN WriteOffEstOverride != -1 THEN WriteOffEstOverride ELSE WriteOffEst END),0)) WriteoffEstimate ";
			}
			command+="FROM appointment "
				+"LEFT JOIN procedurelog ON appointment.AptNum = procedurelog.AptNum AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP)+" "
				+"LEFT JOIN claimproc ON procedurelog.ProcNum = claimproc.ProcNum "
					+"AND claimproc.Status IN("+POut.Int((int)ClaimProcStatus.Estimate)+","+POut.Int((int)ClaimProcStatus.CapEstimate)+") "
					+" AND (WriteOffEst != -1 OR WriteOffEstOverride != -1) "
				+"WHERE appointment.AptStatus = "+POut.Int((int)ApptStatus.Scheduled)+" "
				+"AND "+DbHelper.DtimeToDate("appointment.AptDateTime")+" >= "+POut.Date(dateFrom)+" "
				+"AND "+DbHelper.DtimeToDate("appointment.AptDateTime")+" <= "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin
				+" GROUP BY procedurelog.ProcNum) t "//without this, there can be duplicate proc rows due to the claimproc join with dual insurance.
				+"GROUP BY SchedDate,ClinicNum "
				+"ORDER BY SchedDate";
			if(isCEMT) {
				tableSched=Db.GetTable(command);
			}
			else { 
				tableSched=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableSched.TableName="tableSched";
			#endregion
			#region TableProdGoal
			//1. Find all schedules for the month
			//2. ClinicNum will come from the schedule's operatory
			//3. Fetch HourlyProdGoalAmt from provider on the schedule
			//4. Sum scheduled hours, grouped by prov and clinic
			//5. Multiply the scheduled hours by the provider's HourlyProdGoalAmt
			DataTable tableProdGoal=new DataTable();
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv="AND schedule.ProvNum IN ("+string.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND COALESCE(operatory.ClinicNum,0) IN ("+string.Join(",",listClinicNums)+") ";
			}
			//Fetch all schedules for the month and associated information (clinic from operatory, HourlyProdGoalAmt from provider)
			command="SELECT "+DbHelper.DtimeToDate("schedule.SchedDate")+@" AS SchedDate, schedule.StartTime AS StartTime, schedule.StopTime AS StopTime,
				COALESCE(operatory.ClinicNum,0) AS ClinicNum, provider.HourlyProdGoalAmt AS ProvProdGoal, provider.ProvNum AS ProvNum
				FROM schedule 
				INNER JOIN provider ON provider.ProvNum=schedule.ProvNum 
				LEFT JOIN scheduleop ON scheduleop.ScheduleNum=schedule.ScheduleNum 
				LEFT JOIN operatory ON scheduleop.OperatoryNum=operatory.OperatoryNum 
				WHERE schedule.SchedType="+POut.Int((int)ScheduleType.Provider)+" "
				+"AND schedule.Status="+POut.Int((int)SchedStatus.Open)+" "
				+"AND schedule."+DbHelper.BetweenDates("SchedDate",dateFrom,dateTo)+" "
				+whereProv
				+whereClin;
			if(isCEMT) {
				tableProdGoal=Db.GetTable(command);
			}
			else {
				tableProdGoal=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableProdGoal.TableName="tableProdGoal";	
			#endregion
			#region WriteOffAdjustments
			DataTable tableWriteOffAdjustments=new DataTable();
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv="AND claimproc.ProvNum IN ("+string.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND claimproc.ClinicNum IN ("+string.Join(",",listClinicNums)+") ";
			}			
			if(writeoffPayType==PPOWriteoffDateCalc.ClaimPayDate) {
				//Insurance WriteOff Adjustments----------------------------------------------------------------------------
				command=$@"SELECT claimproc.DateCP Date,claimproc.ClinicNum,
					-SUM(CASE WHEN COALESCE(claimsnapshot.WriteOff,-1)=-1 THEN 0 ELSE claimsnapshot.WriteOff END) WriteOffEst,
					SUM(claimproc.WriteOff) WriteOff,
					claimproc.ClaimNum
					FROM claimproc
					LEFT JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum
					WHERE claimproc.DateCP BETWEEN {POut.Date(dateFrom)} AND {POut.Date(dateTo)}
					AND claimproc.Status IN ({(int)ClaimProcStatus.Received},{(int)ClaimProcStatus.Supplemental})
					{whereProv}
					{whereClin}
					GROUP BY ClinicNum,DATE(claimproc.DateCP)";
				if(isCEMT) {
					tableWriteOffAdjustments=Db.GetTable(command);
				}
				else { 
					tableWriteOffAdjustments=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
				}
			}
			tableWriteOffAdjustments.TableName="tableWriteOffAdjustments";
			#endregion WriteOffAdjustments
			DataSet dataSet=new DataSet();
			dataSet.Tables.Add(tableProduction);
			dataSet.Tables.Add(tableAdj);
			dataSet.Tables.Add(tableInsWriteoff);
			dataSet.Tables.Add(tableWriteOffAdjustments);
			dataSet.Tables.Add(tableSched);
			dataSet.Tables.Add(tableProdGoal);
			return dataSet;
		}
	}

	class ProvProdGoal {
		public long ProvNum;
		public long ClinicNum;
		public DateTime Date;
		public double ProdGoal;
		public double Hours;
	}

	class SchedRange {
		private static long AutoIncrementValue=1;
		///<summary>PriKey will be unique and automatically assigned.</summary>
		public long PriKey = (AutoIncrementValue++);
		public TimeSpan StartTime;
		public TimeSpan EndTime;
	}

}
