using CodeBase;
using System;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using OpenDentBusiness.Eclaims;
using System.Globalization;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Benefits {
		///<summary></summary>
		public static List<Benefit> GetPatientData(List<PatPlan> listPatPlans,List<InsSub> listInsSubs) {
			//No need to check MiddleTierRole; no call to db.
			return Refresh(listPatPlans,listInsSubs);
		}

		///<summary>Gets a list of all benefits for a given list of patplans for one patient.</summary>
		public static List<Benefit> Refresh(List<PatPlan> listPatPlans,List<InsSub> listInsSubs) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Benefit>>(MethodBase.GetCurrentMethod(),listPatPlans,listInsSubs);
			}
			//Only need to check listPatPlans for null / empty because InsSubs.GetSub() handles a null / empty subList.
			if(listPatPlans==null || listPatPlans.Count==0) {
				return new List<Benefit>();
			}
			string command="SELECT * FROM benefit "
				//null safe, returns new InsSub with PlanNum 0 if GetSub doesn't find a match in neither subList nor the db
				+"WHERE PlanNum IN ("+string.Join(",",listPatPlans.Select(x => POut.Long(InsSubs.GetSub(x.InsSubNum,listInsSubs).PlanNum)))+") "
				+"OR PatPlanNum IN ("+string.Join(",",listPatPlans.Select(x => POut.Long(x.PatPlanNum)))+")";
			List<Benefit> listBenefits=Crud.BenefitCrud.SelectMany(command);
			listBenefits.Sort();
			return listBenefits;
		}

		///<summary>Gets a list of all benefits for a given list of patplans where the benefit has a PatPlanNum that matches a PatPlan.PatPlanNum in
		///listPatPlans or the benefit has a PlanNum that matches the inssub.PlanNum for the inssubs linked to the PatPlans in listPatPlans.</summary>
		public static List<Benefit> GetAllForPatPlans(List<PatPlan> listPatPlans,List<InsSub> listInsSubs) {
			if(listPatPlans.IsNullOrEmpty()) {
				return new List<Benefit>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Benefit>>(MethodBase.GetCurrentMethod(),listPatPlans,listInsSubs);
			}
			Dictionary<long,long> dictionaryInsSubNumsPlanNums=listInsSubs.GroupBy(x => x.InsSubNum).ToDictionary(x => x.Key,x => x.Last().PlanNum);
			List<long> listPlanNums=listPatPlans.Select(x => x.InsSubNum).Distinct()
				.Select(x => dictionaryInsSubNumsPlanNums.TryGetValue(x,out long planNum)?planNum:(InsSubs.GetOne(x)?.PlanNum??0)).Distinct().ToList();
			string command="SELECT * FROM benefit "
				//null safe, returns new InsSub with PlanNum 0 if GetSub doesn't find a match in neither subList nor the db
				+"WHERE PlanNum IN ("+string.Join(",",listPlanNums.Select(x => POut.Long(x)))+") "
				+"OR PatPlanNum IN ("+string.Join(",",listPatPlans.Select(x => x.PatPlanNum).Distinct().Select(x => POut.Long(x)))+")";
			return Crud.BenefitCrud.SelectMany(command);
		}

		///<summary>Returns a sorted list of benefits for the specified plan or pat plan.  patPlanNum can be 0.</summary>
		public static List<Benefit> GetForPlanOrPatPlan(long planNum,long patPlanNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Benefit>>(MethodBase.GetCurrentMethod(),planNum,patPlanNum);
			}
			string command="SELECT * FROM benefit WHERE PlanNum = "+POut.Long(planNum);
			if(patPlanNum!=0) {
				command+=" OR PatPlanNum = "+POut.Long(patPlanNum);
			}
			List<Benefit> listBenefits=Crud.BenefitCrud.SelectMany(command);
			listBenefits.Sort();
			return listBenefits;
		}

		///<summary></summary>
		public static void Update(Benefit benefit,Benefit benefitOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),benefit,benefitOld);
				return;
			}
			Crud.BenefitCrud.Update(benefit,benefitOld);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			InsEditLogs.MakeLogEntry(benefit,benefitOld,InsEditLogType.Benefit,Security.CurUser.UserNum);
		}

		///<summary></summary>
		public static long Insert(Benefit benefit) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				benefit.BenefitNum=Meth.GetLong(MethodBase.GetCurrentMethod(),benefit);
				return benefit.BenefitNum;
			}
			long benefitNum=Crud.BenefitCrud.Insert(benefit);
			if(benefit.PlanNum != 0) {//Does not log PatPlan benefits
				//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				InsEditLogs.MakeLogEntry(benefit,null,InsEditLogType.Benefit,Security.CurUser.UserNum);
			}
			return benefitNum;
		}

		///<summary></summary>
		public static void Delete(Benefit benefit) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),benefit);
				return;
			}
			string command="DELETE FROM benefit WHERE BenefitNum ="+POut.Long(benefit.BenefitNum);
			Db.NonQ(command);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			InsEditLogs.MakeLogEntry(null,benefit,InsEditLogType.Benefit,Security.CurUser.UserNum);
		}

		///<summary>Only for display purposes rather than any calculations.  Gets an annual max from the supplied list of benefits.  Ignores benefits that do not match either the planNum or the patPlanNum.  Because it starts at the top of the benefit list, it will get the most general limitation first.  Returns -1 if none found.  Usually, set isFam to false unless we are specifically interested in that value.</summary>
		public static double GetAnnualMaxDisplay(List<Benefit> listBenefits,long planNum,long patPlanNum,bool isFam) {
			//No need to check MiddleTierRole; no call to db.
			List<Benefit> listBenefitsMatching=new List<Benefit>();
			for(int i=0;i<listBenefits.Count;i++) {
				if(listBenefits[i].PlanNum==0 && listBenefits[i].PatPlanNum!=patPlanNum) {
					continue;
				}
				if(listBenefits[i].PatPlanNum==0 && listBenefits[i].PlanNum!=planNum) {
					continue;
				}
				if(listBenefits[i].BenefitType!=InsBenefitType.Limitations) {
					continue;
				}
				if(listBenefits[i].QuantityQualifier!=BenefitQuantity.None) {
					continue;
				}
				if(listBenefits[i].TimePeriod!=BenefitTimePeriod.CalendarYear && listBenefits[i].TimePeriod!=BenefitTimePeriod.ServiceYear) {
					continue;
				}
				if(isFam){
					if(listBenefits[i].CoverageLevel!=BenefitCoverageLevel.Family){//individ or none
						continue;
					}
				}
				else{
					if(listBenefits[i].CoverageLevel!=BenefitCoverageLevel.Individual) {//Family or None
						continue;
					}
				}
				//coverage level?
				if(listBenefits[i].CodeNum != 0) {
					continue;
				}
				if(listBenefits[i].CovCatNum != 0) {
					EbenefitCategory eBenefitCategory=CovCats.GetEbenCat(listBenefits[i].CovCatNum);
					if(eBenefitCategory != EbenefitCategory.General && eBenefitCategory != EbenefitCategory.None) {
						continue;
					}
				}
				listBenefitsMatching.Add(listBenefits[i]);
			}
			if(listBenefitsMatching.Count==0) {
				return -1;
			}
			//Get minimum benefit amount, preferring benefits with no catagory.
			return listBenefitsMatching.OrderBy(x => x.CovCatNum!=0).ThenBy(x => x.MonetaryAmt).First().MonetaryAmt;
		}

		///<summary>Only for display purposes rather than any calculations.  Gets a general deductible from the supplied list of benefits.  Ignores benefits that do not match either the planNum or the patPlanNum.</summary>
		public static double GetDeductGeneralDisplay(List<Benefit> listBenefits,long planNum,long patPlanNum,BenefitCoverageLevel benefitCoverageLevel) {
			//No need to check MiddleTierRole; no call to db.
			List<Benefit> listBenefitsMatching=new List<Benefit>();
			for(int i=0;i<listBenefits.Count;i++) {
				if(listBenefits[i].PlanNum==0 && listBenefits[i].PatPlanNum!=patPlanNum) {
					continue;
				}
				if(listBenefits[i].PatPlanNum==0 && listBenefits[i].PlanNum!=planNum) {
					continue;
				}
				if(listBenefits[i].BenefitType!=InsBenefitType.Deductible) {
					continue;
				}
				if(listBenefits[i].QuantityQualifier!=BenefitQuantity.None) {
					continue;
				}
				if(listBenefits[i].TimePeriod!=BenefitTimePeriod.CalendarYear && listBenefits[i].TimePeriod!=BenefitTimePeriod.ServiceYear) {
					continue;
				}
				if(listBenefits[i].CoverageLevel != benefitCoverageLevel) {
					continue;
				}
				if(listBenefits[i].CodeNum != 0) {
					continue;
				}
				if(listBenefits[i].CovCatNum != 0) {
					EbenefitCategory eBenefitCategory=CovCats.GetEbenCat(listBenefits[i].CovCatNum);
					if(eBenefitCategory != EbenefitCategory.General && eBenefitCategory != EbenefitCategory.None) {
						continue;
					}
				}
				listBenefitsMatching.Add(listBenefits[i]);
			}
			if(listBenefitsMatching.Count > 0) {
				//Get the largest benefit amount from the matching benefits with no catagory or of the General / None category.
				return listBenefitsMatching.Max(x => x.MonetaryAmt);
			}
			return -1;
		}

		///<summary>Used only in ClaimProcs.ComputeBaseEst.  Gets a deductible amount from the supplied list of benefits.  Ignores benefits that do not 
		///match either the planNum or the patPlanNum.  It figures out how much was already used and reduces the returned value by that amount.  
		///Both individual and family deductibles will reduce the returned value independently.  Works for individual procs, categories, and general.</summary>
		public static double GetDeductibleByCode(List<Benefit> listBenefits,long planNum,long patPlanNum,DateTime dateProc,string procCode,
			List<ClaimProcHist> listClaimProcHists,List<ClaimProcHist> listClaimProcHistsLoop,InsPlan insPlan,long patNum) 
		{
			//No need to check MiddleTierRole; no call to db.
			if(IsExcluded(procCode,listBenefits,planNum,patPlanNum)) {
				return 0;
			}
			#region filter benList for deductibles
			//first, create a much shorter list with only relevant benefits in it.
			List<Benefit> listBenefitsShort=new List<Benefit>();
			for(int i=0;i<listBenefits.Count;i++) {
				if(listBenefits[i].PlanNum==0 && listBenefits[i].PatPlanNum!=patPlanNum) {
					continue;
				}
				if(listBenefits[i].PatPlanNum==0 && listBenefits[i].PlanNum!=planNum) {
					continue;
				}
				if(listBenefits[i].BenefitType!=InsBenefitType.Deductible) {
					continue;
				}
				//if(benList[i].QuantityQualifier!=BenefitQuantity.None) {
				//	continue;
				//}
				if(listBenefits[i].TimePeriod!=BenefitTimePeriod.CalendarYear 
					&& listBenefits[i].TimePeriod!=BenefitTimePeriod.ServiceYear
					&& listBenefits[i].TimePeriod!=BenefitTimePeriod.Lifetime)//this is probably only going to be used in annual max, though
				{
					continue;
				}
				listBenefitsShort.Add(listBenefits[i]);
			}
			#endregion
			#region get individual deductibles
			//look for the best matching individual deduct----------------------------------------------------------------
			Benefit benefitIndGeneral=null;
			Benefit benefitInd=null;
			#region no category
			for(int i=0;i<listBenefitsShort.Count;i++){
				if(listBenefitsShort[i].CoverageLevel == BenefitCoverageLevel.Family){
					continue;
				}
				if(listBenefitsShort[i].CodeNum>0){
					continue;
				}
				if(listBenefitsShort[i].CovCatNum==0){
					benefitInd=listBenefitsShort[i];
					//This deductible must be a general deductible since it has no associated category
					benefitIndGeneral=listBenefitsShort[i];//sum of deductibles should not exceed this amount, even if benInd is less.
				}
			}
			#endregion
			#region specific category.
			CovSpan[] covSpanArrayForCat;
			for(int i=0;i<listBenefitsShort.Count;i++){
				if(listBenefitsShort[i].CoverageLevel == BenefitCoverageLevel.Family){
					continue;
				}
				if(listBenefitsShort[i].CodeNum>0){
					continue;
				}
				if(listBenefitsShort[i].CovCatNum!=0){
					//see if the span matches
					covSpanArrayForCat=CovSpans.GetForCat(listBenefitsShort[i].CovCatNum);
					bool isMatch=false;
					for(int j=0;j<covSpanArrayForCat.Length;j++){
						if(String.Compare(procCode,covSpanArrayForCat[j].FromCode)>=0 && String.Compare(procCode,covSpanArrayForCat[j].ToCode)<=0){
							isMatch=true;
							break;
						}
					}
					if(!isMatch) {
						continue;//no match
					}
					if(benefitInd != null && benefitInd.CovCatNum!=0){//must compare
						//only use the new one if the item order is larger
						if(CovCats.GetOrderShort(listBenefitsShort[i].CovCatNum) > CovCats.GetOrderShort(benefitInd.CovCatNum)){
							benefitInd=listBenefitsShort[i];
						}
					}
					else{//first one encountered for a category
						benefitInd=listBenefitsShort[i];
					}
				}
			}
			#endregion
			#region specific code
			for(int i=0;i<listBenefitsShort.Count;i++){
				if(listBenefitsShort[i].CoverageLevel == BenefitCoverageLevel.Family){
					continue;
				}
				if(listBenefitsShort[i].CodeNum==0){
					continue;
				}
				if(procCode==ProcedureCodes.GetStringProcCode(listBenefitsShort[i].CodeNum)){
					benefitInd=listBenefitsShort[i];
				}
			}
			#endregion
			#endregion
			#region get family deductibles
			//look for the best matching family deduct----------------------------------------------------------------
			Benefit benefitFamGeneral=null;
			Benefit benefitFam=null;
			#region no category
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel != BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum>0) {
					continue;
				}
				if(listBenefitsShort[i].CovCatNum==0) {
					benefitFam=listBenefitsShort[i];
					benefitFamGeneral=listBenefitsShort[i];
				}
			}
			#endregion
			#region specific category.
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel != BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum>0) {
					continue;
				}
				if(listBenefitsShort[i].CovCatNum!=0) {
					//see if the span matches
					covSpanArrayForCat=CovSpans.GetForCat(listBenefitsShort[i].CovCatNum);
					bool isMatch=false;
					for(int j=0;j<covSpanArrayForCat.Length;j++) {
						if(String.Compare(procCode,covSpanArrayForCat[j].FromCode)>=0 && String.Compare(procCode,covSpanArrayForCat[j].ToCode)<=0) {
							isMatch=true;
							break;
						}
					}
					if(!isMatch) {
						continue;//no match
					}
					if(benefitFam != null && benefitFam.CovCatNum!=0) {//must compare
						//only use the new one if the item order is larger
						if(CovCats.GetOrderShort(listBenefitsShort[i].CovCatNum) > CovCats.GetOrderShort(benefitFam.CovCatNum)) {
							benefitFam=listBenefitsShort[i];
						}
					}
					else {//first one encountered for a category
						benefitFam=listBenefitsShort[i];
					}
				}
			}
			#endregion
			#region specific code
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel != BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum==0) {
					continue;
				}
				if(procCode==ProcedureCodes.GetStringProcCode(listBenefitsShort[i].CodeNum)) {
					benefitFam=listBenefitsShort[i];
				}
			}
			#endregion
			#endregion
			//example. $50 individual deduct, $150 family deduct.
			//Only individual deductibles make sense as the starting point.
			//Family deductible just limits the sum of individual deductibles.
			//If there is no individual deductible that matches, then return 0.
			if(benefitInd==null || benefitInd.MonetaryAmt==-1 || benefitInd.MonetaryAmt==0) {
				return 0;
			}
			//Reduce by the sum of the deductibles on claim procs associated with claims in the benefit period for individual or family as appropriate
			#region calculate individual already paid this year
			List<ClaimProcHist> listClaimProcHistsPat=listClaimProcHists.FindAll(x => x.PatNum==patNum);
			double retVal=GetDeductibleRemainingHelper(insPlan,dateProc,benefitInd,benefitIndGeneral,listClaimProcHistsPat);
			#endregion
			#region reduce by amount individual already paid in loopList
			//now, do a similar thing with loopList, individ-----------------------------------------------------------------------
			#region diagnostic/preventive workaround
			//There is a kludgey workaround in the loop below.
			//It handles the very specific problem of a single diagnostic/preventive deductible even though we have two separate benefits for it.
			//The better solution will be benefits with multiple categories or spans.
			//This workaround is only applied if there are both a diagnostic and a preventive deductible for the same amount.
			//If the benefit is diagnostic, then also check the spans of the preventive category
			CovSpan[] covSpanArrayOther=null;
			if(CovCats.GetEbenCat(benefitInd.CovCatNum)==EbenefitCategory.Diagnostic){
				for(int i=0;i<listBenefitsShort.Count;i++){//look through the benefits again
					if(listBenefitsShort[i].CoverageLevel!=BenefitCoverageLevel.Individual){
						continue;
					}
					if(CovCats.GetEbenCat(listBenefitsShort[i].CovCatNum)!=EbenefitCategory.RoutinePreventive){
						continue;
					}
					covSpanArrayOther=CovSpans.GetForCat(listBenefitsShort[i].CovCatNum);
				}
			}
			//if the benefit is preventive, then also check the spans of the diagnostic category
			if(CovCats.GetEbenCat(benefitInd.CovCatNum)==EbenefitCategory.RoutinePreventive){
				for(int i=0;i<listBenefitsShort.Count;i++){//look through the benefits again
					if(listBenefitsShort[i].CoverageLevel!=BenefitCoverageLevel.Individual){
						continue;
					}
					if(CovCats.GetEbenCat(listBenefitsShort[i].CovCatNum)!=EbenefitCategory.Diagnostic){
						continue;
					}
					covSpanArrayOther=CovSpans.GetForCat(listBenefitsShort[i].CovCatNum);
				}
			}
			#endregion
			for(int i=0;i<listClaimProcHistsLoop.Count;i++) {
				#region filter loopList
				if(listClaimProcHistsLoop[i].PlanNum != planNum) {
					continue;//different plan.  Even the loop list can contain info for multiple plans.
				}
				if(listClaimProcHistsLoop[i].PatNum != patNum) {
					continue;//this is for someone else in the family
				}
				//Loop list needs to consider procedure specific codes so that deductibles apply to other procedures within a TP if necessary.  E.g. Unit Test 16
				if(benefitInd.CodeNum!=0) {//specific code
					if(ProcedureCodes.GetStringProcCode(benefitInd.CodeNum)!=listClaimProcHistsLoop[i].StrProcCode) {
						continue;
					}
				}
				else if(benefitInd.CovCatNum!=0) {//specific category
					covSpanArrayForCat=CovSpans.GetForCat(benefitInd.CovCatNum);
					bool isMatch=false;
					for(int j=0;j<covSpanArrayForCat.Length;j++) {
						if(String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayForCat[j].FromCode)>=0 
							&& String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayForCat[j].ToCode)<=0) {
							isMatch=true;
							break;
						}
					}
					if(covSpanArrayOther!=null){
						for(int j=0;j<covSpanArrayOther.Length;j++) {
							if(String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayOther[j].FromCode)>=0 
								&& String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayOther[j].ToCode)<=0) {
								isMatch=true;
								break;
							}
						}
					}
					if(!isMatch) {
						continue;
					}
				}
				//if no category, then benefits are not restricted by proc code.
				if(listClaimProcHistsLoop[i].Deduct==-1) {
					continue;
				}
				#endregion
				retVal-=listClaimProcHistsLoop[i].Deduct;
			}
			#endregion
			if(retVal<=0) {
				return 0;
			}
			double deductUsedInLoopList=0;//sum up the deductibles in looplist for the current plan
			List<ClaimProcHist> listClaimProcHistsLoopInsPlan=listClaimProcHistsLoop.FindAll(x => x.PlanNum==planNum && x.PatNum==patNum);
			for(int i=0;i<listClaimProcHistsLoopInsPlan.Count;i++) {
				deductUsedInLoopList+=Math.Max(listClaimProcHistsLoopInsPlan[i].Deduct,0);
			}
			if(benefitIndGeneral!=null) {//if there exists a general deductible
				if((retVal + deductUsedInLoopList) > benefitIndGeneral.MonetaryAmt) {
					//If the potential deductible amount plus the deductible amount that has been allocated to other TP procedures is greater than the general 
					//deductible amount, reduce the potential deductible by the amount that the potential deductible plus the deductible amount for other TP 
					//procedures exceeds the general deductible. Example: if (25+45) > 50, then return 50-45=5. Where 25 is the potential deductible amount, 
					//45 is the amount that has been allocated to other TP procedures, and 50 is the general deductible amount.
					retVal=benefitIndGeneral.MonetaryAmt - deductUsedInLoopList;// (fix for Unit Test 16)
				}
			}
			//if there is still a deductible, we might still reduce it based on family ded used.
			if((benefitFam==null && benefitFamGeneral==null) || (benefitFam?.MonetaryAmt==-1 && benefitFamGeneral?.MonetaryAmt==-1)) {
				return retVal;
			}
			#region calculate the amount family already paid this year
			double dedFam=GetDeductibleRemainingHelper(insPlan,dateProc,benefitFam,benefitFamGeneral,listClaimProcHists);
			#endregion
			#region reduce by amount family already paid in loopList
			//reduce family ded by amounts already used in loop---------------------------------------------------------------
			for(int i=0;i<listClaimProcHistsLoop.Count;i++) {
				if(listClaimProcHistsLoop[i].PlanNum != planNum) {
					continue;//different plan
				}
				//Loop list needs to consider procedure specific codes so that deductibles apply to other procedures within a TP if necessary.
				if(benefitFam.CodeNum!=0) {//specific code
					if(ProcedureCodes.GetStringProcCode(benefitFam.CodeNum)!=listClaimProcHistsLoop[i].StrProcCode) {
						continue;
					}
				}
				else if(benefitFam.CovCatNum!=0) {//specific category
					covSpanArrayForCat=CovSpans.GetForCat(benefitFam.CovCatNum);
					bool isMatch=false;
					for(int j=0;j<covSpanArrayForCat.Length;j++) {
						if(String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayForCat[j].FromCode)>=0 
							&& String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayForCat[j].ToCode)<=0) {
							isMatch=true;
							break;
						}
					}
					if(!isMatch) {
						continue;
					}
				}
				//if no category, then benefits are not restricted by proc code.
				if(listClaimProcHistsLoop[i].Deduct==-1) {
					continue;
				}
				dedFam-=listClaimProcHistsLoop[i].Deduct;
			}
			#endregion
			//if the family deductible has all been used up on other procs
			if(dedFam<=0) {
				return 0;//then no deductible, regardless of what we computed for individual
			}
			if(retVal > dedFam) {//example; retInd=$50, but 120 of 150 family ded has been used.  famded=30.  We need to return 30.
				return dedFam;
			}
			return retVal;
		}

		///<summary>Figures out the total amount of deductibles used based on the claim proc history and returns how much is left to pay.</summary>
		private static double GetDeductibleRemainingHelper(InsPlan insPlan,DateTime procDate,Benefit benefitInd,Benefit benefitIndGeneral,
			List<ClaimProcHist> listClaimProcHists)
		{
			//Before we call this method, we return 0 if benInd is null. If this is ever called outside of that context, we want to maintain that logic.
			if(benefitInd==null) {
				return 0;
			}
			//establish date range for procedures to consider
			DateTime dateStart=BenefitLogic.ComputeRenewDate(procDate,insPlan.MonthRenew);
			DateTime dateEnd=dateStart.AddYears(1).AddDays(-1);//Consider all claim procs with a ProcDate within one year of the renew date
			double generalUsed=0;
			double categoryUsed=0;
			for(int i=0;i<listClaimProcHists.Count;i++) {
				if(listClaimProcHists[i].PlanNum != insPlan.PlanNum) {
					continue;//different plan
				}
				if(listClaimProcHists[i].ProcDate<dateStart || listClaimProcHists[i].ProcDate>dateEnd) {
					continue;
				}
				if(listClaimProcHists[i].Deduct==-1) {
					continue;
				}
				//Keep track of historic deductibles that have already been used and match benInd's category.
				//All historical deductibles (including category specific ones) count towards the general deductible category so that they are accounted for.
				if(benefitInd.CovCatNum > 0) {
					CovSpan[] covSpanArrayForCats=CovSpans.GetForCat(benefitInd.CovCatNum);
					for(int j=0;j<covSpanArrayForCats.Length;j++) {
						if(String.Compare(listClaimProcHists[i].StrProcCode,covSpanArrayForCats[j].FromCode)>=0 
							&& String.Compare(listClaimProcHists[i].StrProcCode,covSpanArrayForCats[j].ToCode)<=0) 
						{
							categoryUsed+=listClaimProcHists[i].Deduct;
							break;
						}
					}
				}
				//Every single historical deductible will be attributed to the general deductible (regardless of type or category).
				generalUsed+=listClaimProcHists[i].Deduct;
			}
			double retVal;
			//Calculate how much remains in each deductible 'bucket'.
			//Start each variable off as -1 to indicate that the deductible 'bucket' is not present.
			double generalRemain=-1;
			double categoryRemain=-1;
			if(benefitIndGeneral!=null) {
				//A general deductible is present, calculate how much remains and don't let the result go negative.
				generalRemain=Math.Max(benefitIndGeneral.MonetaryAmt-generalUsed,0);
			}
			if(benefitInd.CovCatNum > 0 || benefitInd.CodeNum > 0) {
				//A category or code specific specific deductible is present, calculate how much remains and don't let the result go negative.
				categoryRemain=Math.Max(benefitInd.MonetaryAmt-categoryUsed,0);
			}
			if(generalRemain > -1) {
				//A general deductible is present.
				if(categoryRemain > -1) {
					//Both general and category specific deductibles are present.
					//The patient only owes the minimum value between the two remaining 'buckets'. 
					//E.g. general only has $5 remaining but the category specific 'bucket' has $20 remaining. The patient only owes $5 in deductibles.
					retVal=Math.Min(generalRemain,categoryRemain);
				}
				else {
					//No category specific deductible is present. Use whatever value remains in the general dedubctible 'bucket'.
					retVal=generalRemain;
				}
			}
			else if(categoryRemain > -1) {
				//No general deductible is present. However, a category specific deductible is present.
				//Use whatever value remains in the category specific dedubctible 'bucket'.
				//There are unit tests that have no general deductible benefits but only category specific deductible benefits.
				//Any deductible that was used towards any other category than the one currently being checked will fall into the 'general used'.
				//This value should always be considered even if there is no general deductible benefit.
				retVal=Math.Max(categoryRemain-generalUsed,0);
			}
			else {
				//No general or category specific deductibles are present. The patient owes nothing towards deductibles.
				retVal=0;
			}
			return retVal;
		}

		///<summary>Used only in ClaimProcs.ComputeBaseEst.  Calculates the most specific limitation for the specified code.  
		///This is usually an annual max, ortho max, or fluoride limitation (only if age match).  
		///Ignores benefits that do not match either the planNum or the patPlanNum.  
		///It figures out how much was already used and reduces the returned value by that amount.  
		///Both individual and family limitations will reduce the returned value independently.  
		///Works for individual procs, categories, and general.  Also outputs a string description of the limitation.  
		///There don't seem to be any situations where multiple limitations would each partially reduce coverage for a single code, other than ind/fam.  
		///The returned value will be the original insEstTotal passed in unless there was some limitation that reduced it.
		///Considers InsEstTotalOverride when dynamically writing the EstimateNote.</summary>
		public static double GetLimitationByCode(List<Benefit> listBenefits,long planNum,long patPlanNum,DateTime procDate,
			string procCodeStr,List<ClaimProcHist> listClaimProcHists,List<ClaimProcHist> listClaimProcHistsLoop,InsPlan insPlan,long patNum,out string note,
			double insEstTotal,int patientAge,long insSubNum,double insEstTotalOverride,out LimitationTypeMet limitationTypeMet) {
			//No need to check MiddleTierRole; no call to db.
			note ="";
			limitationTypeMet=LimitationTypeMet.None;
			//first, create a much shorter list with only relevant benefits in it.
			List<Benefit> listBenefitsShort=new List<Benefit>();
			for(int i=0;i<listBenefits.Count;i++) {
				if(listBenefits[i].PlanNum==0 && listBenefits[i].PatPlanNum!=patPlanNum) {
					continue;
				}
				if(listBenefits[i].PatPlanNum==0 && listBenefits[i].PlanNum!=planNum) {
					continue;
				}
				if(listBenefits[i].BenefitType!=InsBenefitType.Limitations) {
					continue;
				}
				//if(benList[i].TimePeriod!=BenefitTimePeriod.CalendarYear 
				//	&& benList[i].TimePeriod!=BenefitTimePeriod.ServiceYear
				//	&& benList[i].TimePeriod!=BenefitTimePeriod.Lifetime)
				//{
				//	continue;
				//}
				listBenefitsShort.Add(listBenefits[i]);
			}
			//look for the best matching individual limitation----------------------------------------------------------------
			Benefit benefitInd=null;
			//start with no category
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel == BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum>0) {
					continue;
				}
				if(listBenefitsShort[i].CovCatNum==0) {
					if(!IsCodeInGeneralSpan(procCodeStr)) {
						continue;
					}
					//Leave this outside of the isOutsideGeneralSpan check incase there is no general category.
					benefitInd=listBenefitsShort[i];
				}
			}
			//then, specific category.
			CovSpan[] covSpanArrayForCat;
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel == BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum>0) {
					continue;
				}
				if(listBenefitsShort[i].CovCatNum!=0) {
					//see if the span matches
					covSpanArrayForCat=CovSpans.GetForCat(listBenefitsShort[i].CovCatNum);
					bool isMatch=false;
					for(int j=0;j<covSpanArrayForCat.Length;j++) {
						if(String.Compare(procCodeStr,covSpanArrayForCat[j].FromCode)>=0 && String.Compare(procCodeStr,covSpanArrayForCat[j].ToCode)<=0) {
							isMatch=true;
							break;
						}
					}
					if(!isMatch) {
						continue;//no match
					}
					if(listBenefitsShort[i].QuantityQualifier==BenefitQuantity.NumberOfServices
						|| listBenefitsShort[i].QuantityQualifier==BenefitQuantity.Months
						|| listBenefitsShort[i].QuantityQualifier==BenefitQuantity.Years) 
					{
						continue;//exclude frequencies
					}
					//If it's an age based limitation, then make sure the patient age matches.
					//If we have an age match, then we exit the method right here.
					if(listBenefitsShort[i].QuantityQualifier==BenefitQuantity.AgeLimit && listBenefitsShort[i].Quantity > 0) {
						if(patientAge > listBenefitsShort[i].Quantity) {
							note=Lans.g("Benefits","Age limitation:")+" "+listBenefitsShort[i].Quantity.ToString();
							limitationTypeMet=LimitationTypeMet.Aging;
							return 0;//not covered if too old.
						}
						continue;//don't use an age limitation for the match if the patient has not reached the age limit
					}
					if(benefitInd != null && benefitInd.CovCatNum!=0) {//must compare
						//only use the new one if the item order is larger
						if(CovCats.GetOrderShort(listBenefitsShort[i].CovCatNum) > CovCats.GetOrderShort(benefitInd.CovCatNum)) {
							benefitInd=listBenefitsShort[i];
						}
					}
					else {//first one encountered for a category
						benefitInd=listBenefitsShort[i];
					}
				}
			}
			//then, specific code
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel == BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum==0) {
					continue;
				}
				if(procCodeStr!=ProcedureCodes.GetStringProcCode(listBenefitsShort[i].CodeNum)) {
					continue;
				}
				if(listBenefitsShort[i].QuantityQualifier==BenefitQuantity.NumberOfServices
					|| listBenefitsShort[i].QuantityQualifier==BenefitQuantity.Months
					|| listBenefitsShort[i].QuantityQualifier==BenefitQuantity.Years) 
				{
					continue;//exclude frequencies
				}
				//if it's an age based limitation, then make sure the patient age matches.
				//If we have an age match, then we exit the method right here.
				if(listBenefitsShort[i].QuantityQualifier==BenefitQuantity.AgeLimit && listBenefitsShort[i].Quantity > 0){
					if(patientAge > listBenefitsShort[i].Quantity){
						note=Lans.g("Benefits","Age limitation:")+" "+listBenefitsShort[i].Quantity.ToString();
						limitationTypeMet=LimitationTypeMet.Aging;
						return 0;//not covered if too old.
					}
				}
				else{//anything but an age limit
					benefitInd=listBenefitsShort[i];
				}
			}
			//Age limit for code group------------------------------------------------------------------------------------
			if(ProcedureCodes.GetCodeNumsForCodeGroupFixed(EnumCodeGroupFixed.Fluoride).Contains(ProcedureCodes.GetCodeNum(procCodeStr))) {
				Benefit benefitAgeLimit=listBenefitsShort.Find(x => IsFluorideAgeLimit(x));
				if(benefitAgeLimit!=null && benefitAgeLimit.Quantity > 0 && patientAge > benefitAgeLimit.Quantity) {
					note=Lans.g("Benefits","Age limitation:")+" "+benefitAgeLimit.Quantity.ToString();
					limitationTypeMet=LimitationTypeMet.Aging;
					return 0;//not covered if too old.
				}
			}
			if(ProcedureCodes.GetCodeNumsForCodeGroupFixed(EnumCodeGroupFixed.Sealant).Contains(ProcedureCodes.GetCodeNum(procCodeStr))) {
				Benefit benAgeLimit=listBenefitsShort.Find(x => IsSealantAgeLimit(x));
				if(benAgeLimit!=null && benAgeLimit.Quantity > 0 && patientAge > benAgeLimit.Quantity) {
					note=Lans.g("Benefits","Age limitation:")+" "+benAgeLimit.Quantity.ToString();
					limitationTypeMet=LimitationTypeMet.Aging;
					return 0;//not covered if too old.
				}
			}
			//look for the best matching family limitation----------------------------------------------------------------
			Benefit benefitFam=null;
			//start with no category
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel != BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum>0) {
					continue;
				}
				if(listBenefitsShort[i].CovCatNum==0) {
					if(!IsCodeInGeneralSpan(procCodeStr)) {
						continue;
					}
					//Leave this outside of the IsCodeInGeneralSpan check incase there is no general category.
					benefitFam=listBenefitsShort[i];
				}
			}
			//then, specific category.
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel != BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum>0) {
					continue;
				}
				if(listBenefitsShort[i].CovCatNum!=0) {
					//see if the span matches
					covSpanArrayForCat=CovSpans.GetForCat(listBenefitsShort[i].CovCatNum);
					bool isMatch=false;
					for(int j=0;j<covSpanArrayForCat.Length;j++) {
						if(String.Compare(procCodeStr,covSpanArrayForCat[j].FromCode)>=0 && String.Compare(procCodeStr,covSpanArrayForCat[j].ToCode)<=0) {
							isMatch=true;
							break;
						}
					}
					if(!isMatch) {
						continue;//no match
					}
					if(benefitFam != null && benefitFam.CovCatNum!=0) {//must compare
						//only use the new one if the item order is larger
						if(CovCats.GetOrderShort(listBenefitsShort[i].CovCatNum) > CovCats.GetOrderShort(benefitFam.CovCatNum)) {
							benefitFam=listBenefitsShort[i];
						}
					}
					else {//first one encountered for a category
						benefitFam=listBenefitsShort[i];
					}
				}
			}
			//then, specific code
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CoverageLevel != BenefitCoverageLevel.Family) {
					continue;
				}
				if(listBenefitsShort[i].CodeNum==0) {
					continue;
				}
				if(procCodeStr==ProcedureCodes.GetStringProcCode(listBenefitsShort[i].CodeNum)) {
					benefitFam=listBenefitsShort[i];
				}
			}
			//example. $1000 individual max, $3000 family max.
			//Only individual max makes sense as the starting point.								Only family max is now being considered as well.
			//Family max just limits the sum of individual maxes.										Family max may be the only cap being used.
			//If there is no individual limitation that matches, then return 0.     No longer valid. Return amount covered by ins, whether individual or family max.
			//fluoride age limit already handled, so all that's left is maximums.   ...
			if((benefitInd==null || benefitInd.MonetaryAmt==-1 || benefitInd.MonetaryAmt==0) && (benefitFam==null || benefitFam.MonetaryAmt==-1 || benefitFam.MonetaryAmt==0)){ 
			//if(benInd==null || benInd.MonetaryAmt==-1 || benInd.MonetaryAmt==0) {
				return insEstTotal;//no max found for this code.
			}
			double maxInd=0;
			if(benefitInd!=null) {
				maxInd=benefitInd.MonetaryAmt;
			}
			//reduce individual max by amount already paid this year/lifetime---------------------------------------------------
			//establish date range for procedures to consider
			DateTime dateStart=BenefitLogic.ComputeRenewDate(procDate,insPlan.MonthRenew);
			DateTime dateEnd=dateStart.AddYears(1).AddDays(-1);//Consider all claim procs with a ProcDate within one year of the renew date
			//Get deep copies of some cache classes so that we do not waste time in loops down below.
			if(benefitInd!=null) {
				if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
					dateStart=DateTime.MinValue;
				}
				for(int i=0;i<listClaimProcHists.Count;i++) {
					if(listClaimProcHists[i].InsSubNum != insSubNum) {
						continue;//different plan
					}
					if(listClaimProcHists[i].ProcDate<dateStart || listClaimProcHists[i].ProcDate>dateEnd) {
						continue;
					}
					if(listClaimProcHists[i].PatNum != patNum) {
						continue;//this is for someone else in the family //SHOULD PROBABLY NOT SKIP THIS IN THE CASE OF FAM BUT NO IND MAX. :(
					}
					if(benefitInd.CodeNum!=0) {//specific code
						//Enhance this later when code spans are supported.
						if(ProcedureCodes.GetStringProcCode(benefitInd.CodeNum)!=listClaimProcHists[i].StrProcCode) {
							continue;
						}
					}
					else if(benefitInd.CovCatNum!=0) {//specific category
						covSpanArrayForCat=CovSpans.GetForCat(benefitInd.CovCatNum);
						bool isMatch=false;
						if(listClaimProcHists[i].StrProcCode=="") {//If this was a 'total' payment that was not attached to a procedure
							if(CovCats.GetEbenCat(benefitInd.CovCatNum)==EbenefitCategory.General) {//And if this is the general category
								//Then it should affect this max.
								isMatch=true;
							}
						}
						else {//If the payment was attached to a proc, then the proc must be in the coderange of this annual max benefit
							for(int j=0;j<covSpanArrayForCat.Length;j++) {
								if(String.Compare(listClaimProcHists[i].StrProcCode,covSpanArrayForCat[j].FromCode)>=0 
								&& String.Compare(listClaimProcHists[i].StrProcCode,covSpanArrayForCat[j].ToCode)<=0) {
									isMatch=true;
									break;
								}
							}
						}
						if(!isMatch) {
							continue;
						}
					}
					//if no category, then benefits are not restricted by proc code.
					//In other words, the benefit applies to all codes.
					//At this point, we know that the proc in the loopList falls within this max benefit.
					//But it may also fall within a more restrictive benefit which would take precedence over this one.
					if(TighterLimitExists(listBenefitsShort,benefitInd,listClaimProcHists[i])) {
						continue;
					}
					maxInd-=listClaimProcHists[i].Amount;
				}
			}
			//reduce individual max by amount in loop ------------------------------------------------------------------
			if(benefitInd!=null) {
				for(int i=0;i<listClaimProcHistsLoop.Count;i++) {
					//no date restriction, since all TP or part of current claim
					//if(histList[i].ProcDate<dateStart || histList[i].ProcDate>dateEnd) {
					//	continue;
					//}
					if(listClaimProcHistsLoop[i].InsSubNum != insSubNum) {
						continue;//different plan.  Even the loop list can contain info for multiple plans.
					}
					if(listClaimProcHistsLoop[i].PatNum != patNum) {
						continue;//this is for someone else in the family
					}
					if(benefitInd.CodeNum!=0) {//specific code
						//Enhance this later when code spans are supported.
						if(ProcedureCodes.GetStringProcCode(benefitInd.CodeNum)!=listClaimProcHistsLoop[i].StrProcCode) {
							continue;
						}
					}
					else if(benefitInd.CovCatNum!=0) {//specific category
						covSpanArrayForCat=CovSpans.GetForCat(benefitInd.CovCatNum);
						bool isMatch=false;
						if(listClaimProcHistsLoop[i].StrProcCode=="") {//If this was a 'total' payment that was not attached to a procedure
							if(CovCats.GetEbenCat(benefitInd.CovCatNum)==EbenefitCategory.General) {//And if this is the general category
								//Then it should affect this max.
								isMatch=true;
							}
						}
						else {//If the payment was attached to a proc, then the proc must be in the coderange of this annual max benefit
							for(int j=0;j<covSpanArrayForCat.Length;j++) {
								if(String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayForCat[j].FromCode)>=0 
								&& String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayForCat[j].ToCode)<=0) {
									isMatch=true;
									break;
								}
							}
						}
						if(!isMatch) {
							continue;
						}
					}
					else {//if no category, then benefits are not normally restricted by proc code.
						//The problem is that if the amount in the loop is from an ortho proc, then the general category will exclude ortho.
						//But sometimes, the annual max is in the system as no category instead of general category.
						CovCat covCatGeneral=CovCats.GetForEbenCat(EbenefitCategory.General);
						if(covCatGeneral!=null) {//If there is a general category, then we only consider codes within it.  This is how we exclude ortho.
							CovSpan[] covSpanArray=CovSpans.GetForCat(covCatGeneral.CovCatNum);
							if(listClaimProcHistsLoop[i].StrProcCode!="" && !CovSpans.IsCodeInSpans(listClaimProcHistsLoop[i].StrProcCode,covSpanArray)) {//for example, ortho
								continue;
							}
						}
					}
					//At this point, we know that the proc in the loopList falls within this max benefit.
					//But it may also fall within a more restrictive benefit which would take precedence over this one.
					if(TighterLimitExists(listBenefitsShort,benefitInd,listClaimProcHistsLoop[i])) {
						continue;
					}
					maxInd-=listClaimProcHistsLoop[i].Amount;
				}
			}
			if(benefitInd!=null) {
				if(maxInd <= 0) {//then patient has used up all of their annual max, so no coverage.
					if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
						note+=Lans.g("Benefits","Over lifetime max");
					}
					else if(benefitInd.TimePeriod==BenefitTimePeriod.CalendarYear
						|| benefitInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
						note+=Lans.g("Benefits","Over annual max");
					}
					limitationTypeMet=LimitationTypeMet.PeriodMax;
					return 0;
				}
			}
			double retVal=insEstTotal;
			if(benefitInd!=null){
				if(maxInd < insEstTotal) {//if there's not enough left in the individual annual max to cover this proc.
					retVal=maxInd;//insurance will only cover up to the remaining annual max
				}
			}
			double insRemainingOverride=insEstTotalOverride;  //insEstTotalOverride or the amount ind remaining.  Same concept as retVal but never returned.
			if(benefitInd!=null) {
				if(maxInd < insEstTotalOverride) {//if there's not enough left in the individual annual max to cover this proc.
					insRemainingOverride=maxInd;//insurance will only cover up to the remaining annual max
				}
			}
			//Three situations:
			//1. Ind only. 
 			//Handled in the next 10 lines, then return.
			//
			//2. Ind and Fam max present.  
			//There seems to be enough to cover at least part of this procedure.
			//There may also be a family max that has been met which may partially or completely reduce coverage of this proc.
			//
			//3. Fam only.  We don't know how much is left.
			//maxInd=-1
			//benInd=null
			if(benefitFam==null || benefitFam.MonetaryAmt==-1) {//if no family max.  Ind only.
				//If there is an override, calculate annual max note from it instead
				if(insEstTotalOverride==-1 && retVal != insEstTotal){//and procedure is not fully covered by ind max
					if(benefitInd!=null) {//redundant
						if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
							note+=Lans.g("Benefits","Over lifetime max");
						}
						else if(benefitInd.TimePeriod==BenefitTimePeriod.CalendarYear
							|| benefitInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
							note+=Lans.g("Benefits","Over annual max");
						}
					}
					limitationTypeMet=LimitationTypeMet.PeriodMax;
				}
				else if(insEstTotalOverride!=-1 && insRemainingOverride != insEstTotalOverride) {
					if(benefitInd!=null) {//redundant
						if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
							note+=Lans.g("Benefits","Over lifetime max");
						}
						else if(benefitInd.TimePeriod==BenefitTimePeriod.CalendarYear
								|| benefitInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
							note+=Lans.g("Benefits","Over annual max");
						}
					}
					limitationTypeMet=LimitationTypeMet.PeriodMax;
				}
				return retVal;//no family max anyway, so no need to go further.
			}
			double maxFam=benefitFam.MonetaryAmt;
			//reduce the family max by amounts already used----------------------------------------------------------
			for(int i=0;i<listClaimProcHists.Count;i++) {
				if(listClaimProcHists[i].ProcDate<dateStart || listClaimProcHists[i].ProcDate>dateEnd) {
					continue;
				}
				if(listClaimProcHists[i].PlanNum != planNum) {
					continue;//different plan
				}
				//now, we do want to see all family members.
				//if(histList[i].PatNum != patNum) {
				//	continue;//this is for someone else in the family
				//}
				if(benefitFam.CodeNum!=0) {//specific code
					if(ProcedureCodes.GetStringProcCode(benefitFam.CodeNum)!=listClaimProcHists[i].StrProcCode) {
						continue;
					}
				}
				else if(benefitFam.CovCatNum!=0) {//specific category
					covSpanArrayForCat=CovSpans.GetForCat(benefitFam.CovCatNum);
					bool isMatch=false;
					if(listClaimProcHists[i].StrProcCode=="") {//If this was a 'total' payment that was not attached to a procedure
						if(CovCats.GetEbenCat(benefitFam.CovCatNum)==EbenefitCategory.General) {//And if this is the general category
							//Then it should affect this max.
							isMatch=true;
						}
					}
					else {
						for(int j=0;j<covSpanArrayForCat.Length;j++) {
							if(String.Compare(listClaimProcHists[i].StrProcCode,covSpanArrayForCat[j].FromCode)>=0 
							&& String.Compare(listClaimProcHists[i].StrProcCode,covSpanArrayForCat[j].ToCode)<=0) {
								isMatch=true;
								break;
							}
						}
					}
					if(!isMatch) {
						continue;
					}
				}
				//If the used family max is an umbrella max, 
				//don't subtract the estimates from procedures that are outside of the general coverage category 
				//StrProcCode can be blank for claimprocs with Status Adjustment
				if(benefitFam.CovCatNum==0 && !IsCodeInGeneralSpan(listClaimProcHists[i].StrProcCode) && listClaimProcHists[i].StrProcCode!="") {
					continue;
				}
				//if no category, then benefits are not restricted by proc code.
				maxFam-=listClaimProcHists[i].Amount;
			}
			//reduce family max by amounts already used in loop---------------------------------------------------------------
			for(int i=0;i<listClaimProcHistsLoop.Count;i++) {
				if(listClaimProcHistsLoop[i].PlanNum != planNum) {
					continue;//different plan
				}
				if(benefitFam.CodeNum!=0) {//specific code
					if(ProcedureCodes.GetStringProcCode(benefitFam.CodeNum)!=listClaimProcHistsLoop[i].StrProcCode) {
						continue;
					}
				}
				else if(benefitFam.CovCatNum!=0) {//specific category
					covSpanArrayForCat=CovSpans.GetForCat(benefitFam.CovCatNum);
					bool isMatch=false;
					if(listClaimProcHistsLoop[i].StrProcCode=="") {//If this was a 'total' payment that was not attached to a procedure
						if(CovCats.GetEbenCat(benefitFam.CovCatNum)==EbenefitCategory.General) {//And if this is the general category
							//Then it should affect this max.
							isMatch=true;
						}
					}
					else {
						for(int j=0;j<covSpanArrayForCat.Length;j++) {
							if(String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayForCat[j].FromCode)>=0 
							&& String.Compare(listClaimProcHistsLoop[i].StrProcCode,covSpanArrayForCat[j].ToCode)<=0) {
								isMatch=true;
								break;
							}
						}
					}
					if(!isMatch) {
						continue;
					}
				}
				//If the used family max is an umbrella max, 
				//don't subtract the estimates from procedures that are outside of the general coverage category 
				//StrProcCode can be blank for claimprocs with Status Adjustment
				if(benefitFam.CovCatNum==0 && !IsCodeInGeneralSpan(listClaimProcHistsLoop[i].StrProcCode) && listClaimProcHistsLoop[i].StrProcCode!="") {
					continue;
				}
				//if no category, then benefits are not restricted by proc code.
				maxFam-=listClaimProcHistsLoop[i].Amount;
			}
			//if the family max has all been used up on other procs 
			if(maxFam<=0) {
				if(benefitInd==null) {
					note+=Lans.g("Benefits","Over family max");
				}
				else{//and there is an individual max.
					if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
						note+=Lans.g("Benefits","Over family lifetime max");
					}
					else if(benefitInd.TimePeriod==BenefitTimePeriod.CalendarYear
					|| benefitInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
						note+=Lans.g("Benefits","Over family annual max");
					}
				}
				limitationTypeMet=LimitationTypeMet.FamilyPeriodMax;
				return 0;//then no coverage, regardless of what we computed for individual
			}
			//This section was causing a bug. I'm really not even sure what it was attempting to do, since it's common for family max to be greater than indiv max
			/*if(maxFam > maxInd) {//restrict by maxInd
				//which we already calculated
				if(benInd.TimePeriod==BenefitTimePeriod.Lifetime) {
					note+=Lans.g("Benefits","Over lifetime max");
				}
				else if(benInd.TimePeriod==BenefitTimePeriod.CalendarYear
					|| benInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
					note+=Lans.g("Benefits","Over annual max");
				}
				return retVal;
			}*/
			if((benefitInd==null) || (maxFam < maxInd)) {//restrict by maxFam
				if(insEstTotalOverride==-1) {//No insEstTotalOverride used, use normal insEstTotal
					if(maxFam < retVal) {//if there's not enough left in the annual max to cover this proc.
						//example. retVal=$70.  But 2970 of 3000 family max has been used.  maxFam=30.  We need to return 30.
						if(benefitInd==null) {
							note+=Lans.g("Benefits","Over family max");
						}
						else {//both ind and fam
							if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
								note+=Lans.g("Benefits","Over family lifetime max");
							}
							else if(benefitInd.TimePeriod==BenefitTimePeriod.CalendarYear
								|| benefitInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
								note+=Lans.g("Benefits","Over family annual max");
							}
						}
						limitationTypeMet=LimitationTypeMet.FamilyPeriodMax;
						return maxFam;//insurance will only cover up to the remaining annual max
					}
				}
				else {//If there is an override, calculate family max note from it instead
					//First calculate the note using override
					if(maxFam < insRemainingOverride) {//if there's not enough left in the annual max to cover this proc.
						//example. insRemainingOverride=$70.  But 2970 of 3000 family max has been used.  maxFam=30.
						if(benefitInd==null) {
							note+=Lans.g("Benefits","Over family max");
						}
						else {//both ind and fam
							if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
								note+=Lans.g("Benefits","Over family lifetime max");
							}
							else if(benefitInd.TimePeriod==BenefitTimePeriod.CalendarYear
								|| benefitInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
								note+=Lans.g("Benefits","Over family annual max");
							}
						}
						limitationTypeMet=LimitationTypeMet.FamilyPeriodMax;
					}
					//Now return the same amount you would have if there wasn't an override
					if(maxFam < retVal) {
						return maxFam;
					}
				}
			}
			if(insEstTotalOverride==-1) {//No insEstTotalOverride used, use normal insEstTotal
				if(retVal < insEstTotal) {//must have been an individual restriction
					if(benefitInd==null) {//js I don't understand this situation. It will probably not happen, but this is safe.
						note+=Lans.g("Benefits","Over annual max");
					}
					else {
						if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
							note+=Lans.g("Benefits","Over lifetime max");
						}
						else if(benefitInd.TimePeriod==BenefitTimePeriod.CalendarYear
							|| benefitInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
							note+=Lans.g("Benefits","Over annual max");
						}
					}
					limitationTypeMet=LimitationTypeMet.PeriodMax;
				}
			}
			//If there is an override, calculate max note from it instead.  Must have been an individual restriction.
			else if(insRemainingOverride < insEstTotalOverride) {
				if(benefitInd==null) {//js I don't understand this situation. It will probably not happen, but this is safe.
					note+=Lans.g("Benefits","Over annual max");
				}
				else {
					if(benefitInd.TimePeriod==BenefitTimePeriod.Lifetime) {
						note+=Lans.g("Benefits","Over lifetime max");
					}
					else if(benefitInd.TimePeriod==BenefitTimePeriod.CalendarYear
						|| benefitInd.TimePeriod==BenefitTimePeriod.ServiceYear) {
						note+=Lans.g("Benefits","Over annual max");
					}
				}
				limitationTypeMet=LimitationTypeMet.PeriodMax;
			}
			return retVal;
		}
		
		///<summary>Returns true if the passed in code is within the "General" insurance coverage category.</summary>
		private static bool IsCodeInGeneralSpan(string procCodeStr) {
			//No need to check MiddleTierRole; no call to db.
			bool retVal=true;
			CovCat covCatGen=CovCats.GetForEbenCat(EbenefitCategory.General);
			if(covCatGen!=null) {//Should always happen.  This would mean they are missing the General Coverage Category.
				CovSpan[] covSpanArrayForGeneral=CovSpans.GetForCat(covCatGen.CovCatNum);
				bool isMatch=false;
				for(int i=0;i<covSpanArrayForGeneral.Length;i++) {
					if(String.Compare(procCodeStr,covSpanArrayForGeneral[i].FromCode)>=0 && String.Compare(procCodeStr,covSpanArrayForGeneral[i].ToCode)<=0) {
						isMatch=true;
						break;
					}
				}
				if(isMatch) {
					retVal=true;
				}
				else {
					retVal=false;
					//no match for the code in the general category.  This will specifically happen for ortho codes, 
					//which should never consider general limitations (per Nathan, see TaskNum 807256)
				}
			}
			return retVal;
		}

		private static bool TighterLimitExists(List<Benefit> listBenefitsShort,Benefit benefit,ClaimProcHist claimProcHist) 
		{
			//No need to check MiddleTierRole; no call to db.
			if(claimProcHist.StrProcCode=="") {//If this was a 'total' payment that was not attached to a procedure
				//there won't be anything more restrictive.
				return false;
			}
			//The list is not in order by restrictive/broad, so tests will need to be done.
			if(benefit.CodeNum!=0) {//The benefit is already restrictive.  There isn't currently a way to have a more restrictive benefit, although in the future when code ranges are supported, a tighter code range would be more restrictive.
				return false;
			}
			for(int b=0;b<listBenefitsShort.Count;b++) {
				if(listBenefitsShort[b].BenefitNum==benefit.BenefitNum) {
					continue;//skip self.
				}
				if(listBenefitsShort[b].QuantityQualifier!=BenefitQuantity.None) {
					continue;//it must be some other kind of limitation other than an amount limit.  For example, BW frequency.
				}
				if(listBenefitsShort[b].CodeNum != 0) {
					long codeNum=0;
					if(ProcedureCodes.GetContainsKey(claimProcHist.StrProcCode)) {
						codeNum=ProcedureCodes.GetOne(claimProcHist.StrProcCode).CodeNum;
					}
					if(listBenefitsShort[b].CodeNum==codeNum) {//Enhance later for code ranges when supported by program
						return true;//a tighter limitation benefit exists for this specific procedure code.
					}
					continue;
				}
				if(listBenefitsShort[b].CovCatNum==0) {
					continue;
				}
				//specific category
				if(benefit.CovCatNum!=0) {//If benefit is a category limitation,
					//then we can only consider categories that are more restrictive (further down on list).
					//either of these could be -1 if isHidden
					int orderBenefit=CovCats.GetOrderShort(benefit.CovCatNum);
					int orderTest=CovCats.GetOrderShort(listBenefitsShort[b].CovCatNum);
					if(orderBenefit==-1) {
						//nothing to do here.  Treat it like a general limitation 
					}
					else if(orderTest<orderBenefit) {//the CovCat of listShort is further up in the list and less restrictive, so should not be considered.
						//this handles orderTest=-1, skipping the hidden category
						continue;
					}
				}
				else {//But if this is a general limitation,
					//then we don't need to do the above check because any match can be considered more restrictive.
				}
				//see if the claimProcHist is in this more restrictive category
				CovSpan[] covSpanArrayForCat=CovSpans.GetForCat(listBenefitsShort[b].CovCatNum);//get the spans 
				//This must be a payment that was attached to a proc, so test the proc to be in the coderange of this annual max benefit
				for(int j=0;j<covSpanArrayForCat.Length;j++) {
					if(String.Compare(claimProcHist.StrProcCode,covSpanArrayForCat[j].FromCode)>=0 
						&& String.Compare(claimProcHist.StrProcCode,covSpanArrayForCat[j].ToCode)<=0) 
					{
						return true;
					}
				}
			}
			return false;
		}

		///<summary>Only used from InsPlans.GetInsUsedDisplay.  If a procedure is handled by some limitation other than a general annual max, then we don't want it to count towards the annual max.</summary>
		public static bool LimitationExistsNotGeneral(List<Benefit> listBenefits,long planNum,long patPlanNum,string strProcCode) {
			EbenefitCategory ebenefitCategory;
			CovSpan[] covSpanArray;
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<listBenefits.Count;i++) {
				if(listBenefits[i].PlanNum==0 && listBenefits[i].PatPlanNum!=patPlanNum) {
					continue;
				}
				if(listBenefits[i].PatPlanNum==0 && listBenefits[i].PlanNum!=planNum) {
					continue;
				}
				if(listBenefits[i].BenefitType!=InsBenefitType.Limitations) {
					continue;
				}
				if(listBenefits[i].QuantityQualifier!=BenefitQuantity.None) {
					continue;
				}
				if(listBenefits[i].TimePeriod!=BenefitTimePeriod.CalendarYear 
					&& listBenefits[i].TimePeriod!=BenefitTimePeriod.ServiceYear
					&& listBenefits[i].TimePeriod!=BenefitTimePeriod.Lifetime)//allows ortho max to be caught further down 
				{
					continue;
				}
				//coverage level?
				if(listBenefits[i].CodeNum != 0) {
					if(listBenefits[i].CodeNum==ProcedureCodes.GetCodeNum(strProcCode)) {//Enhance later for code ranges when supported by program
						return true;//a limitation benefit exists for this specific procedure code.
					}
				}
				if(listBenefits[i].CovCatNum != 0) {
					ebenefitCategory=CovCats.GetEbenCat(listBenefits[i].CovCatNum);
					if(ebenefitCategory==EbenefitCategory.General || ebenefitCategory==EbenefitCategory.None) {
						continue;//ignore this general benefit
					}
					covSpanArray=CovSpans.GetForCat(listBenefits[i].CovCatNum);
					if(CovSpans.IsCodeInSpans(strProcCode,covSpanArray)) {
						return true;//a limitation benefit exists for a category that contains this procedure code.
					}
				}
			}
			return false;
		}

		///<summary>Used from ClaimProc.ComputeBaseEst and in sheet output. This is a low level function to get the percent to store in a claimproc.  It does not consider any percentOverride.  Always returns a number between 0 and 100.  Handles general, category, or procedure level.  Does not handle pat vs family coveragelevel.  Does handle patient override by using patplan.  Does not need to be aware of procedure history or loop history.</summary>
		public static int GetPercent(string procCodeStr,string planType,long planNum,long patPlanNum,List<Benefit> listBenefits) {
			//No need to check MiddleTierRole; no call to db.
			if(planType=="f" || planType=="c"){
				return 100;//flat and cap are always covered 100%
			}
			//first, create a much shorter list with only relevant benefits in it.
			List<Benefit> listBenefitsShort=new List<Benefit>();
			for(int i=0;i<listBenefits.Count;i++) {
				if(listBenefits[i].PlanNum==0 && listBenefits[i].PatPlanNum!=patPlanNum) {
					continue;
				}
				if(listBenefits[i].PatPlanNum==0 && listBenefits[i].PlanNum!=planNum) {
					continue;
				}
				if(listBenefits[i].BenefitType!=InsBenefitType.CoInsurance) {
					continue;
				}
				if(listBenefits[i].Percent == -1) {
					continue;
				}
				listBenefitsShort.Add(listBenefits[i]);
			}
			//Find the best benefit matches.
			//Plan and Pat here indicate patplan override and have nothing to do with pat vs family coverage level.
			Benefit benefitPlan=null;
			Benefit benefitPat=null;
			//start with no category
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CodeNum > 0) {
					continue;
				}
				if(listBenefitsShort[i].CovCatNum != 0) {
					continue;
				}
				if(listBenefitsShort[i].PatPlanNum !=0) {
					benefitPat=listBenefitsShort[i];
				}
				else {
					benefitPlan=listBenefitsShort[i];
				}
			}
			//then, specific category.
			CovSpan[] covSpanArrayForCat;
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CodeNum>0) {
					continue;
				}
				if(listBenefitsShort[i].CovCatNum==0) {
					continue;
				}
				//see if the span matches
				covSpanArrayForCat=CovSpans.GetForCat(listBenefitsShort[i].CovCatNum);
				bool isMatch=false;
				for(int j=0;j<covSpanArrayForCat.Length;j++) {
					if(String.Compare(procCodeStr,covSpanArrayForCat[j].FromCode)>=0 && String.Compare(procCodeStr,covSpanArrayForCat[j].ToCode)<=0) {
						isMatch=true;
						break;
					}
				}
				if(!isMatch) {
					continue;//no match
				}
				if(listBenefitsShort[i].PatPlanNum ==0) {
					if(benefitPlan != null && benefitPlan.CovCatNum!=0) {//must compare
						//only use the new one if the item order is larger
						if(CovCats.GetOrderShort(listBenefitsShort[i].CovCatNum) > CovCats.GetOrderShort(benefitPlan.CovCatNum)) {
							benefitPlan=listBenefitsShort[i];
						}
					}
					else {//first one encountered for a category
						benefitPlan=listBenefitsShort[i];
					}
					continue;
				}
				if(benefitPat != null && benefitPat.CovCatNum!=0) {//must compare
					//only use the new one if the item order is larger
					if(CovCats.GetOrderShort(listBenefitsShort[i].CovCatNum) > CovCats.GetOrderShort(benefitPat.CovCatNum)) {
						benefitPat=listBenefitsShort[i];
					}
				}
				else {//first one encountered for a category
					benefitPat=listBenefitsShort[i];
				}
			}
			//then, specific code
			for(int i=0;i<listBenefitsShort.Count;i++) {
				if(listBenefitsShort[i].CodeNum==0) {
					continue;
				}
				if(procCodeStr!=ProcedureCodes.GetStringProcCode(listBenefitsShort[i].CodeNum)) {
					continue;
				}
				if(listBenefits[i].PatPlanNum !=0) {
					benefitPat=listBenefitsShort[i];
				}
				else {
					benefitPlan=listBenefitsShort[i];
				}				
			}
			if(benefitPat != null) {
				return benefitPat.Percent;
			}
			if(benefitPlan != null) {
				return benefitPlan.Percent;
			}
			return 0;
		}

		public static List<Benefit> GetForPatPlansAndProcs(List<long> listPatPlanNums,List<long> listCodeNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Benefit>>(MethodBase.GetCurrentMethod(),listPatPlanNums,listCodeNums);
			}
			if(listPatPlanNums.Count==0 || listCodeNums.Count==0) {
				return new List<Benefit>();
			}
			string command="SELECT * FROM benefit WHERE PatPlanNum IN("+string.Join(",",listPatPlanNums)+") AND CodeNum IN("+string.Join(",",listCodeNums)+")";
			return Crud.BenefitCrud.SelectMany(command);
		}

		///<summary>Only used from ClaimProc.ComputeBaseEst. This is a low level function to determine if a given procedure is completely excluded from coverage.  It does not consider any dates of service or history.</summary>
		public static bool IsExcluded(string strProcCode,List<Benefit> listBenefits,long planNum,long patPlanNum) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<listBenefits.Count;i++) {
				if(listBenefits[i].PlanNum==0 && listBenefits[i].PatPlanNum!=patPlanNum) {
					continue;
				}
				if(listBenefits[i].PatPlanNum==0 && listBenefits[i].PlanNum!=planNum) {
					continue;
				}
				if(listBenefits[i].BenefitType!=InsBenefitType.Exclusions) {
					continue;
				}
				if(listBenefits[i].CodeNum > 0) {
					if(strProcCode==ProcedureCodes.GetStringProcCode(listBenefits[i].CodeNum)) {
						return true;//specific procedure code excluded
					}
					continue;
				}
				if(listBenefits[i].CovCatNum==0) {
					continue;
					//General exclusion with no category.  This is considered an unsupported type of benefit.
					//Nobody should be able to exclude everything with one benefit entry.
				}
				//see if the span matches
				CovSpan[] covSpanArrayForCat=CovSpans.GetForCat(listBenefits[i].CovCatNum);
				//bool isMatch=false;
				for(int j=0;j<covSpanArrayForCat.Length;j++) {
					if(String.Compare(strProcCode,covSpanArrayForCat[j].FromCode)>=0 && String.Compare(strProcCode,covSpanArrayForCat[j].ToCode)<=0) {
						return true;//span matches
					}
				}
			}
			return false;//no exclusions found for this code
		}

		///<summary>Used in FormInsPlan to sych database with changes user made to the benefit list for a plan.  
		///Must supply an old list for comparison.  Only the differences are saved.</summary>
		public static void UpdateList(List<Benefit> listBenefitsOld,List<Benefit> listBenefitsNew){
			//No need to check MiddleTierRole; no call to db.
			Benefit benefitNew;
			for(int i=0;i<listBenefitsOld.Count;i++){//loop through the old list
				benefitNew=null;
				for(int j=0;j<listBenefitsNew.Count;j++){
					if(listBenefitsNew[j]==null || listBenefitsNew[j].BenefitNum==0){
						continue;
					}
					if(listBenefitsOld[i].BenefitNum==listBenefitsNew[j].BenefitNum){
						benefitNew=listBenefitsNew[j];
						break;
					}
				}
				if(benefitNew==null){
					//benefit with matching benefitNum was not found, so it must have been deleted
					Delete(listBenefitsOld[i]);
					continue;
				}
				//benefit was found with matching benefitNum, so check for changes
				if(benefitNew.PlanNum != listBenefitsOld[i].PlanNum
					|| benefitNew.PatPlanNum != listBenefitsOld[i].PatPlanNum
					|| benefitNew.CovCatNum != listBenefitsOld[i].CovCatNum
					|| benefitNew.BenefitType != listBenefitsOld[i].BenefitType
					|| benefitNew.Percent != listBenefitsOld[i].Percent
					|| benefitNew.MonetaryAmt != listBenefitsOld[i].MonetaryAmt
					|| benefitNew.TimePeriod != listBenefitsOld[i].TimePeriod
					|| benefitNew.QuantityQualifier != listBenefitsOld[i].QuantityQualifier
					|| benefitNew.Quantity != listBenefitsOld[i].Quantity
					|| benefitNew.CodeNum != listBenefitsOld[i].CodeNum
					|| benefitNew.CoverageLevel != listBenefitsOld[i].CoverageLevel
					|| benefitNew.CodeGroupNum != listBenefitsOld[i].CodeGroupNum
					|| benefitNew.TreatArea != listBenefitsOld[i].TreatArea)
				{
					Benefits.Update(benefitNew,listBenefitsOld[i]); //logging taken care of in update
				}
			}
			for(int i=0;i<listBenefitsNew.Count;i++){//loop through the new list
				if(listBenefitsNew[i]==null){
					continue;	
				}
				if(listBenefitsNew[i].BenefitNum!=0){
					continue;
				}
				//benefit with benefitNum=0, so it's new
				Benefits.Insert(listBenefitsNew[i]); //logging taken care of in Insert method.
			}
		}

		///<summary>Used in family module display to get a list of benefits.  
		///The main purpose of this function is to group similar benefits for each plan on the same row, making it easier to display in a simple grid.  
		///Supply a list of all benefits for the patient, and the patPlans for the patient.</summary>
		public static Benefit[,] GetDisplayMatrix(List<Benefit> listBenefitsForPat,List<PatPlan> listPatPlans,List<InsSub> listInsSubs){
			//No need to check MiddleTierRole; no call to db.
			Benefit[] benefitArrayRow;//Closely related benefits, one column for each pat plan. Entries can be null.
			//Dictionary's value is a list of arrays of benefits that are closely related. The key is a benefit representative of the ones in the value.
			//Jordan-This dict is terrible. When it gets refactored some day, we will also
			//be able to improve the BenefitArraySorter and remove the ArrayList.
			Dictionary<Benefit,List<Benefit[]>> dictionaryBenefits=new Dictionary<Benefit,List<Benefit[]>>();
			InsSub insSub;
			for(int i=0;i<listBenefitsForPat.Count;i++) {
				if(CodeGroups.IsHidden(listBenefitsForPat[i].CodeGroupNum)) {
					continue;
				}
				for(int j=0;j<listPatPlans.Count;j++) {//loop through columns
					insSub=InsSubs.GetSub(listPatPlans[j].InsSubNum,listInsSubs);
					if(listPatPlans[j].PatPlanNum!=listBenefitsForPat[i].PatPlanNum
						&& insSub.PlanNum!=listBenefitsForPat[i].PlanNum) 
					{
						continue;//Benefit doesn't apply to this column
					}
					List<Benefit[]>  listBenefitArrayRows=null;
					dictionaryBenefits.TryGetValue(listBenefitsForPat[i],out listBenefitArrayRows);
					//if no matching type found, add a row
					if(listBenefitArrayRows==null || listBenefitArrayRows.Count==0) {
						benefitArrayRow=new Benefit[listPatPlans.Count];
						benefitArrayRow[j]=listBenefitsForPat[i].Copy();
						dictionaryBenefits.Add(listBenefitsForPat[i].Copy(),new List<Benefit[]> { benefitArrayRow });
						continue;
					}
					//if the column for the matching row is null, then use that row
					if(listBenefitArrayRows[0][j]==null) {
						listBenefitArrayRows[0][j]=listBenefitsForPat[i].Copy();
						continue;
					}
					//if not null, then add another row.
					benefitArrayRow=new Benefit[listPatPlans.Count];
					benefitArrayRow[j]=listBenefitsForPat[i].Copy();
					dictionaryBenefits[listBenefitsForPat[i]].Add(benefitArrayRow);
				}
			}
			ArrayList arrayList=new ArrayList();//each object is a Benefit[]
			foreach(Benefit[] arrBen in dictionaryBenefits.SelectMany(x => x.Value)) {
				arrayList.Add(arrBen);
			}
			IComparer iComparer = new BenefitArraySorter();
			arrayList.Sort(iComparer);
			Benefit[,] retVal=new Benefit[listPatPlans.Count,arrayList.Count];
			for(int y=0;y<arrayList.Count;y++){
				for(int x=0;x<listPatPlans.Count;x++){
					if(((Benefit[])arrayList[y])[x]!=null){
						retVal[x,y]=((Benefit[])arrayList[y])[x].Copy();
					}
				}
			}
			return retVal;
		}

		///<summary>Deletes all benefits for a plan from the database.  Only used in FormInsPlan when picking a plan from the list.  
		///Need to clear out benefits so that they won't be picked up when choosing benefits for all.</summary>
		public static void DeleteForPlan(long planNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planNum);
				return;
			}
			string command="SELECT * FROM benefit WHERE PlanNum="+POut.Long(planNum);
			List<Benefit> listBenefits = Crud.BenefitCrud.SelectMany(command);
			command ="DELETE FROM benefit WHERE PlanNum="+POut.Long(planNum);
			Db.NonQ(command);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			for(int i=0;0<listBenefits.Count;i++){
				InsEditLogs.MakeLogEntry(null,listBenefits[i],InsEditLogType.Benefit,Security.CurUser.UserNum);
			}
		}

		///<summary>Get the string for displaying the frequency for the specified type for the specified plan (primary, secondary).</summary>
		public static string GetFrequencyDisplay(FrequencyType frequencyType,List<Benefit> listBenefits, long planNum) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<listBenefits.Count;i++){
				if(listBenefits[i].PlanNum!=planNum) {
					continue;
				}
				switch(frequencyType) {
					case FrequencyType.BW:
						if(IsBitewingFrequency(listBenefits[i])) {
							return GetFrequencyString(listBenefits[i]);
						}
						continue;
					case FrequencyType.PanoFMX:
						if(IsPanoFrequency(listBenefits[i])) {
							return GetFrequencyString(listBenefits[i]);
						}
						continue;
					case FrequencyType.Exam:
						if(IsExamFrequency(listBenefits[i])) {
							return GetFrequencyString(listBenefits[i]);
						}
						continue;
				}//end switch
			}//end i loop
			return "";
		}

		///<summary>Gets the a string like "Once every 6 months" for the frequency benefit.</summary>
		private static string GetFrequencyString(Benefit benefit) {
			//No need to check MiddleTierRole; no call to db.
			string retVal="";
			if(benefit.QuantityQualifier==BenefitQuantity.Months) {
				if(benefit.Quantity==1) {
					retVal+=Lans.g("Benefits","Once every month.")+"\r\n";
				}
				else {
					retVal+=Lans.g("Benefits","Once every")+" "+benefit.Quantity+" "+Lans.g("Benefits","months.")+"\r\n";
				}
			}
			else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
				if(benefit.Quantity==1) {
					retVal+=Lans.g("Benefits","Once every year.")+"\r\n";
				}
				else {
					retVal+=Lans.g("Benefits","Once every")+" "+benefit.Quantity+" "+Lans.g("Benefits","years.")+"\r\n";
				}
			}
			else if(benefit.QuantityQualifier==BenefitQuantity.NumberOfServices
				&& benefit.TimePeriod==BenefitTimePeriod.NumberInLast12Months)
			{
				if(benefit.Quantity==1) {
					retVal+=Lans.g("Benefits","Once in the last 12 months.")+"\r\n";
				}
				else {
					retVal+=benefit.Quantity+" "+Lans.g("Benefits","times in the last 12 months.")+"\r\n";
				}
			}
			else {//number of services
				if(benefit.Quantity==1) {
					retVal+=Lans.g("Benefits","Once per year.")+"\r\n";
				}
				else {
					retVal+=benefit.Quantity+" "+Lans.g("Benefits","times per year.")+"\r\n";
				}
			}
			return retVal;
		}

		///<summary>Gets the string that displays in the "Category" column of the benefits table in FormInsPlans. 
		///Pass in a list of CovCats to not make multiple deep copies of the cache.</summary>
		public static string GetCategoryString(Benefit benefit) {
			//No need to check MiddleTierRole; no call to db.
			string retVal = "";
			if(IsFluorideAgeLimit(benefit)) {
				retVal=Lans.g("benefitCategory","Fluoride");
			}
			else if(IsSealantAgeLimit(benefit)) {
				retVal=Lans.g("benefitCategory","Sealants");
			}
			else if(benefit.CodeGroupNum!=0) {
				retVal=CodeGroups.GetGroupName(benefit.CodeGroupNum);
			}
			else if(benefit.CovCatNum!=0) {
				retVal=CovCats.GetDesc(benefit.CovCatNum);
			}
			else {
				ProcedureCode proccode=ProcedureCodes.GetProcCode(benefit.CodeNum);
				retVal=proccode.ProcCode+"-"+proccode.AbbrDesc;
			}
			return retVal;
		}

		public static bool IsFrequencyLimitationForCodeGroupFixed(Benefit benefit,EnumCodeGroupFixed enumCodeGroupFixed,long patPlanNum=0,bool isQuantityRequired=true) {
			//No need to check MiddleTierRole; no call to db.
			CodeGroup codeGroup=CodeGroups.GetOneForCodeGroupFixed(enumCodeGroupFixed);
			//If there is no valid code group then the benefit is not a frequency limitation.
			if(codeGroup==null) {
				return false;
			}
			List<long> listCodeNums=ProcedureCodes.GetCodeNumsForProcCodes(codeGroup.ProcCodes);
			if(benefit.CodeGroupNum!=codeGroup.CodeGroupNum && !listCodeNums.Contains(benefit.CodeNum)) {
				//The benefit is not associated with the code group itself nor any of the procedures within said code group.
				return false;
			}
			//PatPlanNum either needs to be 0 (which is always allowed) or it needs to exactly match the PatPlanNum passed in.
			if(benefit.PatPlanNum > 0 && benefit.PatPlanNum!=patPlanNum) {
				return false;
			}
			if(!IsFrequencyLimitation(benefit,isQuantityRequired:isQuantityRequired)) {
				return false;
			}
			return true;
		}

		public static bool IsFrequencyLimitation(Benefit benefit,bool isQuantityRequired=false) {
			//No need to check MiddleTierRole; no call to db.
			//Frequency limitations follow the following rules:
			//1. BenefitType is InsBenefitType.Limitations
			//2. MonetaryAmt is -1
			//3. Percent is -1
			//4. QuantityQualifier is Months or Years while TimePeriod is None
			//   or
			//   QuantityQualifier is NumberOfServices while TimePeriod is ServiceYear, or CalendarYear, or NumberInLast12Months
			//5. CoverageLevel is None
			//6. (optional) Require that Quantity is not 0
			//Note that we do not test whether it's using a CodeGroup rather than a CodeNum or CovCatNum.
			//This is because frequency limitations will allow those other two.
			//We strongly discourage this in the UI by hiding those, but users could manually add them, or there could be old clutter.
			//Also, we're not really sure if they would even work and function properly as frequency limitations.
			//But they will show in FormBenefitFrequencies instead of FormInsBenefits.
			//If they show in FormBenefitFrequencies, we will do additional testing to treat CodeNum and CovCatNum differently.
			if(benefit==null) {
				return false;
			}
			if(benefit.BenefitType!=InsBenefitType.Limitations) {
				return false;
			}
			if(benefit.MonetaryAmt!=-1) {
				return false;
			}
			if(benefit.Percent!=-1) {
				return false;
			}
			if(benefit.QuantityQualifier.In(BenefitQuantity.Months,BenefitQuantity.Years)) {
				if(benefit.TimePeriod!=BenefitTimePeriod.None) {
					return false;
				}
			}
			else if(benefit.QuantityQualifier.In(BenefitQuantity.NumberOfServices)) {
				if(!benefit.TimePeriod.In(BenefitTimePeriod.ServiceYear,BenefitTimePeriod.CalendarYear,BenefitTimePeriod.NumberInLast12Months)) {
					return false;
				}
			}
			else {
				//All other qualifier and time period combinations are not ones made by the program, but custom ones made by the user.
				return false;
			}
			if(benefit.CoverageLevel!=BenefitCoverageLevel.None) {
				return false;
			}
			if(isQuantityRequired && benefit.Quantity==0) {
				return false;
			}
			return true;
		}

		///<summary>Returns true if this benefit is for fluoride age limitation.</summary>
		public static bool IsFluorideAgeLimit(Benefit benefit) {
			//No need to check MiddleTierRole; no call to db.
			CodeGroup codeGroup=CodeGroups.GetOneForCodeGroupFixed(EnumCodeGroupFixed.Fluoride);
			if(codeGroup==null) {
				return false;
			}
			List<long> listCodeNums=ProcedureCodes.GetCodeNumsForProcCodes(codeGroup.ProcCodes);
			if((benefit.CodeGroupNum==codeGroup.CodeGroupNum || listCodeNums.Contains(benefit.CodeNum))
				&& benefit.BenefitType==InsBenefitType.Limitations
				//&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Db).CovCatNum//ignored
				&& benefit.MonetaryAmt==-1
				&& benefit.PatPlanNum==0
				&& benefit.Percent==-1
				&& benefit.QuantityQualifier==BenefitQuantity.AgeLimit
				&& benefit.CoverageLevel==BenefitCoverageLevel.None)
				//&& ben.TimePeriod might be none or calYear, or ServiceYear.
			{
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for sealant age limitation.</summary>
		public static bool IsSealantAgeLimit(Benefit benefit) {
			//No need to check MiddleTierRole; no call to db.
			CodeGroup codeGroup=CodeGroups.GetOneForCodeGroupFixed(EnumCodeGroupFixed.Sealant);
			if(codeGroup==null) {
				return false;
			}
			List<long> listCodeNums=ProcedureCodes.GetCodeNumsForProcCodes(codeGroup.ProcCodes);
			if((benefit.CodeGroupNum==codeGroup.CodeGroupNum || listCodeNums.Contains(benefit.CodeNum))
				&& benefit.BenefitType==InsBenefitType.Limitations
				&& benefit.MonetaryAmt==-1
				&& benefit.PatPlanNum==0
				&& benefit.Percent==-1
				&& benefit.QuantityQualifier==BenefitQuantity.AgeLimit
				&& benefit.CoverageLevel==BenefitCoverageLevel.None)
			{
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for bitewing frequency.</summary>
		public static bool IsBitewingFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.BW,patPlanNum:patPlanNum,isQuantityRequired:false)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for pano frequency.</summary>
		public static bool IsPanoFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.PanoFMX,patPlanNum:patPlanNum,isQuantityRequired:false)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for exam frequency.</summary>
		public static bool IsExamFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.Exam,patPlanNum:patPlanNum,isQuantityRequired:false)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for prophy frequency.</summary>
		public static bool IsProphyFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.Prophy,patPlanNum:patPlanNum)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for fluoride frequency.</summary>
		public static bool IsFluorideFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.Fluoride,patPlanNum:patPlanNum)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for sealant frequency.</summary>
		public static bool IsSealantFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.Sealant,patPlanNum:patPlanNum)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for SRP frequency.</summary>
		public static bool IsSRPFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.SRP,patPlanNum:patPlanNum)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for full mouth debridement frequency.</summary>
		public static bool IsFullDebridementFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.FMDebride,patPlanNum:patPlanNum)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if this benefit is for perio maintenance frequency.</summary>
		public static bool IsPerioMaintFrequency(Benefit benefit,long patPlanNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(IsFrequencyLimitationForCodeGroupFixed(benefit,EnumCodeGroupFixed.Perio,patPlanNum:patPlanNum)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if the given benefit represents an annual max benefit. Can be either family or individual based on coverageLevel passed in.</summary>
		public static bool IsAnnualMax(Benefit benefit,BenefitCoverageLevel benefitCoverageLevel) {
			//No need to check MiddleTierRole; no call to db.
			if(benefit.CodeNum==0
				&& benefit.BenefitType==InsBenefitType.Limitations
				&& (benefit.CovCatNum==0
				|| benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum)
				&& benefit.PatPlanNum==0
				&& benefit.Quantity==0
				&& benefit.QuantityQualifier==BenefitQuantity.None
				&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear || benefit.TimePeriod==BenefitTimePeriod.ServiceYear)
				&& benefit.CoverageLevel==benefitCoverageLevel) 
			{
				return true;
			}
			return false;
		}

		///<summary>Returns true if the given benefit represents a general deductible benefit. Can be either family or individual based on coverageLevel passed in.</summary>
		public static bool IsGeneralDeductible(Benefit benefit,BenefitCoverageLevel benefitCoverageLevel) {
			//No need to check MiddleTierRole; no call to db.
			if(benefit.CodeNum==0
				&& benefit.BenefitType==InsBenefitType.Deductible
				&& (benefit.CovCatNum==0
				|| benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum)
				&& benefit.PatPlanNum==0
				&& benefit.Quantity==0
				&& benefit.QuantityQualifier==BenefitQuantity.None
				&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear || benefit.TimePeriod==BenefitTimePeriod.ServiceYear)
				&& benefit.CoverageLevel==benefitCoverageLevel) 
			{
				return true;
			}
			return false;
		}

		public static Benefit CreateFrequencyBenefit(EbenefitCategory ebenefitCategory,byte byteQty,BenefitQuantity benefitQuantityQualifier,long planNum,
			BenefitTimePeriod benefitTimePeriod) 
		{
			//No need to check MiddleTierRole; no call to db.
			Benefit benefit = new Benefit();
			benefit.BenefitType=InsBenefitType.Limitations;
			benefit.CodeNum=0;
			benefit.CovCatNum=CovCats.GetForEbenCat(ebenefitCategory)?.CovCatNum??0;
			benefit.MonetaryAmt=-1;
			benefit.Percent=-1;
			benefit.PlanNum=planNum;
			benefit.Quantity=byteQty;
			benefit.QuantityQualifier=benefitQuantityQualifier;
			//Only NumberOfServices uses serviceyear, calendaryear, or NumberInLast12Months time periods.  The others use None.
			benefit.TimePeriod=BenefitTimePeriod.None;
			if(benefitQuantityQualifier==BenefitQuantity.NumberOfServices){
				benefit.TimePeriod=benefitTimePeriod;
			}
			return benefit;
		}

		///<summary>Creates a Limitation benefit for the given quantity.</summary>
		public static Benefit CreateFrequencyBenefit(long codeNum,byte quantity,BenefitQuantity benefitQuantityQualifier,long planNum,
			BenefitTimePeriod benefitTimePeriod,TreatmentArea treatArea=TreatmentArea.None) 
		{
			//No need to check MiddleTierRole; no call to db.
			Benefit benefit = new Benefit();
			benefit.BenefitType=InsBenefitType.Limitations;
			benefit.CodeNum=codeNum;
			benefit.CovCatNum=0;
			benefit.MonetaryAmt=-1;
			benefit.Percent=-1;
			benefit.PlanNum=planNum;
			benefit.Quantity=quantity;
			benefit.QuantityQualifier=benefitQuantityQualifier;
			//Only NumberOfServices uses serviceyear, calendaryear, or NumberInLast12Months time periods.  The others use None.
			benefit.TimePeriod=BenefitTimePeriod.None;
			if(benefitQuantityQualifier==BenefitQuantity.NumberOfServices){
				benefit.TimePeriod=benefitTimePeriod;
			}
			benefit.TreatArea=treatArea;
			return benefit;
		}

		///<summary>Creates an AgeLimit benefit for the given quantity.</summary>
		public static Benefit CreateAgeLimitationBenefit(long codeGroupNum,byte quantity,long planNum) {
			//No need to check MiddleTierRole; no call to db.
			Benefit benefit = new Benefit();
			benefit.BenefitType=InsBenefitType.Limitations;
			benefit.CodeGroupNum=codeGroupNum;
			benefit.CovCatNum=0;
			benefit.MonetaryAmt=-1;
			benefit.Percent=-1;
			benefit.PlanNum=planNum;
			benefit.Quantity=quantity;
			benefit.QuantityQualifier=BenefitQuantity.AgeLimit;
			benefit.TimePeriod=BenefitTimePeriod.None;
			return benefit;
		}

		///<summary>Gets all Benefits for a passed in InsPlan or PatPlan from the database. Returns empty list if not found.</summary>
		public static List<Benefit> GetBenefitsForApi(int limit,int offset,long planNum,long patPlanNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Benefit>>(MethodBase.GetCurrentMethod(),limit,offset,planNum,patPlanNum);
			}
			string command="SELECT * FROM benefit ";
			if(planNum>0) {
				command+="WHERE PlanNum="+POut.Long(planNum)+" ";
			}
			if(patPlanNum>0) {
				command+="WHERE PatPlanNum="+POut.Long(patPlanNum)+" ";
			}
			command+="ORDER BY BenefitNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.BenefitCrud.SelectMany(command);
		}

		///<summary>Gets a Benefit from the database by BenefitNum for the API. Returns null if not found.</summary>
		public static Benefit GetBenefitForApi(long benefitNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Benefit>(MethodBase.GetCurrentMethod(),benefitNum);
			}
			string command="SELECT * FROM benefit WHERE BenefitNum="+POut.Long(benefitNum)+" ";
			return Crud.BenefitCrud.SelectOne(command);	
		}

	}

	public enum FrequencyType {
		BW,
		PanoFMX,
		Exam
	}













}










