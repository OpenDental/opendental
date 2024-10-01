using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class DiscountPlans{
		#region Methods - Insert
		///<summary></summary>
		public static long Insert(DiscountPlan discountPlan){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				discountPlan.DiscountPlanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),discountPlan);
				return discountPlan.DiscountPlanNum;
			}
			return Crud.DiscountPlanCrud.Insert(discountPlan);
		}
		#endregion
		#region Methods - Get
		///<summary></summary>
		public static List<DiscountPlan> GetAll(bool includeHidden){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DiscountPlan>>(MethodBase.GetCurrentMethod(),includeHidden);
			}
			string command="SELECT * FROM discountplan";
			if(!includeHidden) {
				command+=" WHERE IsHidden=0";
			}
			return Crud.DiscountPlanCrud.SelectMany(command);
		}

		///<summary>Returns a list of DiscountPlans for a list of passed in DiscountPlanNums.</summary>
		public static List<DiscountPlan> GetDiscountPlansByPlanNum(List<long> listDiscountPlanNums){
			if(listDiscountPlanNums.Count==0) {
				return new List<DiscountPlan>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DiscountPlan>>(MethodBase.GetCurrentMethod(),listDiscountPlanNums);
			}
			string command="SELECT * "
				+"FROM discountplan "
				+"WHERE discountplan.DiscountPlanNum IN ("+string.Join(",",listDiscountPlanNums)+")";
			return Crud.DiscountPlanCrud.SelectMany(command);
		}

		///<summary>Returns a list of discountplans for a list of passed in patnums. There is no guarantee that a patnum will have a discount plan.</summary>
		public static List<DiscountPlan> GetForPats(List<long> listPatNums) {
			if(listPatNums.Count<1) {
				return new List<DiscountPlan>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DiscountPlan>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT discountplan.* "
				+"FROM discountplan "
				+"INNER JOIN discountplansub ON discountplansub.DiscountPlanNum=discountplan.DiscountPlanNum "
				+"WHERE discountplansub.PatNum IN ("+string.Join(",",listPatNums)+")";
			return Crud.DiscountPlanCrud.SelectMany(command);
		}

		/// <summary>Takes in a list of patnums and returns a dictionary of PatNum to DiscountPlan.FeeSchedNum pairs. Value is 0 if no discount plan exists</summary>
		public static SerializableDictionary<long,long> GetFeeSchedNumsByPatNums(List<long> listPatNums) {
			if(listPatNums.IsNullOrEmpty()) {
				return new SerializableDictionary<long,long>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,long>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT patient.PatNum,discountplan.FeeSchedNum "
				+"FROM patient "
				+"INNER JOIN discountplansub ON discountplansub.PatNum=patient.PatNum "
				+"INNER JOIN discountplan ON discountplan.DiscountPlanNum=discountplansub.DiscountPlanNum "
				+"WHERE patient.PatNum IN ("+string.Join(",",listPatNums)+")";
			return Db.GetTable(command).Select()
				.ToSerializableDictionary(x => PIn.Long(x["PatNum"].ToString()),x => PIn.Long(x["FeeSchedNum"].ToString()));
		}

		///<summary>Returns an empty list if planNum is 0.</summary>
		public static List<string> GetPatNamesForPlan(long discountPlanNum) {
			if(discountPlanNum==0) {
				return new List<string>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),discountPlanNum);
			}
			string command="SELECT patient.LName,patient.FName "
				+"FROM discountplansub "
				+"LEFT JOIN patient ON discountplansub.PatNum=patient.PatNum "
				+"WHERE discountplansub.DiscountPlanNum="+POut.Long(discountPlanNum)+" "
				+"AND patient.PatStatus NOT IN ("+POut.Int((int)PatientStatus.Deleted)+","+POut.Int((int)PatientStatus.Deceased)+") ";
			//No Preferred or MiddleI needed because this logic needs to match FormInsPlan.
			return Db.GetTable(command).Select().Select(x => Patients.GetNameLFnoPref(x["LName"].ToString(),x["FName"].ToString(),"")).ToList();
		}

		public static int GetPatCountForPlan(long discountPlanNum) {
			if(discountPlanNum==0) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),discountPlanNum);
			}
			string command="SELECT COUNT(discountplansub.PatNum) "
				+"FROM discountplansub "
				+"LEFT JOIN patient ON discountplansub.PatNum=patient.PatNum "
				+"WHERE discountplansub.DiscountPlanNum="+POut.Long(discountPlanNum)+" "
				+"AND patient.PatStatus NOT IN ("+POut.Int((int)PatientStatus.Deleted)+","+POut.Int((int)PatientStatus.Deceased)+")";
			return PIn.Int(Db.GetCount(command));
		}

		
		public class CountPerPlan {
			public long DiscountPlanNum;
			public int Count;
		}

		///<summary>Returns a list of the count of patients for the DiscountPlanNum.
		///Returns an empty list if the list of plan nums is empty.</summary>
		public static List<CountPerPlan> GetPatCountsForPlans(List<long> listPlanNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CountPerPlan>>(MethodBase.GetCurrentMethod(),listPlanNums);
			}
			List<CountPerPlan> listCountPerPlan=new List<CountPerPlan>();
			if(listPlanNums.Count==0) {
				return listCountPerPlan;
			}
			string command="SELECT discountplansub.DiscountPlanNum,COUNT(discountplansub.PatNum) PatCount "
				+"FROM discountplansub "
				+"LEFT JOIN patient ON discountplansub.PatNum=patient.PatNum "
				+"WHERE discountplansub.DiscountPlanNum IN ("+string.Join(",",listPlanNums)+") "
				+"AND patient.PatStatus NOT IN ("+POut.Int((int)PatientStatus.Deleted)+","+POut.Int((int)PatientStatus.Deceased)+") "
				+"GROUP BY discountplansub.DiscountPlanNum";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				CountPerPlan countPerPlan=new CountPerPlan();
				countPerPlan.DiscountPlanNum=PIn.Long(table.Rows[i]["DiscountPlanNum"].ToString());
				countPerPlan.Count=PIn.Int(table.Rows[i]["PatCount"].ToString());
				listCountPerPlan.Add(countPerPlan);
			}
			return listCountPerPlan;
		}

		///<summary>Returns a list of ProcedureCodes that are stored in PrefName.DiscountPlan[x]Code. 
		///The pref must store a comma delimited list of D-Codes.</summary>
		private static List<ProcedureCode> GetProcedureCodesByPref(PrefName prefName) {
			List<long> listCodeNum=ProcedureCodes.GetCodeNumsForPref(prefName);
			return ProcedureCodes.GetCodesForCodeNums(listCodeNum);
		}

		///<summary>Gets one DiscountPlan from the db.</summary>
		public static DiscountPlan GetPlan(long discountPlanNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<DiscountPlan>(MethodBase.GetCurrentMethod(),discountPlanNum);
			}
			return Crud.DiscountPlanCrud.SelectOne(discountPlanNum);
		}

		///<summary>Gets a list of all DiscountPlans from the database.</summary>
		public static List<DiscountPlan> GetPlans(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DiscountPlan>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM discountplan";
			return Crud.DiscountPlanCrud.SelectMany(command);
		}

		///<summary>Returns a DiscountPlanProc object for every procedure passed in. It is assumed that all procedures are for the same patient. Passing in an empty list of Adjustments will assume
		///no prior adjustments of the discountPlan.DefNum had been applied.</summary>
		public static List<DiscountPlanProc> GetDiscountPlanProc(List<Procedure> listProcedures,DiscountPlanSub discountPlanSub=null,DiscountPlan discountPlan=null,List<Adjustment> listAdjustments=null) {
			if(listProcedures.IsNullOrEmpty() || discountPlanSub==null || discountPlan==null) {
				return new List<DiscountPlanProc>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//Remoting role check here to reduce round-trips to the server.
				return Meth.GetObject<List<DiscountPlanProc>>(MethodBase.GetCurrentMethod(),listProcedures,discountPlanSub,discountPlan,listAdjustments);
			}
			List<DiscountPlanProc> listDiscountPlanProcs=new List<DiscountPlanProc>();
			List<Procedure> listProceduresHist=new List<Procedure>();
			List<double> listAnnualTots=Adjustments.GetAnnualTotalsForPatByDiscountPlan(discountPlanSub.PatNum,discountPlanSub.DateEffective,discountPlanSub.DateTerm,discountPlan,listProcedures.Max(x=>x.ProcDate),listAdjustments:listAdjustments);
			for(int i=0;i<listProcedures.Count;i++) {
				int annualBucketIndex=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub.DateEffective,discountPlanSub.DateTerm,listProcedures[i].ProcDate);
				double discountPlanAmt=0;
				if(annualBucketIndex!=-1 && listAnnualTots.Count > annualBucketIndex) {
					discountPlanAmt=Procedures.GetDiscountAmountForDiscountPlanAndValidate(listProcedures[i],discountPlanSub:discountPlanSub,discountPlan:discountPlan,runningTotal:listAnnualTots[annualBucketIndex],listAddHistProcs:listProceduresHist);
					listAnnualTots[annualBucketIndex]+=discountPlanAmt;
				}
				listDiscountPlanProcs.Add(new DiscountPlanProc() {
					DiscountPlanAmt=discountPlanAmt,
					doesExceedAnnualMax=Procedures.ExceedsAnnualMax,
					doesExceedFreqLimit=Procedures.ExceedsFreqLimitation,
					ProcNum=listProcedures[i].ProcNum,
				});
				listProceduresHist.Add(listProcedures[i]);
			}
			return listDiscountPlanProcs;
		}

		///<summary>Returns a DiscountPlanProc object for every procedure passed in. It is assumed that all procedures are for the same patient. Passing in an empty list of Adjustments will assume
		///no prior adjustments of the discountPlan.DefNum had been applied.</summary>
		public static List<DiscountPlanProc> GetDiscountPlanProcEstimate(List<Procedure> listProcedures,long patNum,DateTime dateEffective,DateTime dateTerm,DiscountPlan discountPlan=null,List<Adjustment> listAdjustments=null) {
			if(listProcedures.IsNullOrEmpty() || discountPlan==null) {
				return new List<DiscountPlanProc>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//Remoting role check here to reduce round-trips to the server.
				return Meth.GetObject<List<DiscountPlanProc>>(MethodBase.GetCurrentMethod(),listProcedures,patNum,dateEffective,dateTerm,discountPlan,listAdjustments);
			}
			List<DiscountPlanProc> listDiscountPlanProcs=new List<DiscountPlanProc>();
			List<Procedure> listProceduresHist=new List<Procedure>();
			List<double> listAnnualTots=Adjustments.GetAnnualTotalsForPatByDiscountPlan(patNum,dateEffective,dateTerm,discountPlan,listProcedures.Max(x=>x.ProcDate),listAdjustments:listAdjustments);
			for(int i=0;i<listProcedures.Count;i++) {
				int annualBucketIndex=Adjustments.GetAnnualMaxSegmentIndex(dateEffective,dateTerm,listProcedures[i].ProcDate);
				double discountPlanAmt=0;
				if(annualBucketIndex!=-1 && listAnnualTots.Count > annualBucketIndex) {
					discountPlanAmt=Procedures.GetDiscountAmountForDiscountPlanEstimate(listProcedures[i],patNum,dateEffective,dateTerm,discountPlan:discountPlan,runningTotal:listAnnualTots[annualBucketIndex],listAddHistProcs:listProceduresHist);
					listAnnualTots[annualBucketIndex]+=discountPlanAmt;
				}
				listDiscountPlanProcs.Add(new DiscountPlanProc() {
					DiscountPlanAmt=discountPlanAmt,
					doesExceedAnnualMax=Procedures.ExceedsAnnualMax,
					doesExceedFreqLimit=Procedures.ExceedsFreqLimitation,
					ProcNum=listProcedures[i].ProcNum,
				});
				listProceduresHist.Add(listProcedures[i]);
			}
			return listDiscountPlanProcs;
		}

		#endregion
		#region Methods - Update

		///<summary></summary>
		public static void Update(DiscountPlan discountPlan){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),discountPlan);
				return;
			}
			Crud.DiscountPlanCrud.Update(discountPlan);
		}

		///<summary>Changes the DiscountPlanNum of all discountplansub that have _planFrom.DiscountPlanNum to _planInto.DiscountPlanNum</summary>
		public static void MergeTwoPlans(DiscountPlan discountPlanInto,DiscountPlan discountPlanFrom) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),discountPlanInto,discountPlanFrom);
				return;
			}
			string command="UPDATE discountplansub SET DiscountPlanNum="+POut.Long(discountPlanInto.DiscountPlanNum)
				+" WHERE DiscountPlanNum="+POut.Long(discountPlanFrom.DiscountPlanNum);
			Db.NonQ(command);
			//Delete the discount plan from the database.
			Crud.DiscountPlanCrud.Delete(discountPlanFrom.DiscountPlanNum);
		}

		#endregion
		#region Methods - Delete
		#endregion
		#region Methods - Misc
		///<summary>Checks for frequency conflicts with the passed-in list of procedures.
		///Returns empty string if there are no conflicts, new line delimited list of proc codes if there are ANY frequency limitations exceeded. 
		///Optionally add procedures to the list of historic procedures. Throws exceptions. listProcs should not contain any completed procs, 
		///as those get pulled from the db.</summary>
		public static string CheckDiscountFrequencyAndValidateDiscountPlanSub(List<Procedure> listProcedures,long patNum,DateTime aptDateTime,DiscountPlanSub discountPlanSub=null,
			List<Procedure> listProceduresAddHist=null)
		{
			Meth.NoCheckMiddleTierRole();
			//Note: If the passed in list contains procs in categories that have already exceeded limits, then this method will
			//return 0;
			Patient patient=Patients.GetPat(patNum);
			if(patient==null) {
				throw new ArgumentException("Patient not found in database.",nameof(patNum));
			}
			if(aptDateTime==null) {
				throw new ArgumentException("Appointment Date not present.",nameof(aptDateTime));
			}
			if(discountPlanSub==null) {
				discountPlanSub=DiscountPlanSubs.GetSubForPat(patient.PatNum);
			}
			//if this patient is a subscriber to the discount plan.
			if(discountPlanSub==null) {
				return "";
			}
			DiscountPlan discountPlan=DiscountPlans.GetPlan(discountPlanSub.DiscountPlanNum);
			if(discountPlan==null) {
				return "";
			}
			return CheckDiscountFrequency(listProcedures,patient.PatNum,aptDateTime,discountPlanSub.DateEffective,discountPlanSub.DateTerm,discountPlan,listProceduresAddHist);
		}

		///<summary>Checks for frequency conflicts with the passed-in list of procedures.
		///Returns empty string if there are no conflicts, new line delimited list of proc codes if there are ANY frequency limitations exceeded. 
		///Optionally add procedures to the list of historic procedures. Throws exceptions. listProcs should not contain any completed procs, 
		///as those get pulled from the db.</summary>
		public static string CheckDiscountFrequency(List<Procedure> listProcedures,long patNum,DateTime aptDateTime,DateTime dateEffective,DateTime dateTerm,DiscountPlan discountPlan,
			List<Procedure> listProceduresAddHist=null)
		{
			//if aptDateTime out of Discount plan range, bounce outta this bad boyo with no conflict/
			if((aptDateTime<dateEffective) || (dateTerm.Year>=1880 && aptDateTime>dateTerm)) {
				return "";
			}
			if(listProcedures.IsNullOrEmpty()) {
				return "";
			}
			//get completed procs for date range of discount plan
			DateTime termDate=dateTerm;
			if(termDate==DateTime.MinValue) {
				termDate=DateTime.MaxValue;
			}
			List<Procedure> listProceduresSorted=Procedures.SortListByTreatPlanPriority(listProcedures).ToList();
			listProceduresSorted.RemoveAll(x => x.ProcStatus==ProcStat.C);
			listProceduresSorted.InsertRange(0,listProcedures.FindAll(x => x.ProcStatus==ProcStat.C));
			List<Procedure> listProceduresHist=Procedures.GetCompletedForDateRange(dateEffective,termDate,listPatNums:ListTools.FromSingle(patNum));
			if(listProceduresAddHist!=null) {
				listProceduresHist.AddRange(listProceduresAddHist);
			}
			listProceduresHist.RemoveAll(x=> listProcedures.Select(y => y.ProcNum).ToList().Contains(x.ProcNum));
			//get multiple lists of ProcCodes for each catagory of frequency limit dcodes
			List<ProcedureCode> listProcedureCodesExam=GetProcedureCodesByPref(PrefName.DiscountPlanExamCodes);
			List<ProcedureCode> listProcedureCodesXray=GetProcedureCodesByPref(PrefName.DiscountPlanXrayCodes);
			List<ProcedureCode> listProcedureCodesProphy=GetProcedureCodesByPref(PrefName.DiscountPlanProphyCodes);
			List<ProcedureCode> listProcedureCodesFluoride=GetProcedureCodesByPref(PrefName.DiscountPlanFluorideCodes);
			List<ProcedureCode> listProcedureCodesPerio=GetProcedureCodesByPref(PrefName.DiscountPlanPerioCodes);
			List<ProcedureCode> listProcedureCodesLimited=GetProcedureCodesByPref(PrefName.DiscountPlanLimitedCodes);
			List<ProcedureCode> listPACodes=GetProcedureCodesByPref(PrefName.DiscountPlanPACodes);
			List<long> listCodeNumsAll=new List<long>();
			//Create a master list to reference the ProcCodes of the Procedurelogs against
			listCodeNumsAll.AddRange(listProcedureCodesExam.Select(x=>x.CodeNum).ToList());
			listCodeNumsAll.AddRange(listProcedureCodesXray.Select(x=>x.CodeNum).ToList());
			listCodeNumsAll.AddRange(listProcedureCodesProphy.Select(x=>x.CodeNum).ToList());
			listCodeNumsAll.AddRange(listProcedureCodesFluoride.Select(x=>x.CodeNum).ToList());
			listCodeNumsAll.AddRange(listProcedureCodesPerio.Select(x=>x.CodeNum).ToList());
			listCodeNumsAll.AddRange(listProcedureCodesLimited.Select(x=>x.CodeNum).ToList());
			listCodeNumsAll.AddRange(listPACodes.Select(x=>x.CodeNum).ToList());
			string frequencyConflicts="";
			//Iterates over the passed in list of procedures, and checks if adding them will exceed frequency limitations.
			for(int i=0;i<listProceduresSorted.Count;i++) {
				if(!listCodeNumsAll.Contains(listProceduresSorted[i].CodeNum)) {
					continue;
				}
				string frequencyConflict=HasMetDiscountFrequencyLimitation(listProceduresSorted[i],discountPlan,dateEffective,dateTerm,listProceduresHist,listProcedureCodesExam,listProcedureCodesXray,listProcedureCodesProphy,
					listProcedureCodesFluoride,listProcedureCodesPerio,listProcedureCodesLimited,listPACodes);
				if(!frequencyConflict.IsNullOrEmpty()) {
					frequencyConflicts+=frequencyConflict+"\r\n";
				}
				listProceduresHist.Add(listProceduresSorted[i]);
			}
			//for each list, if the number of procedurelogs+1 > frequency limit for that catagory bounce with a conflict that includes the Proccode
			return frequencyConflicts;
		}

		///<summary>Returns an empty string if there is no limitation met for the given proc, otherwise the CodeNum is returned.</summary>
		public static string HasMetDiscountFrequencyLimitation(Procedure procedure,DiscountPlan discountPlan,DateTime dateEffective,DateTime dateTerm,
			List<Procedure> listProceduresHist,List<ProcedureCode> listExamCodes,List<ProcedureCode> listXrayCodes,List<ProcedureCode> listProphyCodes,
			List<ProcedureCode> listFluorideCodes,List<ProcedureCode> listPerioCodes,List<ProcedureCode> listLimitedExamCodes,List<ProcedureCode> listPACodes)
		{
			Meth.NoCheckMiddleTierRole();
			if(HasMetFrequencyLimitForCategory(procedure,listProceduresHist,listExamCodes,discountPlan.ExamFreqLimit,dateEffective,dateTerm)
				|| HasMetFrequencyLimitForCategory(procedure,listProceduresHist,listXrayCodes,discountPlan.XrayFreqLimit,dateEffective,dateTerm)
				|| HasMetFrequencyLimitForCategory(procedure,listProceduresHist,listProphyCodes,discountPlan.ProphyFreqLimit,dateEffective,dateTerm)
				|| HasMetFrequencyLimitForCategory(procedure,listProceduresHist,listFluorideCodes,discountPlan.FluorideFreqLimit,dateEffective,dateTerm)
				|| HasMetFrequencyLimitForCategory(procedure,listProceduresHist,listPerioCodes,discountPlan.PerioFreqLimit,dateEffective,dateTerm)
				|| HasMetFrequencyLimitForCategory(procedure,listProceduresHist,listLimitedExamCodes,discountPlan.LimitedExamFreqLimit,dateEffective,dateTerm)
				|| HasMetFrequencyLimitForCategory(procedure,listProceduresHist,listPACodes,discountPlan.PAFreqLimit,dateEffective,dateTerm)
				) 
			{
				return ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode;
			}
			return "";
		}

		///<summary>Returns false when procCode is not in listCodesForCategory, or when procCode would not exceed the frequencyLimit for the passed in listCodesForCategory. Returns true otherwise</summary>
		public static bool HasMetFrequencyLimitForCategory(Procedure procedure,List<Procedure> listProceduresHist,List<ProcedureCode> listCodesForCategory,int frequencyLimit,DateTime dateEffective,DateTime dateTerm) {
			Meth.NoCheckMiddleTierRole();
			if(frequencyLimit==-1 || !listCodesForCategory.Any(x=>x.CodeNum==procedure.CodeNum)) {
				return false;
			}
			dateEffective=DiscountPlanSubs.GetAnnualMaxDateEffective(dateEffective);
			dateTerm=DiscountPlanSubs.GetAnnualMaxDateTerm(dateTerm);
			if(procedure.ProcDate<dateEffective || procedure.ProcDate>dateTerm) {
				return false;
			}
			DateTime dateEffectiveFinal=DiscountPlanSubs.GetDateEffectiveForAnnualDateRangeSegment(procedure.ProcDate,dateEffective,dateTerm);
			DateTime dateTermFinal=DiscountPlanSubs.GetDateTermForAnnualDateRangeSegment(procedure.ProcDate,dateEffective,dateTerm);
			int count=listProceduresHist.Count(x=> listCodesForCategory.Select(y => y.CodeNum).Contains(x.CodeNum) && x.ProcDate >= dateEffectiveFinal && x.ProcDate <= dateTermFinal);
			return (count>=frequencyLimit);
		}
		#endregion
	}

	///<summary>Helper class that holds information regarding a specific procedure and its relation to discount plan related things.</summary>
	[Serializable]
	public class DiscountPlanProc {
		public long ProcNum;
		///<summary>The discounted amount for the procedure that will be turned into an adjustment once the procedure is completed.</summary>
		public double DiscountPlanAmt;
		///<summary>Set to true when the amount of previously tallied procedures have already met the frequency limitation for this procedure's code. Otherwise false.</summary>
		public bool doesExceedFreqLimit;
		///<summary>Set to true when the total sum of this procedure and previously tallied procedures exceeds the annual max. Otherwise false.</summary>
		public bool doesExceedAnnualMax;

		///<summary>For serialization purposes.</summary>
		public DiscountPlanProc() {

		}

		///<summary>Only sets the ProcNum and DiscountPlanAmt fields.</summary>
		public DiscountPlanProc(Procedure procedure) {
			ProcNum=procedure.ProcNum;
			DiscountPlanAmt=procedure.DiscountPlanAmt;
		}
	}
}