using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class RpProdInc {
		///<summary>Used in RowComparer to limit using the cache in the CEMT.</summary>
		private static bool _hasClinics;

		//IMPORTANT NOTE FOR ANYBODY WHO CODES IN HERE:  This is used in the CEMT so everything MUST be coded in such a way that they don't use the 
		//cache to look up information.  The CEMT does NOT keep copies of the remote database caches when this is used so things such as 
		//PrefC.GetBool or Clinics.GetDesc will return incorrect results.

		#region Daily and Provider P&I Reports
		///<summary>If not using clinics then supply an empty list of clinicNums.  Also used for the CEMT Provider P and I report.  
		///showUnearedCEMT should only be considered from CEMT (or if you have a "Show Unearned" checkbox, which currently only applies to CEMT).</summary>
		//////<param name="hasChangeInWriteoff">If true, then this will include Write Off Est and Write Off Adjustments instead of Write Off.</param>
		public static DataSet GetDailyData(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool hasAllProvs
			,bool hasAllClinics,bool hasBreakdown,bool hasClinicInfo,bool isUnearnedIncluded,PPOWriteoffDateCalc writeoffType,bool isCEMT=false) 
		{
			//No need to check RemotingRole; no call to db.
			if(listClinics.Count>0) {
				_hasClinics=true;
			}
			if(!hasClinicInfo) {
				_hasClinics=false;
			}
			DataSet dataSet=GetDailyProdIncDataSet(dateFrom,dateTo,listProvs,listClinics,hasAllProvs,hasAllClinics,isUnearnedIncluded,writeoffType,isCEMT);
			DataTable tableProduction=dataSet.Tables["tableProduction"];
			DataTable tableAdj=dataSet.Tables["tableAdj"];
			DataTable tableInsWriteoff=dataSet.Tables["tableInsWriteoff"];
			DataTable tablePay=dataSet.Tables["tablePay"];
			DataTable tableIns=dataSet.Tables["tableIns"];
			DataTable tableWriteOffAdjustments=dataSet.Tables["tableWriteOffAdjustments"];
			DataTable tableDailyProd=new DataTable("DailyProd");
			tableDailyProd.Columns.Add(new DataColumn("Date"));
			tableDailyProd.Columns.Add(new DataColumn("Name"));
			tableDailyProd.Columns.Add(new DataColumn("Description"));
			tableDailyProd.Columns.Add(new DataColumn("Provider"));
			if(_hasClinics) {
				tableDailyProd.Columns.Add(new DataColumn("Clinic"));
			}
			tableDailyProd.Columns.Add(new DataColumn("Production",typeof(double)));
			tableDailyProd.Columns.Add(new DataColumn("Adjust",typeof(double)));
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				tableDailyProd.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				tableDailyProd.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				tableDailyProd.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			tableDailyProd.Columns.Add(new DataColumn("Pt Income",typeof(double)));
			tableDailyProd.Columns.Add(new DataColumn("Ins Income",typeof(double)));
			tableDailyProd.Columns.Add(new DataColumn("ClinicSplit"));
			for(int i=0;i<tableProduction.Rows.Count;i++) {
				if(_hasClinics && !listClinics.Exists(x => x.ClinicNum==PIn.Long(tableProduction.Rows[i]["Clinic"].ToString()))) {
					continue;//Using clinics and the current row is for a clinic that is NOT in the list of clinics we care about.
				}
				DataRow row=tableDailyProd.NewRow();
				row["Date"]=PIn.Date(tableProduction.Rows[i]["Date"].ToString()).ToShortDateString();
				row["Name"]=tableProduction.Rows[i]["namelf"].ToString();
				row["Description"]=tableProduction.Rows[i]["Description"].ToString();
				row["Provider"]=tableProduction.Rows[i]["Abbr"].ToString();
				if(_hasClinics) {
					row["Clinic"]=tableProduction.Rows[i]["Clinic"].ToString();
				}
				row["Production"]=tableProduction.Rows[i]["Production"].ToString();
				row["Adjust"]=0;
				if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
					row["Writeoff Est"]=0;
					row["Writeoff Adj"]=0;
				}
				else {
					row["Writeoff"]=0;
				}				
				row["Pt Income"]=0;
				row["Ins Income"]=0;
				row["ClinicSplit"]=hasBreakdown ? tableProduction.Rows[i]["Clinic"].ToString():"";
				tableDailyProd.Rows.Add(row);
			}
			for(int i=0;i<tableAdj.Rows.Count;i++) {
				if(_hasClinics && !listClinics.Exists(x => x.ClinicNum==PIn.Long(tableAdj.Rows[i]["Clinic"].ToString()))) {
					continue;//Using clinics and the current row is for a clinic that is NOT in the list of clinics we care about.
				}
				DataRow row=tableDailyProd.NewRow();
				row["Date"]=PIn.Date(tableAdj.Rows[i]["Date"].ToString()).ToShortDateString();
				row["Name"]=tableAdj.Rows[i]["namelf"].ToString();
				row["Description"]=tableAdj.Rows[i]["Description"].ToString();
				row["Provider"]=tableAdj.Rows[i]["Abbr"].ToString();
				if(_hasClinics) {
					row["Clinic"]=tableAdj.Rows[i]["Clinic"].ToString();
				}
				row["Production"]=0;
				row["Adjust"]=tableAdj.Rows[i]["AdjAmt"].ToString();
				if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
					row["Writeoff Est"]=0;
					row["Writeoff Adj"]=0;
				}
				else {
					row["Writeoff"]=0;
				}
				row["Pt Income"]=0;
				row["Ins Income"]=0;
				row["ClinicSplit"]=hasBreakdown ? tableAdj.Rows[i]["Clinic"].ToString():"";
				tableDailyProd.Rows.Add(row);
			}
			for(int i=0;i<tableInsWriteoff.Rows.Count;i++) {
				if(_hasClinics && !listClinics.Exists(x => x.ClinicNum==PIn.Long(tableInsWriteoff.Rows[i]["Clinic"].ToString()))) {
					continue;//Using clinics and the current row is for a clinic that is NOT in the list of clinics we care about.
				}
				DataRow row=tableDailyProd.NewRow();
				row["Date"]=PIn.Date(tableInsWriteoff.Rows[i]["Date"].ToString()).ToShortDateString();
				row["Name"]=tableInsWriteoff.Rows[i]["namelf"].ToString();
				row["Description"]=tableInsWriteoff.Rows[i]["Description"].ToString();
				row["Provider"]=tableInsWriteoff.Rows[i]["Abbr"].ToString();
				if(_hasClinics) {
					row["Clinic"]=tableInsWriteoff.Rows[i]["Clinic"].ToString();
				}
				row["Production"]=0;
				row["Adjust"]=0;
				if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
					row["Writeoff Est"]=tableInsWriteoff.Rows[i]["WriteOffEst"].ToString();
					row["Writeoff Adj"]=0;
				}
				else {
					row["Writeoff"]=tableInsWriteoff.Rows[i]["WriteOff"].ToString();
				}
				row["Pt Income"]=0;
				row["Ins Income"]=0;
				row["ClinicSplit"]=hasBreakdown ? tableInsWriteoff.Rows[i]["Clinic"].ToString():"";
				tableDailyProd.Rows.Add(row);
			}
			for(int i = 0;i<tableWriteOffAdjustments.Rows.Count;i++) {
				if(_hasClinics && !listClinics.Exists(x => x.ClinicNum==PIn.Long(tableWriteOffAdjustments.Rows[i]["Clinic"].ToString()))) {
					continue;//Using clinics and the current row is for a clinic that is NOT in the list of clinics we care about.
				}
				DataRow row=tableDailyProd.NewRow();
				row["Date"]=PIn.Date(tableWriteOffAdjustments.Rows[i]["Date"].ToString()).ToShortDateString();
				row["Name"]=tableWriteOffAdjustments.Rows[i]["namelf"].ToString();
				row["Description"]=tableWriteOffAdjustments.Rows[i]["Description"].ToString();
				row["Provider"]=tableWriteOffAdjustments.Rows[i]["Abbr"].ToString();
				if(_hasClinics) {
					row["Clinic"]=tableWriteOffAdjustments.Rows[i]["Clinic"].ToString();
				}
				row["Production"]=0;
				row["Adjust"]=0;
				row["Writeoff Est"]=0;
				double adjust=PIn.Double(tableWriteOffAdjustments.Rows[i]["WriteOffEst"].ToString())+PIn.Double(tableWriteOffAdjustments.Rows[i]["WriteOff"].ToString());
				row["Writeoff Adj"]=adjust;
				row["Pt Income"]=0;
				row["Ins Income"]=0;
				row["ClinicSplit"]=hasBreakdown ? tableWriteOffAdjustments.Rows[i]["Clinic"].ToString() : "";
				if(!CompareDouble.IsEqual(adjust,0)) {
					tableDailyProd.Rows.Add(row);
				}
			}
			for(int i=0;i<tablePay.Rows.Count;i++) {
				if(_hasClinics && !listClinics.Exists(x => x.ClinicNum==PIn.Long(tablePay.Rows[i]["Clinic"].ToString()))) {
					continue;//Using clinics and the current row is for a clinic that is NOT in the list of clinics we care about.
				}
				DataRow row=tableDailyProd.NewRow();
				row["Date"]=PIn.Date(tablePay.Rows[i]["Date"].ToString()).ToShortDateString();
				row["Name"]=tablePay.Rows[i]["namelf"].ToString();
				row["Description"]=tablePay.Rows[i]["Description"].ToString();
				row["Provider"]=tablePay.Rows[i]["Abbr"].ToString();
				if(_hasClinics) {
					row["Clinic"]=tablePay.Rows[i]["Clinic"].ToString();
				}
				row["Production"]=0;
				row["Adjust"]=0;
				if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
					row["Writeoff Est"]=0;
					row["Writeoff Adj"]=0;
				}
				else {
					row["Writeoff"]=0;
				}
				row["Pt Income"]=(PIn.Decimal(tablePay.Rows[i]["UnearnedIncome"].ToString())+PIn.Decimal(tablePay.Rows[i]["PayAmt"].ToString()))
					.ToString();
				row["Ins Income"]=0;
				row["ClinicSplit"]=hasBreakdown ? tablePay.Rows[i]["Clinic"].ToString():"";
				tableDailyProd.Rows.Add(row);
			}
			for(int i=0;i<tableIns.Rows.Count;i++) {
				if(_hasClinics && !listClinics.Exists(x => x.ClinicNum==PIn.Long(tableIns.Rows[i]["Clinic"].ToString()))) {
					continue;//Using clinics and the current row is for a clinic that is NOT in the list of clinics we care about.
				}
				DataRow row=tableDailyProd.NewRow();
				row["Date"]=PIn.Date(tableIns.Rows[i]["Date"].ToString()).ToShortDateString();
				row["Name"]=tableIns.Rows[i]["namelf"].ToString();
				row["Description"]=tableIns.Rows[i]["Description"].ToString();
				row["Provider"]=tableIns.Rows[i]["Abbr"].ToString();
				if(_hasClinics) {
					row["Clinic"]=tableIns.Rows[i]["Clinic"].ToString();
				}
				row["Production"]=0;
				row["Adjust"]=0;
				if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
					row["Writeoff Est"]=0;
					row["Writeoff Adj"]=0;
				}
				else {
					row["Writeoff"]=0;
				}
				row["Pt Income"]=0;
				row["Ins Income"]=tableIns.Rows[i]["InsPayAmt"].ToString();
				row["ClinicSplit"]=hasBreakdown ? tableIns.Rows[i]["Clinic"].ToString():"";
				tableDailyProd.Rows.Add(row);
			}
			List<DataRow> listTableDailyProdRows=new List<DataRow>();
			for(int i=0;i<tableDailyProd.Rows.Count;i++) {
				listTableDailyProdRows.Add(tableDailyProd.Rows[i]);
			}
			listTableDailyProdRows.Sort(RowComparer);
			DataTable tableDailyProdSorted=tableDailyProd.Clone();
			tableDailyProdSorted.Rows.Clear();
			for(int i=0;i<listTableDailyProdRows.Count;i++) {
				tableDailyProdSorted.Rows.Add(listTableDailyProdRows[i].ItemArray);
				//Replace the ClinicNum with the actual description of the clinic.
				if(_hasClinics) {
					string clinicDesc=listClinics.Find(x => x.ClinicNum==PIn.Long(tableDailyProdSorted.Rows[i]["Clinic"].ToString())).Description;
					tableDailyProdSorted.Rows[i]["Clinic"]=clinicDesc=="" ? Lans.g("FormRpProdInc","Unassigned"):clinicDesc;
					if(hasBreakdown) {
						tableDailyProdSorted.Rows[i]["ClinicSplit"]=clinicDesc=="" ? Lans.g("FormRpProdInc","Unassigned"):clinicDesc;
					}
				}
			}
			DataSet ds=new DataSet("DailyData");
			ds.Tables.Add(tableDailyProdSorted);
			return ds;
		}

		///<summary>isCEMT should only be considered from CEMT or if you have a "Show Uneared" checkbox (which currently only applies to CEMT).</summary>
		public static DataSet GetProviderDataForClinics(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics
			,bool hasAllProvs,bool hasAllClinics,bool isUnearnedIncluded,PPOWriteoffDateCalc writeoffType,bool isCEMT=false) 
		{
			if(listClinics.Count>0) {
				_hasClinics=true;
			}
			DataSet dataSet=GetDailyProdIncDataSet(dateFrom,dateTo,listProvs,listClinics,hasAllProvs,hasAllClinics,isUnearnedIncluded,writeoffType,isCEMT);
			DataTable tableProduction=dataSet.Tables["tableProduction"];
			DataTable tableAdj=dataSet.Tables["tableAdj"];
			DataTable tableInsWriteoff=dataSet.Tables["tableInsWriteoff"];
			DataTable tablePay=dataSet.Tables["tablePay"];
			DataTable tableIns=dataSet.Tables["tableIns"];
			DataTable tableWriteOffAdjustments=dataSet.Tables["tableWriteOffAdjustments"];
			decimal production;
			decimal adjust;
			decimal inswriteoffest;
			decimal inswriteoff;
			decimal inswriteoffadj;
			decimal totalproduction;
			decimal ptincome;
			decimal unearnedPtIncome;
			decimal totalPtIncome;
			decimal insincome;
			decimal totalincome;
			DataTable dt=new DataTable("Total");
			dt.Columns.Add(new DataColumn("Provider"));
			dt.Columns.Add(new DataColumn("Production",typeof(double)));
			dt.Columns.Add(new DataColumn("Adjustments",typeof(double)));
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				dt.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				dt.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				dt.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			dt.Columns.Add(new DataColumn("Tot Prod",typeof(double)));
			dt.Columns.Add(new DataColumn("Pt Income",typeof(double)));
			if(isUnearnedIncluded) {
				dt.Columns.Add(new DataColumn("Unearned Pt Income",typeof(double)));
			}
			dt.Columns.Add(new DataColumn("Ins Income",typeof(double)));
			dt.Columns.Add(new DataColumn("Total Income",typeof(double)));
			dt.Columns.Add(new DataColumn("Total Pt Income",typeof(double)));
			DataTable dtClinic=new DataTable("Clinic");
			dtClinic.Columns.Add(new DataColumn("Provider"));
			dtClinic.Columns.Add(new DataColumn("Production",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Adjustments",typeof(double)));
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				dtClinic.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				dtClinic.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				dtClinic.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			dtClinic.Columns.Add(new DataColumn("Tot Prod",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Pt Income",typeof(double)));
			if(isUnearnedIncluded) {
				dtClinic.Columns.Add(new DataColumn("Unearned Pt Income",typeof(double)));
			}
			dtClinic.Columns.Add(new DataColumn("Ins Income",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Total Income",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Clinic"));
			dtClinic.Columns.Add(new DataColumn("Total Pt Income",typeof(double)));
			//length of array is number of months between the two dates plus one.
			//The from date and to date will not be more than one year and must will be within the same year due to FormRpProdInc UI validation enforcement.
			//Get a list of clinics so that we have access to their descriptions for the report.
			bool hasData;
			//09-17-2021 Jason Salmon - Jordan approved the usage of dictionaries within this method for speed improvements. See B31372.
			#region Group Tables by Clinic
			Dictionary<long,List<DataRow>> dictionaryClinicTableProduction=tableProduction.Select()
				.GroupBy(x => PIn.Long(x["Clinic"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryClinicTableAdj=tableAdj.Select()
				.GroupBy(x => PIn.Long(x["Clinic"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryClinicTableInsWriteoff=tableInsWriteoff.Select()
				.GroupBy(x => PIn.Long(x["Clinic"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryClinicTablePay=tablePay.Select()
				.GroupBy(x => PIn.Long(x["Clinic"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryClinicTableIns=tableIns.Select()
				.GroupBy(x => PIn.Long(x["Clinic"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryClinicTableWriteOffAdjustments=tableWriteOffAdjustments.Select()
				.GroupBy(x => PIn.Long(x["Clinic"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			#endregion
			for(int it=0;it<listClinics.Count;it++) {//For each clinic
				#region Group Clinic Rows by Provider
				Dictionary<long,List<DataRow>> dictionaryProvNumTableProduction=new Dictionary<long,List<DataRow>>();
				if(dictionaryClinicTableProduction.TryGetValue(listClinics[it].ClinicNum,out List<DataRow> listDataRowsProductionForClinic)) {
					dictionaryProvNumTableProduction=listDataRowsProductionForClinic.GroupBy(x => PIn.Long(x["ProvNum"].ToString())).ToDictionary(x => x.Key,x => x.ToList());
				}
				Dictionary<long,List<DataRow>> dictionaryProvNumTableAdj=new Dictionary<long,List<DataRow>>();
				if(dictionaryClinicTableAdj.TryGetValue(listClinics[it].ClinicNum,out List<DataRow> listDataRowsAdjForClinic)) {
					dictionaryProvNumTableAdj=listDataRowsAdjForClinic.GroupBy(x => PIn.Long(x["ProvNum"].ToString())).ToDictionary(x => x.Key,x => x.ToList());
				}
				Dictionary<long,List<DataRow>> dictionaryProvNumTableInsWriteoff=new Dictionary<long,List<DataRow>>();
				if(dictionaryClinicTableInsWriteoff.TryGetValue(listClinics[it].ClinicNum,out List<DataRow> listDataRowsInsWriteoffForClinic)) {
					dictionaryProvNumTableInsWriteoff=listDataRowsInsWriteoffForClinic.GroupBy(x => PIn.Long(x["ProvNum"].ToString())).ToDictionary(x => x.Key,x => x.ToList());
				}
				Dictionary<long,List<DataRow>> dictionaryProvNumTableWriteOffAdjustments=new Dictionary<long,List<DataRow>>();
				if(dictionaryClinicTableWriteOffAdjustments.TryGetValue(listClinics[it].ClinicNum,out List<DataRow> listDataRowsWriteOffAdjustmentsForClinic)) {
					dictionaryProvNumTableWriteOffAdjustments=listDataRowsWriteOffAdjustmentsForClinic.GroupBy(x => PIn.Long(x["ProvNum"].ToString())).ToDictionary(x => x.Key,x => x.ToList());
				}
				Dictionary<long,List<DataRow>> dictionaryProvNumTablePay=new Dictionary<long,List<DataRow>>();
				if(dictionaryClinicTablePay.TryGetValue(listClinics[it].ClinicNum,out List<DataRow> listDataRowsPayForClinic)) {
					dictionaryProvNumTablePay=listDataRowsPayForClinic.GroupBy(x => PIn.Long(x["ProvNum"].ToString())).ToDictionary(x => x.Key,x => x.ToList());
				}
				Dictionary<long,List<DataRow>> dictionaryProvNumTableIns=new Dictionary<long,List<DataRow>>();
				if(dictionaryClinicTableIns.TryGetValue(listClinics[it].ClinicNum,out List<DataRow> listDataRowsInsForClinic)) {
					dictionaryProvNumTableIns=listDataRowsInsForClinic.GroupBy(x => PIn.Long(x["ProvNum"].ToString())).ToDictionary(x => x.Key,x => x.ToList());
				}
				#endregion
				for(int i=0;i<listProvs.Count;i++) {
					Provider provCur=listProvs[i];
					hasData=false;
					DataRow row=dtClinic.NewRow();
					row["Provider"]=provCur.Abbr;
					production=0;
					adjust=0;
					inswriteoffest=0;
					inswriteoff=0;
					inswriteoffadj=0;
					totalproduction=0;
					ptincome=0;
					unearnedPtIncome=0;
					totalPtIncome=0;
					insincome=0;
					totalincome=0;
					if(dictionaryProvNumTableProduction.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsProductionForProvAndClinic)) {
						production=listDataRowsProductionForProvAndClinic.Sum(x => PIn.Decimal(x["Production"].ToString()));
						hasData=true;
					}
					if(dictionaryProvNumTableAdj.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsAdjForProvAndClinic)) {
						adjust=listDataRowsAdjForProvAndClinic.Sum(x => PIn.Decimal(x["AdjAmt"].ToString()));
						hasData=true;
					}
					if(dictionaryProvNumTableInsWriteoff.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsInsWriteoffForProvAndClinic)) {
						if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
							inswriteoffest=listDataRowsInsWriteoffForProvAndClinic.Sum(x => PIn.Decimal(x["WriteOffEst"].ToString()));
						}
						else {
							inswriteoff=listDataRowsInsWriteoffForProvAndClinic.Sum(x => PIn.Decimal(x["WriteOff"].ToString()));
						}
						hasData=true;
					}
					if(dictionaryProvNumTableWriteOffAdjustments.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsWriteOffAdjustmentsForProvAndClinic)) {
						inswriteoffadj=listDataRowsWriteOffAdjustmentsForProvAndClinic.Sum(x => PIn.Decimal(x["WriteOffEst"].ToString()) + PIn.Decimal(x["WriteOff"].ToString()));
						hasData=true;
					}
					if(dictionaryProvNumTablePay.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsPayForProvAndClinic)) {
						ptincome=listDataRowsPayForProvAndClinic.Sum(x => PIn.Decimal(x["PayAmt"].ToString()));
						unearnedPtIncome=listDataRowsPayForProvAndClinic.Sum(x => PIn.Decimal(x["UnearnedIncome"].ToString()));
						hasData=true;
					}
					if(dictionaryProvNumTableIns.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsInsForProvAndClinic)) {
						insincome=listDataRowsInsForProvAndClinic.Sum(x => PIn.Decimal(x["InsPayAmt"].ToString()));
						hasData=true;
					}
					if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
						totalproduction=production+adjust+inswriteoffest+inswriteoffadj;
					}
					else {
						totalproduction=production+adjust+inswriteoff;
					}
					totalPtIncome=ptincome+unearnedPtIncome;
					totalincome=totalPtIncome+insincome;
					string clinicDesc=listClinics[it].Description;
					row["Production"]=production.ToString("n");
					row["Adjustments"]=adjust.ToString("n");
					if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
						row["Writeoff Est"]=inswriteoffest.ToString("n");
						row["Writeoff Adj"]=inswriteoffadj.ToString("n");
					}					
					else {
						row["Writeoff"]=inswriteoff.ToString("n");
					}
					row["Tot Prod"]=totalproduction.ToString("n");
					row["Pt Income"]=ptincome.ToString("n");
					if(isUnearnedIncluded) {
						row["Unearned Pt Income"]=unearnedPtIncome.ToString("n");
					}
					row["Total Pt Income"]=totalPtIncome.ToString("n");
					row["Ins Income"]=insincome.ToString("n");
					row["Total Income"]=totalincome.ToString("n");
					row["Clinic"]=clinicDesc=="" ? Lans.g("FormRpProdInc","Unassigned"):clinicDesc;
					if(hasData) {
						dtClinic.Rows.Add(row);//prevents adding row if there is no data for the provider
					}
				}
			}
			#region Group Tables by Provider
			Dictionary<long,List<DataRow>> dictionaryProvTableProduction=tableProduction.Select()
				.GroupBy(x => PIn.Long(x["ProvNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryProvTableAdj=tableAdj.Select()
				.GroupBy(x => PIn.Long(x["ProvNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryProvTableInsWriteoff=tableInsWriteoff.Select()
				.GroupBy(x => PIn.Long(x["ProvNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryProvTablePay=tablePay.Select()
				.GroupBy(x => PIn.Long(x["ProvNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryProvTableIns=tableIns.Select()
				.GroupBy(x => PIn.Long(x["ProvNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			Dictionary<long,List<DataRow>> dictionaryProvTableWriteOffAdjustments=tableWriteOffAdjustments.Select()
				.GroupBy(x => PIn.Long(x["ProvNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			#endregion
			for(int i=0;i<listProvs.Count;i++) {
				Provider provCur=listProvs[i];
				hasData=false;
				DataRow row=dt.NewRow();
				row[0]=provCur.Abbr;
				production=0;
				adjust=0;
				inswriteoffest=0;
				inswriteoff=0;
				inswriteoffadj=0;
				totalproduction=0;
				ptincome=0;
				unearnedPtIncome=0;
				totalPtIncome=0;
				insincome=0;
				totalincome=0;
				if(dictionaryProvTableProduction.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsProductionForProvider)) {
					production=listDataRowsProductionForProvider.Sum(x => PIn.Decimal(x["Production"].ToString()));
					hasData=true;
				}
				if(dictionaryProvTableAdj.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsAdjForProvider)) {
					adjust=listDataRowsAdjForProvider.Sum(x => PIn.Decimal(x["AdjAmt"].ToString()));
					hasData=true;
				}
				if(dictionaryProvTableInsWriteoff.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsInsWriteoffForProvider)) {
					if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
						inswriteoffest=listDataRowsInsWriteoffForProvider.Sum(x => PIn.Decimal(x["WriteOffEst"].ToString()));
					}
					else {
						inswriteoff=listDataRowsInsWriteoffForProvider.Sum(x => PIn.Decimal(x["WriteOff"].ToString()));
					}
					hasData=true;
				}
				if(dictionaryProvTableWriteOffAdjustments.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsWriteOffAdjustmentsForProvider)) {
					inswriteoffadj=listDataRowsWriteOffAdjustmentsForProvider.Sum(x => PIn.Decimal(x["WriteOffEst"].ToString()) + PIn.Decimal(x["WriteOff"].ToString()));
					hasData=true;
				}
				if(dictionaryProvTablePay.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsPayForProvider)) {
					ptincome=listDataRowsPayForProvider.Sum(x => PIn.Decimal(x["PayAmt"].ToString()));
					unearnedPtIncome=listDataRowsPayForProvider.Sum(x => PIn.Decimal(x["UnearnedIncome"].ToString()));
					hasData=true;
				}
				if(dictionaryProvTableIns.TryGetValue(provCur.ProvNum,out List<DataRow> listDataRowsInsForProvider)) {
					insincome=listDataRowsInsForProvider.Sum(x => PIn.Decimal(x["InsPayAmt"].ToString()));
					hasData=true;
				}
				if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
					totalproduction=production+adjust+inswriteoffest+inswriteoffadj;
				}
				else {
					totalproduction=production+adjust+inswriteoff;
				}
				totalPtIncome=ptincome+unearnedPtIncome;
				totalincome=totalPtIncome+insincome;
				row["Production"]=production.ToString("n");
				row["Adjustments"]=adjust.ToString("n");
				if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
					row["Writeoff Est"]=inswriteoffest.ToString("n");
					row["Writeoff Adj"]=inswriteoffadj.ToString("n");
				}
				else {
					row["Writeoff"]=inswriteoff.ToString("n");
				}
				row["Tot Prod"]=totalproduction.ToString("n");
				row["Pt Income"]=ptincome.ToString("n");
				if(isUnearnedIncluded) {
					row["Unearned Pt Income"]=unearnedPtIncome.ToString("n");
				}
				row["Total Pt Income"]=totalPtIncome.ToString("n");
				row["Ins Income"]=insincome.ToString("n");
				row["Total Income"]=totalincome.ToString("n");
				if(hasData) {
					dt.Rows.Add(row);//prevents adding row if there is no data for the provider
				}
			}
			DataSet ds=null;
			ds=new DataSet("ProviderData");
			ds.Tables.Add(dt);
			if(listClinics.Count!=0) {
				ds.Tables.Add(dtClinic);
			}
			return ds;
		}
		
		///<summary></summary>
		public static DataSet GetProviderPayrollDataForClinics(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool hasAllProvs,bool hasAllClinics,bool isDetailed) {
			if(listClinics.Count>0) {
				_hasClinics=true;
			}
			DataSet dataSet=GetProviderPayrollDataSet(dateFrom,dateTo,listProvs,listClinics,hasAllProvs,hasAllClinics);
			DataTable tableProduction=dataSet.Tables["tableProduction"];
			DataTable tableInsWOEst=dataSet.Tables["tableInsWOEst"];
			DataTable tableAdj=dataSet.Tables["tableAdj"];
			DataTable tableInsWriteOff=dataSet.Tables["tableInsWriteOff"];
			DataTable tableAllocatedPatInc=dataSet.Tables["tableAllocatedPatInc"];
			DataTable tableUnallocatedPatInc=dataSet.Tables["tableUnallocatedPatInc"];
			DataTable tableInsIncome=dataSet.Tables["tableInsIncome"];
			DataTable tableInsIncomeNotFinalized=dataSet.Tables["tableInsIncomeNotFinalized"];
			decimal production=0;
			decimal insWriteoffEst=0;
			decimal adjust=0;
			decimal insWriteoffEstMinusWriteoff=0;
			decimal netProduction=0;
			decimal ptIncomeAllocated=0;
			decimal ptIncomeUnallocated=0;
			decimal insIncome=0;
			decimal insIncomeNotFinalized=0;
			decimal netIncome=0;
			DataTable dt=new DataTable("Total");
			dt.Columns.Add(new DataColumn("Date"));
			dt.Columns.Add(new DataColumn("Day"));
			if(isDetailed) {
				dt.Columns.Add(new DataColumn("Patient"));
			}
			dt.Columns.Add(new DataColumn("UCR Poduction"));
			dt.Columns.Add(new DataColumn("Original Est Writeoff"));
			dt.Columns.Add(new DataColumn("Prod Adj"));
			dt.Columns.Add(new DataColumn("Est Minus Actual Writeoff Adjs"));
			dt.Columns.Add(new DataColumn("Net Prod (NPR)"));
			dt.Columns.Add(new DataColumn("Allocated Patient Income"));
			dt.Columns.Add(new DataColumn("Unallocated Patient Income"));
			dt.Columns.Add(new DataColumn("Ins Income"));
			dt.Columns.Add(new DataColumn("Ins Not Finalized"));
			dt.Columns.Add(new DataColumn("Net Income"));
			List<long> listPatNums=new List<long>();
			List<ProviderPayrollRow> listProduction=new List<ProviderPayrollRow>();
			List<ProviderPayrollRow> listInsWOEst=new List<ProviderPayrollRow>();
			List<ProviderPayrollRow> listAdj=new List<ProviderPayrollRow>();
			List<ProviderPayrollRow> listInsWriteOff=new List<ProviderPayrollRow>();
			List<ProviderPayrollRow> listAllocatedPatInc=new List<ProviderPayrollRow>();
			List<ProviderPayrollRow> listUnallocatedPatInc=new List<ProviderPayrollRow>();
			List<ProviderPayrollRow> listInsIncome=new List<ProviderPayrollRow>();
			List<ProviderPayrollRow> listInsIncomeNotFinalized=new List<ProviderPayrollRow>();
			foreach(DataRow row in tableProduction.Rows) {
				listProduction.Add(ProviderPayrollRow.DataRowToPayrollRow(row,false));
			}
			foreach(DataRow row in tableInsWOEst.Rows) {
				listInsWOEst.Add(ProviderPayrollRow.DataRowToPayrollRow(row,false));
			}
			foreach(DataRow row in tableAdj.Rows) {
				listAdj.Add(ProviderPayrollRow.DataRowToPayrollRow(row,false));
			}
			foreach(DataRow row in tableInsWriteOff.Rows) {
				listInsWriteOff.Add(ProviderPayrollRow.DataRowToPayrollRow(row,true));
			}
			foreach(DataRow row in tableAllocatedPatInc.Rows) {
				listAllocatedPatInc.Add(ProviderPayrollRow.DataRowToPayrollRow(row,false));
			}
			foreach(DataRow row in tableUnallocatedPatInc.Rows) {
				listUnallocatedPatInc.Add(ProviderPayrollRow.DataRowToPayrollRow(row,false));
			}
			foreach(DataRow row in tableInsIncome.Rows) {
				listInsIncome.Add(ProviderPayrollRow.DataRowToPayrollRow(row,false));
			}
			foreach(DataRow row in tableInsIncomeNotFinalized.Rows) {
				listInsIncomeNotFinalized.Add(ProviderPayrollRow.DataRowToPayrollRow(row,false));
			}
			if(isDetailed) {
				for(int i=0;i<tableProduction.Rows.Count;i++) {
					AddPatNumToListIfNeeded(listPatNums,PIn.Long(tableProduction.Rows[i]["PatNum"].ToString()));
				}
				for(int i=0;i<tableInsWOEst.Rows.Count;i++) {
					AddPatNumToListIfNeeded(listPatNums,PIn.Long(tableInsWOEst.Rows[i]["PatNum"].ToString()));
				}
				for(int i=0;i<tableAdj.Rows.Count;i++) {
					AddPatNumToListIfNeeded(listPatNums,PIn.Long(tableAdj.Rows[i]["PatNum"].ToString()));
				}
				for(int i=0;i<tableInsWriteOff.Rows.Count;i++) {
					AddPatNumToListIfNeeded(listPatNums,PIn.Long(tableInsWriteOff.Rows[i]["PatNum"].ToString()));
				}
				for(int i=0;i<tableAllocatedPatInc.Rows.Count;i++) {
					AddPatNumToListIfNeeded(listPatNums,PIn.Long(tableAllocatedPatInc.Rows[i]["PatNum"].ToString()));
				}
				for(int i=0;i<tableUnallocatedPatInc.Rows.Count;i++) {
					AddPatNumToListIfNeeded(listPatNums,PIn.Long(tableUnallocatedPatInc.Rows[i]["PatNum"].ToString()));
				}
				for(int i=0;i<tableInsIncome.Rows.Count;i++) {
					AddPatNumToListIfNeeded(listPatNums,PIn.Long(tableInsIncome.Rows[i]["PatNum"].ToString()));
				}
				for(int i=0;i<tableInsIncomeNotFinalized.Rows.Count;i++) {
					AddPatNumToListIfNeeded(listPatNums,PIn.Long(tableInsIncomeNotFinalized.Rows[i]["PatNum"].ToString()));
				}
			}
			for(DateTime dayCur=dateFrom;dayCur<=dateTo;dayCur=dayCur.AddDays(1)) {
				if(isDetailed) {
					List<Patient> listPatientsForReport=Patients.GetMultPats(listPatNums).ToList();
					foreach(Patient patCur in listPatientsForReport) {
						DataRow rowCur=dt.NewRow();
						production=0;
						insWriteoffEst=0;
						adjust=0;
						insWriteoffEstMinusWriteoff=0;
						netProduction=0;
						ptIncomeAllocated=0;
						ptIncomeUnallocated=0;
						insIncome=0;
						insIncomeNotFinalized=0;
						netIncome=0;
						listProduction.FindAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum).ForEach(x => production+=x.Amount);
						listProduction.RemoveAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum);
						listInsWOEst.FindAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum).ForEach(x => insWriteoffEst+=x.Amount);
						listInsWOEst.RemoveAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum);
						listAdj.FindAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum).ForEach(x => adjust+=x.Amount);
						listAdj.RemoveAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum);
						listInsWriteOff.FindAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum).ForEach(x => insWriteoffEstMinusWriteoff+=(x.WriteOffEst-x.WriteOff));
						listInsWriteOff.RemoveAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum);
						listAllocatedPatInc.FindAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum).ForEach(x => ptIncomeAllocated+=x.Amount);
						listAllocatedPatInc.RemoveAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum);
						listUnallocatedPatInc.FindAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum).ForEach(x => ptIncomeUnallocated+=x.Amount);
						listUnallocatedPatInc.RemoveAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum);
						listInsIncome.FindAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum).ForEach(x => insIncome+=x.Amount);
						listInsIncome.RemoveAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum);
						listInsIncomeNotFinalized.FindAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum).ForEach(x => insIncomeNotFinalized+=x.Amount);
						listInsIncomeNotFinalized.RemoveAll(x => x.Date==dayCur && x.PatNum==patCur.PatNum);
						netProduction=production+insWriteoffEst+adjust+insWriteoffEstMinusWriteoff;
						netIncome=ptIncomeAllocated+insIncome;
						rowCur[0]=dayCur.ToShortDateString();
						rowCur[1]=patCur.GetNameLFnoPref();
						rowCur[2]=production.ToString("n");
						rowCur[3]=insWriteoffEst.ToString("n");
						rowCur[4]=adjust.ToString("n");
						rowCur[5]=insWriteoffEstMinusWriteoff.ToString("n");
						rowCur[6]=netProduction.ToString("n");
						rowCur[7]=ptIncomeAllocated.ToString("n");
						rowCur[8]=ptIncomeUnallocated.ToString("n");
						rowCur[9]=insIncome.ToString("n");
						rowCur[10]=insIncomeNotFinalized.ToString("n");
						rowCur[11]=netIncome.ToString("n");
						if(production!=0 
							|| insWriteoffEst!=0 
							|| adjust!=0 
							|| insWriteoffEstMinusWriteoff!=0 
							|| netProduction!=0 
							|| ptIncomeAllocated!=0 
							|| ptIncomeUnallocated!=0 
							|| insIncome!=0 
							|| insIncomeNotFinalized!=0 
							|| netIncome!=0) 
						{//Only add a row that has data and that data doesn't equate to a row full of '0's
							dt.Rows.Add(rowCur);
						}
					}
				}
				else {
					DataRow rowCur=dt.NewRow();
					production=0;
					insWriteoffEst=0;
					adjust=0;
					insWriteoffEstMinusWriteoff=0;
					netProduction=0;
					ptIncomeAllocated=0;
					ptIncomeUnallocated=0;
					insIncome=0;
					insIncomeNotFinalized=0;
					netIncome=0;
					listProduction.FindAll(x => x.Date==dayCur).ForEach(x => production+=x.Amount);
					listProduction.RemoveAll(x => x.Date==dayCur);
					listInsWOEst.FindAll(x => x.Date==dayCur).ForEach(x => insWriteoffEst+=x.Amount);
					listInsWOEst.RemoveAll(x => x.Date==dayCur);
					listAdj.FindAll(x => x.Date==dayCur).ForEach(x => adjust+=x.Amount);
					listAdj.RemoveAll(x => x.Date==dayCur);
					listInsWriteOff.FindAll(x => x.Date==dayCur).ForEach(x => insWriteoffEstMinusWriteoff+=(x.WriteOffEst-x.WriteOff));
					listInsWriteOff.RemoveAll(x => x.Date==dayCur);
					listAllocatedPatInc.FindAll(x => x.Date==dayCur).ForEach(x => ptIncomeAllocated+=x.Amount);
					listAllocatedPatInc.RemoveAll(x => x.Date==dayCur);
					listUnallocatedPatInc.FindAll(x => x.Date==dayCur).ForEach(x => ptIncomeUnallocated+=x.Amount);
					listUnallocatedPatInc.RemoveAll(x => x.Date==dayCur);
					listInsIncome.FindAll(x => x.Date==dayCur).ForEach(x => insIncome+=x.Amount);
					listInsIncome.RemoveAll(x => x.Date==dayCur);
					listInsIncomeNotFinalized.FindAll(x => x.Date==dayCur).ForEach(x => insIncomeNotFinalized+=x.Amount);
					listInsIncomeNotFinalized.RemoveAll(x => x.Date==dayCur);
					netProduction=production+insWriteoffEst+adjust+insWriteoffEstMinusWriteoff;
					netIncome=ptIncomeAllocated+insIncome;
					rowCur[0]=dayCur.ToShortDateString();
					rowCur[1]=dayCur.DayOfWeek.ToString();
					rowCur[2]=production.ToString("n");
					rowCur[3]=insWriteoffEst.ToString("n");
					rowCur[4]=adjust.ToString("n");
					rowCur[5]=insWriteoffEstMinusWriteoff.ToString("n");
					rowCur[6]=netProduction.ToString("n");
					rowCur[7]=ptIncomeAllocated.ToString("n");
					rowCur[8]=ptIncomeUnallocated.ToString("n");
					rowCur[9]=insIncome.ToString("n");
					rowCur[10]=insIncomeNotFinalized.ToString("n");
					rowCur[11]=netIncome.ToString("n");
					if(production!=0 
						|| insWriteoffEst!=0 
						|| adjust!=0 
						|| insWriteoffEstMinusWriteoff!=0 
						|| netProduction!=0 
						|| ptIncomeAllocated!=0 
						|| ptIncomeUnallocated!=0 
						|| insIncome!=0 
						|| insIncomeNotFinalized!=0 
						|| netIncome!=0) 
					{//Only add a row that has data and that data doesn't equate to a row full of '0's
						dt.Rows.Add(rowCur);
					}
				}
			}
			DataSet ds=null;
			ds=new DataSet("ProviderPayrollData");
			ds.Tables.Add(dt);
			return ds;
		}

		private static void AddPatNumToListIfNeeded(List<long> listPatNums, long patNum) {
			if(!listPatNums.Contains(patNum)) {
				listPatNums.Add(patNum);
			}
		}

		///<summary>Gets the defnums that are for hidden unearned types.</summary>
		private static List<long> GetHiddenUnearnedDefNums(bool isCEMT) {
			if(isCEMT) {
				return Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue))
					.Select(x => x.DefNum).ToList();
			}
			return ReportsComplex.RunFuncOnReportServer(() => Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue))
					.Select(x => x.DefNum).ToList());
		}

		///<summary>Returns a dataset that contains 6 tables used to generate the daily report.  If not using clinics then simply supply an empty list of clinicNums.  Also used for the CEMT Provider P and I report</summary>
		public static DataSet GetDailyProdIncDataSet(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics
			,bool hasAllProvs,bool hasAllClinics,bool isUnearnedIncluded,PPOWriteoffDateCalc writeoffType,bool isCEMT=false) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvs,listClinics,hasAllProvs,hasAllClinics,isUnearnedIncluded,writeoffType,isCEMT);
			}
			List<long> listProvNums=listProvs.Select(x => x.ProvNum).ToList();
			List<long> listClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
			List<long> listHiddenUnearnedDefNums=GetHiddenUnearnedDefNums(isCEMT);
			#region Procedures
			string whereProv="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv="AND provider.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			string whereClin="";
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			string command="SELECT "
				+"procedurelog.ProcDate Date, "
				+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName, ', '),patient.FName),' '),patient.MiddleI) namelf, "
				+"procedurecode.Descript Description, "
				+"provider.Abbr, "
				+"procedurelog.ClinicNum Clinic, "
				+"procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)-IFNULL(SUM(claimproc.WriteOff),0) Production, "
				+"procedurelog.ProcNum, "
				+"provider.ProvNum "
				+"FROM patient "
				+"INNER JOIN procedurelog ON patient.PatNum=procedurelog.PatNum "
					+"AND procedurelog.ProcStatus='2' "
					+"AND procedurelog.ProcDate >= "+POut.Date(dateFrom)+" "
					+"AND procedurelog.ProcDate <= "+POut.Date(dateTo)+" "
					+whereClin+" "
				+"INNER JOIN provider ON procedurelog.ProvNum=provider.ProvNum "
					+whereProv
				+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				+"LEFT JOIN claimproc ON procedurelog.ProcNum=claimproc.ProcNum "
					+"AND claimproc.Status='7' "
				+"GROUP BY procedurelog.ProcNum "
				+"ORDER BY Date,namelf";
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
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND adjustment.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT "
				+"adjustment.AdjDate Date, "
				+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName, ', '),patient.FName),' '),patient.MiddleI) namelf, "
				+"definition.ItemName Description, "
				+"provider.Abbr, "
				+"adjustment.ClinicNum Clinic, "
				+"adjustment.AdjAmt AdjAmt, "
				+"adjustment.AdjNum, "
				+"provider.ProvNum "
				+"FROM adjustment "
				+"INNER JOIN patient ON adjustment.PatNum=patient.PatNum "
				+"INNER JOIN definition ON adjustment.AdjType=definition.DefNum "
				+"INNER JOIN provider ON adjustment.ProvNum=provider.ProvNum "
					+whereProv
				+"WHERE adjustment.AdjDate >= "+POut.Date(dateFrom)+" "
					+"AND adjustment.AdjDate <= "+POut.Date(dateTo)+" "
					+whereClin+" "
				+"ORDER BY Date,namelf";
			DataTable tableAdj=new DataTable();
			if(isCEMT) {
				tableAdj=Db.GetTable(command);
			}
			else { 
				tableAdj=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableAdj.TableName="tableAdj";
			#endregion
			#region InsWriteoff
			string whereInsWriteoffProvs="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereInsWriteoffProvs="AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			//PPO Insurance Pay Date
			if(writeoffType==PPOWriteoffDateCalc.InsPayDate) {
				command="SELECT claimproc.DateCP Date, "
					+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) namelf, "
					+"CONCAT(CONCAT(procedurecode.AbbrDesc,' '),carrier.CarrierName) Description, "//AbbrDesc might be null, which is ok.
					+"provider.Abbr, "
					+"claimproc.ClinicNum Clinic, "
					+"-SUM(claimproc.WriteOff) WriteOff, "
					+"claimproc.ClaimNum, "
					+"claimproc.ClaimProcNum, "
					+"provider.ProvNum "
					+"FROM claimproc "
					+"LEFT JOIN patient ON claimproc.PatNum = patient.PatNum "
					+"LEFT JOIN provider ON provider.ProvNum = claimproc.ProvNum "
					+"LEFT JOIN insplan ON insplan.PlanNum = claimproc.PlanNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"LEFT JOIN procedurelog ON procedurelog.ProcNum=claimproc.ProcNum "
					+"LEFT JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
					+"WHERE claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "//received or supplemental
					+whereInsWriteoffProvs
					+whereClin
					+"AND (claimproc.WriteOff > '.0001' OR claimproc.WriteOff < -.0001)  "
					+"AND claimproc.DateCP >= "+POut.Date(dateFrom)+" "
					+"AND claimproc.DateCP <= "+POut.Date(dateTo)+" "
					+"GROUP BY claimproc.ClaimProcNum "
					+"ORDER BY Date,namelf";
			}
			//Procedure Date
			else if(writeoffType==PPOWriteoffDateCalc.ProcDate) {
				command="SELECT claimproc.ProcDate Date,"
					+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) namelf,"
					+"CONCAT(CONCAT(procedurecode.AbbrDesc,' '),carrier.CarrierName) Description,"
					+"provider.Abbr,"
					+"claimproc.ClinicNum Clinic,"
					+"-SUM(claimproc.WriteOff) WriteOff,"
					+"claimproc.ClaimNum, "
					+"claimproc.ClaimProcNum, "
					+"provider.ProvNum "
					+"FROM claimproc "
					+"LEFT JOIN patient ON claimproc.PatNum = patient.PatNum "
					+"LEFT JOIN provider ON provider.ProvNum = claimproc.ProvNum "
					+"LEFT JOIN insplan ON insplan.PlanNum = claimproc.PlanNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"LEFT JOIN procedurelog ON procedurelog.ProcNum=claimproc.ProcNum "
					+"LEFT JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
					+"WHERE claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "//received or supplemental or notreceived
					+whereInsWriteoffProvs
					+whereClin
					+"AND (claimproc.WriteOff > '.0001' OR claimproc.WriteOff < -.0001) "
					+"AND claimproc.ProcDate >= "+POut.Date(dateFrom)+" "
					+"AND claimproc.ProcDate <= "+POut.Date(dateTo)+" "
					+"GROUP BY claimproc.ClaimProcNum "
					+"ORDER BY Date,namelf";
			}
			else {//writeoffType==PPOWriteoffDateCalc.ClaimPayDate, both
				// -- Writeoff estimates come from the ClaimSnapshot table when hasChangeInWriteOff is true.
				if(!hasAllProvs && listProvNums.Count>0) {
					whereProv="AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
				}
				if(!hasAllClinics && listClinicNums.Count>0) {
					whereClin="AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
				}
				command="SELECT provider.Abbr, claimsnapshot.DateTEntry Date, "
					+"-("+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+") WriteOffEst, "
					+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) namelf, "
					+"CONCAT(CONCAT(procedurecode.AbbrDesc,' '),COALESCE(carrier.CarrierName,'')) Description, "//CarrierName might be null, which is ok.
					+"procedurelog.ClinicNum Clinic, "
					+"claimproc.ClaimProcNum, "
					+"provider.ProvNum "
					+"FROM procedurelog "
					+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
					+"INNER JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum "
					+"INNER JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
					+"INNER JOIN provider ON claimproc.ProvNum=provider.ProvNum "
					+"INNER JOIN patient ON patient.PatNum=procedurelog.PatNum "
					+"LEFT JOIN insplan ON insplan.PlanNum = claimproc.PlanNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"WHERE "+DbHelper.BetweenDates("claimsnapshot.DateTEntry",dateFrom,dateTo)+" "
					+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "
					+whereProv
					+whereClin
					+"GROUP BY claimproc.ClaimProcNum "
					+"ORDER BY Date,namelf";
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
			#region PtIncome
			string wherePtIncomeProvs="";
			if(!hasAllProvs && listProvNums.Count>0) {
				wherePtIncomeProvs=" AND paysplit.ProvNum IN ("+String.Join(",",listProvNums);
				wherePtIncomeProvs+=") ";
			}
			if(!isUnearnedIncluded) {//UnearnedType of 0 means the paysplit is NOT unearned
				wherePtIncomeProvs+=" AND paysplit.UnearnedType=0 ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND paysplit.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT "
				+"paysplit.DatePay Date, "
				+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) namelf, "
				+"definition.ItemName Description, "
				+"IFNULL(provider.Abbr,'Unearned') Abbr, "
				+"paysplit.ClinicNum Clinic, "
				+"SUM(IF(paysplit.UnearnedType = 0,paysplit.SplitAmt,0)) PayAmt, "
				+"SUM(IF(paysplit.UnearnedType != 0,paysplit.SplitAmt,0)) UnearnedIncome, "
				+"payment.PayNum, "
				+"provider.ProvNum "
				+"FROM paysplit "
				+"LEFT JOIN payment ON payment.PayNum=paysplit.PayNum "
				+"LEFT JOIN patient ON patient.PatNum=paysplit.PatNum "
				+"LEFT JOIN provider ON provider.ProvNum=paysplit.ProvNum "
				+"LEFT JOIN definition ON payment.PayType=definition.DefNum "
				+"WHERE payment.PayDate >= "+POut.Date(dateFrom)+" "
				+"AND payment.PayDate <= "+POut.Date(dateTo)+" ";
			if(listHiddenUnearnedDefNums.Count>0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
				command+=wherePtIncomeProvs
				+whereClin
				+"GROUP BY paysplit.PatNum,paysplit.ProvNum,paysplit.ClinicNum,PayType,paysplit.DatePay "
				+"ORDER BY Date,namelf";
			DataTable tablePay=new DataTable();
			if(isCEMT) {
				tablePay=Db.GetTable(command);
			}
			else { 
				tablePay=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tablePay.TableName="tablePay";
			#endregion
			#region InsIncome
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv="AND provider.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT "
				+"claimpayment.CheckDate Date, "
				+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) namelf, "
				+"carrier.CarrierName Description, "
				+"provider.Abbr, "
				+"claimproc.ClinicNum Clinic, "
				+"SUM(claimproc.InsPayAmt) InsPayAmt, "
				+"claimproc.ClaimNum, "
				+"provider.ProvNum "
				+"FROM claimproc "
				+"INNER JOIN patient ON claimproc.PatNum=patient.PatNum "
				+"INNER JOIN insplan ON claimproc.PlanNum=insplan.PlanNum "
				+"INNER JOIN carrier ON insplan.CarrierNum=carrier.CarrierNum "
				+"INNER JOIN provider ON claimproc.ProvNum=provider.ProvNum "
					+whereProv+" "
				+"INNER JOIN claimpayment ON claimproc.ClaimPaymentNum=claimpayment.ClaimPaymentNum "
				+"WHERE (claimproc.Status=1 OR claimproc.Status=4) "//received or supplemental
					+"AND claimpayment.CheckDate >= "+POut.Date(dateFrom)+" "
					+"AND claimpayment.CheckDate <= "+POut.Date(dateTo)+" "
					+whereClin+" "
				+"GROUP BY claimproc.PatNum,claimproc.ProvNum,claimproc.PlanNum,claimproc.ClinicNum,claimpayment.CheckDate "
				+"ORDER BY Date,namelf";
			DataTable tableIns=new DataTable();
			if(isCEMT) {
				tableIns=Db.GetTable(command);
			}
			else { 
				tableIns=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableIns.TableName="tableIns";
			#endregion
			#region WriteOffAdjustments
			DataTable tableWriteOffAdjustments=new DataTable();
			//Both
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				//WriteOff Adjustments----------------------------------------------------------------------------
				if(!hasAllProvs && listProvNums.Count>0) {
					whereInsWriteoffProvs="AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
				}
				if(!hasAllClinics && listClinicNums.Count>0) {
					whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
				}
				command="SELECT claimproc.DateCP Date, "
					+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) namelf, "
					+"CONCAT(COALESCE(CONCAT(procedurecode.AbbrDesc,' '),''),COALESCE(carrier.CarrierName,'')) Description, "//AbbrDesc might be null, which is ok.
					+"provider.Abbr, "
					+"claimproc.ClinicNum Clinic, "
					+"SUM("+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+") WriteOffEst, "
					+"-SUM(claimproc.WriteOff) WriteOff, "
					+"claimproc.ClaimNum, "
					+"claimproc.ClaimProcNum, "
					+"provider.ProvNum "
					+"FROM claimproc "
					+"LEFT JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
					+"LEFT JOIN patient ON claimproc.PatNum = patient.PatNum "
					+"LEFT JOIN provider ON provider.ProvNum = claimproc.ProvNum "
					+"LEFT JOIN insplan ON insplan.PlanNum = claimproc.PlanNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"LEFT JOIN procedurelog ON procedurelog.ProcNum=claimproc.ProcNum "
					+"LEFT JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
					+"WHERE claimproc.DateCP BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "
					+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "
					+whereInsWriteoffProvs
					+whereClin
					+"GROUP BY claimproc.ClaimProcNum "
					+"ORDER BY Date,namelf";
				if(isCEMT) {
					tableWriteOffAdjustments=Db.GetTable(command);
				}
				else { 
					tableWriteOffAdjustments=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
				}
			}
			tableWriteOffAdjustments.TableName="tableWriteOffAdjustments";
			#endregion
			DataSet dataSet=new DataSet();
			dataSet.Tables.Add(tableProduction);
			dataSet.Tables.Add(tableAdj);
			dataSet.Tables.Add(tableInsWriteoff);
			dataSet.Tables.Add(tablePay);
			dataSet.Tables.Add(tableIns);
			dataSet.Tables.Add(tableWriteOffAdjustments);
			return dataSet;
		}
		#endregion

		///<summary>Returns a dataset that contains 5 tables used to generate the provider payroll report.  If not using clinics then simply supply an empty list of clinicNums.</summary>
		public static DataSet GetProviderPayrollDataSet(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics
			,bool hasAllProvs,bool hasAllClinics) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvs,listClinics,hasAllProvs,hasAllClinics);
			}
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			DataTable tableProduction;
			DataTable tableInsWOEst;
			DataTable tableAdj;
			DataTable tableInsWriteOff;
			DataTable tableAllocatedPatInc;
			DataTable tableUnallocatedPatInc;
			DataTable tableInsIncome;
			DataTable tableInsIncomeNotFinalized;
			string tableProductionName="tableProduction";
			string tableInsWOEstName="tableInsWOEst";
			string tableAdjName="tableAdj";
			string tableInsWriteOffName="tableInsWriteOff";
			string tableAllocatedPatIncName="tableAllocatedPatInc";
			string tableUnallocatedPatIncName="tableUnallocatedPatInc";
			string tableInsIncomeName="tableInsIncome";
			string tableInsIncomeNotFinalizedName="tableInsIncomeNotFinalized";
			List<long> listProvNums=new List<long>();
			for(int i=0;i<listProvs.Count;i++) {
				listProvNums.Add(listProvs[i].ProvNum);
			}
			List<long> listClinicNums=new List<long>();
			for(int i=0;i<listClinics.Count;i++) {
				listClinicNums.Add(listClinics[i].ClinicNum);
			}
			List<int> listClaimProcStatuses=new List<int>();
			listClaimProcStatuses.Add((int)ClaimProcStatus.Received);
			listClaimProcStatuses.Add((int)ClaimProcStatus.Supplemental);
			string whereProv="";
			string whereClin="";
			string command="";
			bool hasProvs=false;
			bool hasClinics=false;
			if(!hasAllProvs && listProvNums.Count>0) {
				hasProvs=true;
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				hasClinics=true;
			}
			//Production-------------------------------------------------------
			if(hasProvs) {
				whereProv="AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin="AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT procedurelog.PatNum, procedurelog.ProvNum, procedurelog.ClinicNum, procedurelog.DateComplete TranDate, "
				+"procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits) Amount,0 as WriteOff,0 as WriteOffEst "
				+"FROM procedurelog "
				+"LEFT JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum "
					+"AND claimproc.Status="+POut.Int((int)ClaimProcStatus.CapComplete)+" "
				+"WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+whereProv
				+whereClin
				+"AND procedurelog.DateComplete BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" ";
			tableProduction=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			tableProduction.TableName=tableProductionName;
			//Insurance WriteOff Estimates----------------------------------------------------------------------------
			if(hasProvs) {
				whereProv="AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin="AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT procedurelog.PatNum, procedurelog.ProvNum, procedurelog.ClinicNum, procedurelog.DateComplete TranDate, "
				+"-("+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)",
					"0",false)+") Amount,0 as WriteOff,0 as WriteOffEst "
				+"FROM procedurelog "
				+"INNER JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum "
				+"INNER JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
				+"WHERE procedurelog.DateComplete BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin;
			tableInsWOEst=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			tableInsWOEst.TableName=tableInsWOEstName;
			//Adjustments----------------------------------------------------------------------------
			if(hasProvs) {
				whereProv="AND adjustment.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin="AND adjustment.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			string listBadDebtAdj= ReportsComplex.RunFuncOnReportServer(() => PrefC.GetStringNoCache(PrefName.BadDebtAdjustmentTypes));
			if(String.IsNullOrEmpty(listBadDebtAdj)) {
				listBadDebtAdj="0";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin="AND adjustment.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT adjustment.PatNum, adjustment.ProvNum, adjustment.ClinicNum, adjustment.DateEntry TranDate,adjustment.AdjAmt Amount,0 as WriteOff,0 as WriteOffEst "
				+"FROM adjustment "
				+"WHERE adjustment.AdjType NOT IN("+listBadDebtAdj+") "
				+whereProv
				+whereClin
				+"AND adjustment.DateEntry BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" ";
			tableAdj=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			tableAdj.TableName=tableAdjName;
			//InsWriteoff--------------------------------------------------------------------------
			if(hasProvs) {
				whereProv="AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin="AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT claimproc.PatNum, claimproc.ProvNum, claimproc.ClinicNum, claimproc.DateSuppReceived TranDate,0 as Amount, claimproc.WriteOff WriteOff, "
				+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+" WriteoffEst "
				+"FROM claimproc "
				+"LEFT JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
				+"WHERE claimproc.DateSuppReceived BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin;
			tableInsWriteOff=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			tableInsWriteOff.TableName=tableInsWriteOffName;
			//AllocatedPtIncome--------------------------------------------------------------------------------
			if(hasProvs) {
				whereProv=" AND paysplit.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin=" AND paysplit.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT paysplit.PatNum, paysplit.ProvNum, paysplit.ClinicNum, paysplit.DateEntry TranDate,paysplit.SplitAmt Amount,0 as WriteOff,0 as WriteOffEst "
				+"FROM paysplit "
				+"WHERE paysplit.IsDiscount=0 "
				+whereProv
				+whereClin
				+"AND paysplit.ProcNum!=0 ";
			if(listHiddenUnearnedDefNums.Count>0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			command+="AND paysplit.DateEntry BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" ";
			tableAllocatedPatInc=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			tableAllocatedPatInc.TableName=tableAllocatedPatIncName;
			//UnallocatedPtIncome--------------------------------------------------------------------------------
			whereProv="";
			if(hasProvs) {
				whereProv="AND paysplit.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin=" AND paysplit.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT paysplit.PatNum, paysplit.ProvNum, paysplit.ClinicNum, paysplit.DateEntry TranDate,paysplit.SplitAmt Amount,0 as WriteOff,0 as WriteOffEst "
				+"FROM paysplit "
				+"WHERE paysplit.IsDiscount=0 "
				+whereProv
				+whereClin
				+"AND paysplit.ProcNum=0 ";
			if(listHiddenUnearnedDefNums.Count>0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			command+="AND paysplit.DateEntry BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" ";
			tableUnallocatedPatInc=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			tableUnallocatedPatInc.TableName=tableUnallocatedPatIncName;
			//InsIncome---------------------------------------------------------------------------------
			if(hasProvs) {
				whereProv="AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT claimproc.PatNum, claimproc.ProvNum, claimproc.ClinicNum, claimproc.DateSuppReceived TranDate,claimproc.InsPayAmt Amount,0 as WriteOff,0 as WriteOffEst "
				+"FROM claimproc "
				+"INNER JOIN claimpayment ON claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum "
				+"WHERE claimproc.Status IN ("+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+") "
				+whereProv
				+whereClin
				+"AND claimproc.DateSuppReceived BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" ";
			tableInsIncome=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			tableInsIncome.TableName=tableInsIncomeName;
			//InsIncomeNotFinalized---------------------------------------------------------------------------------
			if(hasProvs) {
				whereProv="AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT claimproc.PatNum, claimproc.ProvNum, claimproc.ClinicNum, claimproc.DateSuppReceived TranDate,claimproc.InsPayAmt Amount,0 as WriteOff,0 as WriteOffEst "
				+"FROM claimproc "
				+"WHERE claimproc.DateSuppReceived BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin
				+"AND claimproc.ClaimPaymentNum=0";
			tableInsIncomeNotFinalized=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			tableInsIncomeNotFinalized.TableName=tableInsIncomeNotFinalizedName;
			DataSet dataSet=new DataSet();
			dataSet.Tables.Add(tableProduction);
			dataSet.Tables.Add(tableInsWOEst);
			dataSet.Tables.Add(tableAdj);
			dataSet.Tables.Add(tableInsWriteOff);
			dataSet.Tables.Add(tableAllocatedPatInc);
			dataSet.Tables.Add(tableUnallocatedPatInc);
			dataSet.Tables.Add(tableInsIncome);
			dataSet.Tables.Add(tableInsIncomeNotFinalized);
			return dataSet;
		}

		///<summary>Returns a dataset that contains 3 tables used to generate the provider payroll transaction detail report.  
		///This is also used to provide a "Daily" Provider Payroll Transaction Detail report, 
		///which will have slightly different logic to calculate due to claimsnapshot eConnector trigger timing issues.
		///If not using clinics then simply supply an empty list of clinicNums.</summary>
		public static DataTable GetNetProductionDetailDataSet(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool hasAllProvs,bool hasAllClinics,bool useSnapshotForToday) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvs,listClinics,hasAllProvs,hasAllClinics,useSnapshotForToday);
			}
			List<long> listProvNums=new List<long>();
			for(int i=0;i<listProvs.Count;i++) {
				listProvNums.Add(listProvs[i].ProvNum);
			}
			List<long> listClinicNums=new List<long>();
			for(int i=0;i<listClinics.Count;i++) {
				listClinicNums.Add(listClinics[i].ClinicNum);
			}
			string whereProv="";
			string whereClin="";
			string command="";
			bool hasProvs=false;
			bool hasClinics=false;
			if(!hasAllProvs && listProvNums.Count>0) {
				hasProvs=true;
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				hasClinics=true;
			}
			bool isToday=false;
			if(dateFrom==DateTime.Today && dateTo==DateTime.Today) {
				isToday=true;
			}
			command=@" 
				SELECT TranType, 
				CalendarDate, 
				Clinic.Abbr AS ClinicName, 
				details.PatNum, 
				CONCAT(patient.Lname, ', ', patient.FName) AS PatName, 
				procedurecode.ProcCode, 
				CONCAT(provider.Lname, ', ', provider.FName) AS ProvName, 
				UCR, 
				OrigEstWO, 
				EstVsActualWO, 
				Adjustment, 
				NPR 
				FROM ( ";
			#region Production Subquery
			//Production-------------------------------------------------------
			if(hasProvs) {
				whereProv="AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin="AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			if(isToday && !useSnapshotForToday) {
				command+=@"
					SELECT 'Procedure Completed' AS TranType,procedurelog.ProvNum,procedurelog.ClinicNum,procedurelog.PatNum,procedurelog.ProcNum,procedurelog.DateComplete AS CalendarDate, 
					procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits) AS UCR, 
					"+DbHelper.IfNull("NULLIF(writeoffs.WriteOffEst, -1)","0",false)+@" AS OrigEstWO, 
					0 AS EstVsActualWO, 
					0 AS Adjustment, 
					procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)-"+DbHelper.IfNull("NULLIF(writeoffs.WriteOffEst, -1)","0",false)+@" AS NPR 
					FROM procedurelog  
					LEFT JOIN	( 
						SELECT DISTINCT claimproc.ProcNum,"+DbHelper.IfNull("NULLIF(claimproc.WriteOffEst, -1)","0",false)+@" AS WriteOffEst 
						FROM claimproc 
						INNER JOIN procedurelog ON claimproc.ProcNum=procedurelog.ProcNum 
						INNER JOIN patplan ON claimproc.PatNum=patplan.PatNum 
							AND claimproc.InsSubNum=patplan.InsSubNum 
							AND patplan.Ordinal=1 
						WHERE procedurelog.DateComplete="+POut.Date(DateTime.Today)+@" 
						AND claimproc.Status!="+POut.Int((int)ClaimProcStatus.Preauth)+@" 
					) writeoffs ON procedurelog.ProcNum=writeoffs.ProcNum 
					WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+@" 
					AND procedurelog.DateComplete BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "//Since isToday is only true if dateFrom and dateTo are today's date, it doesn't matter which is used here.
					+whereProv
					+whereClin;
			}
			else {
				command+=@" 
					SELECT 'Procedure Completed' AS TranType,procedurelog.ProvNum,procedurelog.ClinicNum,procedurelog.PatNum,procedurelog.ProcNum,procedurelog.DateComplete AS CalendarDate, 
					procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits) AS UCR, 
					"+DbHelper.IfNull("NULLIF(snapshot.OrigEstWO, -1)","0",false)+@" AS OrigEstWO, 
					0 AS EstVsActualWO, 
					0 AS Adjustment, 
					procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)-"+DbHelper.IfNull("NULLIF(snapshot.OrigEstWO, -1)","0",false)+@" AS NPR 
					FROM procedurelog 
					LEFT JOIN ( 
						SELECT procedurelog.ProcNum,claimsnapshot.Writeoff AS OrigEstWO 
						FROM procedurelog 
						INNER JOIN claimproc ON procedurelog.ProcNum=claimproc.ProcNum 
						INNER JOIN claimsnapshot ON claimproc.ClaimProcNum=claimsnapshot.ClaimProcNum
						WHERE ProcStatus="+POut.Int((int)ProcStat.C)+@"  
						AND procedurelog.DateComplete BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+@"  
					) snapshot 
					ON procedurelog.ProcNum=snapshot.ProcNum 
					WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+@" 
					AND procedurelog.DateComplete BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "
					+whereProv
					+whereClin;
			} 
			//Adds a union for the rest of the query.
			command+=@"
				
				UNION ALL
				
				";
			#endregion
			#region Adjustments Subquery
			//Adjustments----------------------------------------------------------------------------
			if(hasProvs) {
				whereProv="AND adjustment.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin="AND adjustment.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			string listBadDebtAdj = ReportsComplex.RunFuncOnReportServer(() => PrefC.GetStringNoCache(PrefName.BadDebtAdjustmentTypes));
			if(String.IsNullOrEmpty(listBadDebtAdj)) {
				listBadDebtAdj="0";
			}
			if(!hasAllClinics&&listClinicNums.Count>0) {
				whereClin="AND adjustment.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command+=@"SELECT 'Prod Adjustment' AS TranType,adjustment.ProvNum, adjustment.ClinicNum, adjustment.PatNum, adjustment.ProcNum, adjustment.DateEntry AS CalendarDate, 
				0 AS UCR, 
				0 AS OrigEstWO, 
				0 AS EstVsActualWO, 
				adjustment.AdjAmt AS Adjustment, 
				adjustment.AdjAmt AS NPR 
				FROM adjustment 
				WHERE adjustment.AdjType NOT IN ("+listBadDebtAdj+@") "
				+whereProv
				+whereClin+@" 
				AND adjustment.DateEntry BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "; 
			//Adds a union for the rest of the query.
			command+=@"
				
				UNION ALL
				
				";
			#endregion
			#region WriteOffs Subquery
			//InsWriteoff--------------------------------------------------------------------------
			if(hasProvs) {
				whereProv="AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinics) {
				whereClin="AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command+=@"SELECT 'Claim Received' AS TranType,claimproc.ProvNum,claimproc.ClinicNum,claimproc.PatNum,claimproc.ProcNum,claimproc.DateSuppReceived AS CalendarDate, 
				0 AS UCR, 
				0 AS OrigEstWO, 
				IFNULL(NULLIF(claimsnapshot.Writeoff, -1), 0)-claimproc.writeoff AS EstVsActualWO, 
				0 AS Adjustment, 
				IFNULL(NULLIF(claimsnapshot.Writeoff, -1), 0)-claimproc.writeoff AS NPR 
				FROM claimproc 
				LEFT JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum 
				WHERE claimproc.DateSuppReceived BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin; 
			#endregion
			command+=@"
				) details
				LEFT JOIN clinic ON Clinic.ClinicNum=details.ClinicNum
				LEFT JOIN patient ON Patient.PatNum=details.PatNum
				LEFT JOIN procedurelog ON procedurelog.ProcNum=details.ProcNum
				LEFT JOIN provider ON provider.ProvNum=details.ProvNum
				LEFT JOIN procedurecode on procedurecode.CodeNum=procedurelog.CodeNum
				WHERE NOT (UCR=0 AND OrigEstWO=0 AND EstVsActualWO=0 AND Adjustment=0 AND NPR=0)
				ORDER BY TranType,CalendarDate,PatNum";
			DataTable retVal=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			return retVal;
		}

		#region Monthly P&I Report
		///<summary>If not using clinics then supply an empty list of clinics.
		///showUnearnedCEMT should only be considered from CEMT (or if you have a "Show Unearned" checkbox, which currently only applies to CEMT).</summary>
		public static DataSet GetMonthlyData(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool writeOffPay,bool hasAllProvs,bool hasAllClinics,bool hasChangeInWriteoff,bool isUnearnedIncluded,bool isCEMT=false) {
			//No need to check RemotingRole; no call to db.
			if(listClinics.Count>0) {
				_hasClinics=true;
			}
			DataSet dataSet=GetMonthlyProdIncDataSet(dateFrom,dateTo,listProvs,listClinics,writeOffPay,hasAllProvs,hasAllClinics,hasChangeInWriteoff,isUnearnedIncluded,isCEMT);
			DataTable tableProduction=dataSet.Tables["tableProduction"];
			DataTable tableAdj=dataSet.Tables["tableAdj"];
			DataTable tableInsWriteoff=dataSet.Tables["tableInsWriteoff"];
			DataTable tableWriteoffAdjustments=dataSet.Tables["tableWriteOffAdjustments"];
			DataTable tablePay=dataSet.Tables["tablePay"];
			DataTable tableIns=dataSet.Tables["tableIns"];
			DataTable tableSched=dataSet.Tables["tableSched"];
			decimal sched;
			decimal production;
			decimal adjust;
			decimal inswriteoffest;
			decimal inswriteoff;	//spk 5/19/05
			decimal inswriteoffadj;
			decimal totalproduction;
			decimal ptincome;
			decimal unearnedPtIncome;
			decimal totalPtIncome;
			decimal insincome;
			decimal totalincome;
			DataTable dt=new DataTable("Total");
			dt.Columns.Add(new DataColumn("Month"));
			dt.Columns.Add(new DataColumn("Day"));
			dt.Columns.Add(new DataColumn("Production",typeof(double)));
			dt.Columns.Add(new DataColumn("Sched",typeof(double)));
			dt.Columns.Add(new DataColumn("Adjustments",typeof(double)));
			if(hasChangeInWriteoff) {
				dt.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				dt.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				dt.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			dt.Columns.Add(new DataColumn("Tot Prod",typeof(double)));
			dt.Columns.Add(new DataColumn("Pt Income",typeof(double)));
			if(isUnearnedIncluded) {
				dt.Columns.Add(new DataColumn("Unearned Pt Income",typeof(double)));
			}
			dt.Columns.Add(new DataColumn("Ins Income",typeof(double)));
			dt.Columns.Add(new DataColumn("Total Income",typeof(double)));
			dt.Columns.Add(new DataColumn("Total Pt Income",typeof(double)));
			DataTable dtClinic=new DataTable("Clinic");
			dtClinic.Columns.Add(new DataColumn("Month"));
			dtClinic.Columns.Add(new DataColumn("Day"));
			dtClinic.Columns.Add(new DataColumn("Production",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Sched",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Adjustments",typeof(double)));
			if(hasChangeInWriteoff) {
				dtClinic.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				dtClinic.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				dtClinic.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			dtClinic.Columns.Add(new DataColumn("Tot Prod",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Pt Income",typeof(double)));
			if(isUnearnedIncluded) {
				dtClinic.Columns.Add(new DataColumn("Unearned Pt Income",typeof(double)));
			}
			dtClinic.Columns.Add(new DataColumn("Ins Income",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Total Income",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Clinic"));
			dtClinic.Columns.Add(new DataColumn("Total Pt Income",typeof(double)));
			//length of array is number of months between the two dates plus one.
			//The from date and to date will not be more than one year and must will be within the same year due to FormRpProdInc UI validation enforcement.
			DateTime[] dates=null;
			dates=new DateTime[dateTo.Subtract(dateFrom).Days+1];//Make a DateTime array with one position for each day in the report.
			//Get a list of clinics so that we have access to their descriptions for the report.
			for(int it=0;it<listClinics.Count;it++) {//For each clinic
				for(int i=0;i<dates.Length;i++) {//usually 12 months in loop for annual.  Loop through the DateTime array, each position represents one date in the report.
					dates[i]=dateFrom.AddDays(i);//Monthly/Daily report, add a day
					DataRow row=dtClinic.NewRow();
					row["Month"]=dates[i].ToShortDateString();
					row["Day"]=dates[i].ToString("ddd");//Abbreviated name of day
					sched=0;
					production=0;
					adjust=0;
					inswriteoffest=0;
					inswriteoff=0;	//spk 5/19/05
					inswriteoffadj=0;
					totalproduction=0;
					ptincome=0;
					unearnedPtIncome=0;
					totalPtIncome=0;
					insincome=0;
					totalincome=0;
					for(int j=0;j<tableProduction.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableProduction.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;//Only counting unassigned this time around.
						}
						else if(listClinics[it].ClinicNum!=0 && tableProduction.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Year
								&& dates[i].Month==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Month) //If the proc was in the month and year that we're making a row for
						{
							if(dates[i].Day==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Day) {//If the proc is also on the day (Only monthly report)
								production+=PIn.Decimal(tableProduction.Rows[j]["Production"].ToString());
							}
						}
					}
					for(int j=0;j<tableAdj.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableAdj.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableAdj.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Year
								&& dates[i].Month==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Month) //If the adjustment was in the month and year that we're making a row for.
						{
							if(dates[i].Day==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Day) {//If the adjustment is also on the day (Only monthly report)
								adjust+=PIn.Decimal(tableAdj.Rows[j]["Adjustment"].ToString());
							}
						}
					}
					for(int j=0;j<tableInsWriteoff.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableInsWriteoff.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableInsWriteoff.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Year
							&& dates[i].Month==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Month
							&& dates[i].Day==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Day)
						{
							if(hasChangeInWriteoff) {
								inswriteoffest-=PIn.Decimal(tableInsWriteoff.Rows[j]["WriteoffEst"].ToString());
							}
							else {
								inswriteoff-=PIn.Decimal(tableInsWriteoff.Rows[j]["Writeoff"].ToString());
							}								
						}			
					}
					for(int j=0;j<tableWriteoffAdjustments.Rows.Count;j++) {
						if(tableWriteoffAdjustments.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableWriteoffAdjustments.Rows[j]["Date"].ToString()).Year
							&& dates[i].Month==PIn.Date(tableWriteoffAdjustments.Rows[j]["Date"].ToString()).Month
							&& dates[i].Day==PIn.Date(tableWriteoffAdjustments.Rows[j]["Date"].ToString()).Day)
						{
							inswriteoffadj-=PIn.Decimal(tableWriteoffAdjustments.Rows[j]["WriteOffEst"].ToString())+PIn.Decimal(tableWriteoffAdjustments.Rows[j]["WriteOff"].ToString());
						}
					}
					for(int j=0;j<tableSched.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableSched.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableSched.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i]==(PIn.Date(tableSched.Rows[j]["SchedDate"].ToString()))) {
							sched+=PIn.Decimal(tableSched.Rows[j]["Amount"].ToString());
						}
					}
					for(int j=0;j<tablePay.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tablePay.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tablePay.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Year
								&& dates[i].Month==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Month) //If the payment was in the month and year that we're making a row for.
						{
							if(dates[i].Day==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Day) {//If the payment is also on the day (Only monthly report)
								ptincome+=PIn.Decimal(tablePay.Rows[j]["Income"].ToString());
								unearnedPtIncome+=PIn.Decimal(tablePay.Rows[j]["UnearnedIncome"].ToString());
							}
						}
					}
					for(int j=0;j<tableIns.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableIns.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableIns.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Year
								&& dates[i].Month==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Month) //If the ins payment was in the month and year that we're making a row for.
						{
							if(dates[i].Day==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Day) {//If the ins payment is also on the day (Only monthly report)
								insincome+=PIn.Decimal(tableIns.Rows[j]["Ins"].ToString());
							}
						}
					}
					if(hasChangeInWriteoff) {
						totalproduction=production+adjust+inswriteoffest+inswriteoffadj+sched;
					}
					else {
						totalproduction=production+adjust+inswriteoff+sched;
					}
					totalPtIncome=ptincome+unearnedPtIncome;
					totalincome=totalPtIncome+insincome;
					string clinicDesc=listClinics[it].Description;
					row["Production"]=production.ToString("n");
					row["Sched"]=sched.ToString("n");
					row["Adjustments"]=adjust.ToString("n");
					if(hasChangeInWriteoff) {
						row["Writeoff Est"]=inswriteoffest.ToString("n");
						row["Writeoff Adj"]=inswriteoffadj.ToString("n");
					}
					else {
						row["Writeoff"]=inswriteoff.ToString("n");
					}
					row["Tot Prod"]=totalproduction.ToString("n");
					row["Pt Income"]=ptincome.ToString("n");
					if(isUnearnedIncluded) {
						row["Unearned Pt Income"]=unearnedPtIncome.ToString("n");
					}
					row["Total Pt Income"]=totalPtIncome.ToString("n");
					row["Ins Income"]=insincome.ToString("n");
					row["Total Income"]=totalincome.ToString("n");
					row["Clinic"]=clinicDesc=="" ? Lans.g("FormRpProdInc","Unassigned"):clinicDesc;
					dtClinic.Rows.Add(row);
				}
			}
			for(int i=0;i<dates.Length;i++) {//usually 12 months in loop
				dates[i]=dateFrom.AddDays(i);
				DataRow row=dt.NewRow();
				row["Month"]=dates[i].ToShortDateString();
				row["Day"]=dates[i].ToString("ddd");//Abbreviated name of day
				sched=0;
				production=0;
				adjust=0;
				inswriteoffest=0;
				inswriteoff=0;	//spk 5/19/05
				inswriteoffadj=0;
				totalproduction=0;
				ptincome=0;
				unearnedPtIncome=0;
				totalPtIncome=0;
				insincome=0;
				totalincome=0;
				for(int j=0;j<tableProduction.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Month) {
						if(dates[i].Day==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Day) {//If the proc is also on the day (Only monthly report)
							production+=PIn.Decimal(tableProduction.Rows[j]["Production"].ToString());
						}
					}
				}
				for(int j=0;j<tableAdj.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Month) {
						if(dates[i].Day==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Day) {
							adjust+=PIn.Decimal(tableAdj.Rows[j]["Adjustment"].ToString());
						}
					}
				}
				for(int j=0;j<tableInsWriteoff.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Month
						&& dates[i].Day==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Day)
					{
						if(hasChangeInWriteoff) {
							inswriteoffest-=PIn.Decimal(tableInsWriteoff.Rows[j]["WriteoffEst"].ToString());
						}
						else {
							inswriteoff-=PIn.Decimal(tableInsWriteoff.Rows[j]["Writeoff"].ToString());
						}
					}
				}
				for(int j=0;j<tableWriteoffAdjustments.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableWriteoffAdjustments.Rows[j]["Date"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableWriteoffAdjustments.Rows[j]["Date"].ToString()).Month
						&& dates[i].Day==PIn.Date(tableWriteoffAdjustments.Rows[j]["Date"].ToString()).Day)
					{
						inswriteoffadj-=PIn.Decimal(tableWriteoffAdjustments.Rows[j]["WriteOffEst"].ToString())+PIn.Decimal(tableWriteoffAdjustments.Rows[j]["WriteOff"].ToString());
					}
				}
				for(int j=0;j<tableSched.Rows.Count;j++) {
					if(dates[i]==(PIn.Date(tableSched.Rows[j]["SchedDate"].ToString()))) {
						sched+=PIn.Decimal(tableSched.Rows[j]["Amount"].ToString());
					}
				}
				for(int j=0;j<tablePay.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Year
						&& dates[i].Month==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Month) {
						if(dates[i].Day==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Day) {
							ptincome+=PIn.Decimal(tablePay.Rows[j]["Income"].ToString());
							unearnedPtIncome+=PIn.Decimal(tablePay.Rows[j]["UnearnedIncome"].ToString());
						}
					}
				}
				for(int j=0;j<tableIns.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Month) {
						if(dates[i].Day==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Day) {
							insincome+=PIn.Decimal(tableIns.Rows[j]["Ins"].ToString());
						}
					}
				}
				if(hasChangeInWriteoff) {
					totalproduction=production+adjust+inswriteoffest+inswriteoffadj+sched;
				}
				else {
					totalproduction=production+adjust+inswriteoff+sched;
				}
				totalPtIncome=ptincome+unearnedPtIncome;
				totalincome=totalPtIncome+insincome;
				row["Production"]=production.ToString("n");
				row["Sched"]=sched.ToString("n");
				row["Adjustments"]=adjust.ToString("n");
				if(hasChangeInWriteoff) {
					row["Writeoff Est"]=inswriteoffest.ToString("n");
					row["Writeoff Adj"]=inswriteoffadj.ToString("n");
				}
				else {
					row["Writeoff"]=inswriteoff.ToString("n");
				}
				row["Tot Prod"]=totalproduction.ToString("n");
				row["Pt Income"]=ptincome.ToString("n");
				if(isUnearnedIncluded) {
					row["Unearned Pt Income"]=unearnedPtIncome.ToString("n");
				}
				row["Ins Income"]=insincome.ToString("n");
				row["Total Income"]=totalincome.ToString("n");
				row["Total Pt Income"]=totalPtIncome.ToString("n");
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

		///<summary>Returns a dataset that contains 7 tables used to generate the monthly report. If not using clinics then supply an empty list of clinics.
		/// Does not work for Oracle (by chance not by design). Consider enhancing with DbHelper.Year(),DbHelper.Month(), DbHelper.Day() and enhancing the GroupBy Logic.</summary>
		public static DataSet GetMonthlyProdIncDataSet(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool writeOffPay,
			bool hasAllProvs,bool hasAllClinics,bool hasChangeInWriteoff,bool isUnearnedIncluded,bool isCEMT=false) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvs,listClinics,writeOffPay,hasAllProvs,hasAllClinics,hasChangeInWriteoff,isUnearnedIncluded,isCEMT);
			}
			List<long> listClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
			List<long> listProvNums=listProvs.Select(x => x.ProvNum).ToList();
			List<long> listHiddenUnearnedDefNums=GetHiddenUnearnedDefNums(isCEMT);
			#region Procedures
			string whereProv="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv=" AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			string whereClin="";
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			//==Travis (04/11/2014): In the case that you have two capitation plans for a single patient the query below will cause a duplicate row, incorectly increasing your production.
			//	We now state in the manual that having two capitation plans is not advised and will cause reporting to be off.
			string command="SELECT "
				+"procedurelog.ProcDate,procedurelog.ClinicNum,"
				+"SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits))-IFNULL(SUM(cp.WriteOff),0) Production "
				+"FROM procedurelog "
				+"LEFT JOIN (SELECT SUM(claimproc.WriteOff) AS WriteOff, claimproc.ProcNum FROM claimproc "
				+$"WHERE claimproc.Status={POut.Int((int)ClaimProcStatus.CapComplete)} "
				+"GROUP BY claimproc.ProcNum) cp ON procedurelog.ProcNum=cp.ProcNum "
				+$"WHERE procedurelog.ProcStatus = {POut.Int((int)ProcStat.C)} "
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
				whereProv=" AND adjustment.ProvNum IN ("+String.Join(",",listProvNums)+") ";
				whereProcProv=" AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND adjustment.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
				whereProcClin=" AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
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
				whereProv=" AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			if(writeOffPay) {
				command="SELECT "
					+"claimproc.DateCP Date," 
					+"claimproc.ClinicNum,"
					+"SUM(claimproc.WriteOff) WriteOff "
					+"FROM claimproc "
					+"WHERE claimproc.DateCP >= "+POut.Date(dateFrom)+" "
					+"AND claimproc.DateCP <= "+POut.Date(dateTo)+" "
					+whereProv
					+whereClin
					+"AND claimproc.Status IN("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "//Received or supplemental
					+"GROUP BY ClinicNum,YEAR(claimproc.DateCP),MONTH(claimproc.DateCP),DAY(claimproc.DateCP) "
					+"ORDER BY ClinicNum,DateCP";
			}
			else if(hasChangeInWriteoff) {
				command="SELECT claimsnapshot.DateTEntry Date, "
					+"SUM("+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+") WriteOffEst, "
					+"claimproc.ClinicNum, "
					+"claimproc.ClaimProcNum "
					+"FROM claimproc "
					+"INNER JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
					+"WHERE "+DbHelper.BetweenDates("claimsnapshot.DateTEntry",dateFrom,dateTo)+" "
					+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "
					+whereProv
					+whereClin
					+"GROUP BY ClinicNum,YEAR(claimsnapshot.DateTEntry),MONTH(claimsnapshot.DateTEntry),DAY(claimsnapshot.DateTEntry) "
					+"ORDER BY ClinicNum,Date";
			}
			else {
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
					+"GROUP BY ClinicNum,YEAR(claimproc.ProcDate), MONTH(claimproc.ProcDate),DAY(claimproc.ProcDate)";
				command+=" ORDER BY ClinicNum,ProcDate";
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
				whereProv=" AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command= "SELECT "+DbHelper.DtimeToDate("t.AptDateTime")+" SchedDate,SUM(t.Fee-t.WriteoffEstimate) Amount,ClinicNum "
				+"FROM (SELECT appointment.AptDateTime,IFNULL(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits),0) Fee,appointment.ClinicNum,";
			bool isSchedSubWO;
			if(isCEMT) {
				isSchedSubWO=Prefs.GetBoolNoCache(PrefName.ReportPandIschedProdSubtractsWO);
			}
			else {
				isSchedSubWO=ReportsComplex.RunFuncOnReportServer(() => Prefs.GetBoolNoCache(PrefName.ReportPandIschedProdSubtractsWO));
			}
			if(isSchedSubWO) {
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
				+"AND "+DbHelper.BetweenDates("appointment.AptDateTime",dateFrom,dateTo)+" "
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
			#region PtIncome
			whereProv="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv=" AND paysplit.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!isUnearnedIncluded) {//UnearnedType of 0 means the paysplit is NOT unearned
				whereProv+=" AND paysplit.UnearnedType=0 ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND paysplit.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT "
				+"paysplit.DatePay,"
				+"paysplit.ClinicNum,"
				+"SUM(IF(paysplit.UnearnedType = 0,paysplit.SplitAmt,0)) Income,"
				+"SUM(IF(paysplit.UnearnedType != 0,paysplit.SplitAmt,0)) UnearnedIncome "
				+"FROM paysplit "
				+"WHERE paysplit.IsDiscount=0 "//AND paysplit.PayNum=payment.PayNum "
				+whereProv
				+whereClin;
			if(listHiddenUnearnedDefNums.Count>0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			command+="AND paysplit.DatePay >= "+POut.Date(dateFrom)+" "
				+"AND paysplit.DatePay <= "+POut.Date(dateTo)+" "
				+"GROUP BY ClinicNum,YEAR(paysplit.DatePay),MONTH(paysplit.DatePay),DAY(paysplit.DatePay)";
			command+=" ORDER BY ClinicNum,DatePay";
			DataTable tablePay=new DataTable();
			if(isCEMT) {
				tablePay=Db.GetTable(command);
			}
			else { 
				tablePay=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tablePay.TableName="tablePay";
			#endregion
			#region InsIncome
			whereProv="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv=" AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT claimpayment.CheckDate,claimproc.ClinicNum,SUM(claimproc.InsPayamt) Ins "
				+"FROM claimpayment,claimproc WHERE "
				+"claimproc.ClaimPaymentNum = claimpayment.ClaimPaymentNum "
				+"AND claimpayment.CheckDate >= " + POut.Date(dateFrom)+" "
				+"AND claimpayment.CheckDate <= " + POut.Date(dateTo)+" "
				+whereProv
				+whereClin
				+" GROUP BY claimpayment.CheckDate,ClinicNum ORDER BY ClinicNum,CheckDate";
			DataTable tableIns=new DataTable();
			if(isCEMT) {
				tableIns=Db.GetTable(command);
			}
			else { 
				tableIns=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableIns.TableName="tableIns";
			#endregion
			#region WriteOffAdjustments
			DataTable tableWriteOffAdjustments=new DataTable();
			if(hasChangeInWriteoff) {
				//Insurance WriteOff Adjustments----------------------------------------------------------------------------
				command="SELECT claimproc.DateCP Date, "
				+"claimproc.ClinicNum, "
				+"-SUM("+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+") WriteOffEst, "
				+"SUM(claimproc.WriteOff) WriteOff, "
				+"claimproc.ClaimNum "
				+"FROM claimproc "
				+"LEFT JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
				+"WHERE claimproc.DateCP BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "
				+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "
				+whereProv
				+whereClin
				+"GROUP BY ClinicNum,YEAR(claimproc.DateCP), MONTH(claimproc.DateCP),DAY(claimproc.DateCP)";
				if(isCEMT) {
					tableWriteOffAdjustments=Db.GetTable(command);
				}
				else { 
					tableWriteOffAdjustments=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
				}
			}
			tableWriteOffAdjustments.TableName="tableWriteOffAdjustments";
			#endregion
			DataSet dataSet=new DataSet();
			dataSet.Tables.Add(tableProduction);
			dataSet.Tables.Add(tableAdj);
			dataSet.Tables.Add(tableInsWriteoff);
			dataSet.Tables.Add(tableSched);
			dataSet.Tables.Add(tablePay);
			dataSet.Tables.Add(tableIns);
			dataSet.Tables.Add(tableWriteOffAdjustments);
			return dataSet;
		}
		#endregion

		#region Annual P&I Report
		///<summary>If not using clinics then supply an empty list of clinics.
		///showUnearedCEMT should only be considered from CEMT (or if you have a "Show Uneared" checkbox, which currently only applies to CEMT).</summary>
		public static DataSet GetAnnualData(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool writeOffPay,bool hasAllProvs,bool hasAllClinics,bool hasChangeInWriteoff,bool isUnearnedIncluded,bool isCEMT=false) {
			//No need to check RemotingRole; no call to db.
			if(listClinics.Count>0) {
				_hasClinics=true;
			}
			DataSet dataSet=GetAnnualProdIncDataSet(dateFrom,dateTo,listProvs,listClinics,writeOffPay,hasAllProvs,hasAllClinics,hasChangeInWriteoff,isUnearnedIncluded,isCEMT);
			DataTable tableProduction=dataSet.Tables["tableProduction"];
			DataTable tableAdj=dataSet.Tables["tableAdj"];
			DataTable tableInsWriteoff=dataSet.Tables["tableInsWriteoff"];
			DataTable tablePay=dataSet.Tables["tablePay"];
			DataTable tableIns=dataSet.Tables["tableIns"];
			DataTable tableWriteOffAdjustments=dataSet.Tables["tableWriteOffAdjustments"];
			decimal production;
			decimal adjust;
			decimal inswriteoffest;
			decimal inswriteoff;	//spk 5/19/05
			decimal inswriteoffadj;
			decimal totalproduction;
			decimal ptincome;
			decimal unearnedPtIncome;
			decimal totalPtIncome;
			decimal insincome;
			decimal totalincome;
			DataTable dt=new DataTable("Total");
			dt.Columns.Add(new DataColumn("Month"));
			dt.Columns.Add(new DataColumn("Production",typeof(double)));
			dt.Columns.Add(new DataColumn("Adjustments",typeof(double)));
			if(hasChangeInWriteoff) {
				dt.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				dt.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				dt.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			dt.Columns.Add(new DataColumn("Tot Prod",typeof(double)));
			dt.Columns.Add(new DataColumn("Pt Income",typeof(double)));
			if(isUnearnedIncluded) {
				dt.Columns.Add(new DataColumn("Unearned Pt Income",typeof(double)));
			}
			dt.Columns.Add(new DataColumn("Ins Income",typeof(double)));
			dt.Columns.Add(new DataColumn("Total Income",typeof(double)));
			dt.Columns.Add(new DataColumn("Total Pt Income",typeof(double)));
			DataTable dtClinic=new DataTable("Clinic");
			dtClinic.Columns.Add(new DataColumn("Month"));
			dtClinic.Columns.Add(new DataColumn("Production",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Adjustments",typeof(double)));
			if(hasChangeInWriteoff) {
				dtClinic.Columns.Add(new DataColumn("Writeoff Est",typeof(double)));
				dtClinic.Columns.Add(new DataColumn("Writeoff Adj",typeof(double)));
			}
			else {
				dtClinic.Columns.Add(new DataColumn("Writeoff",typeof(double)));
			}
			dtClinic.Columns.Add(new DataColumn("Tot Prod",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Pt Income",typeof(double)));
			if(isUnearnedIncluded) {
				dtClinic.Columns.Add(new DataColumn("Unearned Pt Income",typeof(double)));
			}
			dtClinic.Columns.Add(new DataColumn("Ins Income",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Total Income",typeof(double)));
			dtClinic.Columns.Add(new DataColumn("Clinic"));
			dtClinic.Columns.Add(new DataColumn("Total Pt Income",typeof(double)));
			//length of array is number of months between the two dates plus one.
			//The from date and to date will not be more than one year and must will be within the same year due to FormRpProdInc UI validation enforcement.
			DateTime[] dates=null;
			dates=new DateTime[(dateTo.Year-dateFrom.Year)*12+(dateTo.Month-dateFrom.Month)+1];
			//Make a DateTime array representing one position for each month in the report.  User can't specify different years in the report.
			//Get a list of clinics so that we have access to their descriptions for the report.
			for(int it=0;it<listClinics.Count;it++) {//For each clinic
				for(int i=0;i<dates.Length;i++) {//usually 12 months in loop for annual.  Loop through the DateTime array, each position represents one date in the report.
					dates[i]=dateFrom.AddMonths(i);//only the month and year are important.  For each month slot, add i to the dateFrom and put it in the array.
					DataRow row=dtClinic.NewRow();
					row["Month"]=dates[i].ToString("MMM yyyy");//JAN 2014
					production=0;
					adjust=0;
					inswriteoffest=0;
					inswriteoff=0;	//spk 5/19/05
					inswriteoffadj=0;
					totalproduction=0;
					ptincome=0;
					unearnedPtIncome=0;
					totalPtIncome=0;
					insincome=0;
					totalincome=0;
					for(int j=0;j<tableProduction.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableProduction.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;//Only counting unassigned this time around.
						}
						else if(listClinics[it].ClinicNum!=0 && tableProduction.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Year
								&& dates[i].Month==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Month) //If the proc was in the month and year that we're making a row for
						{
							production+=PIn.Decimal(tableProduction.Rows[j]["Production"].ToString());
						}
					}
					for(int j=0;j<tableAdj.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableAdj.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableAdj.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Year
								&& dates[i].Month==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Month) //If the adjustment was in the month and year that we're making a row for.
						{
							adjust+=PIn.Decimal(tableAdj.Rows[j]["Adjustment"].ToString());
						}
					}
					for(int j=0;j<tableInsWriteoff.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableInsWriteoff.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableInsWriteoff.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Year
							&& dates[i].Month==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Month) //If the claim writeoff was in the month and year that we're making a row for.
						{
							if(hasChangeInWriteoff) {
								inswriteoffest-=PIn.Decimal(tableInsWriteoff.Rows[j]["WriteOffEst"].ToString());
							}
							else {
								inswriteoff-=PIn.Decimal(tableInsWriteoff.Rows[j]["WriteOff"].ToString());
							}
							
						}
					}
					for(int j=0;j<tableWriteOffAdjustments.Rows.Count;j++) {
						if(tableWriteOffAdjustments.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableWriteOffAdjustments.Rows[j]["Date"].ToString()).Year
							&& dates[i].Month==PIn.Date(tableWriteOffAdjustments.Rows[j]["Date"].ToString()).Month) 
						{
							inswriteoffadj-=PIn.Decimal(tableWriteOffAdjustments.Rows[j]["WriteOffEst"].ToString())+PIn.Decimal(tableWriteOffAdjustments.Rows[j]["WriteOff"].ToString());
						}
					}
					for(int j=0;j<tablePay.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tablePay.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tablePay.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Year
								&& dates[i].Month==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Month) //If the payment was in the month and year that we're making a row for.
						{
							ptincome+=PIn.Decimal(tablePay.Rows[j]["Income"].ToString());
							unearnedPtIncome+=PIn.Decimal(tablePay.Rows[j]["UnearnedIncome"].ToString());
						}
					}
					for(int j=0;j<tableIns.Rows.Count;j++) {
						if(listClinics[it].ClinicNum==0 && tableIns.Rows[j]["ClinicNum"].ToString()!="0") {
							continue;
						}
						else if(listClinics[it].ClinicNum!=0 && tableIns.Rows[j]["ClinicNum"].ToString()!=POut.Long(listClinics[it].ClinicNum)) {
							continue;
						}
						if(dates[i].Year==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Year
								&& dates[i].Month==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Month) //If the ins payment was in the month and year that we're making a row for.
						{
							insincome+=PIn.Decimal(tableIns.Rows[j]["Ins"].ToString());
						}
					}
					if(hasChangeInWriteoff) {
						totalproduction=production+adjust+inswriteoffest+inswriteoffadj;
					}
					else {
						totalproduction=production+adjust+inswriteoff;
					}
					totalPtIncome=ptincome+unearnedPtIncome;
					totalincome=totalPtIncome+insincome;
					string clinicDesc=listClinics[it].Description;
					row["Production"]=production.ToString("n");
					row["Adjustments"]=adjust.ToString("n");
					if(hasChangeInWriteoff) {
						row["Writeoff Est"]=inswriteoffest.ToString("n");
						row["Writeoff Adj"]=inswriteoffadj.ToString("n");
					}
					else {
						row["Writeoff"]=inswriteoff.ToString("n");
					}
					row["Tot Prod"]=totalproduction.ToString("n");
					row["Pt Income"]=ptincome.ToString("n");
					if(isUnearnedIncluded) {
						row["Unearned Pt Income"]=unearnedPtIncome.ToString("n");
					}
					row["Ins Income"]=insincome.ToString("n");
					row["Total Income"]=totalincome.ToString("n");
					row["Total Pt Income"]=totalPtIncome.ToString("n");
					row["Clinic"]=clinicDesc=="" ? Lans.g("FormRpProdInc","Unassigned"):clinicDesc;
					dtClinic.Rows.Add(row);  //adds row to table
				}
			}
			for(int i=0;i<dates.Length;i++) {//usually 12 months in loop
				dates[i]=dateFrom.AddMonths(i);//only the month and year are important
				DataRow row=dt.NewRow();
				row["Month"]=dates[i].ToString("MMM yyyy");//JAN 2014
				production=0;
				adjust=0;
				inswriteoffest=0;
				inswriteoff=0;	//spk 5/19/05
				inswriteoffadj=0;
				totalproduction=0;
				ptincome=0;
				unearnedPtIncome=0;
				totalPtIncome=0;
				insincome=0;
				totalincome=0;
				for(int j=0;j<tableProduction.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableProduction.Rows[j]["ProcDate"].ToString()).Month) {
						production+=PIn.Decimal(tableProduction.Rows[j]["Production"].ToString());
					}
				}
				for(int j=0;j<tableAdj.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableAdj.Rows[j]["AdjDate"].ToString()).Month) {
						adjust+=PIn.Decimal(tableAdj.Rows[j]["Adjustment"].ToString());
					}
				}
				for(int j=0;j<tableInsWriteoff.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableInsWriteoff.Rows[j]["Date"].ToString()).Month) 
					{
						if(hasChangeInWriteoff) {
							inswriteoffest-=PIn.Decimal(tableInsWriteoff.Rows[j]["WriteOffEst"].ToString());
						}
						else {
							inswriteoff-=PIn.Decimal(tableInsWriteoff.Rows[j]["WriteOff"].ToString());
						}						
					}
				}
				for(int j=0;j<tableWriteOffAdjustments.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableWriteOffAdjustments.Rows[j]["Date"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableWriteOffAdjustments.Rows[j]["Date"].ToString()).Month) 
					{
						inswriteoffadj-=PIn.Decimal(tableWriteOffAdjustments.Rows[j]["WriteOffEst"].ToString())+PIn.Decimal(tableWriteOffAdjustments.Rows[j]["WriteOff"].ToString());
					}
				}
				for(int j=0;j<tablePay.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Year
						&& dates[i].Month==PIn.Date(tablePay.Rows[j]["DatePay"].ToString()).Month)
					{
						ptincome+=PIn.Decimal(tablePay.Rows[j]["Income"].ToString());
						unearnedPtIncome+=PIn.Decimal(tablePay.Rows[j]["UnearnedIncome"].ToString());
					}
				}
				for(int j=0;j<tableIns.Rows.Count;j++) {
					if(dates[i].Year==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Year
						&& dates[i].Month==PIn.Date(tableIns.Rows[j]["CheckDate"].ToString()).Month)
					{
						insincome+=PIn.Decimal(tableIns.Rows[j]["Ins"].ToString());
					}
				}
				if(hasChangeInWriteoff) {
					totalproduction=production+adjust+inswriteoffest+inswriteoffadj;
				}
				else {
					totalproduction=production+adjust+inswriteoff;
				}
				totalPtIncome=ptincome+unearnedPtIncome;
				totalincome=totalPtIncome+insincome;
				row["Production"]=production.ToString("n");
				row["Adjustments"]=adjust.ToString("n");
				if(hasChangeInWriteoff) {
					row["Writeoff Est"]=inswriteoffest.ToString("n");
					row["Writeoff Adj"]=inswriteoffadj.ToString("n");
				}
				else {
					row["Writeoff"]=inswriteoff.ToString("n");
				}
				row["Tot Prod"]=totalproduction.ToString("n");
				row["Pt Income"]=ptincome.ToString("n");
				if(isUnearnedIncluded) {
					row["Unearned Pt Income"]=unearnedPtIncome.ToString("n");
				}
				row["Ins Income"]=insincome.ToString("n");
				row["Total Income"]=totalincome.ToString("n");
				row["Total Pt Income"]=totalPtIncome.ToString();
				dt.Rows.Add(row);
			}
			DataSet ds=null;
			ds=new DataSet("AnnualData");
			ds.Tables.Add(dt);
			if(listClinics.Count!=0) {
				ds.Tables.Add(dtClinic);
			}
			return ds;
		}

		///<summary>Returns a dataset that contains 6 tables used to generate the annual report. If not using clinics then supply an empty list of clinics.
		/// Does not work for Oracle (by chance not by design). Consider enhancing with DbHelper.Year(),DbHelper.Month(), DbHelper.Day() and enhancing the GroupBy Logic.</summary>
		public static DataSet GetAnnualProdIncDataSet(DateTime dateFrom,DateTime dateTo,List<Provider> listProvs,List<Clinic> listClinics,bool writeOffPay,
			bool hasAllProvs,bool hasAllClinics,bool hasChangeInWriteoff,bool isUnearnedIncluded,bool isCEMT=false) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvs,listClinics,writeOffPay,hasAllProvs,hasAllClinics,hasChangeInWriteoff,isUnearnedIncluded,isCEMT);
			}
			List<long> listClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
			List<long> listProvNums=listProvs.Select(x => x.ProvNum).ToList();
			List<long> listHiddenUnearnedDefNums=GetHiddenUnearnedDefNums(isCEMT);
			#region Procedures
			string whereProv="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv=" AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			string whereClin="";
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			//==Travis (04/11/2014): In the case that you have two capitation plans for a single patient the query below will cause a duplicate row, incorectly increasing your production.
			//	We now state in the manual that having two capitation plans is not advised and will cause reporting to be off.
			string command="SELECT "
				+"procedurelog.ProcDate,procedurelog.ClinicNum,"
				+"SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits))-IFNULL(SUM(cp.WriteOff),0) Production "
				+"FROM procedurelog "
				+"LEFT JOIN (SELECT SUM(claimproc.WriteOff) AS WriteOff, claimproc.ProcNum FROM claimproc "
				+$"WHERE claimproc.Status={POut.Int((int)ClaimProcStatus.CapComplete)} "
				+"GROUP BY claimproc.ProcNum) cp ON procedurelog.ProcNum=cp.ProcNum "
				+$"WHERE procedurelog.ProcStatus = {POut.Int((int)ProcStat.C)} "
				+whereProv
				+whereClin
				+"AND procedurelog.ProcDate >= " +POut.Date(dateFrom)+" "
				+"AND procedurelog.ProcDate <= " +POut.Date(dateTo)+" "
				+"GROUP BY ClinicNum,YEAR(procedurelog.ProcDate),MONTH(procedurelog.ProcDate)";//Does not work for Oracle. Consider enhancing with DbHelper.Year(),DbHelper.Month()
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
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv=" AND adjustment.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND adjustment.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT "
				+"adjustment.AdjDate,"
				+"adjustment.ClinicNum,"
				+"SUM(adjustment.AdjAmt) Adjustment "
				+"FROM adjustment "
				+"WHERE adjustment.AdjDate >= "+POut.Date(dateFrom)+" "
				+"AND adjustment.AdjDate <= "+POut.Date(dateTo)+" "
				+whereProv
				+whereClin
				+"GROUP BY ClinicNum,YEAR(adjustment.AdjDate),MONTH(adjustment.AdjDate)";
			command+=" ORDER BY ClinicNum,AdjDate";
			DataTable tableAdj=new DataTable();
			if(isCEMT) {
				tableAdj=Db.GetTable(command);
			}
			else { 
				tableAdj=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableAdj.TableName="tableAdj";
			#endregion
			#region InsWriteoff
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv=" AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			if(writeOffPay) {
				command="SELECT "
					+"claimproc.DateCP Date," 
					+"claimproc.ClinicNum,"
					+"SUM(claimproc.WriteOff) WriteOff "
					+"FROM claimproc "
					+"WHERE claimproc.DateCP >= "+POut.Date(dateFrom)+" "
					+"AND claimproc.DateCP <= "+POut.Date(dateTo)+" "
					+whereProv
					+whereClin
					+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "//Received or supplemental
					+"GROUP BY ClinicNum,YEAR(claimproc.DateCP),MONTH(claimproc.DateCP)";
				command+=" ORDER BY ClinicNum,DateCP";
			}
			else if(hasChangeInWriteoff) {
				command="SELECT claimsnapshot.DateTEntry Date, "
					+"SUM("+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+") WriteOffEst, "
					+"claimproc.ClinicNum, "
					+"claimproc.ClaimProcNum "
					+"FROM claimproc "
					+"INNER JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
					+"WHERE "+DbHelper.BetweenDates("claimsnapshot.DateTEntry",dateFrom,dateTo)+" "
					+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "
					+whereProv
					+whereClin
					+"GROUP BY ClinicNum,YEAR(claimsnapshot.DateTEntry),MONTH(claimsnapshot.DateTEntry) "
					+"ORDER BY ClinicNum,Date";
			}
			else {
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
					+"GROUP BY ClinicNum,YEAR(claimproc.ProcDate), MONTH(claimproc.ProcDate)";
				command+=" ORDER BY ClinicNum,ProcDate";
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
			#region PtIncome
			whereProv="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv=" AND paysplit.ProvNum IN ("+String.Join(",",listProvNums);
				whereProv+=") ";
			} 
			if(!isUnearnedIncluded) {//UnearnedType of 0 means the paysplit is NOT unearned
				whereProv+=" AND paysplit.UnearnedType=0 ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND paysplit.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT "
				+"paysplit.DatePay,"
				+"paysplit.ClinicNum,"
				+"SUM(IF(paysplit.UnearnedType = 0,paysplit.SplitAmt,0)) Income,"
				+"SUM(IF(paysplit.UnearnedType != 0,paysplit.SplitAmt,0)) UnearnedIncome "
				+"FROM paysplit "
				+"WHERE paysplit.IsDiscount=0 "//AND paysplit.PayNum=payment.PayNum "
				+whereProv
				+whereClin;
				if(listHiddenUnearnedDefNums.Count>0){
					command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
				}
				command+="AND paysplit.DatePay >= "+POut.Date(dateFrom)+" "
				+"AND paysplit.DatePay <= "+POut.Date(dateTo)+" "
				+"GROUP BY ClinicNum,YEAR(paysplit.DatePay),MONTH(paysplit.DatePay)";
			command+=" ORDER BY ClinicNum,DatePay";
			DataTable tablePay=new DataTable();
			if(isCEMT) {
				tablePay=Db.GetTable(command);
			}
			else { 
				tablePay=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tablePay.TableName="tablePay";
			#endregion
			#region InsIncome
			whereProv="";
			if(!hasAllProvs && listProvNums.Count>0) {
				whereProv=" AND claimproc.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(!hasAllClinics && listClinicNums.Count>0) {
				whereClin=" AND claimproc.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			command="SELECT claimpayment.CheckDate,claimproc.ClinicNum,SUM(claimproc.InsPayamt) Ins "
				+"FROM claimpayment,claimproc WHERE "
				+"claimproc.ClaimPaymentNum = claimpayment.ClaimPaymentNum "
				+"AND claimpayment.CheckDate >= " + POut.Date(dateFrom)+" "
				+"AND claimpayment.CheckDate <= " + POut.Date(dateTo)+" "
				+whereProv
				+whereClin
				+" GROUP BY claimpayment.CheckDate,ClinicNum ORDER BY ClinicNum,CheckDate";
			DataTable tableIns=new DataTable();
			if(isCEMT) {
				tableIns=Db.GetTable(command);
			}
			else { 
				tableIns=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			}
			tableIns.TableName="tableIns";
			#endregion
			#region WriteOffAdjustments
			DataTable tableWriteOffAdjustments=new DataTable();
			if(hasChangeInWriteoff) {
				//Insurance WriteOff Adjustments----------------------------------------------------------------------------
				command="SELECT claimproc.DateCP Date, "
				+"claimproc.ClinicNum, "
				+"-SUM("+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+") WriteOffEst, "
				+"SUM(claimproc.WriteOff) WriteOff, "
				+"claimproc.ClaimNum "
				+"FROM claimproc "
				+"LEFT JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "
				+"WHERE claimproc.DateCP BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" "
				+"AND claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "
				+whereProv
				+whereClin
				+" GROUP BY ClinicNum,YEAR(claimproc.DateCP), MONTH(claimproc.DateCP) ";
				if(isCEMT) {
					tableWriteOffAdjustments=Db.GetTable(command);
				}
				else { 
					tableWriteOffAdjustments=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
				}
			}
			tableWriteOffAdjustments.TableName="tableWriteOffAdjustments";
			#endregion
			DataSet dataSet=new DataSet();
			dataSet.Tables.Add(tableProduction);
			dataSet.Tables.Add(tableAdj);
			dataSet.Tables.Add(tableInsWriteoff);
			dataSet.Tables.Add(tablePay);
			dataSet.Tables.Add(tableIns);
			dataSet.Tables.Add(tableWriteOffAdjustments);
			return dataSet;
		}
		#endregion

		private static int RowComparer(DataRow x,DataRow y) {
			if(_hasClinics) {
				string xClinic=x["Clinic"].ToString();
				string yClinic=y["Clinic"].ToString();
				if(xClinic!=yClinic) {//Sort by clinic first, if no clinic then they'll be the same empty string.
					return String.Compare(xClinic,yClinic);
				}
			}
			DateTime xDate=PIn.Date(x["Date"].ToString());
			DateTime yDate=PIn.Date(y["Date"].ToString());
			if(xDate!=yDate) {//Then by date
				return DateTime.Compare(xDate,yDate);
			}
			string xName=x["Name"].ToString();
			string yName=y["Name"].ToString();
			if(xName!=yName) {//Then by name
				return String.Compare(xName,yName);
			}
			//We might want to include transaction type here but procedures have all different kinds of descriptions.
			string xProvider=x["Provider"].ToString();
			string yProvider=y["Provider"].ToString();
			if(xProvider!=yProvider) {//Then by provider (just for looks).
				return String.Compare(xProvider,yProvider);
			}
			return 0;
		}
		
		///<summary>This class is only used for the Provider Payroll report.  It is to be able to use Linq to cut down on processing time.</summary>
		private class ProviderPayrollRow {
			public DateTime Date;
			public long PatNum;
			public decimal Amount;
			///<summary>Only used by the InsWriteOff table</summary>
			public decimal WriteOff;
			///<summary>Only used by the InsWriteOff table</summary>
			public decimal WriteOffEst;
			
			///<summary>Used by all tabled except the InsWriteOff table</summary>
			public ProviderPayrollRow(DateTime date,decimal amount,long patNum) {
				Date=date;
				Amount=amount;
				PatNum=patNum;
			}
			
			///<summary>Only used by the InsWriteOff table</summary>
			public ProviderPayrollRow(DateTime date,decimal writeOffAmt,decimal writeOffEst,long patNum) {
				Date=date;
				WriteOff=writeOffAmt;
				WriteOffEst=writeOffEst;
				PatNum=patNum;
			}

			public static ProviderPayrollRow DataRowToPayrollRow(DataRow row,bool isWriteOffTable) {
				DateTime date=PIn.Date(row["TranDate"].ToString());
				decimal amount=PIn.Decimal(row["Amount"].ToString());
				decimal writeOffAmt=PIn.Decimal(row["WriteOff"].ToString());
				decimal writeOffEst=PIn.Decimal(row["WriteOffEst"].ToString());
				long patNum=PIn.Long(row["PatNum"].ToString());
				if(isWriteOffTable) {
					return new ProviderPayrollRow(date,writeOffAmt,writeOffEst,patNum);
				}
				else {
					return new ProviderPayrollRow(date,amount,patNum);
				}
			}
		}
	}
}
