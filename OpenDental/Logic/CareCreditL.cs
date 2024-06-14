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
			TryLaunchCareCreditPage(CareCreditLandingPage.HM,null,0,0,CareCredit.IsMerchantNumberByProv,serviceType:EnumCareCreditPrefillServiceType.A,userName:userName,merchantNum:merchantNum);
		}

		///<summary>Opens the CareCredit Admin page.</summary>
		public static void LaunchAdminPage(long provNum,long clinicNum,bool isProvOverride,string userName=null) {
			TryLaunchCareCreditPage(CareCreditLandingPage.HM,null,provNum,clinicNum,isProvOverride:isProvOverride,serviceType:EnumCareCreditPrefillServiceType.A,userName:userName);
		}

		///<summary>Opens the CareCredit Credit Application request page.</summary>
		public static void LaunchCreditApplicationPage(Patient patient,long provNum,long clinicNum,double purchaseAmt,double estimatedFeeAmt,
			string practiceMemo=null,string userName=null,string oldRefId=null)
		{
			TryLaunchCareCreditPage(CareCreditLandingPage.AP,patient,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,purchaseAmt:purchaseAmt,
				estimatedFeeAmt:estimatedFeeAmt,noteMemo:practiceMemo,userName:userName,oldRefId:oldRefId);
		}

		///<summary>Opens the CareCredit Lookup request page.</summary>
		public static void LaunchLookupPage(Patient patient,long provNum,long clinicNum,string userName=null,string oldRefId=null) {
			TryLaunchCareCreditPage(CareCreditLandingPage.LU,patient,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,userName:userName,oldRefId:oldRefId,serviceType:EnumCareCreditPrefillServiceType.L);
		}

		///<summary>Returns the Purchase request webpage url.</summary>
		public static string GetPurchasePageUrl(Patient patient,long provNum,long clinicNum,double purchaseAmt,double estimatedFeeAmt,string invoiceNum=null,
			string practiceMemo=null,string promoCode=null,string userName=null,string oldRefId=null,long payNum=0) {
			return TryLaunchCareCreditPage(CareCreditLandingPage.PR,patient,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,purchaseAmt:purchaseAmt,
				estimatedFeeAmt:estimatedFeeAmt,invoiceNumber:invoiceNum,noteMemo:practiceMemo,promoCode:promoCode,userName:userName,oldRefId:oldRefId,payNum:payNum,
				doShowPage:false);
		}

		///<summary>Returns the QuickScreen request webpage url.</summary>
		public static string GetQSPageUrl(Patient patient,long provNum,long clinicNum,double estimatedFeeAmt,string invoiceNum=null,string practiceMemo=null,string userName=null) {
			CareCreditWebResponse careCreditWebResponseBatchOriginal=CareCreditWebResponses.GetOriginalCompletedBatchForPatNum(patient);
			string refId=careCreditWebResponseBatchOriginal?.RefNumber??null;
			return TryLaunchCareCreditPage(CareCreditLandingPage.QS,patient,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,estimatedFeeAmt:estimatedFeeAmt,
				invoiceNumber:invoiceNum,noteMemo:practiceMemo,userName:userName,oldRefId:refId,doShowPage:false);
		}

		///<summary>Opens the CareCredit quickscreen individual request page.</summary>
		public static void LaunchQuickScreenIndividualPage(Patient patient,long provNum,long clinicNum,double estimatedFeeAmt,string invoiceNum=null,
			string practiceMemo=null,string userName=null)
		{
			CareCreditWebResponse careCreditWebResponseBatchOriginal=CareCreditWebResponses.GetOriginalCompletedBatchForPatNum(patient);
			string refId=careCreditWebResponseBatchOriginal?.RefNumber??null;
			TryLaunchCareCreditPage(CareCreditLandingPage.QS,patient,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,estimatedFeeAmt:estimatedFeeAmt,
				invoiceNumber:invoiceNum,noteMemo:practiceMemo,userName:userName,oldRefId:refId);
		}

		///<summary>Opens the CareCredit Refund request page.</summary>
		public static void RefundTransaction(CareCreditWebResponse careCreditWebResponse) {
			if(careCreditWebResponse==null) {
				MsgBox.Show("FormPayment","CareCreditWebResponse is null or no longer exists. Request could not be completed.");
				return;
			}
			string practiceMemo=$"Refund PayNum {careCreditWebResponse.PayNum}";
			TryLaunchCareCreditPage(CareCreditLandingPage.RF,Patients.GetPat(careCreditWebResponse.PatNum),0,careCreditWebResponse.ClinicNum,CareCredit.IsMerchantNumberByProv,
				refundAmt:careCreditWebResponse.Amount,invoiceNumber:careCreditWebResponse.PayNum.ToString(),noteMemo:practiceMemo,userName:CareCredit.GetUserName(careCreditWebResponse),
				oldRefId:careCreditWebResponse.RefNumber,merchantNum:careCreditWebResponse.MerchantNumber);
		}

		///<summary>Opens the CareCredit Refund request page.</summary>
		public static void LaunchRefundPage(Patient patient,long provNum,long clinicNum,double refundAmt,string invoiceNum=null,string practiceMemo=null,
			string promoCode=null,string userName=null,string oldRefId=null) 
		{
			TryLaunchCareCreditPage(CareCreditLandingPage.RF,patient,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,refundAmt:refundAmt,
				invoiceNumber:invoiceNum,noteMemo:practiceMemo,promoCode:promoCode,userName:userName,oldRefId:oldRefId);
		}

		///<summary>Opens the CareCredit Reports page.</summary>
		public static void LaunchReportsPage(long provNum,long clinicNum,string userName=null) {
			TryLaunchCareCreditPage(CareCreditLandingPage.HM,null,provNum,clinicNum,CareCredit.IsMerchantNumberByProv,userName:userName,serviceType:EnumCareCreditPrefillServiceType.R);
		}

		///<summary>Tries to send prefill request with the specified landing page and launches the CareCredit portal. Set doShowPage to false if you don't want the page to be displayed. Displays error messages to user.</summary>
		private static string TryLaunchCareCreditPage(CareCreditLandingPage careCreditLandingPage,Patient patient,long provNum,long clinicNum,bool isProvOverride,
			string invoiceNumber=null,double purchaseAmt=0,double refundAmt=0,string noteMemo=null,string promoCode=null,string userName=null,
			double estimatedFeeAmt=0,string oldRefId=null,EnumCareCreditPrefillServiceType serviceType=EnumCareCreditPrefillServiceType.C,long payNum=0,string merchantNum=null,bool doShowPage=true) 
		{
			if(string.IsNullOrEmpty(merchantNum)) {//No merchant number passed in
				try {
					merchantNum=CareCredit.GetMerchantNumber(clinicNum,provNum,isProvOverride);
				}
				catch(ODException oDException) {
					if(isProvOverride) {
						provNum=TryGetNewProvOrRegisterProv(provNum,clinicNum);
						if(provNum>-1) {//a new provider was selected
							return TryLaunchCareCreditPage(careCreditLandingPage,patient,provNum,clinicNum,isProvOverride,invoiceNumber,purchaseAmt,refundAmt,noteMemo,promoCode,userName,
								estimatedFeeAmt,oldRefId,serviceType,payNum:payNum,doShowPage:doShowPage);//No need to pass merchantNum because a new provider was selected.
						}
					}
					else {
						FriendlyException.Show(oDException.Message,oDException);
					}
					return "";
				}
				catch(Exception ex) {
					FriendlyException.Show("Unexpected Error. Please call support.",ex);
					return "";
				}
			}
			try {
				string pageUrl=CareCredit.SendPrefillReturnPageUrl(careCreditLandingPage,patient,invoiceNumber,purchaseAmt,refundAmt,noteMemo,promoCode,userName,estimatedFeeAmt,
					oldRefId,merchantNum,serviceType,payNum,clinicNum:clinicNum);
				if(doShowPage) {
					CareCredit.ShowPage(pageUrl);
				}
				return pageUrl;
			}
			catch(CareCredit.CareCreditException careCreditException) {
				string errorMsg="CareCredit returned ";
				switch(careCreditException.ErrorCode) {
					case 400:
						errorMsg+="an Invalid Input";
						break;
					case 401:
						errorMsg+="an Invalid Access Token";
						break;
					case 404:
						errorMsg+="a Request URL Not Found";
						break;
					case 422:
						errorMsg+="a status of Training Suspended. Please contact CareCredit for more information.";
						break;
					case 500:
						errorMsg+="an Internal Server Error";
						break;
					default:
						errorMsg+="an Unknown Error";
						break;
				}
				FriendlyException.Show(errorMsg,careCreditException);
			}
			catch(ODException oDException) {
				FriendlyException.Show(oDException.Message,oDException);
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
			InputBox inputBox=new InputBox(msg,listOptions);
			inputBox.Text="CareCredit";//Don't translate.
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return provNumNew;
			}
			if(inputBox.SelectedIndex==0) {
				List<Provider> listProvsWithOverrides=Providers.GetProvsWithCareCreditOverrides(clinicNum);
				if(listProvsWithOverrides.IsNullOrEmpty()) {
					MsgBox.Show("CareCreditL","No providers with CareCredit Merchant Numbers found.");
					return provNumNew;
				}
				FrmProviderPick frmProvPick=new FrmProviderPick(listProvsWithOverrides);
				frmProvPick.ShowDialog();
				if(!frmProvPick.IsDialogOK) {
					return provNumNew;
				}
				provNumNew=frmProvPick.ProvNumSelected;
			}
			else if(inputBox.SelectedIndex==1) {
				CareCredit.ShowPage(CareCredit.ProviderSignupURL);
			}
			return provNumNew;
		}

	}
}
