using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;
using Avalara.AvaTax.RestClient;

namespace OpenDentBusiness{
	///<summary>Handles database commands related to the adjustment table in the db.</summary>
	public class Adjustments {
		#region Get Methods
		public static List<Adjustment> GetMany(List<long> listAdjNums) {
			if(listAdjNums.IsNullOrEmpty()) {
				return new List<Adjustment>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),listAdjNums);
			}
			string command=$"SELECT * FROM adjustment WHERE adjustment.AdjNum IN ({string.Join(",",listAdjNums.Select(x => POut.Long(x)))})";
			return Crud.AdjustmentCrud.SelectMany(command);
		}

		///<summary>Gets all adjustments for a single patient.</summary>
		public static Adjustment[] Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Adjustment[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM adjustment"
				+" WHERE PatNum = "+POut.Long(patNum)+" ORDER BY AdjDate";
			return Crud.AdjustmentCrud.SelectMany(command).ToArray();
		}

		///<summary></summary>
		public static List<Adjustment> GetPatientData(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM adjustment"
				+" WHERE PatNum = "+POut.Long(patNum)+" ORDER BY AdjDate";
			return Crud.AdjustmentCrud.SelectMany(command);
		}

		///<summary>Gets adjustments for a given set of parameters. Can filter by AdjType or ProcNum. 
		///Returns an empty list if not found.</summary>
		public static List<Adjustment> GetAdjustmentsForApi(int limit,int offset,long patNum,long adjType=0,long procNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),limit,offset,patNum,adjType,procNum);
			}
			string command=
				"SELECT * FROM adjustment"
				+" WHERE PatNum = "+POut.Long(patNum);
			if(adjType!=0) {
				command+=" AND AdjType = "+POut.Long(adjType);
			}
			if(procNum!=0) {
				command+=" AND ProcNum = "+POut.Long(procNum);
			}
			command+=" ORDER BY AdjDate "
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.AdjustmentCrud.SelectMany(command);
		}

		///<summary>Gets one adjustment from the db.</summary>
		public static Adjustment GetOne(long adjNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Adjustment>(MethodBase.GetCurrentMethod(),adjNum);
			}
			string command=
				"SELECT * FROM adjustment"
				+" WHERE AdjNum = "+POut.Long(adjNum);
			return Crud.AdjustmentCrud.SelectOne(adjNum);
		}

		///<summary>Gets the amount used for the specified adjustment (Sums paysplits that have AdjNum passed in).  Pass in PayNum to exclude splits on that payment.</summary>
		public static double GetAmtAllocated(long adjNum,long payNumExcluded,List<PaySplit> listPaySplits=null) {
			if(listPaySplits!=null) {
				return listPaySplits.FindAll(x => x.PayNum!=payNumExcluded).Sum(x => x.SplitAmt);
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod(),adjNum,payNumExcluded,listPaySplits);
			}
			string command="SELECT SUM(SplitAmt) FROM paysplit WHERE AdjNum="+POut.Long(adjNum);
			if(payNumExcluded!=0) {
				command+=" AND PayNum!="+POut.Long(payNumExcluded);
			}
			return PIn.Double(Db.GetScalar(command));
		}

		///<summary>Gets all adjustments for the patients passed in.</summary>
		public static List<Adjustment> GetAdjustForPats(List<long> listPatNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * FROM adjustment "
				+"WHERE PatNum IN("+String.Join(", ",listPatNums)+") ";
			return Crud.AdjustmentCrud.SelectMany(command);
		}

		///<summary>Loops through the supplied list of adjustments and returns an ArrayList of adjustments for the given proc.</summary>
		public static List<Adjustment> GetForProc(long procNum,Adjustment[] adjustmentArray) {
			//No need to check MiddleTierRole; no call to db.
			List<Adjustment> listAdjustments=new List<Adjustment>();
			for(int i=0;i<adjustmentArray.Length;i++){
				if(adjustmentArray[i].ProcNum==procNum){
					listAdjustments.Add(adjustmentArray[i]);
				}
			}
			return listAdjustments;
		}

		public static List<Adjustment> GetListForProc(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT * FROM adjustment WHERE ProcNum="+POut.Long(procNum);
			return Crud.AdjustmentCrud.SelectMany(command);
		}

		public static List<Adjustment> GetForProcs(List<long> listProcNums) {
			List<Adjustment> listAdjustments=new List<Adjustment>();
			if(listProcNums==null || listProcNums.Count < 1) {
				return listAdjustments;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			string command="SELECT * FROM adjustment WHERE ProcNum IN("+string.Join(",",listProcNums)+")";
			return Crud.AdjustmentCrud.SelectMany(command);
		}

		///<summary>(HQ Only) Returns the AvaTax sales tax adjustment attached to this procedure with a TaxTransID. We do not use the AvaTax.SalesTaxAdjType in case the
		///defnum has changed in the past. </summary>
		public static Adjustment GetSalesTaxForProc(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Adjustment>(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT * FROM adjustment WHERE ProcNum="+POut.Long(procNum)+" AND AdjType="+POut.Long(AvaTax.GetSalesTaxAdjType());
			return Crud.AdjustmentCrud.SelectMany(command).FirstOrDefault(x => x.AdjType==AvaTax.GetSalesTaxAdjType());
		}

		///<summary>Sums all adjustments for a proc then returns that sum. Pass false to canIncludeTax in order to exclude sales tax from the end amount.</summary>
		public static double GetTotForProc(long procNum,bool canIncludeTax=true) {
			//No need to check MiddleTierRole; no call to db.
			List<long> listProcNums=new List<long>(){procNum};
			return GetTotForProcs(listProcNums,canIncludeTax);
		}

		///<summary>Sums all adjustments for a proc then returns that sum. Pass false to canIncludeTax in order to exclude sales tax from the end amount.</summary>
		public static double GetTotForProcs(List<long> listProcNums,bool canIncludeTax=true) {
			if(listProcNums.IsNullOrEmpty()) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod(),listProcNums,canIncludeTax);
			}
			string command="SELECT SUM(AdjAmt) FROM adjustment"
				+" WHERE ProcNum IN("+string.Join(",",listProcNums.Select(x => POut.Long(x)))+")";
			if(AvaTax.IsEnabled() && !canIncludeTax) {
				command+=" AND AdjType NOT IN ("+string.Join(",",POut.Long(AvaTax.GetSalesTaxAdjType()),POut.Long(AvaTax.GetSalesTaxReturnAdjType()))+")";
			}
			return PIn.Double(Db.GetScalar(command));
		}

		///<summary></summary>
		public static double GetTotTaxForProc(Procedure procedure) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod(),procedure);
			}
			if(!AvaTax.DoSendProcToAvalara(procedure)) {
				return 0;
			}
			string command="SELECT SUM(AdjAmt) FROM adjustment"
				+" WHERE ProcNum="+POut.Long(procedure.ProcNum)
				+" AND AdjType IN ("+string.Join(",",POut.Long(AvaTax.GetSalesTaxAdjType()),POut.Long(AvaTax.GetSalesTaxReturnAdjType()))+")";
			return PIn.Double(Db.GetScalar(command));
		}

		/// <summary>Returns a DataTable of adjustments of a given adjustment type and for a given pat</summary>
		public static List<Adjustment> GetAdjustForPatByType(long patNum,long adjType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),patNum,adjType);
			}
			string queryBrokenApts="SELECT * FROM adjustment WHERE PatNum="+POut.Long(patNum)
				+" AND AdjType="+POut.Long(adjType);
			return Crud.AdjustmentCrud.SelectMany(queryBrokenApts);
		}

		/// <summary>
		///Returns a list of adjustments of a given adjustment type and for the given patNums in the listPatNums.
		/// Only considers adjs where AdjDate is strictly less than the given maxAdjDate.</summary>
		public static List<Adjustment> GetAdjustForPatsByType(List<long> listPatNums,long adjType,DateTime dateAdjMax) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),listPatNums,adjType,dateAdjMax);
			}
			if(listPatNums==null || listPatNums.Count==0) {
				return new List<Adjustment>();
			}
			string queryBrokenApts = "SELECT * FROM adjustment "
				+"WHERE PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND AdjType="+POut.Long(adjType)+" "
				+"AND "+DbHelper.DateTConditionColumn("AdjDate",ConditionOperator.LessThan,dateAdjMax);
			List<Adjustment> listAdjustments = Crud.AdjustmentCrud.SelectMany(queryBrokenApts);
			return listAdjustments;
		}
		///<summary>Used from ContrAccount and ProcEdit to display and calculate adjustments attached to procs.</summary>
		public static double GetTotForProc(long procNum,Adjustment[] List,long excludedNum=0) {
			//No need to check MiddleTierRole; no call to db.
			double retVal=0;
			for(int i=0;i<List.Length;i++){
				if((List[i].AdjNum==excludedNum)) {
					continue;
				}
				if(List[i].ProcNum==procNum){
					retVal+=List[i].AdjAmt;
				}
			}
			return retVal;
		}

		///<summary>Returns the sum of adjustments for the date range for the passed in operatories or providers.</summary>
		public static decimal GetAdjustAmtForAptView(DateTime dateStart,DateTime dateEnd,long clinicNum,List<long> listOpNums,List<long> listProvNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<decimal>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,clinicNum,listOpNums,listProvNums);
			}
			string command=GetQueryAdjustmentsForAppointments(dateStart,dateEnd,listOpNums,doGetSum:true);
			if(!listProvNums.IsNullOrEmpty()) {
				command+="AND adjustment.ProvNum IN("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+") ";
			}
			if(clinicNum>0 && PrefC.HasClinicsEnabled) {
				command+="AND adjustment.ClinicNum="+POut.Long(clinicNum);
			}
			return PIn.Decimal(Db.GetScalar(command));
		}

		public static List<Adjustment> GetForDateRange(DateTime dateStart,DateTime dateEnd,List<long> listPatNums=null,long adjType=-1,bool useProcDate=false) {
			if(dateEnd<dateStart) {
				return new List<Adjustment>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Adjustment>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listPatNums,adjType,useProcDate);
			}
			string dateColumn="AdjDate";
			if(useProcDate) {
				dateColumn="ProcDate";
			}
			string command=$"SELECT * FROM adjustment WHERE {DbHelper.BetweenDates(dateColumn,dateStart,dateEnd)} ";
			if(!listPatNums.IsNullOrEmpty()) {
				command+=$"AND PatNum IN ({String.Join(",",listPatNums.Select(x => POut.Long(x)))}) ";
			}
			if(adjType!=-1) {
				command+=$"AND AdjType={POut.Long(adjType)} ";
			}
			return Crud.AdjustmentCrud.SelectMany(command);
		}

		///<summary>Returns the sum of all negative adjustments for the given AdjType in the date range.</summary>
		public static double GetTotForPatByType(long patNum,long defNumAdjustmentType,DateTime dateStart,DateTime dateEnd,long procNumExclude=0) {
			//No need to check MiddleTierRole; no call to db.
			double total=0;
			List<Adjustment> listHistAdjustment=GetForDateRange(dateStart,dateEnd,ListTools.FromSingle(patNum),defNumAdjustmentType,useProcDate:true);
			List<Adjustment> listAdjustmentsToSum=listHistAdjustment.FindAll(x => x.AdjAmt < 0 && x.ProcNum > 0 && x.ProcNum!=procNumExclude);
			total=Math.Abs(listAdjustmentsToSum.Sum(x=>x.AdjAmt));
			return total;
		}

		public static List<double> GetAnnualTotalsForPatByDiscountPlanSub(DiscountPlanSub discountPlanSub,DiscountPlan discountPlan,
			DateTime dateProcMax,List<Adjustment> listAdjustments=null)
		{
			//No need to check MiddleTierRole; no call to db.
			List<double> listAnnualRunningTotals=new List<double>();
			if(dateProcMax.Year<1880 && listAdjustments.IsNullOrEmpty()) {
				return listAnnualRunningTotals;
			}
			DateTime startDate=DiscountPlanSubs.GetAnnualMaxDateEffective(discountPlanSub);
			DateTime endDate=DiscountPlanSubs.GetAnnualMaxDateTerm(discountPlanSub);
			return GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,dateProcMax,listAdjustments,startDate,endDate);
		}

		///<summary>Returns a list of Annual Running totals.</summary>
		public static List<double> GetAnnualTotalsForPatByDiscountPlanSub(DiscountPlanSub discountPlanSub,DiscountPlan discountPlan,DateTime maxProcDate,
			List<Adjustment> listAdjustments,DateTime startDate,DateTime endDate)
		{
			//No need to check MiddleTierRole; no call to db.
			if(listAdjustments==null) {
				listAdjustments=GetForDateRange(startDate,endDate,ListTools.FromSingle(discountPlanSub.PatNum));
			}
			List<double> listAnnualRunningTotals=new List<double>();
			if(endDate==DateTime.MaxValue) {
				endDate=maxProcDate;
			}
			if(endDate < startDate) {
				return listAnnualRunningTotals;
			}
			int annualSegments=GetDifferenceNumberOfYears(startDate,endDate);
			for(int i=0;i<annualSegments;i++) {
				listAnnualRunningTotals.Add(0);
			}
			for(int i=0;i<listAdjustments.Count;i++) {
				int index=GetAnnualMaxSegmentIndex(discountPlanSub,listAdjustments[i].ProcDate);
				if(listAdjustments[i].PatNum!=discountPlanSub.PatNum 
					|| listAdjustments[i].AdjType!=discountPlan.DefNum 
					|| listAdjustments[i].AdjAmt > 0
					|| index==-1
					|| index>=listAnnualRunningTotals.Count)
				{
					continue;
				}
				listAnnualRunningTotals[index]+=Math.Abs(listAdjustments[i].AdjAmt);
			}
			return listAnnualRunningTotals;
		}

		///<summary>Returns 0 if endDate is before startDate.</summary>
		public static int GetDifferenceNumberOfYears(DateTime startDate,DateTime endDate) {
			//No need to check MiddleTierRole; no call to db.
			int years=0;
			DateTime iterativeDateTime=startDate;
			while(iterativeDateTime<=endDate) {
				years++;
				iterativeDateTime=iterativeDateTime.AddYears(1);
			}
			return years;
		}

		///<summary>Returns the index for a dateTime to get the annualMax running total from GetAnnualTotalsForPatByDiscountPlanSub. Returns -1 on invalid dateTime, or minVal.</summary>
		public static int GetAnnualMaxSegmentIndex(DiscountPlanSub discountPlanSub,DateTime dateTime) {
			//No need to check MiddleTierRole; no call to db.
			if(dateTime==DateTime.MinValue || discountPlanSub==null) {
				return -1;
			}
			DateTime startDate=DiscountPlanSubs.GetAnnualMaxDateEffective(discountPlanSub);
			DateTime endDate=DiscountPlanSubs.GetAnnualMaxDateTerm(discountPlanSub);
			if(dateTime<startDate || dateTime>endDate) {
				return -1;
			}
			int index=GetDifferenceNumberOfYears(startDate,dateTime);
			if(index > 0) {
				index-=1; //Index is 0 based.
			}
			return index;
		}
		
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(Adjustment adjustment) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				adjustment.AdjNum=Meth.GetLong(MethodBase.GetCurrentMethod(),adjustment);
				return adjustment.AdjNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			adjustment.SecUserNumEntry=Security.CurUser.UserNum;
			long adjNum=Crud.AdjustmentCrud.Insert(adjustment);
			CreateOrUpdateSalesTaxIfNeeded(adjustment); //Do the update after the insert so the AvaTax API can include the new adjustment in the calculation
			return adjNum;
		}

		///<summary>Creates a new discount adjustment for the given procedure.</summary>
		public static void CreateAdjustmentForDiscount(Procedure procedure) {
			//No need to check MiddleTierRole; no call to db.
			Adjustment adjustmentDiscount=new Adjustment();
			adjustmentDiscount.DateEntry=DateTime.Today;
			adjustmentDiscount.AdjDate=DateTime.Today;
			adjustmentDiscount.ProcDate=procedure.ProcDate;
			adjustmentDiscount.ProvNum=procedure.ProvNum;
			adjustmentDiscount.PatNum=procedure.PatNum;
			adjustmentDiscount.AdjType=PrefC.GetLong(PrefName.TreatPlanDiscountAdjustmentType);
			adjustmentDiscount.ClinicNum=procedure.ClinicNum;
			adjustmentDiscount.AdjAmt=-procedure.Discount;//Discount must be negative here.
			adjustmentDiscount.ProcNum=procedure.ProcNum;
			Insert(adjustmentDiscount);
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adjustmentDiscount);
		}

		///<summary>Creates a new discount adjustment for the given procedure using the discount plan fee. Returns the amount of the adjustment that was created.</summary>
		public static double CreateAdjustmentForDiscountPlan(Procedure procedure) {
			//No need to check MiddleTierRole; no call to db.
			DiscountPlanSub discountPlanSub=DiscountPlanSubs.GetSubForPat(procedure.PatNum);
			if(discountPlanSub==null) {
				return 0;
			}
			DiscountPlan discountPlan=DiscountPlans.GetPlan(discountPlanSub.DiscountPlanNum);
			if(discountPlan==null) { //Patient doesn't have a discountPlan
				return 0;
			}
			double adjAmt=Procedures.GetDiscountAmountForDiscountPlan(procedure,discountPlanSub:discountPlanSub,discountPlan:discountPlan);
			if(adjAmt<=0) {
				return 0;
			}
			Adjustment adjustmentDiscountPlan=new Adjustment();
			adjustmentDiscountPlan.DateEntry=DateTime.Today;
			adjustmentDiscountPlan.AdjDate=DateTime.Today;
			adjustmentDiscountPlan.ProcDate=procedure.ProcDate;
			adjustmentDiscountPlan.ProvNum=procedure.ProvNum;
			adjustmentDiscountPlan.PatNum=procedure.PatNum;
			adjustmentDiscountPlan.AdjType=discountPlan.DefNum;
			adjustmentDiscountPlan.ClinicNum=procedure.ClinicNum;
			adjustmentDiscountPlan.AdjAmt=(-adjAmt);
			adjustmentDiscountPlan.ProcNum=procedure.ProcNum;
			Insert(adjustmentDiscountPlan);
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adjustmentDiscountPlan);
			SecurityLogs.MakeLogEntry(Permissions.AdjustmentCreate,procedure.PatNum,"Adjustment made for discount plan: "+adjustmentDiscountPlan.AdjAmt.ToString("f"));
			Procedures.UpdateDiscountPlanAmt(procedure.ProcNum,adjAmt);
			return adjAmt;
		}

		public static void CreateSalesTaxRefundIfNeeded(Procedure procedure, Adjustment adjustmentExistingLocked) {
			//No need to check MiddleTierRole; no call to db.
			Adjustment adjustmentSalesTaxRefund=new Adjustment();
			adjustmentSalesTaxRefund.DateEntry=DateTime.Today;
			adjustmentSalesTaxRefund.AdjDate=DateTime.Today;
			adjustmentSalesTaxRefund.ProcDate=procedure.ProcDate;
			adjustmentSalesTaxRefund.ProvNum=procedure.ProvNum;
			adjustmentSalesTaxRefund.PatNum=procedure.PatNum;
			adjustmentSalesTaxRefund.AdjType=AvaTax.GetSalesTaxReturnAdjType();
			adjustmentSalesTaxRefund.ClinicNum=procedure.ClinicNum;
			adjustmentSalesTaxRefund.ProcNum=procedure.ProcNum;
			if(AvaTax.DoCreateReturnAdjustment(procedure,adjustmentExistingLocked,adjustmentSalesTaxRefund)) {
				Insert(adjustmentSalesTaxRefund);
				TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adjustmentSalesTaxRefund);
			}
		}

		///<summary>THIS IS NOT THE SAME AS AVATAX. Creates a sales tax adjustment for a procedure that has IsTaxed set.
		///This uses the SalesTaxDefaultProvider if it is set.</summary>
		public static void CreateAdjustmentForSalesTax(Procedure procedure,bool canSkipIsTaxed=false) {
			//No need to check MiddleTierRole; no call to db.
			if(PrefC.IsODHQ) {
				return;
			}
			if(!canSkipIsTaxed && !ProcedureCodes.GetProcCode(procedure.CodeNum).IsTaxed) {
				return;
			}
			List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForProc(procedure.ProcNum);
			long provNum;
			//Always use the default global provider if set.
			long salesTaxDefaultProvNum=PrefC.GetLong(PrefName.SalesTaxDefaultProvider);
			if(salesTaxDefaultProvNum!=0) {
				//Note: To have clinics override the SalesTaxDefaultProvider, add clinic code here.
				//This can be done in a future job.
				provNum=salesTaxDefaultProvNum;
			}
			else {
				provNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
				Clinic clinic=Clinics.GetClinic(procedure.ClinicNum);
				if(procedure.ClinicNum!=0 && clinic.DefaultProv!=0) {
					provNum=clinic.DefaultProv;
				}
			}
			Adjustment adjustmentSalesTax=new Adjustment();
			adjustmentSalesTax.AdjDate=DateTime.Today;
			adjustmentSalesTax.ProcDate=procedure.ProcDate;
			adjustmentSalesTax.PatNum=procedure.PatNum;
			adjustmentSalesTax.ClinicNum=procedure.ClinicNum;
			adjustmentSalesTax.AdjAmt=ClaimProcs.ComputeSalesTax(procedure,listClaimProcs,isEstimate:false);
			adjustmentSalesTax.AdjType=PrefC.GetLong(PrefName.SalesTaxAdjustmentType);
			adjustmentSalesTax.ProcNum=procedure.ProcNum;
			adjustmentSalesTax.ProvNum=provNum;
			Insert(adjustmentSalesTax);
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adjustmentSalesTax);
			SecurityLogs.MakeLogEntry(Permissions.AdjustmentCreate,procedure.PatNum,"Adjustment made for sales tax: "+adjustmentSalesTax.AdjAmt);
		}

		///<summary>Creates a late charge adjustment for a statement and returns the AdjNum.</summary>
		public static long CreateLateChargeAdjustment(double lateChargeAmount,DateTime dateCharge,long provNum,long patNum,DateTime dateStatementSent
			,DateTime dateMaxUpdateStmtProd,List<long> listProcNums,List<long> listAdjNums,List<long> listPayPlanChargeNums)
		{
			//No need to check MiddleTierRole; no call to db.
			Adjustment adjustmentLateCharge=new Adjustment();
			adjustmentLateCharge.AdjDate=dateCharge;
			adjustmentLateCharge.PatNum=patNum;
			adjustmentLateCharge.AdjAmt=lateChargeAmount;
			adjustmentLateCharge.AdjType=PrefC.GetLong(PrefName.LateChargeAdjustmentType);
			adjustmentLateCharge.ProvNum=provNum;
			adjustmentLateCharge.AdjNote=Lans.g("Adjustment","Late charge for statement sent on")+$" {dateStatementSent.ToShortDateString()}.";
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adjustmentLateCharge);
			SecurityLogs.MakeLogEntry(Permissions.AdjustmentCreate,patNum,
				$"Adjustment for late charge for statement sent on {dateStatementSent.ToShortDateString()}, Amount: {adjustmentLateCharge.AdjAmt.ToString("c")}");
			long adjNum=Insert(adjustmentLateCharge);
			StatementProds.UpdateLateChargeAdjNumForMany(adjNum,listProcNums,listAdjNums,listPayPlanChargeNums,dateMaxUpdateStmtProd);
			return adjNum;
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(Adjustment adjustment){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),adjustment);
				return;
			}
			Crud.AdjustmentCrud.Update(adjustment);
			CreateOrUpdateSalesTaxIfNeeded(adjustment);
		}

		public static void DetachFromInvoice(long statementNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementNum);
				return;
			}
			string command="UPDATE adjustment SET StatementNum=0 WHERE StatementNum="+POut.Long(statementNum)+"";
			Db.NonQ(command);
		}

		public static void DetachAllFromInvoices(List<long> listStatementNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listStatementNums);
				return;
			}
			if(listStatementNums==null || listStatementNums.Count==0) {
				return;
			}
			string command="UPDATE adjustment SET StatementNum=0 WHERE StatementNum IN ("+string.Join(",",listStatementNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}
		#endregion

		#region Delete
		///<summary>This will soon be eliminated or changed to only allow deleting on same day as EntryDate.</summary>
		public static void Delete(Adjustment adjustment){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),adjustment);
				return;
			}
			Crud.AdjustmentCrud.Delete(adjustment.AdjNum);
			CreateOrUpdateSalesTaxIfNeeded(adjustment);
			PaySplits.UnlinkForAdjust(adjustment);
			StatementProds.UpdateLateChargeAdjNumForMany(0,adjustment.AdjNum);
		}

		///<summary>Deletes all adjustments for a procedure</summary>
		public static void DeleteForProcedure(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNum);
				return;
			}
			//Create log for each adjustment that is going to be deleted.
			string command="SELECT * FROM adjustment WHERE ProcNum = "+POut.Long(procNum); //query for all adjustments of a procedure 
			List<Adjustment> listAdjustments=Crud.AdjustmentCrud.SelectMany(command);
			List<long> listAdjNums=new List<long>();
			for(int i=0;i<listAdjustments.Count;i++) { //loops through the rows
				listAdjNums.Add(listAdjustments[i].AdjNum);
				SecurityLogs.MakeLogEntry(Permissions.AdjustmentEdit,listAdjustments[i].PatNum, //and creates audit trail entry for every row to be deleted
				"Delete adjustment for patient: "
				+Patients.GetLim(listAdjustments[i].PatNum).GetNameLF()+", "
				+(listAdjustments[i].AdjAmt).ToString("c"),0,listAdjustments[i].SecDateTEdit);
			}
			//Delete each adjustment for the procedure.
			command="DELETE FROM adjustment WHERE ProcNum = "+POut.Long(procNum);
			Db.NonQ(command);
			//Late charge adjustments aren't normally attached to procedures, but it is possible for users to attach a procedure to them after they are
			//made, so we must update any StatementProds that might be associated to the deleted adjustment.
			StatementProds.UpdateLateChargeAdjNumForMany(0,listAdjNums.ToArray());
		}
		#endregion

		#region Misc Methods
		///<summary>Centralized logic used to apply sales tax to non-ortho procedures if a sales tax adjustment
		///doesn't already exist.</summary>
		public static void AddSalesTaxIfNoneExists(Procedure procedure) {
			//No need to check MiddleTierRole; no call to db.
			if(PrefC.IsODHQ) {
				return;
			}
			//Don't apply if there's an ortho proc link
			if(OrthoProcLinks.GetByProcNum(procedure.ProcNum)!=null) {
				return;
			}
			//Check if there's already an existing sales tax adjustment
			long salesTaxAdjType=PrefC.GetLong(PrefName.SalesTaxAdjustmentType);
			if(GetListForProc(procedure.ProcNum).Any(x => x.AdjType==salesTaxAdjType)) {
				return;
			}
			//Add sales tax.
			CreateAdjustmentForSalesTax(procedure);
		}

		///<summary>(HQ Only) Automatically creates or updates a sales tax adjustment for the passted in procedure. If an adjustment is passed in, we go 
		///ahead and update that adjustment, otherwise we check if there is already a sales tax adjustment for the given procedure and if not, we create
		///a new one. Pass in false to doCalcTax if we have already called the AvaTax API to get the tax estimate recently to avoid redundant calls
		///(currently only pre-payments uses this flag).
		///isRepeatCharge indicates if the adjustment is being inserted by the repeat charge tool, currently only used to supress error messages
		///in the Avatax API.</summary>
		public static void CreateOrUpdateSalesTaxIfNeeded(Procedure procedure,Adjustment adjustmentSalesTax=null,bool doCalcTax=true,bool isRepeatCharge=false) {
			if(!AvaTax.DoSendProcToAvalara(procedure,isRepeatCharge)) { //tests isHQ
				return;
			}
			//Check for middle tier as crud is called below
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procedure,adjustmentSalesTax,doCalcTax,isRepeatCharge);
				return;
			}
			if(adjustmentSalesTax==null) {
				adjustmentSalesTax=Adjustments.GetSalesTaxForProc(procedure.ProcNum);
			}
			//If we didn't find any existing adjustments to modify, create an adjustment instead
			if(adjustmentSalesTax==null) { 
				adjustmentSalesTax=new Adjustment();
				adjustmentSalesTax.DateEntry=DateTime.Today;
				adjustmentSalesTax.AdjDate=procedure.ProcDate;
				adjustmentSalesTax.ProcDate=procedure.ProcDate;
				adjustmentSalesTax.ProvNum=procedure.ProvNum;
				adjustmentSalesTax.PatNum=procedure.PatNum;
				adjustmentSalesTax.AdjType=AvaTax.GetSalesTaxAdjType();
				adjustmentSalesTax.ClinicNum=procedure.ClinicNum;
				adjustmentSalesTax.ProcNum=procedure.ProcNum;
			}
			//if the sales tax adjustment is locked, create a sales tax refund adjustment instead
			if(procedure.ProcDate <= AvaTax.GetTaxLockDate()) {
				CreateSalesTaxRefundIfNeeded(procedure,adjustmentSalesTax);
				return;
			}
			if(!doCalcTax) { //Should only ever happen for pre-payments, where we've already called the api to get the tax amount
				adjustmentSalesTax.AdjAmt=procedure.TaxAmt;
				Insert(adjustmentSalesTax);
			}
			else if(AvaTax.DidUpdateAdjustment(procedure,adjustmentSalesTax)) {
				string note=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+": Tax amount changed from $"+procedure.TaxAmt.ToString("f2")+" to $"+adjustmentSalesTax.AdjAmt.ToString("f2");
				if(!CompareDouble.IsZero((procedure.TaxAmt-adjustmentSalesTax.AdjAmt))) {
					procedure.TaxAmt=adjustmentSalesTax.AdjAmt;
					Crud.ProcedureCrud.Update(procedure);
				}
				if(adjustmentSalesTax.AdjNum==0) {
					//The only way to get salesTaxAdj.AdjAmt=0 when AvaTax.DidUpdateAdjustment() returns true is if there was an error.
					if(isRepeatCharge && adjustmentSalesTax.AdjAmt==0) {//this is an error; we would normally not save a new adjustment with amt $0
						throw new ODException("Encountered an error communicating with AvaTax.  Skip for repeating charges only.  "+adjustmentSalesTax.AdjNote);
					}
					Insert(adjustmentSalesTax);//This could be an error or a new adjustment/repeating charge, either way we want to insert
				}
				else { //updating an existing adjustment. We don't need to check isRepeatCharge because of 
					if(!string.IsNullOrWhiteSpace(adjustmentSalesTax.AdjNote)) {
						adjustmentSalesTax.AdjNote+=Environment.NewLine;
					}
					adjustmentSalesTax.AdjNote+=note; //If we are updating this adjustment, leave a note indicating what changed
					Update(adjustmentSalesTax);
				}
			}
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adjustmentSalesTax);
		}

		///<summary>(HQ Only) When we create, modify, or delete a non-sales tax adjustment that is attached to a procedure, we may also need to update
		///sales tax for that procedure. Takes a non-sales tax adjustment and checks if the procedure already has a sales tax adjustment. If one already
		///exists, calculate the new tax after the change to the passed in adjustment, and update the sales tax accordingly. If not, calculate and 
		///create a sales tax adjustment only if the procedure is taxable.</summary>
		public static void CreateOrUpdateSalesTaxIfNeeded(Adjustment adjustmentModified) {
			if(AvaTax.IsEnabled() && adjustmentModified.ProcNum>0 && adjustmentModified.AdjType!=AvaTax.GetSalesTaxAdjType() && adjustmentModified.AdjType!=AvaTax.GetSalesTaxReturnAdjType()) {
				Adjustment taxAdjForProc=GetSalesTaxForProc(adjustmentModified.ProcNum);
				if(taxAdjForProc!=null) {
					Procedure procedure=Procedures.GetOneProc(adjustmentModified.ProcNum,includeNote:false);
					CreateOrUpdateSalesTaxIfNeeded(procedure,taxAdjForProc);
				}
			}
		}

		///<summary>Won't delete adjustments that have paysplits attached. Returns ChargeUndoData which holds the count of deleted adjustments and a list of PatNums that we had to skip adjustments for.</summary>
		public static ChargeUndoData UndoFinanceOrBillingCharges(DateTime dateUndo,bool isBillingCharges) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ChargeUndoData>(MethodBase.GetCurrentMethod(),dateUndo,isBillingCharges);
			}
			string adjTypeStr="Finance";
			long adjTypeDefNum=PrefC.GetLong(PrefName.FinanceChargeAdjustmentType);
			if(isBillingCharges) {
				adjTypeStr="Billing";
				adjTypeDefNum=PrefC.GetLong(PrefName.BillingChargeAdjustmentType);
			}
			string command="SELECT adjustment.AdjNum,adjustment.AdjAmt,patient.PatNum"
				+",patient.LName,patient.FName,patient.Preferred,patient.MiddleI,adjustment.SecDateTEdit"
				+",(CASE WHEN paysplit.SplitNum IS NULL THEN 0 ELSE 1 END) AS 'HasPaySplits' "
				+$",(CASE WHEN payplanlink.PayPlanLinkNum IS NULL THEN 0 ELSE 1 END) AS 'HasPayPlan' "
				+"FROM adjustment "
				+"INNER JOIN patient ON patient.PatNum=adjustment.PatNum "
				+"LEFT JOIN paysplit ON adjustment.AdjNum=paysplit.AdjNum "
				+$"LEFT JOIN payplanlink ON adjustment.AdjNum=payplanlink.FKey AND payplanlink.LinkType={POut.Enum<PayPlanLinkType>(PayPlanLinkType.Adjustment)} "
				+"WHERE AdjDate="+POut.Date(dateUndo)+" "
				+"AND AdjType="+POut.Long(adjTypeDefNum)+" "
				+"GROUP BY adjustment.AdjNum";
			DataTable table=Db.GetTable(command);
			ChargeUndoData chargeUndoData=new ChargeUndoData();
			for(int i=table.Rows.Count-1;i>=0;i--) {
				DataRow dataRow=table.Rows[i];
				//Any rows for adjustments that have pay splits attached will not be deleted. Add them to the skipped list and remove them from the table.
				if(PIn.Bool(dataRow["HasPaySplits"].ToString())==false && PIn.Bool(dataRow["HasPayPlan"].ToString())==false) {
					continue;
				}
				chargeUndoData.ListSkippedPatNums.Add(PIn.Long(dataRow["PatNum"].ToString()));
				table.Rows.RemoveAt(i);
			}
			List<long> listAdjNumsToDelete=new List<long>();
			List<Action> listActions=new List<Action>();
			int loopCount=0;
			for(int i=0;i<table.Rows.Count;i++) {//loops through the remaining rows and creates an audit trail entry for every row to be deleted
				DataRow rowCur=table.Rows[i];
				listAdjNumsToDelete.Add(PIn.Long(rowCur["AdjNum"].ToString()));
				Action actionCreateAuditTrailEntry=()=>{
					SecurityLogs.MakeLogEntry(Permissions.AdjustmentEdit,PIn.Long(rowCur["PatNum"].ToString()),
						"Delete adjustment for patient, undo "+adjTypeStr.ToLower()+" charges: "
						+Patients.GetNameLF(rowCur["LName"].ToString(),rowCur["FName"].ToString(),rowCur["Preferred"].ToString(),rowCur["MiddleI"].ToString())
						+", "+PIn.Double(rowCur["AdjAmt"].ToString()).ToString("c"),0,PIn.DateT(rowCur["SecDateTEdit"].ToString()));
					if(++loopCount%5==0) {//Have to use loopCount instead of i because we must increment within the action.
						ProgressBarEvent.Fire(ODEventType.ProgressBar,Lans.g("FinanceCharge","Creating log entries for "+adjTypeStr.ToLower()+" charges ")
							+loopCount+" out of "+table.Rows.Count);
					}
				};
				listActions.Add(actionCreateAuditTrailEntry);
			}
			ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));
			ProgressBarEvent.Fire(ODEventType.ProgressBar,Lans.g("FinanceCharge","Deleting")+" "+table.Rows.Count+" "
				+Lans.g("FinanceCharge",adjTypeStr.ToLower()+" charge adjustments")+"...");
			Crud.AdjustmentCrud.DeleteMany(listAdjNumsToDelete);
			//Doing this because it is possible for a late charge's adjustment type to be changed to a billing or finance charge type.
			//The late charge could then get deleted by this method, and we then need to clean up the associated StatementProds.
			StatementProds.UpdateLateChargeAdjNumForMany(0,listAdjNumsToDelete.ToArray());
			chargeUndoData.CountDeletedAdjustments=listAdjNumsToDelete.Count;
			return chargeUndoData;
		}

		///<summary>Delete all late charge adjustments that have the date passed in. Makes logs and cleans up StatementProds. Won't delete adjustments that have pay splits attached, instead patients with skipped adjustments are added to ChargeUndoData.ListSkippedPatNums so that calling method can handle them.</summary>
		public static ChargeUndoData UndoLateCharges(DateTime dateUndo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ChargeUndoData>(MethodBase.GetCurrentMethod(),dateUndo);
			}
			string command=@$"
				SELECT adjustment.AdjNum,adjustment.AdjAmt,adjustment.PatNum,(CASE WHEN paysplit.SplitNum IS NULL THEN 0 ELSE 1 END) AS 'HasPaySplits', 
				(CASE WHEN payplanlink.PayPlanLinkNum IS NULL THEN 0 ELSE 1 END) AS 'HasPayPlan'
				FROM adjustment
				INNER JOIN statementprod ON adjustment.AdjNum=statementprod.LateChargeAdjNum
				LEFT JOIN paysplit ON adjustment.AdjNum=paysplit.AdjNum
				LEFT JOIN payplanlink ON adjustment.AdjNum=payplanlink.FKey AND payplanlink.LinkType={POut.Enum<PayPlanLinkType>(PayPlanLinkType.Adjustment)}
				WHERE adjustment.AdjDate={POut.Date(dateUndo)} 
				GROUP BY adjustment.AdjNum";
			DataTable table=Db.GetTable(command);
			ChargeUndoData chargeUndoDataLate=new ChargeUndoData();
			List<long> listAdjNumsDeleted=new List<long>();
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow dataRow=table.Rows[i];
				if(PIn.Bool(dataRow["HasPaySplits"].ToString()) || PIn.Bool(dataRow["HasPayPlan"].ToString())) {//We can't delete adjustments that have payments attached.
					chargeUndoDataLate.ListSkippedPatNums.Add(PIn.Long(dataRow["PatNum"].ToString()));
				}
				else {
					listAdjNumsDeleted.Add(PIn.Long(dataRow["AdjNum"].ToString()));
					SecurityLogs.MakeLogEntry(Permissions.AdjustmentEdit,PIn.Long(dataRow["PatNum"].ToString()),
						$"Late charges dated {dateUndo.ToShortDateString()} undone, Adjustment deleted, Amount: "
						+$"{PIn.Decimal(dataRow["AdjAmt"].ToString()).ToString("c")}");
					Crud.AdjustmentCrud.Delete(PIn.Long(dataRow["AdjNum"].ToString()));
					StatementProds.UpdateLateChargeAdjNumForMany(0,PIn.Long(dataRow["AdjNum"].ToString()));
				}
			}
			chargeUndoDataLate.CountDeletedAdjustments=listAdjNumsDeleted.Count;
			return chargeUndoDataLate;
		}

		///<summary>Returns a query string used to get adjustments for all patients who have an appointment in the date range and in one of the operatories
		///passed in.</summary>
		public static string GetQueryAdjustmentsForAppointments(DateTime dateStart,DateTime dateEnd,List<long> listOperatoryNums,bool doGetSum) {
			//No need to check MiddleTierRole; no call to db.
			if(listOperatoryNums.IsNullOrEmpty()) {
				return "SELECT "+(doGetSum ? "SUM(adjustment.AdjAmt)" : "*")
					+" FROM adjustment WHERE AdjDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			}
			string command="SELECT "
				+(doGetSum ? "SUM(adjustment.AdjAmt)" : "*")
				+" FROM adjustment WHERE AdjDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)
					+" AND PatNum IN("
						+"SELECT PatNum FROM appointment "
							+"WHERE AptDateTime BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd.AddDays(1))
							+ "AND AptStatus IN ("+POut.Int((int)ApptStatus.Scheduled)
							+", "+POut.Int((int)ApptStatus.Complete)
							+", "+POut.Int((int)ApptStatus.Broken)
							+", "+POut.Int((int)ApptStatus.PtNote)
							+", "+POut.Int((int)ApptStatus.PtNoteCompleted)+")"
							+" AND Op IN("+string.Join(",",listOperatoryNums)+")) ";
			return command;
		}

		///<summary>Returns a list of negative adjustments, one for each PaySplit in a payment that is attached to a procedure or an adjustment. Adjustments are not inserted into the database. Used to make refunds.</summary>
		public static List<Adjustment> CreateNegativeAdjustmentsForRefund(Payment paymentExisting) {
			//No remoting role check; no call to db.
			List<PaySplit> listPaySplitsExisting=PaySplits.GetForPayment(paymentExisting.PayNum);
			Def def=Defs.GetDef(DefCat.AdjTypes,PrefC.GetLong(PrefName.RefundAdjustmentType));
			Adjustment adjustment;
			List<Adjustment> listAdjustmentsAdded= new List<Adjustment>();
			for(int i=0;i<listPaySplitsExisting.Count;i++) { 
				//if split has adjustments, is unallocated, or is attached to a payplan, don't make negative adjustments, and move on to next splits.         
				if(listPaySplitsExisting[i].IsUnallocated
					|| listPaySplitsExisting[i].PayPlanNum > 0 
					|| listPaySplitsExisting[i].PayPlanChargeNum > 0
					||listPaySplitsExisting[i].UnearnedType > 0)
				{
					continue;
				}
				adjustment=new Adjustment();
				adjustment.IsNew=true;
				adjustment.DateEntry=DateTime.Today;
				adjustment.AdjDate=DateTime.Today;
				adjustment.ProcNum=listPaySplitsExisting[i].ProcNum;
				adjustment.AdjAmt=-listPaySplitsExisting[i].SplitAmt;
				adjustment.PatNum=listPaySplitsExisting[i].PatNum;
				adjustment.ProvNum=listPaySplitsExisting[i].ProvNum;
				adjustment.ClinicNum=listPaySplitsExisting[i].ClinicNum;
				adjustment.AdjType=def.DefNum;
				listAdjustmentsAdded.Add(adjustment);
			}
			return listAdjustmentsAdded;
		}

		/*
		///<summary>Must make sure Refresh is done first.  Returns the sum of all adjustments for this patient.  Amount might be pos or neg.</summary>
		public static double ComputeBal(Adjustment[] List){
			double retVal=0;
			for(int i=0;i<List.Length;i++){
				retVal+=List[i].AdjAmt;
			}
			return retVal;
		}*/
		#endregion

		[Serializable]
		public class ChargeUndoData {
			public List<long> ListSkippedPatNums=new List<long>();
			public int CountDeletedAdjustments=0;
		}
	}

	


	


}










