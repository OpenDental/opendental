using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;


namespace OpenDentBusiness {
	///<summary></summary>
	public class DiscountPlanSubs{
		#region Methods - Get
		///<summary>Gets one DiscountPlanSub from the db.</summary>
		public static DiscountPlanSub GetOne(long discountSubNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<DiscountPlanSub>(MethodBase.GetCurrentMethod(),discountSubNum);
			}
			return Crud.DiscountPlanSubCrud.SelectOne(discountSubNum);
		}

		///<summary></summary>
		public static DiscountPlanSub GetSubForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<DiscountPlanSub>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM discountplansub WHERE PatNum = "+POut.Long(patNum);
			return Crud.DiscountPlanSubCrud.SelectOne(command);
		}

		public static List<DiscountPlanSub> GetSubsForPats(List<long> listPatNums) {
			if(listPatNums.Count<1) {
				return new List<DiscountPlanSub>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DiscountPlanSub>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * FROM discountplansub WHERE PatNum IN ("+string.Join(",",listPatNums)+")";
			return Crud.DiscountPlanSubCrud.SelectMany(command);
		}

		/// <summary>Returns the start date for the passed in effective date, with a modified year to the current year.</summary>
		public static DateTime GetAnnualMaxDateEffective(DateTime dateEffective) {
			Meth.NoCheckMiddleTierRole();
			DateTime dateStart=dateEffective;
			if(dateStart.Year < 1880) {//example 0001
				dateStart=dateStart.AddYears(DateTime.Now.Year-dateStart.Year);//=0001.AddYears(2024-0001)
			}
			return dateStart;
		}

		///<summary></summary>
		public static DateTime GetAnnualMaxDateTerm(DateTime dateTerm) {
			Meth.NoCheckMiddleTierRole();
			DateTime dateEnd=dateTerm;
			if(dateEnd.Year < 1880) {
				dateEnd=DateTime.MaxValue;
			}
			return dateEnd;
		}

		///<summary>Returns a DateTime. If the reference point is within the provided date range, it will set dateEffective.Year to the closest year of the reference point.</summary>
		public static DateTime GetDateEffectiveForAnnualDateRangeSegment(DateTime dateRefPoint,DateTime dateEffective,DateTime dateTerm) {
			Meth.NoCheckMiddleTierRole();
			if(dateRefPoint<dateEffective || dateRefPoint>dateTerm) {//Outside of date range
				return dateEffective;
			}
			if(dateEffective.AddYears(1)<=dateRefPoint) {
				int numYearsLimit=dateRefPoint.Year-dateEffective.Year;
				for(int numYears=0;numYears<numYearsLimit;numYears++) {
					if(dateEffective>dateRefPoint) {
						dateEffective=dateEffective.AddYears(-1);
						break;
					}
					dateEffective=dateEffective.AddYears(1);
				}
			}
			return dateEffective;
		}

		///<summary>Returns a DateTime. If the reference point is within the provided date range, it will set dateTerm.Year to the closest year of the reference point.</summary>
		public static DateTime GetDateTermForAnnualDateRangeSegment(DateTime dateRefPoint,DateTime dateEffective,DateTime dateTerm) {
			Meth.NoCheckMiddleTierRole();
			if(dateRefPoint<dateEffective || dateRefPoint>dateTerm) {//Outside of date range
				return dateTerm;
			}
			if(dateEffective.AddYears(1)<=dateRefPoint) {
				int numYearsLimit=dateRefPoint.Year-dateEffective.Year;
				for(int numYears=0;numYears<numYearsLimit;numYears++) {
					if(dateEffective>dateRefPoint) {
						dateEffective=dateEffective.AddYears(-1);
						break;
					}
					dateEffective=dateEffective.AddYears(1);
				}
			}
			if(dateTerm>dateEffective.AddYears(1)) {
				dateTerm=dateEffective.AddYears(1).AddDays(-1);
			}
			return dateTerm;
		}

		#endregion Methods - Get
		#region Methods - Update
		///<summary></summary>
		public static void Update(DiscountPlanSub discountPlanSub) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),discountPlanSub);
				return;
			}
			Crud.DiscountPlanSubCrud.Update(discountPlanSub);
		}

		///<summary>Updates all TP procedures.DiscountPlanAmt in the associated DiscountPlanSub date range. Order priority is based on TreatPlanPriority.</summary>
		public static void UpdateAssociatedDiscountPlanAmts(List<DiscountPlanSub> listDiscountPlanSubs,bool isDiscountPlanSubBeingDeleted=false) {
			Meth.NoCheckMiddleTierRole();
			if(listDiscountPlanSubs.IsNullOrEmpty()) {
				return;
			}
			List<long> listPatNums=listDiscountPlanSubs.Select(x=>x.PatNum).Distinct().ToList();
			//Get all TP'd procs for all patient DiscountPlanSubs
			List<Procedure> listProcedures=Procedures.GetAllForPatsAndStatuses(listPatNums,ProcStat.TP);
			//Get all adjustments for all patient DiscountPlanSubs
			List<Adjustment> listAdjustments=Adjustments.GetAdjustForPats(listPatNums);
			//Get all DiscountPlans for all patient DiscountPlanSubs
			List<DiscountPlan> listDiscountPlans=DiscountPlans.GetForPats(listPatNums);
			for(int i=0;i<listDiscountPlanSubs.Count;i++) {
				DiscountPlanSub discountPlanSub=listDiscountPlanSubs[i];
				DiscountPlan discountPlan=listDiscountPlans.FirstOrDefault(x=>x.DiscountPlanNum==discountPlanSub.DiscountPlanNum);
				if(discountPlan==null) {
					continue;
				}
				//List of TP procedures for the discountPlanSub, that fall within the discount plan sub date range.
				List<Procedure> listProceduresForPat=listProcedures.FindAll(x=>x.PatNum==discountPlanSub.PatNum);
				//List of Existing discountPlan adjustments, should only be populated if patient had a prior DPlan of the same type.
				List<Adjustment> listAdjustmentsForPat=listAdjustments.FindAll(x=>x.PatNum==discountPlanSub.PatNum && x.AdjType==discountPlan.DefNum);
				if(listProceduresForPat.IsNullOrEmpty()) {
					continue;
				}
				if(isDiscountPlanSubBeingDeleted) { //If the discount plan sub is being dropped, set all associated procs DiscountPlanAmt to 0;
					listProceduresForPat.ForEach(x=>x.DiscountPlanAmt=0);
				}
				else { //Otherwise compute the DiscountPlanAmt for all procedures within the date range.
					listProceduresForPat=Procedures.SortListByTreatPlanPriority(listProceduresForPat);
					//Iterates over all of the listProceduresForPat and sets the procedure.DiscountPlanAmt in memory
					List<DiscountPlanProc> listDiscountPlanProcs=DiscountPlans.GetDiscountPlanProc(listProceduresForPat,discountPlanSub:discountPlanSub,discountPlan:discountPlan,listAdjustments:listAdjustmentsForPat);
					for(int j=0;j<listProceduresForPat.Count;j++) {
						long procNum=listProceduresForPat[j].ProcNum;
						listProceduresForPat[j].DiscountPlanAmt=listDiscountPlanProcs.FirstOrDefault(x=>x.ProcNum==procNum).DiscountPlanAmt;
					}
				}
				Procedures.UpdateDiscountPlanAmts(listProceduresForPat);
			}
		}

		#endregion
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(DiscountPlanSub discountPlanSub) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				discountPlanSub.DiscountSubNum=Meth.GetLong(MethodBase.GetCurrentMethod(),discountPlanSub);
				return discountPlanSub.DiscountSubNum;
			}
			return Crud.DiscountPlanSubCrud.Insert(discountPlanSub);
		}

		///<summary></summary>
		public static void Delete(long discountSubNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),discountSubNum);
				return;
			}
			Crud.DiscountPlanSubCrud.Delete(discountSubNum);
		}

		public static void DeleteForPatient(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="DELETE FROM discountplansub WHERE PatNum = "+POut.Long(patNum);
			Db.NonQ(command);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		///<summary>Returns 0 if the patient has no discount plan, or if the given date is not within the effective and term dates.</summary>
		public static long GetDiscountPlanNumForPat(long patNum,DateTime date=default(DateTime)) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),patNum,date);
			}
			string command="SELECT DiscountPlanNum FROM discountplansub WHERE PatNum = "+POut.Long(patNum)+" ";
			if(date.Year>1880) {
				command+="AND ("+POut.Date(date)+">=DateEffective) "
					+"AND (DateTerm='0001-01-01' OR "+POut.Date(date)+"<=DateTerm)";
			}
			return Db.GetLong(command);
		}

		///<summary>Returns true if the patient passed in is subscribed to a discount plan (includes plans that are out of date). Otherwise false.</summary>
		public static bool HasDiscountPlan(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) FROM discountplansub WHERE PatNum="+POut.Long(patNum);
			return Db.GetInt(command)>0;
		}

		#endregion Methods - Misc
	}
}