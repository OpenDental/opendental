using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	public class RpTreatmentFinder {

		///<summary>Gets the DataTable to display for treatment finder report</summary>
		///<param name="listProviders">Include '0' in the list to get for all providers.</param>
		///<param name="listBilling">Include '0' in the list to get for all billing types.</param>
		///<param name="listClinicNums">Pass in an empty list to get for all clinics.</param>
		public static DataTable GetTreatmentFinderList(bool noIns,bool patsWithAppts,int monthStart,DateTime dateFrom,DateTime dateTo,double aboveAmount,
			List<long> listProviders,List<long> listBilling,string code1,string code2,List<long> listClinicNums,bool isProcsGeneral, bool useTreatingProvider) 
		{
			//No remoting role check; no call to db
			Stopwatch sw=null;
			Stopwatch sTotal=null;
			if(ODBuild.IsDebug()) {
				sw=Stopwatch.StartNew();
				sTotal=Stopwatch.StartNew();
			}
			DataTable table=new DataTable();
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("PatNum");
			table.Columns.Add("LName");
			table.Columns.Add("FName");
			table.Columns.Add("contactMethod");
			table.Columns.Add("address");
			table.Columns.Add("City");
			table.Columns.Add("State");
			table.Columns.Add("Zip");
			table.Columns.Add("annualMaxInd");
			table.Columns.Add("annualMaxFam");
			table.Columns.Add("amountUsedInd");
			table.Columns.Add("amountUsedFam");
			table.Columns.Add("amountPendingInd");
			table.Columns.Add("amountPendingFam");
			table.Columns.Add("amountRemainingInd");
			table.Columns.Add("amountRemainingFam");
			table.Columns.Add("treatmentPlan");
			table.Columns.Add("carrierName");
			table.Columns.Add("clinicAbbr");
			//dictionary with Key=PatNum, Value=AmtPlanned
			Dictionary<long,double> dictAmtPlanned=new Dictionary<long,double>();
			using(DataTable tablePlanned=GetDictAmtPlanned(patsWithAppts,dateFrom,dateTo,listProviders,listBilling,code1,code2,listClinicNums,useTreatingProvider)) {
				if(ODBuild.IsDebug()) {
					sw.Stop();
					Console.WriteLine("Get tablePlanned: "+sw.Elapsed.TotalSeconds+" sec, Rows: "+tablePlanned.Rows.Count);
					sw=Stopwatch.StartNew();
				}
				if(tablePlanned.Rows.Count==0) {
					return table;
				}
				dictAmtPlanned=tablePlanned.Select().ToDictionary(x => PIn.Long(x["PatNum"].ToString()),x => PIn.Double(x["AmtPlanned"].ToString()));
			}
			string patNumStr=string.Join(",",dictAmtPlanned.Keys.Select(x => POut.Long(x)));
			DateTime renewDate=BenefitLogic.ComputeRenewDate(DateTime.Now,monthStart);
			//dictionary with Key=PatPlanNum, Value=Tuple(AmtPending,AmtUsed)
			Dictionary<long,Tuple<double,double>> dictPatInfo=new Dictionary<long,Tuple<double,double>>();
			using(DataTable tablePat=GetPatInfo(isProcsGeneral,renewDate,patNumStr)) {
				dictPatInfo=tablePat.Select().ToDictionary(x => PIn.Long(x["PatPlanNum"].ToString()),
					x => Tuple.Create(PIn.Double(x["AmtPending"].ToString()),PIn.Double(x["AmtUsed"].ToString())));
			}
			if(ODBuild.IsDebug()) {
				sw.Stop();
				Console.WriteLine("Get dictPatInfo: "+sw.Elapsed.TotalSeconds+" sec, Count: "+dictPatInfo.Count);
				sw=Stopwatch.StartNew();
			}
			//dictionary with Key=InsSubNum, Value=Tuple(AmtPending,AmtUsed)
			Dictionary<long,Tuple<double,double>> dictFamInfo=new Dictionary<long,Tuple<double,double>>();
			using(DataTable tableFam=GetFamInfo(isProcsGeneral,renewDate,patNumStr)) {
				dictFamInfo=tableFam.Select().ToDictionary(x => PIn.Long(x["InsSubNum"].ToString()),
					x => Tuple.Create(PIn.Double(x["AmtPending"].ToString()),PIn.Double(x["AmtUsed"].ToString())));
			}
			if(ODBuild.IsDebug()) {
				sw.Stop();
				Console.WriteLine("Get dictFamInfo: "+sw.Elapsed.TotalSeconds+" sec, Rows: "+dictFamInfo.Count);
				sw=Stopwatch.StartNew();
			}
			//dictionary with Key=PlanNum, Value=Tuple(AnnualMaxInd,AnnualMaxFam)
			Dictionary<long,Tuple<double,double>> dictAnnualMax=new Dictionary<long,Tuple<double,double>>();
			using(DataTable tableAnnualMax=GetAnnualMaxInfo(patNumStr)) {
				dictAnnualMax=tableAnnualMax.Select().ToDictionary(x => PIn.Long(x["PlanNum"].ToString()),
					x => Tuple.Create(PIn.Double(x["AnnualMaxInd"].ToString()),PIn.Double(x["AnnualMaxFam"].ToString())));
			}
			if(ODBuild.IsDebug()) {
				sw.Stop();
				Console.WriteLine("Get dictAnnualMax: "+sw.Elapsed.TotalSeconds+" sec, Rows: "+dictAnnualMax.Count);
				sw=Stopwatch.StartNew();
			}
			using(DataTable rawtable=GetTableRaw(noIns,monthStart,patNumStr)) {
				if(ODBuild.IsDebug()) {
					sw.Stop();
					Console.WriteLine("Get RawTable: "+sw.Elapsed.TotalSeconds+" sec, Rows: "+rawtable.Rows.Count);
					sw=Stopwatch.StartNew();
				}
				DataRow row;
				foreach(DataRow rawRow in rawtable.Rows) {
					row=table.NewRow();
					long patNum=PIn.Long(rawRow["PatNum"].ToString());
					long patPlanNum=PIn.Long(rawRow["PatPlanNum"].ToString());
					long planNum=PIn.Long(rawRow["PlanNum"].ToString());
					long insSubNum=PIn.Long(rawRow["InsSubNum"].ToString());
					double amtPlanned=dictAmtPlanned.TryGetValue(patNum,out amtPlanned)?amtPlanned:0;
					Tuple<double,double> tuplePatInfo=dictPatInfo.TryGetValue(patPlanNum,out tuplePatInfo)?tuplePatInfo:Tuple.Create(0d,0d);
					double patAmtPending=tuplePatInfo.Item1;
					double patAmtUsed=tuplePatInfo.Item2;
					Tuple<double,double> tupleFamInfo=dictFamInfo.TryGetValue(insSubNum,out tupleFamInfo)?tupleFamInfo:Tuple.Create(0d,0d);
					double famAmtPending=tupleFamInfo.Item1;
					double famAmtUsed=tupleFamInfo.Item2;
					Tuple<double,double> tupleAnnualMax=dictAnnualMax.TryGetValue(planNum,out tupleAnnualMax)?tupleAnnualMax:Tuple.Create(0d,0d);
					double patAnnualMax=tupleAnnualMax.Item1;
					double famAnnualMax=tupleAnnualMax.Item2;
					if(aboveAmount>0) {
						if(dictAnnualMax.ContainsKey(planNum)
							&& ((patAnnualMax!=-1 && patAnnualMax-patAmtUsed<=aboveAmount) || (famAnnualMax!=-1 && famAnnualMax-famAmtUsed<=aboveAmount)))
						{
							continue;
						}
					}
					row["PatNum"]=patNum;
					row["LName"]=rawRow["LName"].ToString();
					row["FName"]=rawRow["FName"].ToString();
					ContactMethod contmeth=PIn.Enum<ContactMethod>(rawRow["PreferRecallMethod"].ToString());
					switch(contmeth) {
						case ContactMethod.None:
							if(PrefC.GetBool(PrefName.RecallUseEmailIfHasEmailAddress) && !string.IsNullOrEmpty(rawRow["Email"].ToString())) {
								row["contactMethod"]=rawRow["Email"].ToString();
							}
							else {
								row["contactMethod"]=Lans.g("FormRecallList","Hm:")+rawRow["HmPhone"].ToString();
							}
							break;
						case ContactMethod.HmPhone:
							row["contactMethod"]=Lans.g("FormRecallList","Hm:")+rawRow["HmPhone"].ToString();
							break;
						case ContactMethod.WkPhone:
							row["contactMethod"]=Lans.g("FormRecallList","Wk:")+rawRow["WkPhone"].ToString();
							break;
						case ContactMethod.WirelessPh:
							row["contactMethod"]=Lans.g("FormRecallList","Cell:")+rawRow["WirelessPhone"].ToString();
							break;
						case ContactMethod.Email:
							row["contactMethod"]=rawRow["Email"].ToString();
							break;
						case ContactMethod.Mail:
						case ContactMethod.DoNotCall:
						case ContactMethod.SeeNotes:
							row["contactMethod"]=Lans.g("enumContactMethod",contmeth.ToString());
							break;
					}
					row["address"]=rawRow["Address"].ToString()+(string.IsNullOrEmpty(rawRow["Address2"].ToString())?"":("\r\n"+rawRow["Address2"].ToString()));
					row["City"]=rawRow["City"].ToString();
					row["State"]=rawRow["State"].ToString();
					row["Zip"]=rawRow["Zip"].ToString();
					row["annualMaxInd"]=patAnnualMax.ToString("N");
					row["annualMaxFam"]=famAnnualMax.ToString("N");
					row["amountUsedInd"]=patAmtUsed.ToString("N");
					row["amountUsedFam"]=famAmtUsed.ToString("N");
					row["amountPendingInd"]=patAmtPending.ToString("N");
					row["amountPendingFam"]=famAmtPending.ToString("N");
					row["amountRemainingInd"]=(patAnnualMax-patAmtUsed-patAmtPending).ToString("N");
					row["amountRemainingFam"]=(famAnnualMax-famAmtUsed-famAmtPending).ToString("N");
					row["treatmentPlan"]=amtPlanned.ToString("N");
					row["carrierName"]=rawRow["carrierName"].ToString();
					if(PrefC.HasClinicsEnabled) {
						row["clinicAbbr"]=rawRow["clinicAbbr"].ToString();
					}
					table.Rows.Add(row);
				}
			}
			if(ODBuild.IsDebug()) {
				sw.Stop();
				sTotal.Stop();
				Console.WriteLine("Finished Filling DataTable: {0}\r\n\tTotal time: {1}\r\n\tRows: {2}",
					(sw.Elapsed.Minutes>0?(sw.Elapsed.Minutes+" min "):"")+(sw.Elapsed.TotalSeconds-sw.Elapsed.Minutes*60)+" sec",
					(sTotal.Elapsed.Minutes>0?(sTotal.Elapsed.Minutes+" min "):"")+(sTotal.Elapsed.TotalSeconds-sTotal.Elapsed.Minutes*60)+" sec",
					table.Rows.Count);
			}
			return table;
		}

		public static DataTable GetDictAmtPlanned(bool patsWithAppts,DateTime dateFrom,DateTime dateTo,List<long> listProvNums,List<long> listBillTypes,
			string code1,string code2,List<long> listClinicNums,bool useTreatingProvider)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patsWithAppts,dateFrom,dateTo,listProvNums,listBillTypes,code1,code2,listClinicNums,useTreatingProvider);
			}
			string command=$@"SELECT procedurelog.PatNum,SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)) AmtPlanned
				FROM procedurelog
				INNER JOIN patient ON patient.PatNum=procedurelog.PatNum{(string.IsNullOrEmpty(code1)?"":$@"
				INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum")}
				WHERE procedurelog.ProcStatus={(int)ProcStat.TP}
				AND patient.PatStatus={POut.Int((int)PatientStatus.Patient)}{(string.IsNullOrEmpty(code1)?"":$@"
				AND procedurecode.ProcCode>='{POut.String(code1)}'
				AND procedurecode.ProcCode<='{POut.String(code2)}'")}
				{(dateFrom.Year<=1880?"":$@"AND procedurelog.DateTP>={POut.DateT(dateFrom)}")}
				{((dateTo.Year<=1880 || dateFrom>dateTo)?"":$@"AND procedurelog.DateTP<={POut.DateT(dateTo)}")}
				{(listProvNums.IsNullOrEmpty() || listProvNums.Contains(0)?"":$@"
				AND {(useTreatingProvider?"procedurelog.ProvNum":"patient.PriProv")} IN ({string.Join(",",listProvNums)})")}{(listBillTypes.IsNullOrEmpty() || listBillTypes.Contains(0)?"":$@"
				AND patient.BillingType IN ({string.Join(",",listBillTypes)})")}{(!PrefC.HasClinicsEnabled || listClinicNums.IsNullOrEmpty()?"":$@"
				AND patient.ClinicNum IN ({string.Join(",",listClinicNums)})")}{(patsWithAppts?"":$@"
				AND procedurelog.PatNum NOT IN (
					SELECT PatNum FROM appointment
					WHERE appointment.AptStatus={POut.Int((int)ApptStatus.Scheduled)}
					AND appointment.AptDateTime>={DbHelper.Curdate()})")}
				GROUP BY procedurelog.PatNum
				HAVING AmtPlanned>0
				ORDER BY NULL";//Removes filesort reference from query explain
			return Db.GetTable(command);
		}

		public static DataTable GetPatInfo(bool isProcsGeneral,DateTime renewDate,string patNumStr) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),isProcsGeneral,renewDate,patNumStr);
			}
			string commandIndInfo=$@"SELECT patplan.PatPlanNum,
				{GetSelectCoverageStr()}
				INNER JOIN patplan ON patplan.PatNum=claimproc.PatNum
					AND patplan.InsSubNum=claimproc.InsSubNum
				{GetWhereCoverageStr(isProcsGeneral,renewDate,patNumStr)}
				GROUP BY patplan.PatPlanNum
				ORDER BY NULL";//Removes filesort reference from query explain
			return Db.GetTable(commandIndInfo);
		}

		public static DataTable GetFamInfo(bool isProcsGeneral,DateTime renewDate,string patNumStr) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),isProcsGeneral,renewDate,patNumStr);
			}
			string commandFamInfo=$@"SELECT claimproc.InsSubNum,
				{GetSelectCoverageStr()}
				{GetWhereCoverageStr(isProcsGeneral,renewDate,patNumStr)}
				GROUP BY claimproc.InsSubNum
				ORDER BY NULL";//Removes filesort reference from query explain
			return Db.GetTable(commandFamInfo);
		}

		private static string GetSelectCoverageStr() {
			//No remoting role check; no call to db
			return $@"SUM(CASE WHEN claimproc.Status={POut.Int((int)ClaimProcStatus.NotReceived)} AND claimproc.InsPayAmt=0 "
					+$@"THEN claimproc.InsPayEst ELSE 0 END) AmtPending,
				SUM(CASE WHEN claimproc.Status IN ({POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Adjustment)},"
					+$@"{POut.Int((int)ClaimProcStatus.Supplemental)}) THEN claimproc.InsPayAmt ELSE 0 END) AmtUsed
				FROM claimproc";
		}

		///<summary>Returns the query string of the coverage section for the patients.  It is known that patNumStr is not empty</summary>
		private static string GetWhereCoverageStr(bool isProcsGeneral,DateTime renewDate,string patNumStr) {
			//No remoting role check; no call to db
			return (isProcsGeneral?"":$@"LEFT JOIN procedurelog pl ON pl.ProcNum=claimproc.ProcNum
				LEFT JOIN procedurecode pc ON pc.CodeNum=pl.CodeNum
				LEFT JOIN (
					SELECT inssub.InsSubNum,COALESCE(cp.FromCode,pc.ProcCode) AS FromCode,COALESCE(cp.ToCode,pc.ProcCode) AS ToCode
					FROM inssub
					INNER JOIN benefit b ON b.PlanNum=inssub.PlanNum
					LEFT JOIN covcat cc ON cc.CovCatNum=b.CovCatNum 
					LEFT JOIN covspan cp ON cp.CovCatNum=cc.CovCatNum
					LEFT JOIN procedurecode pc ON pc.CodeNum=b.CodeNum
					WHERE b.BenefitType={(int)InsBenefitType.Limitations}
					AND b.QuantityQualifier={(int)BenefitQuantity.None}
					AND b.TimePeriod IN ({(int)BenefitTimePeriod.ServiceYear},{(int)BenefitTimePeriod.CalendarYear})
					AND (cc.CovCatNum IS NOT NULL OR b.CodeNum!=0)
				) procCheck ON procCheck.InsSubNum=claimproc.InsSubNum AND pc.ProcCode BETWEEN procCheck.FromCode AND procCheck.ToCode
				")
				+$@"WHERE claimproc.Status IN ({POut.Int((int)ClaimProcStatus.NotReceived)},{POut.Int((int)ClaimProcStatus.Received)},"
					+$@"{POut.Int((int)ClaimProcStatus.Adjustment)},{POut.Int((int)ClaimProcStatus.Supplemental)})
				AND claimproc.ProcDate BETWEEN {POut.Date(renewDate)} AND {POut.Date(renewDate.AddYears(1).AddDays(-1))}
				AND claimproc.PatNum IN ({patNumStr}){(isProcsGeneral?"":$@"
				AND procCheck.InsSubNum IS NULL")}";
		}
		
		///<summary>It is known that patNumStr is not empty</summary>
		public static DataTable GetAnnualMaxInfo(string patNumStr) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNumStr);
			}
			string commandAnnualMax=$@"SELECT benefit.PlanNum,
				MAX(CASE WHEN CoverageLevel!={POut.Int((int)BenefitCoverageLevel.Family)} THEN MonetaryAmt ELSE -1 END) AnnualMaxInd, 
				MAX(CASE WHEN CoverageLevel={POut.Int((int)BenefitCoverageLevel.Family)} THEN MonetaryAmt ELSE -1 END) AnnualMaxFam
				FROM benefit
				INNER JOIN inssub ON inssub.PlanNum=benefit.PlanNum
				INNER JOIN patplan ON patplan.InsSubNum=inssub.InsSubNum
				LEFT JOIN covcat ON benefit.CovCatNum=covcat.CovCatNum
				WHERE COALESCE(covcat.EbenefitCat,{POut.Int((int)EbenefitCategory.General)})={POut.Int((int)EbenefitCategory.General)}
				AND benefit.BenefitType={POut.Int((int)InsBenefitType.Limitations)} 
				AND benefit.MonetaryAmt>0
				AND benefit.QuantityQualifier={POut.Int((int)BenefitQuantity.None)}
				AND patplan.PatNum IN ({patNumStr})
				GROUP BY benefit.PlanNum
				ORDER BY NULL";//Removes filesort reference from query explain
			return Db.GetTable(commandAnnualMax);
		}
		
		///<summary>It is known that patNumStr is not empty</summary>
		public static DataTable GetTableRaw(bool noIns,int monthStart,string patNumStr) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),noIns,monthStart,patNumStr);
			}
			string command=$@"SELECT patient.PatNum,patient.LName,patient.FName,patient.Email,patient.HmPhone,patient.WirelessPhone,patient.WkPhone,
				patient.PreferRecallMethod,patient.Address,patient.Address2,patient.City,patient.State,patient.Zip,patient.PriProv,patient.BillingType,
				patplan.PatPlanNum,inssub.InsSubNum,inssub.PlanNum,COALESCE(carrier.CarrierName,'') carrierName,COALESCE(clinic.Abbr,'Unassigned') clinicAbbr
				FROM patient
				LEFT JOIN patplan ON patient.PatNum=patplan.PatNum
				LEFT JOIN inssub ON patplan.InsSubNum=inssub.InsSubNum
				LEFT JOIN insplan ON insplan.PlanNum=inssub.PlanNum
				LEFT JOIN carrier ON insplan.CarrierNum=carrier.CarrierNum
				LEFT JOIN clinic ON clinic.ClinicNum=patient.ClinicNum
				WHERE patient.PatStatus={POut.Int((int)PatientStatus.Patient)}
				AND patient.PatNum IN ({patNumStr}){(noIns?"":$@"
				AND patplan.Ordinal=1 AND insplan.MonthRenew={POut.Int(monthStart)}")}";
			return Db.GetTable(command);
		}

	}
}
