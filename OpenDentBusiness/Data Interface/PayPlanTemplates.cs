using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PayPlanTemplates{
		#region Methods - Get
		///<summary>Gets all PayPlanTemplates from the db.</summary>
		public static List<PayPlanTemplate> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PayPlanTemplate>>(MethodBase.GetCurrentMethod());
			}
			string command = "SELECT * FROM payplantemplate";
			return Crud.PayPlanTemplateCrud.SelectMany(command);
		}

		public static List<PayPlanTemplate> GetMany(long clinicNum = 0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PayPlanTemplate>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command = "SELECT * FROM payplantemplate WHERE ClinicNum = "+POut.Long(clinicNum);
			return Crud.PayPlanTemplateCrud.SelectMany(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(PayPlanTemplate payPlanTemplate) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				payPlanTemplate.PayPlanTemplateNum=Meth.GetLong(MethodBase.GetCurrentMethod(),payPlanTemplate);
				return payPlanTemplate.PayPlanTemplateNum;
			}
			return Crud.PayPlanTemplateCrud.Insert(payPlanTemplate);
		}
		///<summary></summary>
		public static void Update(PayPlanTemplate payPlanTemplate) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payPlanTemplate);
				return;
			}
			Crud.PayPlanTemplateCrud.Update(payPlanTemplate);
		}
		#endregion Methods - Modify

		#region Methods - Misc
		/// <summary>Gets all of the templates from the db. Convertes them into Terms objects and then returns a list of terms. </summary>
		public static PayPlanTerms ConvertTemplateToTerms(PayPlanTemplate payPlanTemplate) {
			Meth.NoCheckMiddleTierRole();
			PayPlanTerms payPlanTerms=new PayPlanTerms();
			payPlanTerms.APR=payPlanTemplate.APR;
			payPlanTerms.PeriodPayment=(decimal)payPlanTemplate.PayAmt;
			payPlanTerms.PayCount=payPlanTemplate.NumberOfPayments;//This may need to be removed.
			payPlanTerms.RoundDec=CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
			payPlanTerms.Frequency=payPlanTemplate.ChargeFrequency;
			payPlanTerms.DownPayment=payPlanTemplate.DownPayment;
			payPlanTerms.DynamicPayPlanTPOption=payPlanTemplate.DynamicPayPlanTPOption;
			return payPlanTerms;
		}
		#endregion Methods - Misc

		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		//#region Methods - Get
		/////<summary></summary>
		//public static List<PayPlanTemplate> Refresh(long patNum){
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		return Meth.GetObject<List<PayPlanTemplate>>(MethodBase.GetCurrentMethod(),patNum);
		//	}
		//	string command="SELECT * FROM payplantemplate WHERE PatNum = "+POut.Long(patNum);
		//	return Crud.PayPlanTemplateCrud.SelectMany(command);
		//}

		/////<summary>Gets one PayPlanTemplate from the db.</summary>
		//public static PayPlanTemplate GetOne(long payPlanTemplateNum){
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		//		return Meth.GetObject<PayPlanTemplate>(MethodBase.GetCurrentMethod(),payPlanTemplateNum);
		//	}
		//	return Crud.PayPlanTemplateCrud.SelectOne(payPlanTemplateNum);
		//}
		//#endregion Methods - Get

		//#region Methods - Modify
		/////<summary></summary>
		//public static void Delete(long payPlanTemplateNum) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),payPlanTemplateNum);
		//		return;
		//	}
		//	Crud.PayPlanTemplateCrud.Delete(payPlanTemplateNum);
		//}
		//#endregion Methods - Modify
	}
}