using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class RpUnearnedIncome {

		/// <summary>Retrieves the Line Item Unearned dataset from the database.</summary>
		/// <param name="listClinics">The list of clinics to filter by. Pass in an empty list if this should not be filtered by clinic.</param>
		/// <returns></returns>
		public static DataTable GetLineItemUnearnedData(List<long> listClinics,DateTime date1Start,DateTime date2Start,bool showProvider) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listClinics,date1Start,date2Start,showProvider);
			}
			bool hasClinicsEnabled = ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache);
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			string command = "";
			string whereClin = "";
			//This query is kind-of a mess, but we're trying to account for bugs in previous versions.
			//Logic is meant to match the account module "Unearned" value logic as closely as possible.
			if(listClinics.Count>0) {
				whereClin="AND paysplit.ClinicNum IN ("+string.Join(",",listClinics)+") ";
			}
			//Outer Select
			command="SELECT results.DatePay,"+DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI")+" Patient,"
				+"definition.ItemName,";
			if(hasClinicsEnabled) {
				command+="clinic.Abbr Clinic,";
			}
			if(showProvider) {
				command+="provider.Abbr,";
			}
			command+="results.SplitAmt FROM (";
			command+="SELECT SplitNum,DatePay,PatNum,UnearnedType,ClinicNum,SplitAmt,ProvNum FROM paysplit "
				+"WHERE paysplit.DatePay >= "+POut.Date(date1Start)+" "
				+"AND paysplit.DatePay <= "+POut.Date(date2Start)+" ";
				if(listHiddenUnearnedDefNums.Count>0) {
					command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
				}
				command+=whereClin
				+"AND UnearnedType!=0 ";
			//is to also show unearned that is now allocated. 
			command+=") results "
				+"INNER JOIN patient ON patient.PatNum=results.PatNum "
				+"LEFT JOIN definition ON definition.DefNum=results.UnearnedType ";
			if(showProvider) {
				command+="LEFT JOIN provider on provider.ProvNum=results.ProvNum ";
			}
			if(hasClinicsEnabled) {
				command+="LEFT JOIN clinic ON clinic.ClinicNum=results.ClinicNum ";
			}
			command+="ORDER BY results.DatePay,Patient,results.SplitNum";
			DataTable raw=ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(command));
			return raw;
		}

		public static DataTable GetUnearnedAllocationData(List<long> listClinicNums,List<long> listProvNums,
			List<long> listUnearnedTypeNums,bool isExcludeNetZeroUnearned,bool showProvider) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listClinicNums,listProvNums,listUnearnedTypeNums,isExcludeNetZeroUnearned,showProvider);
			}
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			//get all families that have an unallocated unearned balance.
			//from those, remove families that have not had procedures charted/completed after the unearned amount.

			//All families
			//DatePay = the earliest date of unallocated unearned.
			//Unallocated Amt = the total unallocated amt for the patient.
			string command = $@"
				SELECT patient.Guarantor, MIN(paysplit.DatePay) DatePay, SUM(paysplit.SplitAmt) UnallocAmt{(showProvider ? ", provider.Abbr" : "")}
				FROM paysplit
				INNER JOIN patient ON patient.PatNum = paysplit.PatNum ";
			if(listClinicNums.Count>0 || listProvNums.Count>0) {
				command += "INNER JOIN patient guar ON guar.PatNum = patient.Guarantor ";
				if(listClinicNums.Count>0) {
					command += "AND guar.ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
				}
				if(listProvNums.Count>0) {
					command += "AND guar.PriProv IN ("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+") ";
				}
			}
			if(showProvider) {
				command+="LEFT JOIN provider ON provider.ProvNum = paysplit.ProvNum ";
			}
			command +="WHERE paysplit.UnearnedType != 0 ";
			if(listUnearnedTypeNums.Count>0) {
				command +="AND paysplit.UnearnedType IN ("+string.Join(",",listUnearnedTypeNums.Select(x => POut.Long(x)))+") ";
			}
			if(listHiddenUnearnedDefNums.Count > 0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			command+=$"GROUP BY patient.Guarantor{(showProvider ? ", provider.Abbr" : "")} ";
			if(isExcludeNetZeroUnearned) {
				command+="HAVING ABS(UnallocAmt) > 0.005 ";
			}
			//one row per family
			DataTable tableUnallocatedUnearned = ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			List<long> listGuarantors = tableUnallocatedUnearned.Rows.OfType<DataRow>().Select(x => PIn.Long(x["Guarantor"].ToString())).ToList();
			//all procedures for the families that have not been explicitly paid off.
			//Key: GuarantorNum | Val:ListRemainingProcsForFam
			List<UnearnedProc> listRemProcs = ReportsComplex.RunFuncOnReportServer(() => Procedures.GetRemainingProcsForFamilies(listGuarantors));
			Dictionary<long,List<UnearnedProc>> dictFamRemainingProcs = listRemProcs.GroupBy(x => x.GuarNum)
				.ToDictionary(x => x.Key,y => y.ToList());
			Dictionary<long,double> dictFamilyBalances = ReportsComplex.RunFuncOnReportServer(() => Ledgers.GetBalancesForFamilies(listGuarantors));
			Dictionary<long,string> dictPatNames = ReportsComplex.RunFuncOnReportServer(() => 
				Patients.GetPatientNames(Patients.GetAllFamilyPatNums(listGuarantors)));
			List<ProcedureCode> listProcCodes = ReportsComplex.RunFuncOnReportServer(() => ProcedureCodes.GetAllCodes());
			DataTable retVal = new DataTable();
			retVal.Columns.Add("Guar");
			retVal.Columns.Add("FamBal");
			retVal.Columns.Add("FamUnearned");
			retVal.Columns.Add("FamRemAmt");
			if(showProvider) { 
				retVal.Columns.Add("Prov"); 
			}
			retVal.Columns.Add("Patient");
			retVal.Columns.Add("Code");
			retVal.Columns.Add("Date");
			retVal.Columns.Add("Fee");
			retVal.Columns.Add("RemAmt");
			int rowCount=tableUnallocatedUnearned.Rows.Count;//For brevity
			//This has to be a for-loop instead of foreach so we can access the guarantor number from the next iteration
			//prior to adding the procedures to the report (to validate whether or not we should add another guarantor row
			//for a provider
			for(int i=0; i<rowCount;i++) {
				DataRow guarRowCur=tableUnallocatedUnearned.Rows[i];
				int nextIndex=i+1;
				long guarNum = PIn.Long(guarRowCur["Guarantor"].ToString());
				DateTime dateFirstUnalloc = PIn.Date(guarRowCur["DatePay"].ToString());
				double unallocAmt = PIn.Double(guarRowCur["UnallocAmt"].ToString());
				List<UnearnedProc> listUnearnedProcsForGuar;
				if(!dictFamRemainingProcs.TryGetValue(guarNum,out listUnearnedProcsForGuar)) {
					continue;//This family does not have any procedures that need to have money allocated to.
				}
				listUnearnedProcsForGuar = listUnearnedProcsForGuar.Where(x => x.Proc.ProcDate >= dateFirstUnalloc).OrderBy(x => x.Proc.ProcDate).ToList();
				if(listUnearnedProcsForGuar.Count == 0) {
					continue;//We only want to show families where the procedure was completed after the unallocated prepayment.
				}
				decimal famRemAmt = listUnearnedProcsForGuar.Sum(x => x.UnallocatedAmt);
				DataRow guarRow = retVal.NewRow();
				string guarName;
				double famBal;
				dictPatNames.TryGetValue(guarNum,out guarName);
				dictFamilyBalances.TryGetValue(guarNum,out famBal);
				guarRow["Guar"] = guarName;
				guarRow["FamBal"] = famBal.ToString("f");
				guarRow["FamUnearned"] = unallocAmt.ToString("f");
				guarRow["FamRemAmt"] = famRemAmt.ToString("f");
				if(showProvider) { 
					guarRow["Prov"] = guarRowCur["Abbr"];
				}
				retVal.Rows.Add(guarRow);
				//If the next row has the same guarantor, then we know that it is another provider for this account and we should not populate the procedures yet
				if(nextIndex<rowCount && guarNum==PIn.Long(tableUnallocatedUnearned.Rows[nextIndex]["Guarantor"].ToString())) { 
					continue;
				}
				foreach(UnearnedProc unearnedProc in listUnearnedProcsForGuar) {
					DataRow newRow = retVal.NewRow();
					dictPatNames.TryGetValue(guarNum,out guarName);
					string patName;
					if(dictPatNames.TryGetValue(unearnedProc.Proc.PatNum,out patName)) {
						newRow["Patient"] = patName;
					}
					newRow["Code"] = ProcedureCodes.GetStringProcCode(unearnedProc.Proc.CodeNum,listProcCodes);
					newRow["Date"] = unearnedProc.Proc.ProcDate.ToShortDateString();
					newRow["Fee"] = unearnedProc.Proc.ProcFeeTotal.ToString("f");
					newRow["RemAmt"] = unearnedProc.UnallocatedAmt.ToString("f");
					retVal.Rows.Add(newRow);
				}
			}
			return retVal;
		}
		
		public static DataTable GetNetUnearnedData(List<long> listClinicNums,List<long> listProvNums,
			List<long> listUnearnedTypeNums,bool isExcludeNetZero) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listClinicNums,listProvNums,listUnearnedTypeNums,isExcludeNetZero);
			}
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			//show all families where unearned income was collected and no unallocated procedures exist.
			/*All families with unallocated paysplits*/
			DataTable retVal = new DataTable();
			retVal.Columns.Add("PatientName");
			retVal.Columns.Add("GuarantorName");
			retVal.Columns.Add("PatUnearnedAmt");
			retVal.Columns.Add("FamBal");
			string command = @"
			SELECT patient.Guarantor, paysplit.PatNum, patient.FName, patient.LName,
			guar.FName GuarF, guar.LName GuarL,SUM(paysplit.SplitAmt) UnallocatedAmt
			FROM paysplit
			INNER JOIN patient ON patient.PatNum = paysplit.PatNum ";
			if(listClinicNums.Count>0) {
				command += @"
					AND patient.ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			if(listProvNums.Count>0) {
				command += @"
					AND patient.PriProv IN ("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+") ";
			}
			command += @"
			INNER JOIN patient guar ON guar.PatNum = patient.Guarantor 
			WHERE paysplit.UnearnedType != 0 ";
			if(listUnearnedTypeNums.Count>0) {
				command +="AND paysplit.UnearnedType IN ("+string.Join(",",listUnearnedTypeNums.Select(x => POut.Long(x)))+") ";
			}
			if(listHiddenUnearnedDefNums.Count > 0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			command += "GROUP BY paysplit.PatNum ";
			if(isExcludeNetZero) {
				command+="HAVING ABS(UnallocatedAmt) > 0.005 ";
			}
			DataTable tableUnallocatedPrepayments = ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			//get remaining amount for all procedures of the returned families.
			List<long> listGuarantorNums = tableUnallocatedPrepayments.Rows.OfType<DataRow>().Select(x => PIn.Long(x["Guarantor"].ToString())).ToList();
			if(listGuarantorNums.Count == 0) {
				return retVal;//No families have paysplits with unallocated prepayments. Return empty table.
			}
			/*As long as any patient in the family has AT LEAST ONE procedure that is not fully, explicitly paid off, they will not show in this report.*/
			List<UnearnedProc> listGuarantorRemainingProcs = ReportsComplex.RunFuncOnReportServer(() => 
				Procedures.GetRemainingProcsForFamilies(listGuarantorNums));
			Dictionary<long,double> dictFamilyBalances = ReportsComplex.RunFuncOnReportServer(() => Ledgers.GetBalancesForFamilies(listGuarantorNums));
			foreach(DataRow rowCur in tableUnallocatedPrepayments.Rows) {
				long unallocatedGuarantor = PIn.Long(rowCur["Guarantor"].ToString());
				if(listGuarantorRemainingProcs.Select(x => x.GuarNum).Contains(unallocatedGuarantor)) {
					continue;//Has at least one procedure that is not fully paid off.
				}
				double famBal;
				if(!dictFamilyBalances.TryGetValue(unallocatedGuarantor,out famBal)) {
					famBal=0;
				}
				DataRow newRow = retVal.NewRow();
				newRow["PatientName"] = rowCur["LName"].ToString() + ", " + rowCur["FName"].ToString();
				newRow["GuarantorName"] = rowCur["GuarL"].ToString() + ", " + rowCur["GuarF"].ToString();
				newRow["PatUnearnedAmt"] = PIn.Double(rowCur["UnallocatedAmt"].ToString());
				newRow["FamBal"] = famBal.ToString("f");
				retVal.Rows.Add(newRow);
			}
			return retVal;
		}

		/// <summary>Retrieves the Unearned Accounts dataset from the database.</summary>
		/// <param name="listClinics">The list of clinics to filter by. Pass in an empty list if this should not be filtered by clinic.</param>
		/// <returns></returns>
		public static DataTable GetUnearnedAccountData(List<long> listClinics) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listClinics);
			}
			bool hasClinicsEnabled = ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache);
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			string command = "";
			string whereClin = "";
			//We used to get original paysplits based on FSplitNum being 0 and having an unearned type and then get the offsetting splits from the
			//FSplitNum matching the original paysplit num. 
			//FSplitNums no longer are important when calculating unearned so they are no included in this logic intentionally.
			//The patient table joins are quite slow for large customers, which is why they were moved outside the FROM.
			//If a customer complains we might do some logic to get the patnums of any family member in that clinic first, so we can filter down the unions.
			if(listClinics.Count>0) {
				whereClin="WHERE guar.ClinicNum IN ("+string.Join(",",listClinics)+") ";
			}
			command="SELECT "+DbHelper.Concat("guar.LName","', '","guar.FName","' '","guar.MiddleI")+",";
			command+=DbHelper.GroupConcat("definition.ItemName",true,true,",");
			if(hasClinicsEnabled) {
				command+=",clinic.Abbr";
			}
			command+=",SUM(splits.Amt) Amount FROM (";
			//Prepay. Unearned is simply defined as having an unearned type set. Nothing more. 
			command+="SELECT paysplit.PatNum, paysplit.SplitAmt Amt,paysplit.UnearnedType "
				+"FROM paysplit "
				+"WHERE paysplit.UnearnedType!=0 ";
			if(listHiddenUnearnedDefNums.Count>0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			command+="GROUP BY paysplit.SplitNum";
			command+=") splits "
				+"INNER JOIN patient ON patient.PatNum=splits.PatNum "
				+"INNER JOIN patient guar ON guar.PatNum=patient.Guarantor "
				+"LEFT JOIN definition ON definition.DefNum=splits.UnearnedType ";
			if(hasClinicsEnabled) {
				command+="LEFT JOIN clinic ON clinic.ClinicNum=guar.ClinicNum ";
			}
			command+=whereClin;
			command+="GROUP BY guar.PatNum HAVING ABS(Amount) > 0.005 ";//still won't work for oracle
			command+="ORDER BY guar.LName, guar.FName, guar.MiddleI, Amount";
			DataTable raw = ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(command));
			return raw;
		}

		public class UnearnedProc {
			public Procedure Proc {
				get;
			}
			public long GuarNum {
				get;
			}
			public decimal UnallocatedAmt {
				get;
			}

			public UnearnedProc(Procedure proc,long guarantor,decimal unallocAmt) {
				Proc=proc;
				GuarNum=guarantor;
				UnallocatedAmt=unallocAmt;
			}

		}
	}


}
