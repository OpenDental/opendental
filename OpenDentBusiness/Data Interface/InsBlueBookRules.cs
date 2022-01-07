using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class InsBlueBookRules{
		#region Get Methods
		///<summary>Gets all insbluebookrules from db as list.</summary>
		public static List<InsBlueBookRule> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsBlueBookRule>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT insbluebookrule.* FROM insbluebookrule";
			return Crud.InsBlueBookRuleCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary>Updates an insbluebookrule to the db if it has changed.</summary>
		public static void Update(InsBlueBookRule insBlueBookRule,InsBlueBookRule oldInsBlueBookRule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insBlueBookRule,oldInsBlueBookRule);
				return;
			}
			Crud.InsBlueBookRuleCrud.Update(insBlueBookRule,oldInsBlueBookRule);
		}
		#endregion Modification Methods

		#region Misc Methods
		///<summary>Returns true if a date limitation applies to the rule's InsBlueBookRuleType. Types InsuranceCarrierGroup, InsuranceCarrier, GroupNumber, and InsurancePlan.</summary>
		public static bool IsDateLimitedType(InsBlueBookRule rule) {
			return (ListTools.In(rule.RuleType,
				InsBlueBookRuleType.InsuranceCarrierGroup,
				InsBlueBookRuleType.InsuranceCarrier,
				InsBlueBookRuleType.GroupNumber,
				InsBlueBookRuleType.InsurancePlan));
		}
		#endregion Misc Methods

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<InsBlueBookRule> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsBlueBookRule>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM insbluebookrule WHERE PatNum = "+POut.Long(patNum);
			return Crud.InsBlueBookRuleCrud.SelectMany(command);
		}
		
		///<summary>Gets one InsBlueBookRule from the db.</summary>
		public static InsBlueBookRule GetOne(long insBlueBookRuleNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<InsBlueBookRule>(MethodBase.GetCurrentMethod(),insBlueBookRuleNum);
			}
			return Crud.InsBlueBookRuleCrud.SelectOne(insBlueBookRuleNum);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static long Insert(InsBlueBookRule insBlueBookRule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				insBlueBookRule.InsBlueBookRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),insBlueBookRule);
				return insBlueBookRule.InsBlueBookRuleNum;
			}
			return Crud.InsBlueBookRuleCrud.Insert(insBlueBookRule);
		}
		///<summary></summary>
		public static void Update(InsBlueBookRule insBlueBookRule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insBlueBookRule);
				return;
			}
			Crud.InsBlueBookRuleCrud.Update(insBlueBookRule);
		}
		///<summary></summary>
		public static void Delete(long insBlueBookRuleNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insBlueBookRuleNum);
				return;
			}
			Crud.InsBlueBookRuleCrud.Delete(insBlueBookRuleNum);
		}
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/
	}
}