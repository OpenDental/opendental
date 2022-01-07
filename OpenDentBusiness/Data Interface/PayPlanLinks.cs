using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using OpenDentBusiness;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PayPlanLinks{
		#region Get Methods
		///<summary></summary>
		public static List<PayPlanLink> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanLink>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM payplanlink WHERE PatNum = "+POut.Long(patNum);
			return Crud.PayPlanLinkCrud.SelectMany(command);
		}

		public static List<PayPlanLink> GetListForPayplan(long payplanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanLink>>(MethodBase.GetCurrentMethod(),payplanNum);
			}
			string command=$"SELECT * FROM payplanlink WHERE PayPlanNum={POut.Long(payplanNum)}";
			return Crud.PayPlanLinkCrud.SelectMany(command);
		}

		public static List<PayPlanLink> GetForPayPlans(List<long> listPayPlans) {
			if(listPayPlans.IsNullOrEmpty()) {
				return new List<PayPlanLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanLink>>(MethodBase.GetCurrentMethod(),listPayPlans);
			}
			string command=$"SELECT * FROM payplanlink WHERE PayPlanNum IN ({string.Join(",",listPayPlans.Select(x => POut.Long(x)))}) ";
			return Crud.PayPlanLinkCrud.SelectMany(command);
		}
		
		///<summary>Gets one PayPlanLink from the db.</summary>
		public static PayPlanLink GetOne(long payPlanLinkNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<PayPlanLink>(MethodBase.GetCurrentMethod(),payPlanLinkNum);
			}
			return Crud.PayPlanLinkCrud.SelectOne(payPlanLinkNum);
		}

		///<summary>Gets all of the payplanlink entries for the given fKey and linkType.</summary>
		public static List<PayPlanLink> GetForFKeyAndLinkType(long fKey,PayPlanLinkType linkType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanLink>>(MethodBase.GetCurrentMethod(),fKey,linkType);
			}
			return GetForFKeysAndLinkType(new List<long> {fKey},linkType);
		}

		///<summary>Gets all of the payplanlink entries for the given fKey and linkType.</summary>
		public static List<PayPlanLink> GetForFKeysAndLinkType(List<long> listFKeys,PayPlanLinkType linkType) {
			if(listFKeys.IsNullOrEmpty()) {
				return new List<PayPlanLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanLink>>(MethodBase.GetCurrentMethod(),listFKeys,linkType);
			}
			string command=$"SELECT * FROM payplanlink WHERE payplanlink.FKey IN ({string.Join(",",listFKeys.Select(x => POut.Long(x)))}) " +
				$"AND payplanlink.LinkType={POut.Int((int)linkType)} ";
			return Crud.PayPlanLinkCrud.SelectMany(command);
		}

		///<summary>Returns all procedure links for the list of PayPlanNums.</summary>
		public static List<PayPlanLink> GetForPayPlansAndLinkType(List<long> listPayPlanNums,PayPlanLinkType linkType) {
			if(listPayPlanNums.Count==0) {
				return new List<PayPlanLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanLink>>(MethodBase.GetCurrentMethod(),listPayPlanNums,linkType);
			}
			string command=$"SELECT * FROM payplanlink WHERE payplanlink.PayPlanNum IN ({string.Join(",",listPayPlanNums.Select(x => POut.Long(x)))}) " +
				$"AND payplanlink.LinkType={POut.Int((int)linkType)}";
			return Crud.PayPlanLinkCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Passed in list contains current list and payPlanNum gets current 
		///list from DB.</summary>
		public static void Sync(List<PayPlanLink> listPayPlanLinks,long payPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPayPlanLinks,payPlanNum);
				return;
			}
			List<PayPlanLink> listDB=GetListForPayplan(payPlanNum);
			Crud.PayPlanLinkCrud.Sync(listPayPlanLinks,listDB);
		}
		#endregion Modification Methods

		#region Insert
		///<summary></summary>
		public static long Insert(PayPlanLink payPlanLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				payPlanLink.PayPlanLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),payPlanLink);
				return payPlanLink.PayPlanLinkNum;
			}
			return Crud.PayPlanLinkCrud.Insert(payPlanLink);
		}

		///<summary>If the pay plan is linked to an ortho case and the proc is eligible for linking, it will be linked to the pay plan.
		///If linking is successful, the PayPlanLink is returned. Otherwise, return null.</summary>
		public static PayPlanLink TryLinkOrthoCaseProcToPayPlan(OrthoCaseProcLinkingData orthoCaseProcLinkingData,Procedure proc) {
			if(!orthoCaseProcLinkingData.CanProcLinkToPayPlan(proc)) {
				return null;
			}
			PayPlanLink payPlanProcLink=new PayPlanLink();
			payPlanProcLink.PayPlanNum=orthoCaseProcLinkingData.LinkedPayPlan.PayPlanNum;
			payPlanProcLink.LinkType=PayPlanLinkType.Procedure;
			payPlanProcLink.FKey=proc.ProcNum;
			payPlanProcLink.PayPlanLinkNum=PayPlanLinks.Insert(payPlanProcLink);
			orthoCaseProcLinkingData.DictPayPlanProcLinks.Add(proc.ProcNum,payPlanProcLink);
			return payPlanProcLink;
		}
		#endregion Insert

		#region Update
		///<summary></summary>
		public static void Update(PayPlanLink payPlanLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payPlanLink);
				return;
			}
			Crud.PayPlanLinkCrud.Update(payPlanLink);
		}
		#endregion Update

		#region Delete
		///<summary></summary>
		public static void Delete(long payPlanLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payPlanLinkNum);
				return;
			}
			Crud.PayPlanLinkCrud.Delete(payPlanLinkNum);
		}
		#endregion Delete
	}
}