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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DiscountPlanSub>(MethodBase.GetCurrentMethod(),discountSubNum);
			}
			return Crud.DiscountPlanSubCrud.SelectOne(discountSubNum);
		}

		///<summary></summary>
		public static DiscountPlanSub GetSubForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DiscountPlanSub>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM discountplansub WHERE PatNum = "+POut.Long(patNum);
			return Crud.DiscountPlanSubCrud.SelectOne(command);
		}

		public static List<DiscountPlanSub> GetSubsForPats(List<long> listPatNums) {
			if(listPatNums.Count<1) {
				return new List<DiscountPlanSub>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DiscountPlanSub>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command=$"SELECT * FROM discountplansub WHERE PatNum IN ({string.Join(",",listPatNums)})";
			return Crud.DiscountPlanSubCrud.SelectMany(command);
		}

		/// <summary>Returns the startDate for the passed in discountPlanSub, with a modified year to the current year.</summary>
		public static DateTime GetAnnualMaxDateEffective(DiscountPlanSub discountPlanSub) {
			//No remoting role check; No call to db.
			DateTime effectiveStartDate=discountPlanSub.DateEffective;
			if(effectiveStartDate.Year < 1880) {
				effectiveStartDate=effectiveStartDate.AddYears(DateTime.Now.Year-effectiveStartDate.Year);
			}
			return effectiveStartDate;
		}

		public static DateTime GetAnnualMaxDateTerm(DiscountPlanSub discountPlanSub) {
			//No remoting role check; No call to db.
			DateTime effectiveEndDate=discountPlanSub.DateTerm;
			if(effectiveEndDate.Year < 1880) {
				effectiveEndDate=DateTime.MaxValue;
			}
			return effectiveEndDate;
		}

		///<summary>Returns a bool. True if the reference point is within a DiscountPlans date range. False otherwise. Sets startDate/stopDate.Year to the closest year of referencePoint.
		/// startDate and stopDate will be set to the DiscountPlanSub date range if false is returned.</summary>
		public static bool GetAnnualDateRangeSegmentForGivenDate(DiscountPlanSub discountPlanSub,DateTime referencePoint,out DateTime startDate, out DateTime stopDate) {
			//No remoting role check; No call to db.
			startDate=GetAnnualMaxDateEffective(discountPlanSub);
			stopDate=GetAnnualMaxDateTerm(discountPlanSub);
			return GetAnnualStartStopDates(referencePoint,ref startDate,ref stopDate);
		}

		public static bool GetAnnualStartStopDates(DateTime referencePoint, ref DateTime startDate, ref DateTime stopDate) {
			//No remoting role check; No call to db. Uses refs.
			if(referencePoint < startDate || referencePoint > stopDate) {
				return false;
			}
			if(startDate.Month < referencePoint.Month) {
				startDate=startDate.AddYears(referencePoint.Year-startDate.Year);
			}
			else if(startDate.Month > referencePoint.Month) {
				startDate=startDate.AddYears((referencePoint.Year-startDate.Year)-1);
			}
			else {
				//compare days
				if(startDate.Day < referencePoint.Day) {
					startDate=startDate.AddYears(referencePoint.Year-startDate.Year);
				}
				else if(startDate.Day > referencePoint.Day && (startDate.Day!=29 && startDate.Month!=2)) {
					startDate=startDate.AddYears((referencePoint.Year-startDate.Year)-1);
				}
				else {
					if(startDate.Year < referencePoint.Year) {
						startDate=startDate.AddYears(referencePoint.Year-startDate.Year);
					}
				}
			}
			if(stopDate > startDate.AddYears(1)) {
				stopDate=startDate.AddYears(1).AddDays(-1);
			}
			return true;
		}

		#endregion Methods - Get
		#region Methods - Update
		///<summary></summary>
		public static void Update(DiscountPlanSub discountPlanSub) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),discountPlanSub);
				return;
			}
			Crud.DiscountPlanSubCrud.Update(discountPlanSub);
		}

		///<summary>Updates all TP procedures.DiscountPlanAmt in the associated DiscountPlanSub date range. Order priority is based on TreatPlanPriority.</summary>
		public static void UpdateAssociatedDiscountPlanAmts(List<DiscountPlanSub> listDiscountPlanSubs,bool isDiscountPlanSubBeingDeleted=false) {
			//No remoting role check; No call to db.
			if(listDiscountPlanSubs.IsNullOrEmpty()) {
				return;
			}
			List<long> listPatNums=listDiscountPlanSubs.Select(x=>x.PatNum).Distinct().ToList();
			//Get all TP'd procs for all patient DiscountPlanSubs
			List<Procedure> listProcs=Procedures.GetAllForPatsAndStatuses(listPatNums,ProcStat.TP);
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
				List<Procedure> listProcsForPat=listProcs.FindAll(x=>x.PatNum==discountPlanSub.PatNum);
				//List of Existing discountPlan adjustments, should only be populated if patient had a prior DPlan of the same type.
				List<Adjustment> listAdjForPat=listAdjustments.FindAll(x=>x.PatNum==discountPlanSub.PatNum && x.AdjType==discountPlan.DefNum);
				if(listProcsForPat.IsNullOrEmpty()) {
					continue;
				}
				if(isDiscountPlanSubBeingDeleted) { //If the discount plan sub is being dropped, set all associated procs DiscountPlanAmt to 0;
					listProcsForPat.ForEach(x=>x.DiscountPlanAmt=0);
				}
				else { //Otherwise compute the DiscountPlanAmt for all procedures within the date range.
					listProcsForPat=Procedures.SortListByTreatPlanPriority(listProcsForPat).ToList();
					//Iterates over all of the listProcsForPat and sets the procedure.DiscountPlanAmt in memory
					UpdateAssociatedDiscountPlanAmtsHelper(ref listProcsForPat,listAdjForPat,discountPlanSub,discountPlan);
				}
				Procedures.UpdateDiscountPlanAmts(listProcsForPat);
			}
		}

		///<summary>Helper method, sets the value of listProcs.DiscountPlanAmt. Order dependant.</summary>
		private static void UpdateAssociatedDiscountPlanAmtsHelper(ref List<Procedure> listProcs,List<Adjustment> listAdjs,DiscountPlanSub discountPlanSub,DiscountPlan discountPlan) {
			//No remoting role check; No call to db.
			List<DiscountPlanProc> listDiscountPlanProcs=DiscountPlans.GetDiscountPlanProc(listProcs,discountPlanSub:discountPlanSub,discountPlan:discountPlan,listAdjustments:listAdjs);
			for(int i = 0;i<listProcs.Count;i++) {
				long procNum=listProcs[i].ProcNum;
				listProcs[i].DiscountPlanAmt=listDiscountPlanProcs.FirstOrDefault(x=>x.ProcNum==procNum).DiscountPlanAmt;
			}
		}

		#endregion
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(DiscountPlanSub discountPlanSub) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				discountPlanSub.DiscountSubNum=Meth.GetLong(MethodBase.GetCurrentMethod(),discountPlanSub);
				return discountPlanSub.DiscountSubNum;
			}
			return Crud.DiscountPlanSubCrud.Insert(discountPlanSub);
		}

		///<summary></summary>
		public static void Delete(long discountSubNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),discountSubNum);
				return;
			}
			Crud.DiscountPlanSubCrud.Delete(discountSubNum);
		}

		public static void DeleteForPatient(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) FROM discountplansub WHERE PatNum="+POut.Long(patNum);
			return Db.GetInt(command)>0;
		}

		#endregion Methods - Misc
	}
}