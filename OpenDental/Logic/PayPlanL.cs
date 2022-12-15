using System.Collections.Generic;
using System.Linq;
using System.Windows;
using OpenDentBusiness;
using System;
using CodeBase;
using OpenDental.UI;
using System.Data;
using System.Text;

namespace OpenDental {
	/// <summary>Provides a central location for both PayPlan and PayPlanDynamic to use the same, or similar methods.</summary>
	public class PayPlanL {
		///<summary>Sorts by the first column, as a date column, in ascending order.</summary>
		public static int ComparePayPlanRows(GridRow x,GridRow y) {
			DateTime dateTimeX=DateTime.Parse(x.Cells[0].Text);
			DateTime dateTimeY=DateTime.Parse(y.Cells[0].Text);
			if(dateTimeX<dateTimeY) {
				return -1;
			}
			else if(dateTimeX>dateTimeY) {
				return 1;
			}
			//dateTimeX==dateTimeY
			//We want to put recalculated charges to the bottom of the current date.  This is a "final" point when recalculating and needs to be at the end.
			if(x.Cells[2].Text.Trim().ToLower().Contains("recalculated based on") && !y.Cells[2].Text.Trim().ToLower().Contains("recalculated based on")) {
				return 1;
			}
			if(!x.Cells[2].Text.Trim().ToLower().Contains("recalculated based on") && y.Cells[2].Text.Trim().ToLower().Contains("recalculated based on")) {
				return -1;
			}
			//If there is more than one recalculate charge, sort by descending charge amount. This only matters if one of the recalculated charges is 0
			if(x.Cells[2].Text.Trim().ToLower().Contains("recalculated based on") && y.Cells[2].Text.Trim().ToLower().Contains("recalculated based on")) {
				if(PIn.Double(x.Cells[3].Text)<PIn.Double(y.Cells[3].Text)) {
					return 1;
				}
				return -1;
			}
			//Show charges before Payment on the same date.
			if(x.Tag.GetType()==typeof(PayPlanCharge)) {//x is charge (Type.Equals doesn't seem to work in sorters for some reason)
				if(y.Tag.GetType()==typeof(PaySplit) || y.Tag.GetType()==typeof(DataRow)) {//y is credit, x goes first
					return -1;
				}
				else {//x and y are both charges (Not likely, they shouldn't have same dates) unless they are adjustments
					if(string.IsNullOrEmpty(x.Cells[7].Text) && !string.IsNullOrEmpty(y.Cells[7].Text)) {
						return -1;
					}
					else if(!string.IsNullOrEmpty(x.Cells[7].Text) && string.IsNullOrEmpty(y.Cells[7].Text)) {
						return 1;
					}
				}
			}
			else {//x is credit
				if(y.Tag.GetType()==typeof(PayPlanCharge)) {//y is charge
					return 1;
				}
				//x and y are both Payments
			}
			return x.Cells[2].Text.CompareTo(y.Cells[2].Text);//Sort by description.  This orders the payment plan charges which are on the same date by 
			//their charge number.  Might order payments by check number as well.
		}

		public static int CompareMergedPayPlanRows(GridRow x,GridRow y) {
			DateTime dateTimeX=DateTime.Parse(x.Cells[1].Text);
			DateTime dateTimeY=DateTime.Parse(y.Cells[1].Text);
			DynamicPayPlanRowData rowDataX=(DynamicPayPlanRowData)x.Tag;
			DynamicPayPlanRowData rowDataY=(DynamicPayPlanRowData)x.Tag;
			if(dateTimeX<dateTimeY) {
				return -1;
			}
			else if(dateTimeX>dateTimeY) {
				return 1;
			}
			//Show charges before Payment on the same date.
			if(rowDataX.IsChargeRow()) {//x is charge (Type.Equals doesn't seem to work in sorters for some reason)
				if(rowDataY.IsPaymentRow()) {//y is credit, x goes first
					return -1;
				}
			}
			else {//x is credit
				if(rowDataY.IsChargeRow()) {//y is charge
					return 1;
				}
				//x and y are both Payments
			}
			return 0;
		}

		public static GridRow CreateRowForPayPlanCharge(PayPlanCharge payPlanCharge,int payPlanChargeOrdinal,bool isDynamic=false) {
			string descript="#"+payPlanChargeOrdinal;
			if(isDynamic && payPlanCharge.LinkType==PayPlanLinkType.Procedure) {
				Procedure curProc=Procedures.GetOneProc(payPlanCharge.FKey,false);
				if(curProc!=null) {
					if(payPlanCharge.ChargeDate==DateTime.MaxValue && (curProc.ProcStatus==ProcStat.TP || curProc.ProcStatus==ProcStat.TPi)) {
						descript="";
					}
					ProcedureCode curProcCode=ProcedureCodes.GetProcCodeFromDb(curProc.CodeNum);
					if(curProcCode!=null) {
						descript+=" "+curProcCode.ProcCode;
					}
					if(curProcCode.AbbrDesc!="") {
						descript+=" - "+curProcCode.AbbrDesc;
					}
				}
			}
			if(isDynamic && payPlanCharge.LinkType==PayPlanLinkType.Adjustment) {
				descript+=" - "+Lan.g("Payment Plan","Adjustment");
			}
			if(payPlanCharge.Note!="") {
				descript+=" "+payPlanCharge.Note;
				//Don't add a # if it's a recalculated charge because they aren't "true" payplan charges.
				if(payPlanCharge.Note.Trim().ToLower().Contains("recalculated based on")) {
					descript=payPlanCharge.Note;
				}
			}
			GridRow row=new GridRow();//Charge row
			row.Cells.Add(payPlanCharge.ChargeDate.ToShortDateString());//0 Date
			row.Cells.Add(Providers.GetAbbr(payPlanCharge.ProvNum));//1 Prov Abbr
			row.Cells.Add(descript);//2 Descript
			if(payPlanCharge.Principal<0 && payPlanCharge.IsOffset) {//Offsetting Debits
				row.Cells.Add((payPlanCharge.Principal).ToString("n"));//principal
				row.Cells.Add("");//interest
				row.Cells.Add("");//due
				row.Cells.Add("");//payment
			}
			else if(payPlanCharge.Principal<0) {//adjustment
				row.Cells.Add("");//principal
				row.Cells.Add("");//interest
				row.Cells.Add("");//due
				row.Cells.Add("");//payment
				if(!isDynamic) {
					row.ColorText=Defs.GetDefByExactName(DefCat.AccountColors,"Adjustment").ItemColor;
					row.Cells.Add((payPlanCharge.Principal).ToString("n")); //adjustment
				}
			}
			else {//regular charge
				row.Cells.Add((payPlanCharge.Principal).ToString("n"));//3 Principal
				row.Cells.Add(payPlanCharge.Interest.ToString("n"));//4 Interest
				row.Cells.Add((payPlanCharge.Principal+payPlanCharge.Interest).ToString("n"));//5 Due
				row.Cells.Add("");//6 Payment
				if(!isDynamic) {//Dynamic payment plans do not have pay plan adjustments.
					row.Cells.Add("");//7 Adjustment
				}
			}
			row.Cells.Add("");//8 Balance (filled later)
			if(isDynamic && payPlanCharge.PayPlanChargeNum==0) {
				row.ColorText=System.Drawing.Color.Gray;//it isn't an actual charge yet, it hasn't come due and been inserted into the database. 
			}
			row.Tag=payPlanCharge;
			return row;
		}

		public static GridRow CreateRowForPatientPayPlanSplit(DataRow rowBundlePayment,PaySplit paySplit) {
			string descript=Defs.GetName(DefCat.PaymentTypes,PIn.Long(rowBundlePayment["PayType"].ToString()));
			if(rowBundlePayment["CheckNum"].ToString()!="") {
				descript+=" #"+rowBundlePayment["CheckNum"].ToString();
			}
			descript+=" "+paySplit.SplitAmt.ToString("c");
			if(PIn.Double(rowBundlePayment["PayAmt"].ToString())!=paySplit.SplitAmt) {
				descript+=Lans.g("PayPlanL","(split)");
			}
			GridRow row=new GridRow();
			row.Cells.Add(paySplit.DatePay.ToShortDateString());//0 Date
			row.Cells.Add(Providers.GetAbbr(PIn.Long(rowBundlePayment["ProvNum"].ToString())));//1 Prov Abbr
			row.Cells.Add(descript);//2 Descript
			row.Cells.Add("");//3 Principal
			row.Cells.Add("");//4 Interest
			row.Cells.Add("");//5 Due
			row.Cells.Add(paySplit.SplitAmt.ToString("n"));//6 Payment
			row.Cells.Add("");//7 Adjustment - Does not exist for dynamic payment plans
			row.Cells.Add("");//8 Balance (filled later)
			row.Tag=paySplit;
			row.ColorText=Defs.GetDefByExactName(DefCat.AccountColors,"Payment").ItemColor;
			return row;
		}

		public static GridRow CreateRowForClaimProcs(DataRow rowBundleClaimProc,bool isDynamic=false) {
			//Either a claimpayment or a bundle of claimprocs with no claimpayment that were on the same date.
			string descript=Defs.GetName(DefCat.InsurancePaymentType,PIn.Long(rowBundleClaimProc["PayType"].ToString()));
			if(rowBundleClaimProc["CheckNum"].ToString()!="") {
				descript+=" #"+rowBundleClaimProc["CheckNum"];
			}
			if(PIn.Long(rowBundleClaimProc["ClaimPaymentNum"].ToString())==0) {
				descript+="No Finalized Payment";
			}
			else {
				double checkAmt=PIn.Double(rowBundleClaimProc["CheckAmt"].ToString());
				descript+=" "+checkAmt.ToString("c");
				double insPayAmt=PIn.Double(rowBundleClaimProc["InsPayAmt"].ToString());
				if(checkAmt!=insPayAmt) {
					descript+=" "+Lans.g("PayPlanL","(split)");
				}
			}
			GridRow row=new GridRow();
			row.Cells.Add(PIn.DateT(rowBundleClaimProc["DateCP"].ToString()).ToShortDateString());//0 Date
			row.Cells.Add(Providers.GetLName(PIn.Long(rowBundleClaimProc["ProvNum"].ToString())));//1 Prov Abbr
			row.Cells.Add(descript);//2 Descript
			row.Cells.Add("");//3 Principal
			row.Cells.Add("");//4 Interest
			row.Cells.Add("");//5 Due
			row.Cells.Add(PIn.Double(rowBundleClaimProc["InsPayAmt"].ToString()).ToString("n"));//6 Payment
			if(!isDynamic) {
				row.Cells.Add("");//7 Adjustment
			}
			row.Cells.Add("");//8 Balance (filled later)
			row.Tag=rowBundleClaimProc;
			row.ColorText=Defs.GetDefByExactName(DefCat.AccountColors,"Insurance Payment").ItemColor;
			return row;
		}

		///<summary>Changes to the database are all made when the form is closed, so securitylog entries have been consolidated into this method.
		///Centralized so both Dynamic and Static payment plans can use the same method.</summary>
		public static void MakeSecLogEntries(PayPlan payPlanCur,PayPlan payPlanOld,bool hasSignatureChanged,bool isSigOldValid,bool isSigBlank,bool isSigValid
			,bool isPrinting=false) 
		{
			//logs creating, closing out, deleting, and signing of payment plan.
			//deleted logs are in butDelete_click since that method doesn't call SaveData.
			if(isPrinting) {// Don't make log entry if the print button was clicked.
				return;
			}
			//new
			if(payPlanOld.IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
							(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan created.",payPlanOld.PayPlanNum,DateTime.MinValue);
				return;
			}
			//closed
			if(!payPlanCur.IsClosed && payPlanOld.IsClosed) {
				SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
							(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan reopened.",payPlanOld.PayPlanNum,DateTime.MinValue);
			}
			if(hasSignatureChanged) {
				//signed
				if(!isSigOldValid && !isSigBlank && isSigValid) {
					SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
								(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan signed.",payPlanOld.PayPlanNum,DateTime.MinValue);
				}
				//sig invalidated
				if(isSigOldValid && (!isSigValid || isSigBlank)) {
					SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
								(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan signature invalidated.",payPlanOld.PayPlanNum,DateTime.MinValue);
				}
			}
			//guarantor changed
			if(payPlanOld.Guarantor != payPlanCur.Guarantor) {
				SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
							(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan guarantor changed from "
							+Patients.GetNameLF(payPlanOld.Guarantor)+" to "+Patients.GetNameLF(payPlanCur.Guarantor)+".",payPlanOld.PayPlanNum,DateTime.MinValue);
			}
			//Completed Amt Changed
			if(payPlanOld.CompletedAmt != payPlanCur.CompletedAmt) {
				SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
							(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan completed amount changed.",payPlanOld.PayPlanNum,DateTime.MinValue);
			}
			//Ins Plan Changed
			if(payPlanOld.PlanNum != payPlanCur.PlanNum) {
				SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
							(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan ins plan changed.",payPlanOld.PayPlanNum,DateTime.MinValue);
			}
			//Note Changed
			if(payPlanOld.Note != payPlanCur.Note) {
				SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
							(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan note changed.",payPlanOld.PayPlanNum,DateTime.MinValue);
			}
			//closed
			if(payPlanCur.IsClosed && !payPlanOld.IsClosed) {
				SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,payPlanCur.PatNum,
							(payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan closed.",payPlanOld.PayPlanNum,DateTime.MinValue);
			}
		}

		/// <summary>Creates a list of GridRows where PayPlanCharges are merged into 1 row with the same date, and PaySplits with the same PayNum. Returns the list with 1 Row for 1 date set of charges, and 1 Row for 1 Set of paysplits grouped by PayNum. listPayPlanCharges needs to be sorted by ChargeDate in ascending order.</summary>
		public static List<GridRow> CreateRowsForDynamicPayPlanCharges(List<PayPlanCharge> listPayPlanCharges,List<PaySplit> listPaySplits) {
			List<GridRow> listGridRowsMerged=new List<GridRow>();
			List<DateTime> listChargeDates=listPayPlanCharges.Select(x => x.ChargeDate).Distinct().ToList();
			for(int i=0;i<listChargeDates.Count;i++) {
				List<PayPlanCharge> listPayPlanChargesForDate=listPayPlanCharges.FindAll(x=>x.ChargeDate==listChargeDates[i]);
				double principal=listPayPlanChargesForDate.Sum(x=>x.Principal);
				double interest=listPayPlanChargesForDate.Sum(x=>x.Interest);
				double due=principal+interest;
				GridRow row=new GridRow();
				row.Cells.Add((i+1).ToString());
				row.Cells.Add(listChargeDates[i].ToShortDateString());
				row.Cells.Add("");
				row.Cells.Add(principal.ToString("n"));
				row.Cells.Add(interest.ToString("n"));
				row.Cells.Add(due.ToString("n"));
				row.Cells.Add("");
				row.Cells.Add("");//6 Balance (filled later)
				if(listPayPlanChargesForDate.Any(x => x.PayPlanChargeNum==0)) {
					row.ColorText=System.Drawing.Color.Gray;//it isn't an actual charge yet, it hasn't come due and been inserted into the database. 
				}
				row.Tag=new DynamicPayPlanRowData() {
					ListPayPlanChargeNums=listPayPlanChargesForDate.Select(x => x.PayPlanChargeNum).ToList()
				};
				listGridRowsMerged.Add(row);
			}
			List<long> listPayNums=listPaySplits.Select(x=>x.PayNum).Distinct().ToList(); 
			for(int i=0;i<listPayNums.Count;i++) {
				List<PaySplit> listPaySplitsForPayment=listPaySplits.FindAll(x=>x.PayNum==listPayNums[i]);
				string datePay=listPaySplitsForPayment[0].DatePay.ToShortDateString();
				double sumSplitAmt=listPaySplitsForPayment.Sum(x=>x.SplitAmt);
				GridRow row=new GridRow();
				row.Cells.Add("");//0 Charge number
				row.Cells.Add(datePay);//1 Date
				row.Cells.Add("Payment");//2 Description
				row.Cells.Add("");//3 Principal
				row.Cells.Add("");//4 Interest
				row.Cells.Add("");//5 Due
				row.Cells.Add(sumSplitAmt.ToString("n"));//6 Payment
				row.Cells.Add("");//7 Balance (filled later)
				row.ColorText=Defs.GetDefByExactName(DefCat.AccountColors,"Payment").ItemColor;
				row.Tag=new DynamicPayPlanRowData() {
					PayNum=listPayNums[i]
				};
				listGridRowsMerged.Add(row);
			}
			listGridRowsMerged.Sort(PayPlanL.CompareMergedPayPlanRows);
			return listGridRowsMerged;
		}
	}

	public class DynamicPayPlanRowData {
		public List<long> ListPayPlanChargeNums=new List<long>();
		public long PayNum=0;
		public bool IsChargeRow() {
			return ListPayPlanChargeNums.Count>0;
		}
		public bool IsPaymentRow() {
			return PayNum!=0;
		}
	}

}
