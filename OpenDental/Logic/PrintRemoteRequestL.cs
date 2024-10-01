using Amazon.Auth.AccessControlPolicy;
using Health.Direct.Common.Extensions;
using Newtonsoft.Json;
using OpenDentBusiness;
using OpenDentBusiness.Crud;
using OpenDentBusiness.SheetFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace OpenDental {
	public class PrintRemoteRequestL {

		#region Process Signal
		///<summary>Processes print signals that target this computer. Can handle empty / null lists.</summary>
		public static void ProcessCompPrintSignal(List<Signalod> listSignalods){
			if(listSignalods.IsNullOrEmpty()) { 
				return;
			}
			for(int i=0;i<listSignalods.Count;i++) {
				Signalod signalod=listSignalods[i];
				//If the signal is a print signal, has a computer as the fkey type, and the fkey is this computer.
				if(signalod.IType==InvalidType.Print && signalod.FKeyType==KeyType.Computer && signalod.FKey==Computers.GetCur().ComputerNum){
					PrintRemoteRequest printRemoteRequest=JsonConvert.DeserializeObject<PrintRemoteRequest>(signalod.MsgValue);
					try{
						//Process the print request.
						ProcessRemotePrintRequest(printRemoteRequest);
					}
					catch(Exception ex){
						//Insert an error signal to send back to ODTouch.
						MobileNotifications.InsertPrintError(printRemoteRequest.FKey,EnumAppTarget.ODTouch,ex.Message);
					}
				}
			}
		}

		///<summary>Processes individual print requests.</summary>
		public static bool ProcessRemotePrintRequest(PrintRemoteRequest printRemoteRequest){
			//Since this is a thread-static property that favors the thread static variable, it will be null if no user is truly logged in.
			bool wasUserLoggedInAndDifferent=Security.CurUser!=null && Security.CurUser.UserNum!=printRemoteRequest.RequestUserNum;
			//Set the cur user so the userPref cache can be appropriately set, along side any security log / security permission checks.
			//Since this will be executed on a thread, the active user for this computer should not change.
			if(printRemoteRequest.RequestUserNum!=0){
				Userod userod=Userods.GetUserByUserNumNoCache(printRemoteRequest.RequestUserNum);
				Security.SetUserCurT(userod);
			}
			switch(printRemoteRequest.PrintRequestType){
				case EnumPrintRequestType.Rx:
					//Parameters need to retain order.
					long patNumRx=JsonConvert.DeserializeObject<long>(printRemoteRequest.ListParameters[0]);
					RxPat rxPat=JsonConvert.DeserializeObject<RxPat>(printRemoteRequest.ListParameters[1]);
					bool isInstructions=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[2]);
					long clinicNum=JsonConvert.DeserializeObject<long>(printRemoteRequest.ListParameters[3]);
					//Attempt to print.
					TryPrintRx(patNumRx,rxPat,isInstructions,clinicNum,printerNumOverride:printRemoteRequest.PrinterNumOverride);
					break;
				case EnumPrintRequestType.PayPlan:
					//Parameters need to retain order.
					PayPlan payPlan=JsonConvert.DeserializeObject<PayPlan>(printRemoteRequest.ListParameters[0]);
					//Attempt to print.
					TryPrintPayPlan(payPlan);
					break;
				case EnumPrintRequestType.TreatPlan:
					//Parameters need to retain order.
					long patNumTxPlan=JsonConvert.DeserializeObject<long>(printRemoteRequest.ListParameters[0]);
					long clinicNumTxPlan=JsonConvert.DeserializeObject<long>(printRemoteRequest.ListParameters[1]);
					long treatPlanNum=JsonConvert.DeserializeObject<long>(printRemoteRequest.ListParameters[2]);
					bool showDiscountNotAuto=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[3]);
					bool showDiscount=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[4]);
					bool showMaxDed=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[5]);
					bool showSubTotals=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[6]);
					bool showTotals=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[7]);
					bool showCompleted=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[8]);
					bool showFees=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[9]);
					bool showIns=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[10]);
					//Get data objects
					TreatPlan treatPlan=TreatPlans.GetOne(treatPlanNum);
					treatPlan=TreatPlans.GetTreatPlanListProcTP(treatPlan);
					//Attempt to print.
					TryPrintTxPlan(treatPlan,patNumTxPlan,clinicNumTxPlan,showDiscountNotAuto,showDiscount,showMaxDed,showSubTotals,showTotals,showCompleted,showFees,showIns,printerNumOverride:printRemoteRequest.PrinterNumOverride);
					break;
				case EnumPrintRequestType.Sheet:
					//Parameters need to retain order.
					long sheetNum=JsonConvert.DeserializeObject<long>(printRemoteRequest.ListParameters[0]);
					long statementNum=JsonConvert.DeserializeObject<long>(printRemoteRequest.ListParameters[1]);
					long medLabNum=JsonConvert.DeserializeObject<long>(printRemoteRequest.ListParameters[2]);
					bool isStatement=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[3]);
					bool isRxControlled=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[4]);
					bool isSuperStatement=JsonConvert.DeserializeObject<bool>(printRemoteRequest.ListParameters[5]);
					//Get data objects
					MedLab medLab=MedLabs.GetOne(medLabNum); //Can be null
					DataSet dataSet=null;
					OpenDentBusiness.Statement statement=Statements.GetStatement(statementNum);
					if(statement==null){
						//Attempt to print.
						TryPrintSheet(sheetNum,isStatement,isRxControlled,statement,medLab,dataSet);
						break;
					}
					if(isSuperStatement || statement.LimitedCustomFamily!=EnumLimitedCustomFamily.None) {
						dataSet=AccountModules.GetSuperFamAccount(statement,doShowHiddenPaySplits:statement.IsReceipt,doExcludeTxfrs:true);
					}
					else {
						long patNum=Statements.GetPatNumForGetAccount(statement);
						dataSet=AccountModules.GetAccount(patNum,statement,doShowHiddenPaySplits:statement.IsReceipt,doExcludeTxfrs:true);
					}
					//Attempt to print.
					TryPrintSheet(sheetNum,isStatement,isRxControlled,statement,medLab,dataSet,printerNumOverride:printRemoteRequest.PrinterNumOverride);
					break;
				default: 
					throw new ApplicationException("Unsupported remote print request type.");
			}
			if(wasUserLoggedInAndDifferent){
				//Dump all the caches since we changed users and this could have polluted the caches.
				Cache.ClearCaches();//Clear all caches, as requested by the calling method
			}
			return true;
		}
		#endregion

		#region Rx
		public static bool TryPrintRx(long patNum,RxPat rxPat,bool isInstructions,long clinicNum,long printerNumOverride=0){
			//Logic taken from FormRxEdit.
			SheetDef sheetDef;
			if(isInstructions) {
				sheetDef=SheetDefs.GetInternalOrCustom(SheetInternalType.RxInstruction);
			}
			else {
				sheetDef=SheetDefs.GetSheetsDefault(SheetTypeEnum.Rx,clinicNum);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,patNum);
			SheetParameter.SetParameter(sheet,"RxNum",rxPat.RxNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			if(!SheetPrinting.PrintRx(sheet,rxPat,isRemotePrint:true,printerNumOverride:printerNumOverride)) {
				return false;
			}
			return true;
		}
		#endregion

		#region Pay Plan
		public static void TryPrintPayPlan(PayPlan payPlan,long printerNumOverride=0) {
			//Logic taken from FormpayPlanDynamic.
			if(PrefC.GetBool(PrefName.PayPlansUseSheets)) {
				DynamicPaymentPlanModuleData dynamicPaymentPlanModuleData=PayPlanEdit.GetDynamicPaymentPlanModuleData(payPlan);
				string validationErrors=PayPlanEdit.ValidateDynamicPaymentPlanTerms(
				PayPlanEdit.GetPayPlanTerms(dynamicPaymentPlanModuleData.PayPlan,dynamicPaymentPlanModuleData.ListPayPlanLinks),
					isNew:false,
					dynamicPaymentPlanModuleData.PayPlan.IsLocked,
					dynamicPaymentPlanModuleData.PayPlan.APR!=0,
					dynamicPaymentPlanModuleData.ListPayPlanLinks.Count);
				if(!string.IsNullOrWhiteSpace(validationErrors)){
					throw new ApplicationException(validationErrors);
				}
				Sheet sheet=PayPlanToSheet(payPlan,dynamicPaymentPlanModuleData);
				SheetPrinting.Print(sheet,isPrintRemote:true,printerNumOverride:printerNumOverride);
			}
			else {
				throw new ApplicationException("Report complex forms not supported for remote printing.");
			}
		}

		///<summary>Creates a new sheet from a given Pay plan.</summary>
		public static Sheet PayPlanToSheet(PayPlan payPlan,DynamicPaymentPlanModuleData dynamicPaymentPlanModuleData) {
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.PaymentPlan),dynamicPaymentPlanModuleData.Patient.PatNum);
			sheet.Parameters.Add(new SheetParameter(true,"payplan") { ParamValue=payPlan });
			sheet.Parameters.Add(new SheetParameter(true,"Principal") { ParamValue=dynamicPaymentPlanModuleData.ListPayPlanProductionEntries.Sum(x => (x.AmountOverride==0 ? x.AmountOriginal : x.AmountOverride)).ToString("n") });
			sheet.Parameters.Add(new SheetParameter(true,"totalFinanceCharge") { ParamValue=dynamicPaymentPlanModuleData.TotalInterest });
			sheet.Parameters.Add(new SheetParameter(true,"totalCostOfLoan") {ParamValue=(dynamicPaymentPlanModuleData.ListPayPlanProductionEntries.Sum(x => (x.AmountOverride==0 ? x.AmountOriginal : x.AmountOverride))+(decimal)dynamicPaymentPlanModuleData.TotalInterest).ToString("n")});
			SheetFiller.FillFields(sheet);
			return sheet;
		}
		#endregion

		#region Treatment Plan
		public static void TryPrintTxPlan(TreatPlan treatPlan,long patNum,long clinicNum,bool showDiscountNotAuto,bool showDiscount,bool showMaxDed,bool showSubTotals, 
		bool showTotals, bool showCompleted, bool showFees, bool showIns,long printerNumOverride=0){
			//Logic taken from ControlTreat.
			Sheet sheet=TreatPlanToSheet(treatPlan,patNum,clinicNum,showDiscountNotAuto,showDiscount,showMaxDed,showSubTotals,showTotals,showCompleted,showFees,showIns);
			SheetPrinting.Print(sheet,isPrintRemote:true,printerNumOverride:printerNumOverride);
		}

		///<summary>Setting defaults based on the TxPlan module.</summary>
		private static Sheet TreatPlanToSheet(TreatPlan treatPlan,long patNum,long clinicNum,bool showDiscountNotAuto,bool showDiscount,bool showMaxDed,bool showSubTotals, 
		bool showTotals, bool showCompleted, bool showFees, bool showIns ) {
			Sheet sheetTP=SheetUtil.CreateSheet(SheetDefs.GetSheetsDefault(SheetTypeEnum.TreatmentPlan,clinicNum),patNum);
			sheetTP.Parameters.Add(new SheetParameter(true,"TreatPlan") { ParamValue=treatPlan });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowDiscountNotAutomatic") { ParamValue=showDiscountNotAuto });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowDiscount") { ParamValue=showDiscount });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowMaxDed") { ParamValue=showMaxDed });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowSubTotals") { ParamValue=showSubTotals });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowTotals") { ParamValue=showTotals });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowCompleted") { ParamValue=showCompleted });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowFees") { ParamValue=showFees });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowIns") { ParamValue=showIns });
			sheetTP.Parameters.Add(new SheetParameter(true,"toothChartImg") { ParamValue=SheetPrinting.GetToothChartHelper(patNum,showCompleted,treatPlan)});
			SheetFiller.FillFields(sheetTP);
			SheetUtil.CalculateHeights(sheetTP);
			return sheetTP;
		}
		#endregion

		#region Sheets
		public static void TryPrintSheet(long sheetNum,bool isStatement,bool IsRxControlled,OpenDentBusiness.Statement StatementCur,MedLab MedLabCur,DataSet dataSet,long printerNumOverride=0){
			//Logic taken from FormSheetFillEdit.
			Sheet sheet=Sheets.GetSheet(sheetNum);
			if(!SheetUtil.ValidateStateField(sheet)) {
				return;
			}
			string errorString=SheetUtil.FixFontsForPdf(sheet,true);// validate fonts before printing/creating PDF
			if(isStatement) {
				SheetPrinting.Print(sheet,dataSet,1,IsRxControlled,StatementCur,MedLabCur,isPrintRemote:true,printerNumOverride:printerNumOverride);
			}
			else {
				SheetPrinting.Print(sheet,1,IsRxControlled,StatementCur,MedLabCur,isPrintRemote:true,printerNumOverride:printerNumOverride);
			}
			//So the message gets returned to the device making the request, and we still successfully print. This needs to be done last.
			if(!String.IsNullOrWhiteSpace(errorString)){
				throw new Exception(errorString);
			}
		}
		#endregion
	}
}
