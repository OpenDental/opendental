using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class WebSchedCarrierRules{

		#region Methods - Get

		///<summary>Returns a list of WebSchedCarrierRules for the passed in clinic, orders the list by CarrierName.</summary>
		public static List<WebSchedCarrierRule> GetWebSchedCarrierRulesForClinic(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebSchedCarrierRule>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command = "SELECT * FROM webschedcarrierrule WHERE ClinicNum = "+POut.Long(clinicNum)+" ORDER BY CarrierName";
			return Crud.WebSchedCarrierRuleCrud.SelectMany(command);
		}

		///<summary>Returns a list of WebSchedCarrierRules for the passed in list of clinics.</summary>
		public static List<WebSchedCarrierRule> GetWebSchedCarrierRulesForClinics(List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebSchedCarrierRule>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command = "SELECT * FROM webschedcarrierrule WHERE ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+")";
			return Crud.WebSchedCarrierRuleCrud.SelectMany(command);
		}

		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(WebSchedCarrierRule webSchedCarrierRule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				webSchedCarrierRule.WebSchedCarrierRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webSchedCarrierRule);
				return webSchedCarrierRule.WebSchedCarrierRuleNum;
			}
			return Crud.WebSchedCarrierRuleCrud.Insert(webSchedCarrierRule);
		}

		public static void InsertMany(List<WebSchedCarrierRule> listWebSchedCarrierRules) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listWebSchedCarrierRules);
				return;
			}
			Crud.WebSchedCarrierRuleCrud.InsertMany(listWebSchedCarrierRules);
		}

		///<summary></summary>
		public static void Update(WebSchedCarrierRule webSchedCarrierRule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webSchedCarrierRule);
				return;
			}
			Crud.WebSchedCarrierRuleCrud.Update(webSchedCarrierRule);
		}

		public static void DeleteMany(List<long> listWebSchedCarrierRuleNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listWebSchedCarrierRuleNums);
				return;
			}
			Crud.WebSchedCarrierRuleCrud.DeleteMany(listWebSchedCarrierRuleNums);
		}
		#endregion Methods - Modify

		#region Misc Methods
		#endregion Misc Methods


	}
}