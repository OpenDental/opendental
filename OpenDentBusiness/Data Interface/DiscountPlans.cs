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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				discountPlan.DiscountPlanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),discountPlan);
				return discountPlan.DiscountPlanNum;
			}
			return Crud.DiscountPlanCrud.Insert(discountPlan);
		}
		#endregion
		#region Methods - Get
		///<summary></summary>
		public static List<DiscountPlan> GetAll(bool getHidden){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DiscountPlan>>(MethodBase.GetCurrentMethod(),getHidden);
			}
			string command="SELECT * FROM discountplan";
			if(!getHidden) {
				command+=" WHERE IsHidden=0";
			}
			return Crud.DiscountPlanCrud.SelectMany(command);
		}

		///<summary>Returns a list of discountplans for a list of passed in patnums. There is no guarantee that a patnum will have a discount plan.</summary>
		public static List<DiscountPlan> GetForPats(List<long> listPatNums) {
			if(listPatNums.Count<1) {
				return new List<DiscountPlan>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DiscountPlan>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command=@$"SELECT discountplan.* 
												FROM discountplan
												INNER JOIN discountplansub ON discountplansub.DiscountPlanNum=discountplan.DiscountPlanNum
												WHERE discountplansub.PatNum IN ({string.Join(",",listPatNums)}) ";
			return Crud.DiscountPlanCrud.SelectMany(command);
		}

		/// <summary>Takes in a list of patnums and returns a dictionary of PatNum to DiscountPlan.FeeSchedNum pairs. Value is 0 if no discount plan exists</summary>
		public static SerializableDictionary<long,long> GetFeeSchedNumsByPatNums(List<long> listPatNums) {
			if(listPatNums.IsNullOrEmpty()) {
				return new SerializableDictionary<long,long>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,long>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command=$@"SELECT patient.PatNum,discountplan.FeeSchedNum
				FROM patient
				INNER JOIN discountplansub ON discountplansub.PatNum=patient.PatNum
				INNER JOIN discountplan ON discountplan.DiscountPlanNum=discountplansub.DiscountPlanNum
				WHERE patient.PatNum IN ({string.Join(",",listPatNums)})";
			return Db.GetTable(command).Select()
				.ToSerializableDictionary(x => PIn.Long(x["PatNum"].ToString()),x => PIn.Long(x["FeeSchedNum"].ToString()));
		}

		///<summary>Returns an empty list if planNum is 0.</summary>
		public static List<string> GetPatNamesForPlan(long planNum) {
			if(planNum==0) {
				return new List<string>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),planNum);
			}
			string command="SELECT patient.LName,patient.FName "
				+"FROM discountplansub "
				+"LEFT JOIN patient ON discountplansub.PatNum=patient.PatNum "
				+"WHERE discountplansub.DiscountPlanNum="+POut.Long(planNum)+" "
				+"AND patient.PatStatus NOT IN ("+POut.Int((int)PatientStatus.Deleted)+","+POut.Int((int)PatientStatus.Deceased)+") ";
			//No Preferred or MiddleI needed because this logic needs to match FormInsPlan.
			return Db.GetTable(command).Select().Select(x => Patients.GetNameLFnoPref(x["LName"].ToString(),x["FName"].ToString(),"")).ToList();
		}

		public static int GetPatCountForPlan(long discountPlanNum) {
			if(discountPlanNum==0) {
				return 0;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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

		///<summary>Returns a dictionary where key=DiscountPlanNum and value=count of patients for the DiscountPlanNum.
		///Returns an empty dictionary if the list of plan nums is empty.</summary>
		public static List<CountPerPlan> GetPatCountsForPlans(List<long> listPlanNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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

		///<summary>Gets lists of ProcedureCodes for all 7 catagories of frequency limitations. Returns a list of all CodeNums.</summary>
		public static List<long> GetDiscountPlanFrequencyProcCodes(out List<ProcedureCode> listExamCodes,out List<ProcedureCode> listXrayCodes,
			out List<ProcedureCode> listProphyCodes,out List<ProcedureCode> listFluorideCodes,out List<ProcedureCode> listPerioCodes,
			out List<ProcedureCode> listLimitedExamCodes,out List<ProcedureCode> listPACodes)
		{
			//No need a Remoting Role Check. No call to DB
			listExamCodes=GetProcedureCodesByPref(PrefName.DiscountPlanExamCodes);
			listXrayCodes=GetProcedureCodesByPref(PrefName.DiscountPlanXrayCodes);
			listProphyCodes=GetProcedureCodesByPref(PrefName.DiscountPlanProphyCodes);
			listFluorideCodes=GetProcedureCodesByPref(PrefName.DiscountPlanFluorideCodes);
			listPerioCodes=GetProcedureCodesByPref(PrefName.DiscountPlanPerioCodes);
			listLimitedExamCodes=GetProcedureCodesByPref(PrefName.DiscountPlanLimitedCodes);
			listPACodes=GetProcedureCodesByPref(PrefName.DiscountPlanPACodes);
			List<long> listAllCodes=new List<long>();
			//Create a master list to reference the ProcCodes of the Procedurelogs against
			listAllCodes.AddRange(listExamCodes.Select(x=>x.CodeNum).ToList());
			listAllCodes.AddRange(listXrayCodes.Select(x=>x.CodeNum).ToList());
			listAllCodes.AddRange(listProphyCodes.Select(x=>x.CodeNum).ToList());
			listAllCodes.AddRange(listFluorideCodes.Select(x=>x.CodeNum).ToList());
			listAllCodes.AddRange(listPerioCodes.Select(x=>x.CodeNum).ToList());
			listAllCodes.AddRange(listLimitedExamCodes.Select(x=>x.CodeNum).ToList());
			listAllCodes.AddRange(listPACodes.Select(x=>x.CodeNum).ToList());
			return listAllCodes;
		}

		///<summary>Returns a list of ProcedureCodes that are stored in PrefName.DiscountPlan[x]Code. 
		///The pref must store a comma delimited list of D-Codes.</summary>
		private static List<ProcedureCode> GetProcedureCodesByPref(PrefName pref) {
			return PrefC.GetString(pref).Split(",",StringSplitOptions.RemoveEmptyEntries).SelectMany(x => ProcedureCodes.GetProcCodeStartsWith(x)).ToList();
		}

		///<summary>Gets one DiscountPlan from the db.</summary>
		public static DiscountPlan GetPlan(long discountPlanNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<DiscountPlan>(MethodBase.GetCurrentMethod(),discountPlanNum);
			}
			return Crud.DiscountPlanCrud.SelectOne(discountPlanNum);
		}

		///<summary>Returns a DiscountPlanProc object for every procedure passed in. It is assumed that all procedures are for the same patient. Passing in an empty list of Adjustments will assume
		///no prior adjustments of the discountPlan.DefNum had been applied.</summary>
		public static List<DiscountPlanProc> GetDiscountPlanProc(List<Procedure> listProcs,DiscountPlanSub discountPlanSub=null,DiscountPlan discountPlan=null,List<Adjustment> listAdjustments=null) {
			//No remoting role check; No call to db.
			if(listProcs.IsNullOrEmpty() || discountPlanSub==null || discountPlan==null) {
				return new List<DiscountPlanProc>();
			}
			List<DiscountPlanProc> listDiscountPlanProcs=new List<DiscountPlanProc>();
			List<Procedure> listHistProcs=new List<Procedure>();
			List<double> listAnnualTots=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,listProcs.Max(x=>x.ProcDate),listAdjustments:listAdjustments);
			for(int i=0;i<listProcs.Count;i++) {
				int annualBucketIndex=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,listProcs[i].ProcDate);
				double discountPlanAmt=0;
				string strFreqLimit="",strAnnualMax="";
				if(annualBucketIndex!=-1 && listAnnualTots.Count > annualBucketIndex) {
					discountPlanAmt=Procedures.GetDiscountAmountForDiscountPlan(listProcs[i],out strFreqLimit,out strAnnualMax,
						discountPlanSub:discountPlanSub,discountPlan:discountPlan,runningTotal:listAnnualTots[annualBucketIndex],listAddHistProcs:listHistProcs);
					listAnnualTots[annualBucketIndex]+=discountPlanAmt;
				}
				listDiscountPlanProcs.Add(new DiscountPlanProc() {
					DiscountPlanAmt=discountPlanAmt,
					doesExceedAnnualMax=!string.IsNullOrEmpty(strAnnualMax),
					doesExceedFreqLimit=!string.IsNullOrEmpty(strFreqLimit),
					ProcNum=listProcs[i].ProcNum,
				});
				listHistProcs.Add(listProcs[i]);
			}
			return listDiscountPlanProcs;
		}

		#endregion
		#region Methods - Update

		///<summary></summary>
		public static void Update(DiscountPlan discountPlan){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),discountPlan);
				return;
			}
			Crud.DiscountPlanCrud.Update(discountPlan);
		}

		///<summary>Changes the DiscountPlanNum of all discountplansub that have _planFrom.DiscountPlanNum to _planInto.DiscountPlanNum</summary>
		public static void MergeTwoPlans(DiscountPlan planInto,DiscountPlan planFrom) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planInto,planFrom);
				return;
			}
			string command="UPDATE discountplansub SET DiscountPlanNum="+POut.Long(planInto.DiscountPlanNum)
				+" WHERE DiscountPlanNum="+POut.Long(planFrom.DiscountPlanNum);
			Db.NonQ(command);
			//Delete the discount plan from the database.
			Crud.DiscountPlanCrud.Delete(planFrom.DiscountPlanNum);
		}

		#endregion
		#region Methods - Delete
		#endregion
		#region Methods - Misc
		///<summary>Checks for frequency conflicts with the passed-in list of procedures.
		///Returns empty string if there are no conflicts, new line delimited list of proc codes if there are ANY frequency limitations exceeded. 
		///Optionally add procedures to the list of historic procedures. Throws exceptions. listProcs should not contain any completed procs, 
		///as those get pulled from the db.</summary>
		public static string CheckDiscountFrequency(List<Procedure> listProcs,long patNum,DateTime aptDateTime,DiscountPlanSub discountPlanSub=null,
			List<Procedure> listAddHistProcs=null)
		{
			//Doesnt need a Remoting Role Check. No call to DB
			//Note: If the passed in list contains procs in categories that have already exceeded limits, then this method will
			//return 0;
			if(listProcs.IsNullOrEmpty()) {
				return "";
			}
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				throw new ArgumentException("Patient not found in database.",nameof(patNum));
			}
			if(aptDateTime==null) {
				throw new ArgumentException("Appointment Date not present.",nameof(aptDateTime));
			}
			string frequencyConflicts="";
			if(discountPlanSub==null) {
				discountPlanSub=DiscountPlanSubs.GetSubForPat(pat.PatNum);
			}
			//if this patient is a subscriber to the discount plan.
			if(discountPlanSub==null) {
				return "";
			}
			DiscountPlan discountPlan=DiscountPlans.GetPlan(discountPlanSub.DiscountPlanNum);
			if(discountPlan==null) {
				return "";
			}
			//if aptDateTime out of Discount plan range, bounce outta this bad boyo with no conflict/
			if(!discountPlanSub.IsValidForDate(aptDateTime)) {
				return "";
			}
			//get completed procs for date range of discount plan
			DateTime termDate=discountPlanSub.DateTerm;
			if(termDate==DateTime.MinValue) {
				termDate=DateTime.MaxValue;
			}
			List<Procedure> listSortedProcs=Procedures.SortListByTreatPlanPriority(listProcs).ToList();
			listSortedProcs.RemoveAll(x => x.ProcStatus==ProcStat.C);
			listSortedProcs.InsertRange(0,listProcs.FindAll(x => x.ProcStatus==ProcStat.C));
			List<Procedure> listHistProcs=Procedures.GetCompletedForDateRange(discountPlanSub.DateEffective,termDate,listPatNums:ListTools.FromSingle(pat.PatNum));
			if(listAddHistProcs!=null) {
				listHistProcs.AddRange(listAddHistProcs);
			}
			listHistProcs.RemoveAll(x=>ListTools.In(x.ProcNum,listProcs.Select(y=>y.ProcNum).ToList()));
			//get multiple lists of ProcCodes for each catagory of frequency limit dcodes
			List<long> listAllCodes=GetDiscountPlanFrequencyProcCodes(out List<ProcedureCode> listExamCodes,
				out List<ProcedureCode> listXrayCodes,
				out List<ProcedureCode> listProphyCodes,
				out List<ProcedureCode> listFluorideCodes,
				out List<ProcedureCode> listPerioCodes,
				out List<ProcedureCode> listLimitedExamCodes,
				out List<ProcedureCode> listPACodes);
			//Iterates over the passed in list of procedures, and checks if adding them will exceed frequency limitations.
			for(int i=0;i<listSortedProcs.Count;i++) {
				if(!listAllCodes.Contains(listSortedProcs[i].CodeNum)) {
					continue;
				}
				string frequencyConflict=HasMetDiscountFrequencyLimitation(listSortedProcs[i],discountPlan,discountPlanSub,listHistProcs,listExamCodes,listXrayCodes,listProphyCodes,
					listFluorideCodes,listPerioCodes,listLimitedExamCodes,listPACodes);
				if(!frequencyConflict.IsNullOrEmpty()) {
					frequencyConflicts+=frequencyConflict+"\r\n";
				}
				listHistProcs.Add(listSortedProcs[i]);
			}
			//for each list, if the number of procedurelogs+1 > frequency limit for that catagory bounce with a conflict that includes the Proccode
			return frequencyConflicts;
		}

		///<summary>Returns an empty string if there is no limitation met for the given proc, otherwise the CodeNum is returned.</summary>
		public static string HasMetDiscountFrequencyLimitation(Procedure proc,DiscountPlan discountPlan,DiscountPlanSub discountPlanSub,
			List<Procedure> listProcHist,List<ProcedureCode> listExamCodes,List<ProcedureCode> listXrayCodes,List<ProcedureCode> listProphyCodes,
			List<ProcedureCode> listFluorideCodes,List<ProcedureCode> listPerioCodes,List<ProcedureCode> listLimitedExamCodes,List<ProcedureCode> listPACodes)
		{
			//Doesnt need a Remoting Role Check. No call to DB
			if(HasMetFrequencyLimitForCategory(proc,listProcHist,listExamCodes,discountPlan.ExamFreqLimit,discountPlanSub)
				|| HasMetFrequencyLimitForCategory(proc,listProcHist,listXrayCodes,discountPlan.XrayFreqLimit,discountPlanSub)
				|| HasMetFrequencyLimitForCategory(proc,listProcHist,listProphyCodes,discountPlan.ProphyFreqLimit,discountPlanSub)
				|| HasMetFrequencyLimitForCategory(proc,listProcHist,listFluorideCodes,discountPlan.FluorideFreqLimit,discountPlanSub)
				|| HasMetFrequencyLimitForCategory(proc,listProcHist,listPerioCodes,discountPlan.PerioFreqLimit,discountPlanSub)
				|| HasMetFrequencyLimitForCategory(proc,listProcHist,listLimitedExamCodes,discountPlan.LimitedExamFreqLimit,discountPlanSub)
				|| HasMetFrequencyLimitForCategory(proc,listProcHist,listPACodes,discountPlan.PAFreqLimit,discountPlanSub)
				) 
			{
				return ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode;
			}
			return "";
		}

		///<summary>Returns false when procCode is not in listCodesForCategory, or when procCode would not exceed the frequencyLimit for the passed in listCodesForCategory. Returns true otherwise</summary>
		public static bool HasMetFrequencyLimitForCategory(Procedure proc,List<Procedure> listProcHist,List<ProcedureCode> listCodesForCategory,int frequencyLimit,DiscountPlanSub discountPlanSub) {
			//Doesnt need a Remoting Role Check. No call to DB
			if(frequencyLimit==-1 || !listCodesForCategory.Any(x=>x.CodeNum==proc.CodeNum)) {
				return false;
			}
			if(!DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,proc.ProcDate,out DateTime startDate,out DateTime stopDate)) {
				return false;
			}
			int count=listProcHist.Count(x=>ListTools.In(x.CodeNum,listCodesForCategory.Select(y=>y.CodeNum)) && x.ProcDate >= startDate && x.ProcDate <= stopDate);
			return (count>=frequencyLimit);
		}
		#endregion
	}

	///<summary>Helper class that holds information regarding a specific procedure and it's relation to discount plan related things.</summary>
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
		public DiscountPlanProc(Procedure proc) {
			ProcNum=proc.ProcNum;
			DiscountPlanAmt=proc.DiscountPlanAmt;
		}
	}
}