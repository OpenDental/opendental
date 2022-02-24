using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Bridges;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public class CareCreditL {

		///<summary>Opens the CareCredit Admin page.</summary>
		public static void LaunchAdminPage(string merchantNum,string userName=null) {
			TryLaunchCareCreditPage(CareCreditLandingPage.HM,null,0,0,CareCredit.IsMerchantNumberByProv,serviceType:"A",userName:userName,merchantNum:merchantNum);
		}

		///<summary>Opens the CareCredit Admin page.</summary>
		public static void LaunchAdminPage(long provNum,long clinicNum,bool isProvOverride,string userName=null) {
			TryLaunchCareCreditPage(CareCreditLandingPage.HM,null,provNum,clinicNum,isProvOverride:isProvOverride,serviceType:"A",userName:userName);
		}

		///<summary>Opens the CareCredit Credit Application request page.</summary>
		public static void LaunchCreditApplicationPage(Patient pat,long provNum,long clinicNum,double purchaseAmt,double estimatedFeeAmt,
			string practiceMemo=null,string userName=null,string oldRefId=null)
		{
			TryLaunchCareCreditPage(CareCreditLandingPage.AP,pat,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,purchaseAmt:purchaseAmt,
				estimatedFeeAmt:estimatedFeeAmt,noteMemo:practiceMemo,userName:userName,oldRefId:oldRefId);
		}

		///<summary>Opens the CareCredit Lookup request page.</summary>
		public static void LaunchLookupPage(Patient pat,long provNum,long clinicNum,string userName=null,string oldRefId=null) {
			TryLaunchCareCreditPage(CareCreditLandingPage.LU,pat,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,userName:userName,oldRefId:oldRefId);
		}

		///<summary>Returns the Purchase request webpage url.</summary>
		public static string GetPurchasePageUrl(Patient pat,long provNum,long clinicNum,double purchaseAmt,double estimatedFeeAmt,string invoiceNum=null,
			string practiceMemo=null,string promoCode=null,string userName=null,string oldRefId=null,long payNum=0) {
			return TryLaunchCareCreditPage(CareCreditLandingPage.PR,pat,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,purchaseAmt:purchaseAmt,
				estimatedFeeAmt:estimatedFeeAmt,invoiceNumber:invoiceNum,noteMemo:practiceMemo,promoCode:promoCode,userName:userName,oldRefId:oldRefId,payNum:payNum,
				doShowPage:false);
		}

		///<summary>Opens the CareCredit quickscreen individual request page.</summary>
		public static void LaunchQuickScreenIndividualPage(Patient pat,long provNum,long clinicNum,double estimatedFeeAmt,string invoiceNum=null,
			string practiceMemo=null,string userName=null)
		{
			CareCreditWebResponse ccWebResponseBatchOriginal=CareCreditWebResponses.GetOriginalCompletedBatchForPatNum(pat.PatNum);
			string refId=ccWebResponseBatchOriginal?.RefNumber??null;
			TryLaunchCareCreditPage(CareCreditLandingPage.QS,pat,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,estimatedFeeAmt:estimatedFeeAmt,
				invoiceNumber:invoiceNum,noteMemo:practiceMemo,userName:userName,oldRefId:refId);
		}

		///<summary>Opens the CareCredit Refund request page.</summary>
		public static void RefundTransaction(CareCreditWebResponse response) {
			if(response==null) {
				MsgBox.Show("FormPayment","CareCreditWebResponse is null or no longer exists. Request could not be completed.");
				return;
			}
			string practiceMemo=$"Refund PayNum {response.PayNum}";
			TryLaunchCareCreditPage(CareCreditLandingPage.RF,Patients.GetPat(response.PatNum),0,response.ClinicNum,CareCredit.IsMerchantNumberByProv,
				refundAmt:response.Amount,invoiceNumber:response.PayNum.ToString(),noteMemo:practiceMemo,userName:CareCredit.GetUserName(response),
				oldRefId:response.RefNumber,merchantNum:response.MerchantNumber);
		}

		///<summary>Opens the CareCredit Refund request page.</summary>
		public static void LaunchRefundPage(Patient pat,long provNum,long clinicNum,double refundAmt,string invoiceNum=null,string practiceMemo=null,
			string promoCode=null,string userName=null,string oldRefId=null) 
		{
			TryLaunchCareCreditPage(CareCreditLandingPage.RF,pat,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,refundAmt:refundAmt,
				invoiceNumber:invoiceNum,noteMemo:practiceMemo,promoCode:promoCode,userName:userName,oldRefId:oldRefId);
		}

		///<summary>Opens the CareCredit Reports page.</summary>
		public static void LaunchReportsPage(long provNum,long clinicNum,string userName=null) {
			TryLaunchCareCreditPage(CareCreditLandingPage.HM,null,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,userName:userName,serviceType:"R");
		}

		///<summary>Tries to send prefill request with the specified landing page and launches the CareCredit portal. Set doShowPage to false if you don't want the page to be displayed. Displays error messages to user.</summary>
		private static string TryLaunchCareCreditPage(CareCreditLandingPage page,Patient pat,long provNum,long clinicNum,bool isProvOverride,
			string invoiceNumber=null,double purchaseAmt=0,double refundAmt=0,string noteMemo=null,string promoCode=null,string userName=null,
			double estimatedFeeAmt=0,string oldRefId=null,string serviceType="C",long payNum=0,string merchantNum=null,bool doShowPage=true) 
		{
			if(string.IsNullOrEmpty(merchantNum)) {//No merchant number passed in
				try {
					merchantNum=CareCredit.GetMerchantNumber(clinicNum,provNum,isProvOverride);
				}
				catch(ODException ex) {
					if(isProvOverride) {
						provNum=TryGetNewProvOrRegisterProv(provNum,clinicNum);
						if(provNum>-1) {//a new provider was selected
							return TryLaunchCareCreditPage(page,pat,provNum,clinicNum,isProvOverride,invoiceNumber,purchaseAmt,refundAmt,noteMemo,promoCode,userName,
								estimatedFeeAmt,oldRefId,serviceType,payNum:payNum,doShowPage:doShowPage);//No need to pass merchantNum because a new provider was selected.
						}
					}
					else {
						FriendlyException.Show(ex.Message,ex);
					}
					return "";
				}
				catch(Exception ex) {
					FriendlyException.Show("Unexpected Error. Please call support.",ex);
					return "";
				}
			}
			try {
				string pageUrl=CareCredit.SendPrefillReturnPageUrl(page,pat,invoiceNumber,purchaseAmt,refundAmt,noteMemo,promoCode,userName,estimatedFeeAmt,
					oldRefId,merchantNum,serviceType,payNum,clinicNum:clinicNum);
				if(doShowPage) {
					CareCredit.ShowPage(pageUrl);
				}
				return pageUrl;
			}
			catch(CareCreditException cce) {
				string errorMsg="CareCredit returned an ";
				switch(cce.ErrorCode) {
					case 401:
						errorMsg+="Invalid Input";
						break;
					case 500:
						errorMsg+="Internal Server Error";
						break;
					default:
						errorMsg+="Unknown Error";
						break;
				}
				FriendlyException.Show(errorMsg,cce);
			}
			catch(ODException odex) {
				FriendlyException.Show(odex.Message,odex);
			}
			catch(Exception ex) {
				FriendlyException.Show("There was an issue with the request.",ex);
			}
			return "";
		}

		///<summary>Displays inputbox to the user to decide if they want to select a new provider or get directed to register a new provider.</summary>
		private static long TryGetNewProvOrRegisterProv(long provNum,long clinicNum) {
			long provNumNew=-1;
			List<string> listOptions=new List<string>(){
					"Select a new provider",
					"Sign up for CareCredit",
				};
			string msg=$"{Lan.g("CareCreditL","Provider")} '{Providers.GetAbbr(provNum)}' "
					+Lan.g("CareCreditL","does not have a Merchant Number.\r\nPlease select from the following options or click Cancel.");
			using InputBox inputBox=new InputBox(msg,listOptions);
			inputBox.Text="CareCredit";//Don't translate.
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return provNumNew;
			}
			if(inputBox.SelectedIndex==0) {
				List<Provider> listProvsWithOverrides=Providers.GetProvsWithCareCreditOverrides(clinicNum);
				if(listProvsWithOverrides.IsNullOrEmpty()) {
					MsgBox.Show("CareCreditL","No providers with CareCredit Merchant Numbers found.");
					return provNumNew;
				}
				using FormProviderPick FormProvPick=new FormProviderPick(listProvsWithOverrides);
				if(FormProvPick.ShowDialog()!=DialogResult.OK) {
					return provNumNew;
				}
				provNumNew=FormProvPick.SelectedProvNum;
			}
			else if(inputBox.SelectedIndex==1) {
				CareCredit.ShowPage(CareCredit.ProviderSignupURL);
			}
			return provNumNew;
		}

	}
}
