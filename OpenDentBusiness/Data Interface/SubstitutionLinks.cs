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

		///<summary>Gets one SubstitutionLink from the db.</summary>
		public static SubstitutionLink GetOne(long substitutionLinkNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<SubstitutionLink>(MethodBase.GetCurrentMethod(),substitutionLinkNum);
			}
			return Crud.SubstitutionLinkCrud.SelectOne(substitutionLinkNum);
		}

		///<summary></summary>
		public static void Update(SubstitutionLink substitutionLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),substitutionLink);
				return;
			}
			Crud.SubstitutionLinkCrud.Update(substitutionLink);
		}

		///<summary></summary>
		public static void Delete(long substitutionLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),substitutionLinkNum);
				return;
			}
			Crud.SubstitutionLinkCrud.Delete(substitutionLinkNum);
		}

		

		
		*/

		///<summary></summary>
		public static List<SubstitutionLink> GetAllForPlans(List<InsPlan> listInsPlans) {
			//No need to check RemotingRole; no call to db.
			return GetAllForPlans(listInsPlans.Select(x => x.PlanNum).ToArray());
		}

		///<summary></summary>
		public static List<SubstitutionLink> GetAllForPlans(params long[] arrayPlanNums){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SubstitutionLink>>(MethodBase.GetCurrentMethod(),arrayPlanNums);
			}
			if(arrayPlanNums.Length==0) {
				return new List<SubstitutionLink>();
			}
			List <long> listPlanNums=new List<long>(arrayPlanNums);
			string command="SELECT * FROM substitutionlink WHERE PlanNum IN("+String.Join(",",listPlanNums.Select(x => POut.Long(x)))+")";
			return Crud.SubstitutionLinkCrud.SelectMany(command);
		}

		///<summary>Inserts, updates, or deletes the passed in list against the stale list listOld.  Returns true if db changes were made.</summary>
		public static bool Sync(List<SubstitutionLink> listNew,List<SubstitutionLink> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.SubstitutionLinkCrud.Sync(listNew,listOld);
		}
		
		public static List<SubstitutionLink> FilterSubLinksByCodeNum(long codeNum,List<SubstitutionLink> listSubLinks) {
			if(listSubLinks is null) {
				return new List<SubstitutionLink>();
			}
			return listSubLinks.Where(x=>x.CodeNum==codeNum).ToList();
		}

		///<summary>Follows documented hierarchy to return a sub link based on substitution condition. Function checks that the list of sublinks is already filtered. Can return null. </summary>
		public static SubstitutionLink GetSubLinkByHierarchy(ProcedureCode proc,string toothNum,List<SubstitutionLink> listSubLinks) {
			SubstitutionLink subLink=null;
			//Make a copy of the list in case we need to filter it.
			List<SubstitutionLink> listSubLinksThisCode=FilterSubLinksByCodeNum(proc.CodeNum,listSubLinks);
			//If any of the conditions are 'Never' then we return null here
			if(listSubLinksThisCode.Any(x=>x.SubstOnlyIf==SubstitutionCondition.Never)) {
				subLink=listSubLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.Never);			
			}
			//if a tooth is a second molar, it is also considered a molar and so we need to check for this scenario first
			else if(listSubLinksThisCode.Any(x=>x.SubstOnlyIf==SubstitutionCondition.SecondMolar) && Tooth.IsSecondMolar(toothNum)) {
				subLink=listSubLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.SecondMolar);
			}
			else if(listSubLinksThisCode.Any(x=>x.SubstOnlyIf==SubstitutionCondition.Molar) && Tooth.IsMolar(toothNum)) {
				subLink=listSubLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.Molar);
			}
			else if(listSubLinksThisCode.Any(x=>x.SubstOnlyIf==SubstitutionCondition.Posterior) && Tooth.IsPosterior(toothNum)) {
				subLink=listSubLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.Posterior);
			}
			else {
				subLink=listSubLinksThisCode.FirstOrDefault(x=>x.SubstOnlyIf==SubstitutionCondition.Always);
			}	
			return subLink;
		}

		public static bool HasSubstCodeForPlan(InsPlan insPlan,long codeNum,List<SubstitutionLink> listSubLinks) {
			//No need to check RemotingRole; no call to db.
			if(insPlan.CodeSubstNone) {
				return false;
			}
			return !listSubLinks.Exists(x => x.PlanNum==insPlan.PlanNum && x.CodeNum==codeNum && x.SubstOnlyIf==SubstitutionCondition.Never);
		}

		///<summary>Returns true if the procedure has a substitution code for the give tooth and InsPlans.</summary>
		public static bool HasSubstCodeForProcCode(ProcedureCode procCode,string toothNum,List<SubstitutionLink> listSubLinks,
			List<InsPlan> listPatInsPlans) 
		{
			//No need to check RemotingRole; no call to db.
			foreach(InsPlan plan in listPatInsPlans) {
				//Check to see if any allow substitutions.
				if(HasSubstCodeForPlan(plan,procCode.CodeNum,listSubLinks)) {
					long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(procCode.ProcCode,toothNum,plan.PlanNum,listSubLinks);//for posterior composites
					if(procCode.CodeNum!=subCodeNum && subCodeNum>0) {
						return true;
					}
				}
			}
			return false;
		}

		///<summary>Inserts a copy of all of the planNumOld SubstitutionLinks with the planNumNew. This should be done every time a new insplan gets created
		///and you want to maintain the SubstitutionLink of the old insplan.</summary>
		public static void CopyLinksToNewPlan(long planNumNew,long planNumOld) {
			//No need to check RemotingRole; no call to db.
			//Get a list of the sub links of the old insplan. After the foreach loop below, this list will no longer contain the sub links for the old insplan.
			List<SubstitutionLink> listSubstLinksOfOldPlan=SubstitutionLinks.GetAllForPlans(planNumOld);
			foreach(SubstitutionLink subLink in listSubstLinksOfOldPlan) {
				//Only change the old planNum with the new planNum. Insert will "create" a new SubstitutionLink with a new primary key. 
				subLink.PlanNum=planNumNew;
			}
			InsertMany(listSubstLinksOfOldPlan);
		}

		///<summary></summary>
		public static long Insert(SubstitutionLink substitutionLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				substitutionLink.SubstitutionLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),substitutionLink);
				return substitutionLink.SubstitutionLinkNum;
			}
			return Crud.SubstitutionLinkCrud.Insert(substitutionLink);
		}

		public static void InsertMany(List<SubstitutionLink> listSubstitutionLinks) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSubstitutionLinks);
				return;
			}
			Crud.SubstitutionLinkCrud.InsertMany(listSubstitutionLinks);
		}
	}
}