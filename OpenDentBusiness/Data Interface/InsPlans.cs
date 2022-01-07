using CodeBase;
using DataConnectionBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace OpenDentBusiness {
	///<summary></summary>
	public class InsPlans {
		///<summary>Also fills PlanNum from db.</summary>
		public static long Insert(InsPlan plan) {
			return Insert(plan,false);
		}
		
		///<summary>Also fills PlanNum from db.</summary>
		public static long Insert(InsPlan plan,bool useExistingPK) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				plan.PlanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),plan,useExistingPK);
				return plan.PlanNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			plan.SecUserNumEntry=Security.CurUser.UserNum;
			long planNum=0;
			InsPlan planOld=plan.Copy();
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				planNum=Crud.InsPlanCrud.Insert(plan);//Oracle ALWAYS uses existing PKs because they do not support auto-incrementing.
			}
			else {
				planNum=Crud.InsPlanCrud.Insert(plan,useExistingPK);
			}
			if(planOld.PlanNum==0) {
				InsEditLogs.MakeLogEntry(plan,null,InsEditLogType.InsPlan,plan.SecUserNumEntry);
			}
			else {
				InsEditLogs.MakeLogEntry(plan,planOld,InsEditLogType.InsPlan,plan.SecUserNumEntry);
			}
			InsVerifies.InsertForPlanNum(planNum);
			return planNum;
		}

		///<summary>Pass in the old InsPlan to avoid querying the db for it.</summary>
		public static void Update(InsPlan plan,InsPlan planOld=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),plan,planOld);
				return;
			}
			if(planOld==null) {
				planOld=InsPlans.RefreshOne(plan.PlanNum);
			}
			Crud.InsPlanCrud.Update(plan,planOld);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			InsEditLogs.MakeLogEntry(plan,planOld,InsEditLogType.InsPlan,Security.CurUser.UserNum);
		}

		///<summary>It's fastest if you supply a plan list that contains the plan, but it also works just fine if it can't initally locate the plan in the
		///list.  You can supply a list of length 0 or null.  If not in the list, retrieves from db.  Returns null if planNum is 0 or if it cannot find the insplan from the db.</summary>
		public static InsPlan GetPlan(long planNum,List<InsPlan> planList) {
			//No need to check RemotingRole; no call to db.
			if(planNum==0) {
				return null;
			}
			if(planList==null) {
				planList=new List<InsPlan>();
			}
			//LastOrDefault to preserve old behavior. No other reason.
			return planList.LastOrDefault(x => x.PlanNum==planNum)??RefreshOne(planNum);
		}

		///<summary>Gets a list of plans from the database.</summary>
		public static List<InsPlan> GetPlans(List<long> listPlanNums) {
			if(listPlanNums==null || listPlanNums.Count==0) {
				return new List<InsPlan>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsPlan>>(MethodBase.GetCurrentMethod(),listPlanNums);
			}
			string command="SELECT * FROM insplan WHERE PlanNum IN ("+string.Join(",",listPlanNums)+")";
			return Crud.InsPlanCrud.SelectMany(command);
		}

		/*
		///<summary>Will return null if no active plan for that ordinal.  Ordinal means primary, secondary, etc.</summary>
		public static InsPlan GetPlanByOrdinal(int patNum,int ordinal) {
			string command="SELECT * FROM insplan WHERE EXISTS "
				+"(SELECT * FROM patplan WHERE insplan.PlanNum=patplan.PlanNum "
				+"AND patplan.PatNum="+POut.PInt(patNum)
				+" AND patplan.Ordinal="+POut.PInt(ordinal);
			//num = '"+planNum+"'";
		}*/

		public static InsPlan[] GetByTrojanID(string trojanID) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<InsPlan[]>(MethodBase.GetCurrentMethod(),trojanID);
			} 
			string command="SELECT * FROM insplan WHERE TrojanID = '"+POut.String(trojanID)+"'";
			return Crud.InsPlanCrud.SelectMany(command).ToArray();
		}

		///<summary>Only loads one plan from db. Can return null.</summary>
		public static InsPlan RefreshOne(long planNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<InsPlan>(MethodBase.GetCurrentMethod(),planNum);
			} 
			if(planNum==0){
				return null;
			}
			string command="SELECT * FROM insplan WHERE plannum = '"+planNum+"'";
			return Crud.InsPlanCrud.SelectOne(command);
		}

		//<summary>Deprecated.  Instead, use RefreshForSubList.</summary>
		//public static List<InsPlan> RefreshForFam(){//Family Fam) {
		//	return null;
		//}

		///<summary>Gets List of plans based on the subList.  The list won't be in the same order.</summary>
		public static List<InsPlan> RefreshForSubList(List<InsSub> listInsSubs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsPlan>>(MethodBase.GetCurrentMethod(),listInsSubs);
			}
			if(listInsSubs==null || listInsSubs.Count==0) {
				return new List<InsPlan>();
			}
			string command="SELECT * FROM insplan WHERE PlanNum IN("+string.Join(",",listInsSubs.Select(x => POut.Long(x.PlanNum)))+")";
			return Crud.InsPlanCrud.SelectMany(command);
		}

		///<summary>Tests all fields for equality.</summary>
		public static bool AreEqualValue(InsPlan plan1,InsPlan plan2) {
			if(plan1.PlanNum==plan2.PlanNum
				&& plan1.GroupName==plan2.GroupName
				&& plan1.GroupNum==plan2.GroupNum
				&& plan1.PlanNote==plan2.PlanNote
				&& plan1.FeeSched==plan2.FeeSched
				&& plan1.PlanType==plan2.PlanType
				&& plan1.ClaimFormNum==plan2.ClaimFormNum
				&& plan1.UseAltCode==plan2.UseAltCode
				&& plan1.ClaimsUseUCR==plan2.ClaimsUseUCR
				&& plan1.CopayFeeSched==plan2.CopayFeeSched
				&& plan1.EmployerNum==plan2.EmployerNum
				&& plan1.CarrierNum==plan2.CarrierNum
				&& plan1.AllowedFeeSched==plan2.AllowedFeeSched
				&& plan1.ManualFeeSchedNum==plan2.ManualFeeSchedNum
				&& plan1.TrojanID==plan2.TrojanID
				&& plan1.DivisionNo==plan2.DivisionNo
				&& plan1.IsMedical==plan2.IsMedical
				&& plan1.FilingCode==plan2.FilingCode
				&& plan1.DentaideCardSequence==plan2.DentaideCardSequence
				&& plan1.ShowBaseUnits==plan2.ShowBaseUnits
				&& plan1.CodeSubstNone==plan2.CodeSubstNone
				&& plan1.IsHidden==plan2.IsHidden
				&& plan1.MonthRenew==plan2.MonthRenew
				&& plan1.FilingCodeSubtype==plan2.FilingCodeSubtype
				&& plan1.CanadianPlanFlag==plan2.CanadianPlanFlag
				&& plan1.CobRule==plan2.CobRule
				&& plan1.HideFromVerifyList==plan2.HideFromVerifyList
				&& plan1.OrthoType==plan2.OrthoType
				&& plan1.OrthoAutoProcCodeNumOverride==plan2.OrthoAutoProcCodeNumOverride
				&& plan1.OrthoAutoProcFreq==plan2.OrthoAutoProcFreq
				&& plan1.OrthoAutoClaimDaysWait==plan2.OrthoAutoClaimDaysWait
				&& plan1.OrthoAutoFeeBilled==plan2.OrthoAutoFeeBilled
				&& plan1.BillingType==plan2.BillingType
				&& plan1.HasPpoSubstWriteoffs==plan2.HasPpoSubstWriteoffs
				&& plan1.ExclusionFeeRule==plan2.ExclusionFeeRule
				&& plan1.IsBlueBookEnabled==plan2.IsBlueBookEnabled)
				//When adding a field here, send a task to Web Enhancements so they can update Insurance Plan Information Fields with changes that trigger
				//a new plan.
			{
				return true;
			}
			return false;
		}

		///<summary>Gets all insurance plans where the feeSched or copayFeeSched is equal to feeSchedNum</summary>
		public static List<InsPlan> GetForFeeSchedNum(long feeSchedNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsPlan>>(MethodBase.GetCurrentMethod(),feeSchedNum);
			}
			string command = "SELECT * FROM insplan WHERE insplan.FeeSched = " + POut.Long(feeSchedNum)+" OR insplan.CopayFeeSched="+POut.Long(feeSchedNum);
			return Crud.InsPlanCrud.SelectMany(command);
		}

		/*
		///<summary>Called from FormInsPlan when applying changes to all identical insurance plans. This updates the synchronized fields for all plans like the specified insPlan.  Current InsPlan must be set to the new values that we want.  BenefitNotes and SubscNote are specific to subscriber and are not changed.  PlanNotes are handled separately in a different function after this one is complete.</summary>
		public static void UpdateForLike(InsPlan like, InsPlan plan) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),like,plan);
				return;
			}
			string command= "UPDATE insplan SET "
				+"EmployerNum = '"     +POut.Long   (plan.EmployerNum)+"'"
				+",GroupName = '"      +POut.String(plan.GroupName)+"'"
				+",GroupNum = '"       +POut.String(plan.GroupNum)+"'"
				+",DivisionNo = '"     +POut.String(plan.DivisionNo)+"'"
				+",CarrierNum = '"     +POut.Long   (plan.CarrierNum)+"'"
				+",PlanType = '"       +POut.String(plan.PlanType)+"'"
				+",UseAltCode = '"     +POut.Bool  (plan.UseAltCode)+"'"
				+",IsMedical = '"      +POut.Bool  (plan.IsMedical)+"'"
				+",ClaimsUseUCR = '"   +POut.Bool  (plan.ClaimsUseUCR)+"'"
				+",FeeSched = '"       +POut.Long   (plan.FeeSched)+"'"
				+",CopayFeeSched = '"  +POut.Long   (plan.CopayFeeSched)+"'"
				+",ClaimFormNum = '"   +POut.Long   (plan.ClaimFormNum)+"'"
				+",AllowedFeeSched= '" +POut.Long   (plan.AllowedFeeSched)+"'"
				+",TrojanID = '"       +POut.String(plan.TrojanID)+"'"
				+",FilingCode = '"     +POut.Long   (plan.FilingCode)+"'"
				+",FilingCodeSubtype = '"+POut.Long(plan.FilingCodeSubtype)+"'"
				+",ShowBaseUnits = '"  +POut.Bool  (plan.ShowBaseUnits)+"'"
				//+",DedBeforePerc = '"  +POut.PBool  (plan.DedBeforePerc)+"'"
				+",CodeSubstNone='"    +POut.Bool  (plan.CodeSubstNone)+"'"
				+",IsHidden='"         +POut.Bool  (plan.IsHidden)+"'"
				+",MonthRenew='"       +POut.Int   (plan.MonthRenew)+"'"
				//It is most likely that MonthRenew would be the same for everyone on the same plan.  If we get complaints, we might have to add an option.
				+" WHERE "
				+"EmployerNum = '"        +POut.Long   (like.EmployerNum)+"' "
				+"AND GroupName = '"      +POut.String(like.GroupName)+"' "
				+"AND GroupNum = '"       +POut.String(like.GroupNum)+"' "
				+"AND DivisionNo = '"     +POut.String(like.DivisionNo)+"'"
				+"AND CarrierNum = '"     +POut.Long   (like.CarrierNum)+"' "
				+"AND IsMedical = '"      +POut.Bool  (like.IsMedical)+"'";
			Db.NonQ(command);
		}*/

		///<summary>Gets a description of the specified plan, including carrier name and subscriber. It's fastest if you supply a plan list that contains the plan, but it also works just fine if it can't initally locate the plan in the list.  You can supply an array of length 0 for both family and planlist.</summary>
		public static string GetDescript(long planNum,Family family,List<InsPlan> planList,long insSubNum,List<InsSub> subList) {
			//No need to check RemotingRole; no call to db.
			if(planNum==0) {
				return "";
			}
			InsPlan plan=GetPlan(planNum,planList);
			if(plan==null || plan.PlanNum==0) {
				return "";
			}
			InsSub sub=InsSubs.GetSub(insSubNum,subList);
			if(sub==null || sub.InsSubNum==0) {
				return "";
			}
			string subscriber=family.GetNameInFamFL(sub.Subscriber);
			if(subscriber=="") {//subscriber from another family
				subscriber=Patients.GetLim(sub.Subscriber).GetNameLF();
			}
			string retStr="";
			//loop just to get the index of the plan in the family list
			bool otherFam=true;
			for(int i=0;i<planList.Count;i++) {
				if(planList[i].PlanNum==planNum) {
					otherFam=false;
					//retStr += (i+1).ToString()+": ";
				}
			}
			if(otherFam) {//retStr=="")
				retStr="(other fam):";
			}
			Carrier carrier=Carriers.GetCarrier(plan.CarrierNum);
			string carrierName=carrier.CarrierName;
			if(carrierName.Length>20) {
				carrierName=carrierName.Substring(0,20)+"...";
			}
			retStr+=carrierName;
			retStr+=" ("+subscriber+")";
			return retStr;
		}

		///<summary>Used in Ins lines in Account module and in Family module.</summary>
		public static string GetCarrierName(long planNum,List<InsPlan> planList) {
			//No need to check RemotingRole; no call to db.
			InsPlan plan=GetPlan(planNum,planList);
			if(plan==null) {
				return "";
			}
			Carrier carrier=Carriers.GetCarrier(plan.CarrierNum);
			if(carrier.CarrierNum==0) {//if corrupted
				return "";
			}
			return carrier.CarrierName;
		}

		/// <summary>Returns a DataTable containing the PlanNum, CarrierNum, and CarrierName for a list of PlanNums.</summary>
		public static DataTable GetCarrierNames(List<long> listPlanNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listPlanNums);
			}
			if(listPlanNums.Count==0) {
				return new DataTable();
			}
			string command="SELECT PlanNum,CarrierNum,'' AS CarrierName,'' AS CarrierColor FROM insplan WHERE PlanNum IN ("+string.Join(",",listPlanNums)+")";
			DataTable table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				Carrier carrier=Carriers.GetCarrier(PIn.Long(row["CarrierNum"].ToString()));
				row["CarrierName"]=carrier.CarrierName;
				row["CarrierColor"]=carrier.ApptTextBackColor.ToArgb();
			}
			return table;
		}

		/// <summary>Only used once in Claims.cs.  Gets insurance benefits remaining for one benefit year.  Returns actual remaining insurance based on ClaimProc data, taking into account inspaid and ins pending. Must supply all claimprocs for the patient.  Date used to determine which benefit year to calc.  Usually today's date.  The insplan.PlanNum is the plan to get value for.  ExcludeClaim is the ClaimNum to exclude, or enter -1 to include all.  This does not yet handle calculations where ortho max is different from regular max.  Just takes the most general annual max, and subtracts all benefits used from all categories.</summary>
		public static double GetInsRem(List<ClaimProcHist> histList,DateTime asofDate,long planNum,long patPlanNum,long excludeClaim,List<InsPlan> planList,List<Benefit> benList,long patNum,long insSubNum) {
			//No need to check RemotingRole; no call to db.
			double insUsed=GetInsUsedDisplay(histList,asofDate,planNum,patPlanNum,excludeClaim,planList,benList,patNum,insSubNum);
			InsPlan plan=InsPlans.GetPlan(planNum,planList);
			double insPending=GetPendingDisplay(histList,asofDate,plan,patPlanNum,excludeClaim,patNum,insSubNum,benList);
			double annualMaxFam=Benefits.GetAnnualMaxDisplay(benList,planNum,patPlanNum,true);
			double annualMaxInd=Benefits.GetAnnualMaxDisplay(benList,planNum,patPlanNum,false);
			double annualMax=annualMaxInd;
			if(annualMaxFam>annualMaxInd){
				annualMax=annualMaxFam;
			}
			if(annualMax<0) {
				return 999999;
			}
			if(annualMax-insUsed-insPending<0) {
				return 0;
			}
			return annualMax-insUsed-insPending;
		}

		///<summary>Only for display purposes rather than for calculations.  Get pending insurance for a given plan for one benefit year. Include a history list for the patient/family.  asofDate used to determine which benefit year to calc.  Usually the date of service for a claim.  The planNum is the plan to get value for.</summary>
		public static double GetPendingDisplay(List<ClaimProcHist> histList,DateTime asofDate,InsPlan curPlan,long patPlanNum,long excludeClaim,long patNum,long insSubNum,List<Benefit> benefitList) {
			//No need to check RemotingRole; no call to db.
			//InsPlan curPlan=GetPlan(planNum,PlanList);
			if(curPlan==null) {
				return 0;
			}
			//get the most recent renew date, possibly including today:
			DateTime renewDate=BenefitLogic.ComputeRenewDate(asofDate,curPlan.MonthRenew);
			DateTime stopDate=renewDate.AddYears(1);
			double retVal=0;
			//CovCat generalCat=CovCats.GetForEbenCat(EbenefitCategory.General);
			//CovSpan[] covSpanArray=null;
			//if(generalCat!=null) {
			//  covSpanArray=CovSpans.GetForCat(generalCat.CovCatNum);
			//}
			for(int i=0;i<histList.Count;i++) {
				//if(generalCat!=null) {//If there is a general category, then we only consider codes within it.  This is how we exclude ortho.
				//  if(!CovSpans.IsCodeInSpans(histList[i].StrProcCode,covSpanArray)) {//for example, ortho
				//    continue;
				//  }
				//}
				if(Benefits.LimitationExistsNotGeneral(benefitList,curPlan.PlanNum,patPlanNum,histList[i].StrProcCode)) {
					continue;
				}
				if(histList[i].PlanNum==curPlan.PlanNum
					&& histList[i].InsSubNum==insSubNum
					&& histList[i].ClaimNum != excludeClaim
					&& histList[i].ProcDate < stopDate
					&& histList[i].ProcDate >= renewDate
					//enum ClaimProcStatus{NotReceived,Received,Preauth,Adjustment,Supplemental}
					&& histList[i].Status==ClaimProcStatus.NotReceived
					&& histList[i].PatNum==patNum)
				//Status Adjustment has no insPayEst, so can ignore it here.
				{
					retVal+=histList[i].Amount;
				}
			}
			return retVal;
		}

		/// <summary>Only for display purposes rather than for calculations.  Get insurance benefits used for one benefit year.  Must supply all relevant hist for the patient.  asofDate is used to determine which benefit year to calc.  Usually date of service for a claim.  The insplan.PlanNum is the plan to get value for.  ExcludeClaim is the ClaimNum to exclude, or enter -1 to include all.  It only includes values that apply towards annual max.  So if there is a limitation override for a category like ortho or preventive, then completed procedures in those categories will be excluded.  The benefitList passed in might very well have benefits from other insurance plans included.</summary>
		public static double GetInsUsedDisplay(List<ClaimProcHist> histList,DateTime asofDate,long planNum,long patPlanNum,long excludeClaim,List<InsPlan> planList,List<Benefit> benefitList,long patNum,long insSubNum) {
			//No need to check RemotingRole; no call to db.
			InsPlan curPlan=GetPlan(planNum,planList);
			if(curPlan==null) {
				return 0;
			}
			//get the most recent renew date, possibly including today:
			DateTime renewDate=BenefitLogic.ComputeRenewDate(asofDate,curPlan.MonthRenew);
			DateTime stopDate=renewDate.AddYears(1);
			double retVal=0;
			//CovCat generalCat=CovCats.GetForEbenCat(EbenefitCategory.General);
			//CovSpan[] covSpanArray=null;
			//if(generalCat!=null) {
			//  covSpanArray=CovSpans.GetForCat(generalCat.CovCatNum);
			//}
			for(int i=0;i<histList.Count;i++) {
				if(histList[i].PlanNum!=planNum
					|| histList[i].InsSubNum != insSubNum
					|| histList[i].ClaimNum == excludeClaim
					|| histList[i].ProcDate.Date >= stopDate
					|| histList[i].ProcDate.Date < renewDate
					|| histList[i].PatNum != patNum)
				{
					continue;
				}
				if(Benefits.LimitationExistsNotGeneral(benefitList,planNum,patPlanNum,histList[i].StrProcCode)) {
					continue;
				}
				//if(generalCat!=null){//If there is a general category, then we only consider codes within it.  This is how we exclude ortho.
				//	if(histList[i].StrProcCode!="" && !CovSpans.IsCodeInSpans(histList[i].StrProcCode,covSpanArray)){//for example, ortho
				//		continue;
				//	}
				//}
				//enum ClaimProcStatus{NotReceived,Received,Preauth,Adjustment,Supplemental}
				if(histList[i].Status==ClaimProcStatus.Received 
					|| histList[i].Status==ClaimProcStatus.Adjustment
					|| histList[i].Status==ClaimProcStatus.Supplemental) 
				{
					retVal+=histList[i].Amount;
				}
			}
			return retVal;
		}

		///<summary>Only for display purposes rather than for calculations.  Get insurance deductible used for one benefit year.  Must supply a history list for the patient/family.  asofDate is used to determine which benefit year to calc.  Usually date of service for a claim.  The planNum is the plan to get value for.  ExcludeClaim is the ClaimNum to exclude, or enter -1 to include all.  It includes pending deductibles in the result.</summary>
		public static double GetDedUsedDisplay(List<ClaimProcHist> histList,DateTime asofDate,long planNum,long patPlanNum,long excludeClaim,List<InsPlan> planList,BenefitCoverageLevel coverageLevel,long patNum) {
			//No need to check RemotingRole; no call to db.
			InsPlan curPlan=GetPlan(planNum,planList);
			if(curPlan==null) {
				return 0;
			}
			//get the most recent renew date, possibly including today. Date based on annual max.
			DateTime renewDate=BenefitLogic.ComputeRenewDate(asofDate,curPlan.MonthRenew);
			DateTime stopDate=renewDate.AddYears(1);
			double retVal=0;
			for(int i=0;i<histList.Count;i++) {
				if(histList[i].PlanNum!=planNum
					|| histList[i].ClaimNum == excludeClaim
					|| histList[i].ProcDate >= stopDate
					|| histList[i].ProcDate < renewDate
					//no need to check status, because only the following statuses will be part of histlist:
					//Adjustment,NotReceived,Received,Supplemental
					)
				{
					continue;
				}
				if(coverageLevel!=BenefitCoverageLevel.Family && histList[i].PatNum != patNum) {
					continue;//to exclude histList items from other family members
				}
				retVal+=histList[i].Deduct;
			}
			return retVal;
		}

		///<summary>Only for display purposes rather than for calculations.  Get insurance deductible used for one benefit year.  Must supply a history list for the patient/family.  asofDate is used to determine which benefit year to calc.  Usually date of service for a claim.  The planNum is the plan to get value for.  ExcludeClaim is the ClaimNum to exclude, or enter -1 to include all.  It includes pending deductibles in the result. The ded and dedFam variables are the individual and family deductibles respectively. This function assumes that the individual deductible 'ded' is always available, but that the family deductible 'dedFam' is optional (set to -1 if not available).</summary>
		public static double GetDedRemainDisplay(List<ClaimProcHist> histList,DateTime asofDate,long planNum,long patPlanNum,long excludeClaim,List<InsPlan> planList,long patNum,double ded,double dedFam) {
			//No need to check RemotingRole; no call to db.
			InsPlan curPlan=GetPlan(planNum,planList);
			if(curPlan==null) {
				return 0;
			}
			//get the most recent renew date, possibly including today. Date based on annual max.
			DateTime renewDate=BenefitLogic.ComputeRenewDate(asofDate,curPlan.MonthRenew);
			DateTime stopDate=renewDate.AddYears(1);
			double dedRemInd=ded;
			double dedRemFam=dedFam;
			for(int i=0;i<histList.Count;i++) {
				if(histList[i].PlanNum!=planNum
					|| histList[i].ClaimNum == excludeClaim
					|| histList[i].ProcDate >= stopDate
					|| histList[i].ProcDate < renewDate
					//no need to check status, because only the following statuses will be part of histlist:
					//Adjustment,NotReceived,Received,Supplemental
					) {
					continue;
				}
				dedRemFam-=histList[i].Deduct;
				if(histList[i].PatNum==patNum) {
					dedRemInd-=histList[i].Deduct;
				}
			}
			if(dedFam>=0) {
				return Math.Max(0,Math.Min(dedRemInd,dedRemFam));//never negative
			}
			return Math.Max(0,dedRemInd);//never negative
		}

		/*
		///<summary>Used once from Claims and also in ContrTreat.  Gets insurance deductible remaining for one benefit year which includes the given date.  Must supply all claimprocs for the patient.  Must supply all benefits for patient so that we know if it's a service year or a calendar year.  Date used to determine which benefit year to calc.  Usually today's date.  The insplan.PlanNum is the plan to get value for.  ExcludeClaim is the ClaimNum to exclude, or enter -1 to include all.  The supplied procCode is needed because some deductibles, for instance, do not apply to preventive.</summary>
		public static double GetDedRem(List<ClaimProc> claimProcList,DateTime date,int planNum,int patPlanNum,int excludeClaim,List<InsPlan> PlanList,List<Benefit> benList,string procCode) {
			//No need to check RemotingRole; no call to db.
			double dedTot=Benefits.GetDeductibleByCode(benList,planNum,patPlanNum,procCode);
			double dedUsed=GetDedUsed(claimProcList,date,planNum,patPlanNum,excludeClaim,PlanList,benList);
			if(dedTot-dedUsed<0){
				return 0;
			}
			return dedTot-dedUsed;
		}*/

		/*
		///<Summary>Only used in TP to calculate discount for PPO procedure.  Will return -1 if no fee found.</Summary>
		public static double GetPPOAllowed(int codeNum,InsPlan plan){
			//plan has already been tested to not be null and to be a PPO plan.
			double fee=Fees.GetAmount(codeNum,plan.FeeSched);//could be -1
		}*/

		///<summary>This is used in FormQuery.SubmitQuery to allow display of carrier names.</summary>
		public static Hashtable GetHListAll(){
			//No need to check RemotingRole; no call to db.
			DataTable table=GetCarrierTable();
			Hashtable HListAll=new Hashtable(table.Rows.Count);
			long plannum;
			string carrierName;
			for(int i=0;i<table.Rows.Count;i++){
				plannum=PIn.Long(table.Rows[i][0].ToString());
				carrierName=PIn.String(table.Rows[i][1].ToString());
				HListAll.Add(plannum,carrierName);
			}
			return HListAll;
		}

		public static DataTable GetCarrierTable() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			string command="SELECT insplan.PlanNum,carrier.CarrierName "
				+"FROM insplan,carrier "
				+"WHERE insplan.CarrierNum=carrier.CarrierNum";
			return Db.GetTable(command);
		}
		/*
		///<summary>Used by Trojan.  Gets all distinct notes for the planNums supplied.  Includes blank notes.</summary>
		public static string[] GetNotesForPlans(List<long> planNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<string[]>(MethodBase.GetCurrentMethod(),planNums,excludePlanNum);
			}
			if(planNums.Count==0) {//this should never happen, but just in case...
				return new string[0];
			}
			if(planNums.Count==1 && planNums[0]==excludePlanNum){
				return new string[0];
			}
			string s="";
			for(int i=0;i<planNums.Count;i++) {
				if(planNums[i]==excludePlanNum){
					continue;
				}
				if(s!="") {
					s+=" OR";
				}
				s+=" PlanNum="+POut.Long(planNums[i]);
			}
			string command="SELECT DISTINCT PlanNote FROM insplan WHERE"+s;
			DataTable table=Db.GetTable(command);
			string[] retVal=new string[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++) {
				retVal[i]=PIn.String(table.Rows[i][0].ToString());
			}
			return retVal;
		}

		///<summary>Used by Trojan.  Sets the PlanNote for multiple plans at once.</summary>
		public static void UpdateNoteForPlans(List<long> planNums,string newNote) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planNums,newNote);
				return;
			}
			if(planNums.Count==0){
				return;
			}
			string s="";
			for(int i=0;i<planNums.Count;i++){
				if(i>0){
					s+=" OR";
				}
				s+=" PlanNum="+POut.Long(planNums[i]);
			}
			string command="UPDATE insplan SET PlanNote='"+POut.String(newNote)+"' "
				+"WHERE"+s;
			Db.NonQ(command);
		}*/

		/*
		///<summary>Called from FormInsPlan when user wants to view a benefit note for similar plans.  Should never include the current plan that the user is editing.  This function will get one note from the database, not including blank notes.  If no note can be found, then it returns empty string.</summary>
		public static string GetBenefitNotes(List<long> planNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),planNums);
			}
			if(planNums.Count==0){
				return "";
			}
			string s="";
			for(int i=0;i<planNums.Count;i++) {
				if(i>0) {
					s+=" OR";
				}
				s+=" PlanNum="+POut.Long(planNums[i]);
			}
			string command="SELECT BenefitNotes FROM insplan WHERE BenefitNotes != '' AND ("+s+") "+DbHelper.LimitAnd(1);
			DataTable table=Db.GetTable(command);
			//string[] retVal=new string[];
			if(table.Rows.Count==0){
				return "";
			}
			return PIn.String(table.Rows[0][0].ToString());
		}*/

		/*
		///<summary>Gets a list of PlanNums from the database of plans that have identical info as this one. Used to perform updates to benefits, etc.  Note that you have the option to include the current plan in the list.</summary>
		public static List<long> GetPlanNumsOfSamePlans(string employerName,string groupName,string groupNum,
				string divisionNo,string carrierName,bool isMedical,long planNum,bool includePlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),employerName,groupName,groupNum,divisionNo,carrierName,isMedical,planNum,includePlanNum);
			}
			string command="SELECT PlanNum FROM insplan "
				+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
				+"LEFT JOIN employer ON employer.EmployerNum = insplan.EmployerNum ";
			if(employerName==""){
				command+="WHERE employer.EmpName IS NULL ";
			}
			else{
				command+="WHERE employer.EmpName = '"+POut.String(employerName)+"' ";
			}
			command+="AND insplan.GroupName = '"  +POut.String(groupName)+"' "
				+"AND insplan.GroupNum = '"   +POut.String(groupNum)+"' "
				+"AND insplan.DivisionNo = '" +POut.String(divisionNo)+"' "
				+"AND carrier.CarrierName = '"+POut.String(carrierName)+"' "
				+"AND insplan.IsMedical = '"  +POut.Bool  (isMedical)+"'"
				+"AND insplan.PlanNum != "+POut.Long(planNum);
			DataTable table=Db.GetTable(command);
			List<long> retVal=new List<long>();
			//if(includePlanNum){
			//	retVal=new int[table.Rows.Count+1];
			//}
			//else{
			//	retVal=new int[table.Rows.Count];
			//}
			for(int i=0;i<table.Rows.Count;i++) {
				retVal.Add(PIn.Long(table.Rows[i][0].ToString()));
			}
			if(includePlanNum){
				retVal.Add(planNum);
			}
			return retVal;
		}*/

		///<summary>Used from FormInsPlans to get a big list of many plans, organized by carrier name or by employer.</summary>
		public static DataTable GetBigList(bool byEmployer,string empName,string carrierName,string groupName,string groupNum,string planNum,
			string trojanID,bool showHidden,bool isIncludeAll)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),byEmployer,empName,carrierName,groupName,groupNum,planNum,trojanID,showHidden,isIncludeAll);
			}
			DataTable table=new DataTable();
			DataRow row;
			table.Columns.Add("Address");
			table.Columns.Add("City");
			table.Columns.Add("CarrierName");
			table.Columns.Add("ElectID");
			table.Columns.Add("EmpName");
			table.Columns.Add("GroupName");
			table.Columns.Add("GroupNum");
			table.Columns.Add("noSendElect");
			table.Columns.Add("Phone");
			table.Columns.Add("PlanNum");
			table.Columns.Add("State");
			table.Columns.Add("subscribers");
			table.Columns.Add("trojanID");
			table.Columns.Add("Zip");
			table.Columns.Add("IsCDA");
			string command="SELECT carrier.Address,carrier.City,CarrierName,ElectID,EmpName,GroupName,GroupNum,NoSendElect,"
				+"carrier.Phone,PlanNum,"
				+"(SELECT COUNT(DISTINCT Subscriber) FROM inssub WHERE insplan.PlanNum=inssub.PlanNum) subscribers,"//for Oracle
				+"carrier.State,TrojanID,carrier.Zip, "
				//+"(SELECT COUNT(*) FROM employer WHERE insplan.EmployerNum=employer.EmployerNum) haveName "//for Oracle. Could be higher than 1?
				+"CASE WHEN (EmpName IS NULL) THEN 1 ELSE 0 END as haveName,"//for Oracle
				+"carrier.IsCDA "
				+"FROM insplan "
				+"LEFT JOIN employer ON employer.EmployerNum = insplan.EmployerNum "
				+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
				+"WHERE CarrierName LIKE '%"+POut.String(carrierName)+"%' ";
			if(empName!="") {
				command+="AND EmpName LIKE '%"+POut.String(empName)+"%' ";
			}
			if(groupName!="") {
				command+="AND GroupName LIKE '%"+POut.String(groupName)+"%' ";
			}
			if(groupNum!="") {
				command+="AND GroupNum LIKE '%"+POut.String(groupNum)+"%' ";
			}
			if(planNum!="") {
				command+="AND PlanNum LIKE '%"+POut.String(planNum)+"%' ";
			}
			if(trojanID!=""){
				command+="AND TrojanID LIKE '%"+POut.String(trojanID)+"%' ";
			}
			if(!showHidden){
				command+="AND insplan.IsHidden=0 ";
			}
			if(!isIncludeAll) {
				command+=DbHelper.LimitAnd(200);
			}
			DataTable rawT=Db.GetTable(command);
			IOrderedEnumerable<DataRow> rawRows;
			if(byEmployer) {
				rawRows=rawT.Select().OrderBy(x => x["haveName"].ToString()).ThenBy(x => x["EmpName"].ToString()).ThenBy(x => x["CarrierName"].ToString());
			}
			else {//by carrier
				rawRows=rawT.Select().OrderBy(x => x["CarrierName"].ToString());
			}
			foreach(DataRow rawRow in rawRows) {
				row=table.NewRow();
				row["Address"]=rawRow["Address"].ToString();
				row["City"]=rawRow["City"].ToString();
				row["CarrierName"]=rawRow["CarrierName"].ToString();
				row["ElectID"]=rawRow["ElectID"].ToString();
				row["EmpName"]=rawRow["EmpName"].ToString();
				row["GroupName"]=rawRow["GroupName"].ToString();
				row["GroupNum"]=rawRow["GroupNum"].ToString();
				row["noSendElect"]=(rawRow["NoSendElect"].ToString()=="1"?"X":"");
				row["Phone"]=rawRow["Phone"].ToString();
				row["PlanNum"]=rawRow["PlanNum"].ToString();
				row["State"]=rawRow["State"].ToString();
				row["subscribers"]=rawRow["subscribers"].ToString();
				row["TrojanID"]=rawRow["TrojanID"].ToString();
				row["Zip"]=rawRow["Zip"].ToString();
				row["IsCDA"]=rawRow["IsCDA"].ToString();
				table.Rows.Add(row);
			}
			return table;
		}

		///<summary>Used in FormFeesForIns</summary>
		public static DataTable GetListFeeCheck(string carrierName,string carrierNameNot,long feeSchedWithout,long feeSchedWith,
			FeeScheduleType feeSchedType)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),carrierName,carrierNameNot,feeSchedWithout,feeSchedWith,feeSchedType);
			}
			string pFeeSched="FeeSched";
			if(feeSchedType==FeeScheduleType.OutNetwork){
				pFeeSched="AllowedFeeSched";//This is the name of a column in the insplan table and cannot be changed to OutNetworkFeeSched
			}
			if(feeSchedType==FeeScheduleType.CoPay || feeSchedType==FeeScheduleType.FixedBenefit){
				pFeeSched="CopayFeeSched";
			}
			if(feeSchedType==FeeScheduleType.ManualBlueBook) {
				pFeeSched="ManualFeeSchedNum";
			}
			string command=
				"SELECT insplan.PlanNum,insplan.GroupName,insplan.GroupNum,employer.EmpName,carrier.CarrierName,"
				+"insplan.EmployerNum,insplan.CarrierNum,feesched.Description AS FeeSchedName,insplan.PlanType,"
				+"insplan.IsBlueBookEnabled,insplan."+pFeeSched+" feeSched "
				+"FROM insplan "
				+"LEFT JOIN employer ON employer.EmployerNum = insplan.EmployerNum "
				+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
				+"LEFT JOIN feesched ON feesched.FeeSchedNum = insplan."+pFeeSched+" "
				+"WHERE carrier.CarrierName LIKE '%"+POut.String(carrierName)+"%' ";
			if(carrierNameNot!=""){
				command+="AND carrier.CarrierName NOT LIKE '%"+POut.String(carrierNameNot)+"%' ";
			}
			if(feeSchedWithout!=0){
				command+="AND insplan."+pFeeSched+" !="+POut.Long(feeSchedWithout)+" ";
			}
			if(feeSchedWith!=0) {
				command+="AND insplan."+pFeeSched+" ="+POut.Long(feeSchedWith)+" ";
			}
			command+="ORDER BY carrier.CarrierName,employer.EmpName,insplan.GroupNum";
			return Db.GetTable(command);
		}

		///<summary>Used only in FormFeesForIns. Used to update the passed in list of insurance plans to a few fee schedule</summary>
		public static long ChangeFeeScheds(List<long> listInsPlanNums,long newFeeSchedNum,FeeScheduleType feeSchedType,bool disableBlueBook,bool enableBlueBook) {
			if(listInsPlanNums.IsNullOrEmpty()) {
				return 0;//Count of rows changed.
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),listInsPlanNums,newFeeSchedNum,feeSchedType,disableBlueBook,enableBlueBook);
			}
			if(listInsPlanNums.Count==0) {
				return 0; // no insurance plans to change
			}
			string command="UPDATE insplan SET ";
			if(disableBlueBook) { //mutually exclusive from enableBlueBook, but not the inverse. They will not both be true
				command+="insplan.IsBlueBookEnabled=FALSE, ";
			}
			if(enableBlueBook) {
				command+="insplan.IsBlueBookEnabled=TRUE, ";
			}
			if(feeSchedType==FeeScheduleType.Normal) {
				command+="insplan.FeeSched ="+POut.Long(newFeeSchedNum)
					+" WHERE insplan.FeeSched !="+POut.Long(newFeeSchedNum);
			}
			else if(feeSchedType==FeeScheduleType.OutNetwork) {
				command+="insplan.AllowedFeeSched ="+POut.Long(newFeeSchedNum)
					+" WHERE insplan.AllowedFeeSched !="+POut.Long(newFeeSchedNum);
			}
			else if(feeSchedType==FeeScheduleType.CoPay || feeSchedType==FeeScheduleType.FixedBenefit) {
				command+="insplan.CopayFeeSched ="+POut.Long(newFeeSchedNum);
				if(feeSchedType==FeeScheduleType.CoPay) {
					command+=",insplan.PlanType=''";
				}
				else {
					command+=",insplan.PlanType='f'";
				}
				command+=" WHERE insplan.CopayFeeSched !="+POut.Long(newFeeSchedNum);
			}
			else if(feeSchedType==FeeScheduleType.ManualBlueBook) {
				command+="insplan.ManualFeeSchedNum ="+POut.Long(newFeeSchedNum)
					+" WHERE insplan.ManualFeeSchedNum !="+POut.Long(newFeeSchedNum);
			}
			command+=$" AND insplan.PlanNum IN ({String.Join(",",listInsPlanNums.Select(x => POut.Long(x)))})";
			if(disableBlueBook) {
				InsBlueBooks.DeleteByPlanNums(listInsPlanNums.ToArray());
			}
			List<InsPlan> listInsPlans = InsPlans.GetPlans(listInsPlanNums);
			//log InsPlan's fee schedule update.
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			listInsPlans.ForEach(plan =>
			InsEditLogs.MakeLogEntry(POut.String("FeeSchedNum"),
				Security.CurUser.UserNum,
				POut.String(plan.FeeSched.ToString()),
				newFeeSchedNum.ToString(),
				InsEditLogType.InsPlan,
				PIn.Long(plan.PlanNum.ToString())
				,0
				,plan.GroupNum.ToString()+" - "+plan.GroupName.ToString())
			);
			return Db.NonQ(command);
		}

		///<summary>Based on the four supplied parameters, it updates all similar plans.  Used in a specific tool: FormFeesForIns.</summary>
		public static long SetFeeSched(long employerNum,string carrierName,string groupNum,
			string groupName,long feeSchedNum, FeeScheduleType feeSchedType)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),employerNum,carrierName,groupNum,groupName,feeSchedNum,feeSchedType);
			}
			//Code rewritten so that it is not only MySQL compatible, but Oracle compatible as well.
			string command="SELECT insplan.PlanNum, ";
			switch(feeSchedType) {
				case FeeScheduleType.Normal:
					command+="insplan.FeeSched, ";
					break;
				case FeeScheduleType.FixedBenefit:
				case FeeScheduleType.CoPay:
					command+="insplan.CopayFeeSched, ";
					break;
				case FeeScheduleType.OutNetwork:
					command+="insplan.AllowedFeeSched, ";
					break;
				case FeeScheduleType.ManualBlueBook:
					command+="insplan.ManualFeeSchedNum, ";
					break;
			}
			command+="insplan.GroupNum, insplan.GroupName "
				+"FROM insplan,carrier "
				+"WHERE carrier.CarrierNum = insplan.CarrierNum "//employer.EmployerNum = insplan.EmployerNum "
				+"AND insplan.EmployerNum="+POut.Long(employerNum)+" "
				+"AND carrier.CarrierName='"+POut.String(carrierName)+"' "
				+"AND insplan.GroupNum='"+POut.String(groupNum)+"' "
				+"AND insplan.GroupName='"+POut.String(groupName)+"'";
			//table[i][0] = PlanNum
			//table[i][1] = FeeSchedNum (FeeSched, CopayFeeSched, AllowedFeeSched, FixedBenefit, or ManualFeeSchedNum)
			//table[i][2] = GroupNum 
			//table[i][3] = GroupName
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0){
				return 0;
			}
			command="UPDATE insplan SET ";
			if(feeSchedType==FeeScheduleType.Normal){
				command+="insplan.FeeSched ="+POut.Long(feeSchedNum)
					+" WHERE insplan.FeeSched !="+POut.Long(feeSchedNum);
			}
			else if(feeSchedType==FeeScheduleType.OutNetwork){
				command+="insplan.AllowedFeeSched ="+POut.Long(feeSchedNum)
					+" WHERE insplan.AllowedFeeSched !="+POut.Long(feeSchedNum);
			}
			else if(feeSchedType==FeeScheduleType.CoPay || feeSchedType==FeeScheduleType.FixedBenefit) {
				command+="insplan.CopayFeeSched ="+POut.Long(feeSchedNum);
				if(feeSchedType==FeeScheduleType.CoPay) {
					command+=",insplan.PlanType=''";
				}
				else {
					command+=",insplan.PlanType='f'";
				}
				command+=" WHERE insplan.CopayFeeSched !="+POut.Long(feeSchedNum);
			}
			else if(feeSchedType==FeeScheduleType.ManualBlueBook) {
				command+="insplan.ManualFeeSchedNum ="+POut.Long(feeSchedNum)
					+" WHERE insplan.ManualFeeSchedNum !="+POut.Long(feeSchedNum);
			}
			command+=" AND (";
			for(int i=0;i<table.Rows.Count;i++){
				command+="PlanNum="+table.Rows[i][0].ToString();
				if(i<table.Rows.Count-1){
					command+=" OR ";
				}
				//log InsPlan's fee schedule update.
				//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				InsEditLogs.MakeLogEntry(POut.String(table.Columns[1].ToString()),
					Security.CurUser.UserNum,POut.String(table.Rows[i][1].ToString()),feeSchedNum.ToString(),
					InsEditLogType.InsPlan,PIn.Long(table.Rows[i][0].ToString()),0,
					table.Rows[i][2].ToString()+" - "+table.Rows[i][3].ToString());
			}
			command+=")";
			return Db.NonQ(command);
		}


		///<summary>Returns the number of fee schedules added.  It doesn't inform the user of how many plans were affected, but there will obviously be a
		///certain number of plans for every new fee schedule.</summary>
		public static long GenerateAllowedFeeSchedules() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			//get carrier names for all plans without an allowed fee schedule that are also not hidden.
			string command="SELECT carrier.CarrierName "
				+"FROM insplan,carrier "
				+"WHERE carrier.CarrierNum=insplan.CarrierNum "
				+"AND insplan.AllowedFeeSched=0 "
				+"AND insplan.PlanType='' "
				+"AND insplan.IsHidden='0' "
				+"GROUP BY carrier.CarrierName";
			DataTable table=Db.GetTable(command);
			//loop through all the carrier names
			string carrierName;
			FeeSched sched;
			int itemOrder=FeeScheds.GetCount();
			long retVal=0;
			for(int i=0;i<table.Rows.Count;i++){
				carrierName=PIn.String(table.Rows[i]["CarrierName"].ToString());
				if(carrierName=="" || carrierName==" "){
					continue;
				}
				//add a fee schedule if needed
				sched=FeeScheds.GetByExactName(carrierName,FeeScheduleType.OutNetwork);
				if(sched==null){
					sched=new FeeSched();
					sched.Description=carrierName;
					sched.FeeSchedType=FeeScheduleType.OutNetwork;
					//sched.IsNew=true;
					sched.IsGlobal=true;
					sched.ItemOrder=itemOrder;
					//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
					sched.SecUserNumEntry=Security.CurUser.UserNum;
					FeeScheds.Insert(sched);
					itemOrder++;
				}
				List<long> listCarrierNums;
				//assign the fee sched to many plans
				//for compatibility with Oracle, get a list of all carrierNums that use the carriername
				command="SELECT CarrierNum FROM carrier WHERE CarrierName='"+POut.String(carrierName)+"'";
				listCarrierNums=Db.GetListLong(command);
				if(listCarrierNums.Count==0){
					continue;//I don't see how this could happen
				}
				command="SELECT * FROM insplan "
					+"WHERE AllowedFeeSched = 0 "
					+"AND PlanType='' "
					+"AND IsHidden=0 "
					+"AND CarrierNum IN ("+string.Join(",",listCarrierNums)+")";
				List<InsPlan> listInsPlans = Crud.InsPlanCrud.SelectMany(command);
				command="UPDATE insplan "
					+"SET AllowedFeeSched="+POut.Long(sched.FeeSchedNum)+" "
					+"WHERE PlanNum IN ("+string.Join(",",listInsPlans.Select(x => x.PlanNum))+")";
				retVal+=Db.NonQ(command);
				//log updated InsPlan's AllowedFeeSched
				//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				listInsPlans.ForEach(x => {
					InsEditLogs.MakeLogEntry("AllowedFeeSched",Security.CurUser.UserNum,"0",POut.Long(sched.FeeSchedNum),InsEditLogType.InsPlan,
						x.PlanNum,0,x.GroupNum.ToString()+" - "+x.GroupName.ToString());
				});
			}
			return retVal;
		}

		public static int UnusedGetCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM insplan WHERE IsHidden=0 "
				+"AND NOT EXISTS (SELECT * FROM inssub WHERE inssub.PlanNum=insplan.PlanNum)";
			int count=PIn.Int(Db.GetCount(command));
			return count;
		}
		
		public static void UnusedHideAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="SELECT * FROM insplan "
				+"WHERE IsHidden=0 "
				+"AND NOT EXISTS (SELECT * FROM inssub WHERE inssub.PlanNum=insplan.PlanNum)";
			List<InsPlan> listInsPlans = Crud.InsPlanCrud.SelectMany(command);
			if(listInsPlans.Count==0) { 
				return;
			}
			command="UPDATE insplan SET IsHidden=1 "
				+"WHERE PlanNum IN ("+string.Join(",",listInsPlans.Select(x => x.PlanNum))+")";
			Db.NonQ(command);
			//log newly hidden InsPlans
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			listInsPlans.ForEach(x => {
				InsEditLogs.MakeLogEntry("IsHidden",Security.CurUser.UserNum,"0","1",InsEditLogType.InsPlan,
					x.PlanNum,0,x.GroupNum.ToString()+" - "+x.GroupName.ToString());
			});
		}

		///<summary>Returns -1 if no copay feeschedule.  Can return -1 if copay amount is blank.
		///Leave lookupFees null to retrieve from db.  If not null, it should contain fees for all possible alternate codes.</summary>
		public static double GetCopay(long codeNum,long feeSched,long copayFeeSched,bool codeSubstNone,string toothNum,long clinicNum,long provNum,
			long planNum,List<SubstitutionLink> listSubstLinks=null,Lookup<FeeKey2,Fee> lookupFees=null)//allowing null on these in order to not break unit tests
		{
			//No need to check RemotingRole; no call to db.
			if(copayFeeSched==0) {
				return -1;
			}
			long substCodeNum=codeNum;
			//codeSubstNone, true if the insplan does not allow procedure code downgrade substitution.
			if(!codeSubstNone) {
				//Plan allows substitution codes.  Get the substitution code if one exists.
				substCodeNum=ProcedureCodes.GetSubstituteCodeNum(ProcedureCodes.GetStringProcCode(codeNum),toothNum,planNum,listSubstLinks);//for posterior composites
			}
			//List<Fee> listFees=lookupFees[new FeeKey2(substCodeNum,copayFeeSched)];//couldn't lookup earlier because we didn't know code.
			List<Fee> listFees=null;
			if(lookupFees!=null){
				listFees=lookupFees[new FeeKey2(substCodeNum,copayFeeSched)].ToList();
			}
			double retVal=Fees.GetAmount(substCodeNum,copayFeeSched,clinicNum,provNum,listFees);
			if(retVal==-1) {//blank co-pay
				if(PrefC.GetBool(PrefName.CoPay_FeeSchedule_BlankLikeZero)) {
					return -1;//will act like zero.  No patient co-pay.
				}
				else {
					//The amount from the regular fee schedule
					//In other words, the patient is responsible for procs that are not specified in a managed care fee schedule.
					if(lookupFees!=null){
						listFees=lookupFees[new FeeKey2(substCodeNum,feeSched)].ToList();
					}
					return Fees.GetAmount(substCodeNum,feeSched,clinicNum,provNum,listFees);
				}
			}
			return retVal;
		}

		///<summary>Returns -1 if no allowed feeschedule or fee unknown for this procCode. Otherwise, returns the allowed fee including 0.
		///Can handle a planNum of 0.  Tooth num is used for posterior composites.
		///It can be left blank in some situations.  Provider must be supplied in case plan has no assigned fee schedule.
		///Then it will use the fee schedule for the provider.
		///Leave lookupFees null to retrieve from db.</summary>
		public static double GetAllowed(string procCodeStr,long feeSched,long allowedFeeSched,bool codeSubstNone,string planType,string toothNum
			,long provNum,long clinicNum,long planNum,List<SubstitutionLink> listSubLinks=null,Lookup<FeeKey2,Fee> lookupFees=null)
		{
			//No need to check RemotingRole; no call to db.
			long codeNum=ProcedureCodes.GetCodeNum(procCodeStr);
			long substCodeNum=codeNum;
			if(!codeSubstNone) {
				substCodeNum=ProcedureCodes.GetSubstituteCodeNum(procCodeStr,toothNum,planNum,listSubLinks);//for posterior composites
			}
			//PPO always returns the PPO fee for the code or substituted code. 
			//Flat copay insurances should only ever pay up to their fee schedule amount, regardless of what the procFee is.
			List<Fee> listFees=null;
			if(planType=="p" || planType=="f") {
				if(lookupFees!=null){
					listFees=lookupFees[new FeeKey2(substCodeNum,feeSched)].ToList();
				}
				double allowedSub=Fees.GetAmount(substCodeNum,feeSched,clinicNum,provNum,listFees);
				double allowedNoSub;
				if(codeNum==substCodeNum) {
					allowedNoSub=allowedSub;
				}
				else {
					if(lookupFees!=null){
						listFees=lookupFees[new FeeKey2(codeNum,feeSched)].ToList();
					}
					allowedNoSub=Fees.GetAmount(codeNum,feeSched,clinicNum,provNum,listFees);
				}
				if(allowedSub==-1								//The fee for the substitution code is blank
					|| allowedSub > allowedNoSub) //or the downgrade fee is more expensive than the original fee
				{ 
					return allowedNoSub;//Use the fee from the original code
				}
				return allowedSub;
			}
			//or, if not PPO, and an allowed fee schedule exists, then we use that.
			if(allowedFeeSched!=0) {
				if(lookupFees!=null){
					listFees=lookupFees[new FeeKey2(substCodeNum,allowedFeeSched)].ToList();
				}
				return Fees.GetAmount(substCodeNum,allowedFeeSched,clinicNum,provNum,listFees);//whether post composite or not
			}
			//must be an ordinary fee schedule, so if no substitution code, then no allowed override
			if(codeNum==substCodeNum) {
				return -1;
			}
			//must be posterior composite with an ordinary fee schedule
			//Although it won't happen very often, it's possible that there is no fee schedule assigned to the plan.
			if(feeSched==0) {
				if(provNum==0) {//slight corruption
					if(lookupFees!=null){
						listFees=lookupFees[new FeeKey2(substCodeNum,Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv)).FeeSched)].ToList();
					}
					return Fees.GetAmount(substCodeNum,Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv)).FeeSched,clinicNum,provNum,listFees);
				}
				else{
					if(lookupFees!=null){
						listFees=lookupFees[new FeeKey2(substCodeNum,Providers.GetProv(provNum).FeeSched)].ToList();
					}
					return Fees.GetAmount(substCodeNum,Providers.GetProv(provNum).FeeSched,clinicNum,provNum,listFees);
				}
			}
			if(lookupFees!=null){
				listFees=lookupFees[new FeeKey2(substCodeNum,feeSched)].ToList();
			}
			return Fees.GetAmount(substCodeNum,feeSched,clinicNum,provNum,listFees);
		}

		public static decimal GetAllowedForProc(Procedure proc,ClaimProc claimProc,List<InsPlan> listInsPlans,List<SubstitutionLink> listSubLinks
			,Lookup<FeeKey2,Fee> lookupFees,BlueBookEstimateData blueBookEstimateData=null) 
		{
			//List<Fee> listFees=null) {
			//No need to check RemotingRole; no call to db.
			InsPlan plan=InsPlans.GetPlan(claimProc.PlanNum,listInsPlans);
			decimal carrierAllowedAmount;
			bool codeSubstNone=(!SubstitutionLinks.HasSubstCodeForPlan(plan,proc.CodeNum,listSubLinks));
			if(blueBookEstimateData!=null && blueBookEstimateData.IsValidForEstimate(claimProc)) {
				carrierAllowedAmount=(decimal)blueBookEstimateData.GetAllowed(proc,lookupFees,codeSubstNone,listSubLinks);
			}
			else {
				carrierAllowedAmount=(decimal)GetAllowed(ProcedureCodes.GetStringProcCode(proc.CodeNum),plan.FeeSched,plan.AllowedFeeSched,
					codeSubstNone,plan.PlanType,proc.ToothNum,proc.ProvNum,proc.ClinicNum,plan.PlanNum,listSubLinks,lookupFees);
			}
			if(carrierAllowedAmount==-1) {
				return -1;
			}
			else if(carrierAllowedAmount > (decimal)proc.ProcFee) {//if the Dr's UCR is lower than the Carrier's PPO allowed.
				return (decimal)proc.ProcFeeTotal;
			}
			else {
				return carrierAllowedAmount * (decimal)proc.Quantity;
			}
		}

		public static List<InsPlan> GetByInsSubs(List<long> insSubNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsPlan>>(MethodBase.GetCurrentMethod(),insSubNums);
			}
			if(insSubNums==null || insSubNums.Count < 1) {
				return new List<InsPlan>();
			}
			string command="SELECT DISTINCT insplan.* FROM insplan,inssub "
				+"WHERE insplan.PlanNum=inssub.PlanNum "
				+"AND inssub.InsSubNum IN ("+string.Join(",",insSubNums)+")";
			return Crud.InsPlanCrud.SelectMany(command);
		}

		///<summary>Used when closing the edit plan window to find all patients using this plan and to update all claimProcs for each patient.  This keeps estimates correct.</summary>
		public static void ComputeEstimatesForTrojanPlan(long planNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planNum);
				return;
			}
			//string command="SELECT PatNum FROM patplan WHERE PlanNum="+POut.Long(planNum);
			//The left join will get extra info about each plan, namely the PlanNum.  No need for a GROUP BY.  The PlanNum is used to filter.
			string command=@"SELECT PatNum FROM patplan 
					LEFT JOIN inssub ON patplan.InsSubNum=inssub.InsSubNum
					WHERE inssub.PlanNum="+POut.Long(planNum);
			DataTable table=Db.GetTable(command);
			List<long> patnums=new List<long>();
			for(int i=0;i<table.Rows.Count;i++) {
				patnums.Add(PIn.Long(table.Rows[i][0].ToString()));
			}
			ComputeEstimatesForPatNums(patnums);
		}

		///<summary>Used when closing the edit plan window to find all patients using this subscriber and to update all claimProcs for each patient.  This keeps estimates correct.</summary>
		public static void ComputeEstimatesForSubscriber(long subscriber) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),subscriber);
				return;
			}
			string command="SELECT DISTINCT PatNum FROM patplan,inssub WHERE Subscriber="+POut.Long(subscriber)+" AND patplan.InsSubNum=inssub.InsSubNum";
			List<long> patnums=Db.GetListLong(command);
			ComputeEstimatesForPatNums(patnums);
		}

		///<summary>Computes estimates for all patients passed.</summary>
		public static void ComputeEstimatesForPatNums(List<long> listPatNums) {
			//No need to check RemotingRole; no call to db.
			listPatNums=listPatNums.Distinct().ToList();
			for(int i=0;i<listPatNums.Count;i++) {
				long patNum=listPatNums[i];
				Family fam=Patients.GetFamily(patNum);
				Patient pat=fam.GetPatient(patNum);
				List<Procedure> procs=Procedures.Refresh(patNum);
				//Only grab the procedures that have not been completed yet.
				List<Procedure> listNonCompletedProcs=procs.FindAll(x => x.ProcStatus!=ProcStat.C && x.ProcStatus!=ProcStat.D);
				List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
				//Only use the claim procs associated to the non-completed procedures.
				List<ClaimProc> listNonCompletedClaimProcs=claimProcs.FindAll(x => listNonCompletedProcs.Exists(y => y.ProcNum==x.ProcNum));
				List<InsSub> subs=InsSubs.RefreshForFam(fam);
				List<InsPlan> plans=InsPlans.RefreshForSubList(subs);
				List<PatPlan> patPlans=PatPlans.Refresh(patNum);
				List<Benefit> benefitList=Benefits.Refresh(patPlans,subs);
				List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
				for(int p=0;p<procs.Count;p++){
					ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procs[p].CodeNum);
					listProcedureCodes.Add(procedureCode);//duplicates are ok
				}
				List<SubstitutionLink> listSubstLinks=SubstitutionLinks.GetAllForPlans(plans);
				long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(pat.PatNum);
				List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodes,procs.Select(x=>x.MedicalCode).ToList(),procs.Select(x=>x.ProvNum).ToList(),
					pat.PriProv,pat.SecProv,pat.FeeSched,plans,procs.Select(x=>x.ClinicNum).ToList(),null,//don't need appts to set proc provs
					listSubstLinks,discountPlanNum);
				Procedures.ComputeEstimatesForAll(patNum,listNonCompletedClaimProcs,listNonCompletedProcs,plans,patPlans,benefitList,pat.Age,subs,
					null,false,listSubstLinks,listFees);
				Patients.SetHasIns(patNum);
			}
		}

		///<summary>Throws ApplicationException if any dependencies exist and it is not safe to delete the insurance plan.
		///This is quite complex because it also must update all claimprocs for all patients affected by the deletion.  
		///Also deletes patplans, benefits, and claimprocs. 
		///If canDeleteInsSub is true and there is only one inssub associated to the plan, it will also delete inssubs. 
		///This should only really happen when an existing plan is being deleted.</summary>
		public static void Delete(InsPlan plan,bool canDeleteInsSub=true,bool doInsertInsEditLogs=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),plan,canDeleteInsSub,doInsertInsEditLogs);
				return;
			}
			#region Validation
			//Claims
			string command="SELECT 1 FROM claim WHERE PlanNum="+POut.Long(plan.PlanNum)+" "+DbHelper.LimitAnd(1);
			if(!string.IsNullOrEmpty(Db.GetScalar(command))) {
				throw new ApplicationException(Lans.g("FormInsPlan","Not allowed to delete a plan with existing claims."));
			}
			//Claimprocs
			command="SELECT 1 FROM claimproc "
				+"WHERE PlanNum="+POut.Long(plan.PlanNum)+" AND Status!="+POut.Int((int)ClaimProcStatus.Estimate)+" "//ignore estimates
				+DbHelper.LimitAnd(1);
			if(!string.IsNullOrEmpty(Db.GetScalar(command))) {
				throw new ApplicationException(Lans.g("FormInsPlan","Not allowed to delete a plan attached to procedures."));
			}
			//Appointments
			command="SELECT 1 FROM appointment "
				+"WHERE (InsPlan1="+POut.Long(plan.PlanNum)+" OR InsPlan2="+POut.Long(plan.PlanNum)+") "
				+"AND AptStatus IN ("+POut.Int((int)ApptStatus.Complete)+","
					+POut.Int((int)ApptStatus.Broken)+","
					+POut.Int((int)ApptStatus.PtNote)+","
					+POut.Int((int)ApptStatus.PtNoteCompleted)+") "//We only care about appt statuses that are excluded in Appointments.UpdateInsPlansForPat()
					+DbHelper.LimitAnd(1);
			if(!string.IsNullOrEmpty(Db.GetScalar(command))) {
				throw new ApplicationException(Lans.g("FormInsPlan","Not allowed to delete a plan attached to appointments."));
			}
			//PayPlans
			command="SELECT 1 FROM payplan WHERE PlanNum="+POut.Long(plan.PlanNum)+" "+DbHelper.LimitAnd(1);
			if(!string.IsNullOrEmpty(Db.GetScalar(command))) {
				throw new ApplicationException(Lans.g("FormInsPlan","Not allowed to delete a plan attached to payment plans."));
			}
			//InsSubs
			//we want the InsSubNum if only 1, otherwise only need to know there's more than one.
			command="SELECT InsSubNum FROM inssub WHERE PlanNum="+POut.Long(plan.PlanNum)+" "+DbHelper.LimitAnd(2);
			List<long> listInsSubNums=Db.GetListLong(command);
			if(listInsSubNums.Count>1) {
				throw new ApplicationException(Lans.g("FormInsPlan","Not allowed to delete a plan with more than one subscriber."));
			}
			else if(listInsSubNums.Count==1 && canDeleteInsSub) {//if there's only one inssub, delete it.
				InsSubs.Delete(listInsSubNums[0]);//Checks dependencies first;  If none, deletes the inssub, claimprocs, patplans, and recomputes all estimates.
			}
			#endregion Validation
			command="SELECT * FROM benefit WHERE PlanNum="+POut.Long(plan.PlanNum);
			List<Benefit> listBenefits=Crud.BenefitCrud.SelectMany(command);
			if(listBenefits.Count>0) {
				command="DELETE FROM benefit WHERE PlanNum="+POut.Long(plan.PlanNum);
				Db.NonQ(command);
				//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				if(doInsertInsEditLogs) {
					listBenefits.ForEach(x => {
						InsEditLogs.MakeLogEntry(null,x,InsEditLogType.Benefit,Security.CurUser.UserNum);//log benefit deletion
					});
				}
			}
			ClearFkey(plan.PlanNum);//Zero securitylog FKey column for rows to be deleted.
			command="DELETE FROM insplan WHERE PlanNum="+POut.Long(plan.PlanNum);
			Db.NonQ(command);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			if(doInsertInsEditLogs) {
				InsEditLogs.MakeLogEntry(null,plan,InsEditLogType.InsPlan,Security.CurUser.UserNum); //log insplan deletion
			}
			InsVerifies.DeleteByFKey(plan.PlanNum,VerifyTypes.InsuranceBenefit);
		}

		/// <summary>This changes PlanNum in every place in database where it's used.  It also deletes benefits for the old planNum.</summary>
		public static void ChangeReferences(long planNum,InsPlan planToMergeTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planNum,planToMergeTo);
				return;
			}
			long planNumTo=planToMergeTo.PlanNum;
			string command;
			//change all references to the old plan to point to the new plan.
			//appointment.InsPlan1/2
			command="UPDATE appointment SET InsPlan1="+POut.Long(planNumTo)+" WHERE InsPlan1="+POut.Long(planNum);
			Db.NonQ(command);
			command="UPDATE appointment SET InsPlan2="+POut.Long(planNumTo)+" WHERE InsPlan2="+POut.Long(planNum);
			Db.NonQ(command);
			//benefit.PlanNum -- DELETE unused
			command="SELECT * FROM benefit WHERE PlanNum="+POut.Long(planNum);
			List<Benefit> listBenefits=Crud.BenefitCrud.SelectMany(command);
			command="DELETE FROM benefit WHERE PlanNum="+POut.Long(planNum);
			Db.NonQ(command);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			listBenefits.ForEach(x => {
				InsEditLogs.MakeLogEntry(null,x,InsEditLogType.Benefit,Security.CurUser.UserNum);
			});
			//claim.PlanNum/PlanNum2
			command="UPDATE claim SET PlanNum="+POut.Long(planNumTo)+" WHERE PlanNum="+POut.Long(planNum);
			Db.NonQ(command);
			command="UPDATE claim SET PlanNum2="+POut.Long(planNumTo)+" WHERE PlanNum2="+POut.Long(planNum);
			Db.NonQ(command);
			//claimproc.PlanNum
			command="UPDATE claimproc SET PlanNum="+POut.Long(planNumTo)+" WHERE PlanNum="+POut.Long(planNum);
			Db.NonQ(command);
			//insbluebook.PlanNum
			if(planToMergeTo.PlanType=="" && planToMergeTo.IsBlueBookEnabled) {
				command=$@"
				UPDATE insbluebook 
				SET insbluebook.CarrierNum={POut.Long(planToMergeTo.CarrierNum)},
					insbluebook.PlanNum={POut.Long(planToMergeTo.PlanNum)},
					insbluebook.GroupNum='{POut.String(planToMergeTo.GroupNum)}'
				WHERE PlanNum={POut.Long(planNum)}";
			}
			else {
				command=$"DELETE FROM insbluebook WHERE insbluebook.PlanNum={POut.Long(planNum)}";
			}
			Db.NonQ(command);
			//etrans.PlanNum
			command="UPDATE etrans SET PlanNum="+POut.Long(planNumTo)+" WHERE PlanNum="+POut.Long(planNum);
			Db.NonQ(command);
			//inssub.PlanNum
			command="UPDATE inssub SET PlanNum="+POut.Long(planNumTo)+" WHERE PlanNum="+POut.Long(planNum);
			Db.NonQ(command);
			//payplan.PlanNum
			command="UPDATE payplan SET PlanNum="+POut.Long(planNumTo)+" WHERE PlanNum="+POut.Long(planNum);
			Db.NonQ(command);
			//the old plan should then be deleted.
		}

		///<summary>Returns the number of plans affected.</summary>
		public static long SetAllPlansToShowUCR() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM insplan WHERE ClaimsUseUCR = 0";
			List<InsPlan> listInsPlans = Crud.InsPlanCrud.SelectMany(command);
			command="UPDATE insplan SET ClaimsUseUCR=1";
			Db.NonQ(command);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			listInsPlans.ForEach(x => { //log insplan ClaimsUseUCR change.
				InsEditLogs.MakeLogEntry("ClaimsUseUCR",Security.CurUser.UserNum,"0","1",InsEditLogType.InsPlan,
					x.PlanNum,0,x.GroupNum+" - "+x.GroupName);
			});
			return listInsPlans.Count;
		}
		
		public static List<InsPlan> GetByCarrierName(string carrierName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsPlan>>(MethodBase.GetCurrentMethod(),carrierName);
			}
			string command="SELECT * FROM insplan WHERE CarrierNum IN (SELECT CarrierNum FROM carrier WHERE CarrierName='"+POut.String(carrierName)+"')";
			return Crud.InsPlanCrud.SelectMany(command);
		}

		public static List<long> GetPlanNumsByCarrierNum(long carrierNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),carrierNum);
			}
			string command="SELECT PlanNum FROM insplan WHERE CarrierNum="+POut.Long(carrierNum);
			DataTable table=Db.GetTable(command);
			List<long> planNums=new List<long>();
			for(int i=0;i<table.Rows.Count;i++) {
				planNums.Add(PIn.Long(table.Rows[i]["PlanNum"].ToString()));
			}
			return planNums;
		}

		public static List<InsPlan> GetAllByCarrierNum(long carrierNum) {
			//No need to check RemotingRole; no call to db.
			return GetAllByCarrierNums(carrierNum);
		}

		public static List<InsPlan> GetAllByCarrierNums(params long[] arrayCarrierNums) {
			if(arrayCarrierNums.IsNullOrEmpty()) {
				return new List<InsPlan>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsPlan>>(MethodBase.GetCurrentMethod(),arrayCarrierNums);
			}
			string command=$"SELECT * FROM insplan WHERE CarrierNum IN({string.Join(",",arrayCarrierNums.Select(x => POut.Long(x)))})";
			return Crud.InsPlanCrud.SelectMany(command);
		}
		
		public static void UpdateCobRuleForAll(EnumCobRule cobRule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cobRule);
				return;
			}
			string command = "SELECT * FROM insplan WHERE CobRule != "+POut.Int((int)cobRule);
			List<InsPlan> listInsPlans = Crud.InsPlanCrud.SelectMany(command);
			command="UPDATE insplan SET CobRule="+POut.Int((int)cobRule);
			Db.NonQ(command);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			listInsPlans.ForEach(x => {
				InsEditLogs.MakeLogEntry("CobRule",Security.CurUser.UserNum,x.CobRule.ToString(),POut.Int((int)cobRule),
					InsEditLogType.InsPlan,x.PlanNum,0,x.GroupNum+" - "+x.GroupName);
			});
		}

		///<summary>Checks preference and insurance plan settings to determine if the insurance plan uses UCR fees for exclusions.</summary>
		public static bool UsesUcrFeeForExclusions(InsPlan insPlan) {
			if(insPlan==null) {
				return false;
			}
			if(insPlan.ExclusionFeeRule==ExclusionRule.UseUcrFee
				|| (insPlan.ExclusionFeeRule==ExclusionRule.PracticeDefault && PrefC.GetBool(PrefName.InsPlanUseUcrFeeForExclusions)))
			{
				return true;
			}
			return false;
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching planNum as FKey and are related to InsPlan.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the InsPlan table type.</summary>
		public static void ClearFkey(long planNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planNum);
				return;
			}
			Crud.InsPlanCrud.ClearFkey(planNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching planNums as FKey and are related to InsPlan.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the InsPlan table type.</summary>
		public static void ClearFkey(List<long> listPlanNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPlanNums);
				return;
			}
			Crud.InsPlanCrud.ClearFkey(listPlanNums);
		}

		///<summary>Returns the ortho auto proc code override associated to the plan passed in or returns the default codeNum via pref.</summary>
		public static long GetOrthoAutoProc(InsPlan planCur) {
			//No need to check RemotingRole; no call to db.
			if(planCur.OrthoAutoProcCodeNumOverride != 0) {
				return planCur.OrthoAutoProcCodeNumOverride;
			}
			else {
				return PrefC.GetLong(PrefName.OrthoAutoProcCodeNum);
			}
		}

		///<summary>Searches all appointments for the given invalid InsPlanNum. Sets appointment.Insplan1=0 and appointment.Insplan2=0.
		///This method assumes the planNum is invalid (does not exist in insplan table).</summary>
		public static void ResetAppointmentInsplanNum(long planNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planNum);
				return;
			}
			string command=$"SELECT * FROM appointment WHERE appointment.InsPlan1={planNum} OR appointment.InsPlan2={planNum}";
			List<Appointment> listAppts=Crud.AppointmentCrud.SelectMany(command);
			if(listAppts.Count==0) {
				return;
			}
			List<Appointment> listApptsNew=new List<Appointment>();
			Appointment aptNew;
			//Clear out the planNum from each of the appointments.
			foreach(Appointment apt in listAppts) {
				aptNew=apt.Copy();
				if(aptNew.InsPlan1==planNum) {
					aptNew.InsPlan1=0;
				}
				if(aptNew.InsPlan2==planNum) {
					aptNew.InsPlan2=0;
				}
				listApptsNew.Add(aptNew);
			}
			//Update the changes
			Appointments.Sync(listApptsNew,listAppts,0);
		}
	}
}