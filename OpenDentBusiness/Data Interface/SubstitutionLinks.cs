using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SubstitutionLinks{
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static void Update(SubstitutionLink substitutionLink){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),substitutionLink);
				return;
			}
			Crud.SubstitutionLinkCrud.Update(substitutionLink);
		}		

		///<summary></summary>
		public static void Delete(long substitutionLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),substitutionLinkNum);
				return;
			}
			Crud.SubstitutionLinkCrud.Delete(substitutionLinkNum);
		}
		*/

		///<summary>Gets one SubstitutionLink from the db.</summary>
		public static SubstitutionLink GetOne(long substitutionLinkNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<SubstitutionLink>(MethodBase.GetCurrentMethod(),substitutionLinkNum);
			}
			return Crud.SubstitutionLinkCrud.SelectOne(substitutionLinkNum);
		}

		///<summary></summary>
		public static List<SubstitutionLink> GetAllForPlans(List<InsPlan> listInsPlans) {
			//No need to check MiddleTierRole; no call to db.
			return GetAllForPlans(listInsPlans.Select(x => x.PlanNum).ToArray());
		}

		///<summary></summary>
		public static List<SubstitutionLink> GetAllForPlans(params long[] planNumArray){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SubstitutionLink>>(MethodBase.GetCurrentMethod(),planNumArray);
			}
			if(planNumArray.Length==0) {
				return new List<SubstitutionLink>();
			}
			List <long> listPlanNums=new List<long>(planNumArray);
			string command="SELECT * FROM substitutionlink WHERE PlanNum IN("+String.Join(",",listPlanNums.Select(x => POut.Long(x)))+")";
			return Crud.SubstitutionLinkCrud.SelectMany(command);
		}

		///<summary>Inserts, updates, or deletes the passed in list against the stale list listOld.  Returns true if db changes were made.</summary>
		public static bool Sync(List<SubstitutionLink> listSubstitutionLinksNew,List<SubstitutionLink> listSubstitutionLinksOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listSubstitutionLinksNew,listSubstitutionLinksOld);
			}
			return Crud.SubstitutionLinkCrud.Sync(listSubstitutionLinksNew,listSubstitutionLinksOld);
		}
		
		public static List<SubstitutionLink> FilterSubLinksByCodeNum(long codeNum,List<SubstitutionLink> listSubstitutionLinks) {
			if(listSubstitutionLinks is null) {
				return new List<SubstitutionLink>();
			}
			return listSubstitutionLinks.Where(x=>x.CodeNum==codeNum).ToList();
		}

		///<summary>Follows documented hierarchy to return a sub link based on substitution condition. Function checks that the list of sublinks is already filtered. Can return null. </summary>
		public static SubstitutionLink GetSubLinkByHierarchy(ProcedureCode procedureCode,string strToothNum,List<SubstitutionLink> listSubstitutionLinks) {
			SubstitutionLink substitutionLink=null;
			//Make a copy of the list in case we need to filter it.
			List<SubstitutionLink> listSubstitutionLinksThisCode=FilterSubLinksByCodeNum(procedureCode.CodeNum,listSubstitutionLinks);
			//If any of the conditions are 'Never' then we return null here
			if(listSubstitutionLinksThisCode.Any(x=>x.SubstOnlyIf==SubstitutionCondition.Never)) {
				substitutionLink=listSubstitutionLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.Never);			
			}
			//if a tooth is a second molar, it is also considered a molar and so we need to check for this scenario first
			else if(listSubstitutionLinksThisCode.Any(x=>x.SubstOnlyIf==SubstitutionCondition.SecondMolar) && Tooth.IsSecondMolar(strToothNum)) {
				substitutionLink=listSubstitutionLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.SecondMolar);
			}
			else if(listSubstitutionLinksThisCode.Any(x=>x.SubstOnlyIf==SubstitutionCondition.Molar) && Tooth.IsMolar(strToothNum)) {
				substitutionLink=listSubstitutionLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.Molar);
			}
			else if(listSubstitutionLinksThisCode.Any(x=>x.SubstOnlyIf==SubstitutionCondition.Posterior) && Tooth.IsPosterior(strToothNum)) {
				substitutionLink=listSubstitutionLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.Posterior);
			}
			else {
				substitutionLink=listSubstitutionLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.Always);
			}	
			return substitutionLink;
		}

		public static bool HasSubstCodeForPlan(InsPlan insPlan,long codeNum,List<SubstitutionLink> listSubstitutionLinks) {
			//No need to check MiddleTierRole; no call to db.
			if(insPlan.CodeSubstNone) {
				return false;
			}
			return !listSubstitutionLinks.Exists(x => x.PlanNum==insPlan.PlanNum && x.CodeNum==codeNum && x.SubstOnlyIf==SubstitutionCondition.Never);
		}

		///<summary>Returns true if the procedure has a substitution code for the give tooth and InsPlans.</summary>
		public static bool HasSubstCodeForProcCode(ProcedureCode procedureCode,string strToothNum,List<SubstitutionLink> listSubstitutionLinks,
			List<InsPlan> listInsPlansPat) 
		{
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<listInsPlansPat.Count;i++) {
				//Check to see if any allow substitutions.
				if(!HasSubstCodeForPlan(listInsPlansPat[i],procedureCode.CodeNum,listSubstitutionLinks)) {
					continue;
				}
				long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(procedureCode.ProcCode,strToothNum,listInsPlansPat[i].PlanNum,listSubstitutionLinks);//for posterior composites
				if(procedureCode.CodeNum!=subCodeNum && subCodeNum>0) {
					return true;
				}
			}
			return false;
		}

		///<summary>Inserts a copy of all of the planNumOld SubstitutionLinks with the planNumNew. This should be done every time a new insplan gets created
		///and you want to maintain the SubstitutionLink of the old insplan.</summary>
		public static void CopyLinksToNewPlan(long planNumNew,long planNumOld) {
			//No need to check MiddleTierRole; no call to db.
			//Get a list of the sub links of the old insplan. After the foreach loop below, this list will no longer contain the sub links for the old insplan.
			List<SubstitutionLink> listSubstitutionLinksOfOldPlan=SubstitutionLinks.GetAllForPlans(planNumOld);
			for(int i=0;i<listSubstitutionLinksOfOldPlan.Count;i++) {
				//Only change the old planNum with the new planNum. Insert will "create" a new SubstitutionLink with a new primary key. 
				listSubstitutionLinksOfOldPlan[i].PlanNum=planNumNew;
			}
			InsertMany(listSubstitutionLinksOfOldPlan);
		}

		///<summary></summary>
		public static long Insert(SubstitutionLink substitutionLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				substitutionLink.SubstitutionLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),substitutionLink);
				return substitutionLink.SubstitutionLinkNum;
			}
			return Crud.SubstitutionLinkCrud.Insert(substitutionLink);
		}

		public static void InsertMany(List<SubstitutionLink> listSubstitutionLinks) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSubstitutionLinks);
				return;
			}
			Crud.SubstitutionLinkCrud.InsertMany(listSubstitutionLinks);
		}
	}
}