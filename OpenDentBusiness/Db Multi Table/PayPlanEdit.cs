using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class PayPlanEdit {
		#region PayPlans
		///<summary>Helper method to create a debit charge which will be associated to the current payment plan.</summary>
		public static PayPlanCharge CreateDebitCharge(PayPlan payplan,Family famCur,long provNum,long clinicNum,double principalAmt,double interestAmt
			,DateTime dateCharge,string note) 
		{
			//No remoting role check; no call to db
			PayPlanCharge ppCharge=new PayPlanCharge();
			ppCharge.PayPlanNum=payplan.PayPlanNum;
			//FamCur is the family of the patient, so check to see if the guarantor is in the patient's family. 
			//If the guar and pat are in the same family, then use the patnum. else, use guarantor.
			if(famCur.ListPats.Select(x => x.PatNum).Contains(payplan.Guarantor)) {
				ppCharge.Guarantor=payplan.PatNum;
			}
			else {
				ppCharge.Guarantor=payplan.Guarantor;
			}
			ppCharge.PatNum=payplan.PatNum;
			ppCharge.ChargeDate=dateCharge;
			ppCharge.Interest=interestAmt;
			ppCharge.Principal=principalAmt;
			ppCharge.Note=note;
			ppCharge.ChargeType=PayPlanChargeType.Debit;
			ppCharge.ProvNum=provNum;
			ppCharge.ClinicNum=clinicNum;
			return ppCharge;
		}

		///<summary>Helper method to create a dynamic debit charge which will be associated to the current payment plan.
		///Dynamic debit charges require the fKey and linkType to be set.</summary>
		public static PayPlanCharge CreateDebitChargeDynamic(PayPlan payplan,Family famCur,long provNum,long clinicNum,double principalAmt,
			double interestAmt,DateTime dateCharge,string note,long fKey,PayPlanLinkType linkType) 
		{
			//No remoting role check; no call to db
			PayPlanCharge ppCharge=CreateDebitCharge(payplan,famCur,provNum,clinicNum,principalAmt,interestAmt,dateCharge,note);
			ppCharge.FKey=fKey;
			ppCharge.LinkType=linkType;
			return ppCharge;
		}

		public static bool DoDeleteRecalcChargeIfDynamic(PaySplit splitCur) {
			if(splitCur.PayPlanChargeNum==0) {
				return false;
			}
			PayPlan planCur=PayPlans.GetOne(splitCur.PayPlanNum);
			if(planCur==null || !planCur.IsDynamic) {
				return false;
			}
			PayPlanCharge chargeCur=PayPlanCharges.GetOne(splitCur.PayPlanChargeNum);
			if(chargeCur==null || !chargeCur.Note.StartsWith("Recalculated based on")) {
				return false;
			}
			return true;
		}

		private static void BalancePaySplitForDynamicPayPlan(PaySplit splitCur, PayPlanCharge chargeCur, PayPlanRecalculationData recalcData, 
			PayPlanTerms terms, List<PaySplit> listPaySplitsThisPayment, List<PaySplit> listPaySplitsThisPaymentThisPlan) 
		{
			if(chargeCur.Note.StartsWith("Recalculated based on")) {//This is a recalculation charge, adjust principal if neccessary
				chargeCur.Principal=splitCur.SplitAmt;
				chargeCur.Interest=0;
				PayPlanCharges.Update(chargeCur);
				return;
			}

			//This is a pay plan charge, make sure total charges = total splits
			double chargeTotal=chargeCur.Principal+chargeCur.Interest;
			List<PaySplit> listPaySplitsNotThisPayment=recalcData.ListPaySplits
				.FindAll(x => listPaySplitsThisPayment.All(y => x.SplitNum!=y.SplitNum && x.PayNum!=y.PayNum));
			recalcData.ListPaySplits.Clear();
			recalcData.ListPaySplits.AddRange(listPaySplitsNotThisPayment);
			recalcData.ListPaySplits.AddRange(listPaySplitsThisPayment);
			List<PaySplit> listPaySplitsThisChargeThisPayment=listPaySplitsThisPayment.FindAll(x => x.PayPlanChargeNum==chargeCur.PayPlanChargeNum);
			List<PaySplit> listPaySplitsThisChargeNotThisPayment=listPaySplitsNotThisPayment.FindAll(x => x.PayPlanChargeNum==chargeCur.PayPlanChargeNum);
			//Process negative splits first because the if/else short-circuits once chargeTotal=0
			listPaySplitsThisChargeNotThisPayment.OrderBy(x => x.SplitAmt).ForEach(x => {
				if(chargeTotal==0) {
					return;
				}
				if(x.SplitAmt<chargeTotal) {
					chargeTotal-=x.SplitAmt;
					chargeTotal=Math.Round(chargeTotal,terms.RoundDec);//Fixes rounding error when payment is automatically split into principal and interest
				}
				else if(x.SplitAmt>=chargeTotal) {
					chargeTotal=0;
				}
			});
			//Process negative splits first because the if/else short-circuits once chargeTotal=0
			listPaySplitsThisChargeThisPayment.OrderBy(x => x.SplitAmt).ForEach(x => {
				if(chargeTotal==0) {
					x.PayPlanChargeNum=0;
					return;
				}
				if(x.SplitAmt<chargeTotal) {
					chargeTotal-=x.SplitAmt;
					chargeTotal=Math.Round(chargeTotal,terms.RoundDec);//Fixes rounding error when payment is automatically split into principal and interest
				}
				else if(x.SplitAmt>=chargeTotal) {
					if(x.SplitAmt>chargeTotal) {
						PaySplit newSplit = x.Copy();
						newSplit.SplitAmt = x.SplitAmt-chargeTotal;
						newSplit.PayPlanChargeNum = 0;
						listPaySplitsThisPayment.Add(newSplit);
						listPaySplitsThisPaymentThisPlan.Add(newSplit);
						recalcData.ListPaySplits.Add(newSplit);
						x.SplitAmt = chargeTotal;
					}
					chargeTotal=0;
				}
			});
		}

		public static void BalanceDynamicPayPlanOnPrincipal(PayPlanRecalculationData recalcData, PayPlanTerms terms, ref List<PaySplit> listPaySplitsThisPayment) {
			PaySplit splitCur;
			PayPlanCharge chargeCur;
			listPaySplitsThisPayment=listPaySplitsThisPayment.OrderBy(x => x.DatePay).ThenBy(x => x.SplitNum).ToList();
			List<PaySplit> listPaySplitsThisPaymentThisPlan = listPaySplitsThisPayment.FindAll(x => x.PayPlanNum==recalcData.PayPlan.PayPlanNum);
			for(int i=0; i<listPaySplitsThisPaymentThisPlan.Count; i++) {
				splitCur = listPaySplitsThisPaymentThisPlan[i];
				chargeCur=recalcData.ListPayPlanCharges.Where(x => x.PayPlanChargeNum==splitCur.PayPlanChargeNum).FirstOrDefault();
				if(chargeCur==null) {
					splitCur.PayPlanChargeNum=0;
				}
				else {//This split already has an associated charge
					BalancePaySplitForDynamicPayPlan(splitCur, chargeCur, recalcData, terms, listPaySplitsThisPayment, listPaySplitsThisPaymentThisPlan);
				}
				if(splitCur.PayPlanChargeNum==0) {//This split needs a new charge created
					foreach(PayPlanCharge charge in recalcData.ListPayPlanCharges) {
						if(Math.Round(recalcData.ListPaySplits.Where(x => x.PayPlanChargeNum==charge.PayPlanChargeNum).Sum(x => x.SplitAmt), terms.RoundDec)>=charge.Interest+charge.Principal) {
							continue;
						}
						chargeCur=charge;
						BalancePaySplitForDynamicPayPlan(splitCur, chargeCur, recalcData, terms, listPaySplitsThisPayment, listPaySplitsThisPaymentThisPlan);
						return;
					}
					chargeCur=CreateDebitCharge(recalcData.PayPlan,recalcData.Fam,recalcData.ProvNum,recalcData.ClinicNum,0,0,DateTime.Today,
						"Recalculated based on pay on principal");
					chargeCur.Principal=splitCur.SplitAmt;
					chargeCur.Interest=0;
					splitCur.PayPlanChargeNum=PayPlanCharges.Insert(chargeCur);
					chargeCur.PayPlanChargeNum=splitCur.PayPlanChargeNum;
					recalcData.ListPayPlanCharges.Add(chargeCur);
				}
			}
		}

		public static void BalanceDynamicPayPlanOnPrepay(List<PaySplit> listPaySplitsThisPaymentThisPlan) {
			List<long> listPayPlanChargesToDelete=new List<long>();
			foreach(PaySplit splitCur in listPaySplitsThisPaymentThisPlan) {
				if(splitCur.PayPlanChargeNum==0) {
					continue;
				}
				if(listPayPlanChargesToDelete.Contains(splitCur.PayPlanChargeNum)) {
					continue;
				}
				PayPlanCharge chargeCur=PayPlanCharges.GetOne(splitCur.PayPlanChargeNum);
				if(chargeCur!=null && chargeCur.Note.StartsWith("Recalculated based on")) {
					splitCur.PayPlanChargeNum=0;
					listPayPlanChargesToDelete.Add(chargeCur.PayPlanChargeNum);
				}
			}
			PayPlanCharges.DeleteMany(listPayPlanChargesToDelete);
		}

		/// <summary>Allocate money for payplan adjustments for one or multiple adjustments.</summary>
		public static List<PayPlanCharge> CreatePayPlanAdjustments(double negAdjAmt,List<PayPlanCharge> listPayPlanCharges,double totalNegFutureAdjs) {
			double moneyToAllocate=(totalNegFutureAdjs+negAdjAmt)*-1;//the total amount of ALL future adjustments, existing + new. 
			//Get a list of all our charges for this payplan.
			List<PayPlanCharge> listRemainingCharges=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal >= 0)
				.OrderByDescending(x => x.ChargeDate).ToList();
			//Get a list of all current adjustments for the payment plan. (Existing before the newly added adjustment).
			List<PayPlanCharge> listCurAdjs=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal<0 
				&& x.ChargeDate > DateTime.Today);
			//Remove all the adjustments so we can more easily move money around and redistribute if necessary.
			listPayPlanCharges.RemoveAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal<0 && x.ChargeDate > DateTime.Today);
			foreach(PayPlanCharge charge in listRemainingCharges) {
				double chargeAmtRemaining=(charge.Principal);//The amount that can still be adjusted off of this charge, how much is left.
				#region Re-allocate existing adjustments
				List<PayPlanCharge> listExistNegAdjsForCharge=listCurAdjs.FindAll(x => x.ChargeDate.Date==charge.ChargeDate.Date)
					.OrderBy(x => x.ChargeDate).ToList();
				if(listExistNegAdjsForCharge.Count!=0) {//There exists adjustments for the currrent charge/debit row.
					double existingNegAdjTotal=listExistNegAdjsForCharge.Sum(x => x.Principal);
					if(existingNegAdjTotal*(-1)==chargeAmtRemaining){
						//all of these adjustments apply to this charge. Add them all back, no need to keep looping through the adjs.
						listPayPlanCharges.AddRange(listExistNegAdjsForCharge);
						moneyToAllocate-=chargeAmtRemaining;
						continue; 
					}
					//first loop through the existing adjustments and see what can be applied
					foreach(PayPlanCharge existingAdj in listExistNegAdjsForCharge) {
						if(chargeAmtRemaining==0) {//All adjustments have been allocated for this charge.
							break;
						}
						else if(moneyToAllocate < existingAdj.Principal*-1) {
							//Remaining amount is less then the current adjustment
							//We won't add this adjustment back in but we will create a new adjustment down below to account for this plus the new adj we are adding.
							continue;
						}
						if(existingAdj.Principal*(-1) <= chargeAmtRemaining) { 
							listPayPlanCharges.Add(existingAdj);
							chargeAmtRemaining-=existingAdj.Principal*(-1);
							moneyToAllocate-=existingAdj.Principal*-1;
						}
						else {//adjustment is bigger that amt avaialable to be applied. Keep removed, add amount back into the total make a new adjustment later.
							moneyToAllocate+=existingAdj.Principal*-1;
						}
						//Leave adjustment gone, only allocate for what we have remaining. 
					}
				}
				#endregion
				if(CompareDouble.IsEqual(moneyToAllocate,0)) {
					break;
				}
				PayPlanCharge ppc=new PayPlanCharge();
				ppc.ChargeDate=charge.ChargeDate;
				ppc.PatNum=charge.PatNum;
				ppc.PayPlanNum=charge.PayPlanNum;
				ppc.Guarantor=charge.Guarantor;
				ppc.ProvNum=charge.ProvNum;
				ppc.ProcNum=charge.ProcNum;
				ppc.Note="";
				ppc.ChargeType=PayPlanChargeType.Debit;
				if(moneyToAllocate<=chargeAmtRemaining) {
					//Balance is equal to adjustment, apply whole adj amt to charge and be done.
					//Or adj won't cover the whole remaining charge, apply what we can and be done. 
					ppc.Principal=moneyToAllocate*-1;
					listPayPlanCharges.Add(ppc);
					break;//moneyToAllocate should be 0 here. No need to keep looping. 
				}
				else {//Entire principal can be allocated to this adjustment. Move money to the principal for entire charge amount and keep looping. 
					ppc.Principal=chargeAmtRemaining*-1;
					moneyToAllocate-=chargeAmtRemaining;
					listPayPlanCharges.Add(ppc);
				}
			}
			return listPayPlanCharges;
		}

		public static double CalcPeriodRate(double APR,PayPlanFrequency frequency) {
			double periodRate;
			if(APR==0) {
				periodRate=0;
			}
			else {
				if(frequency==PayPlanFrequency.Weekly) {
					periodRate=APR/100/52;
				}
				else if(frequency==PayPlanFrequency.EveryOtherWeek) {
					periodRate=APR/100/26;
				}
				else if(frequency==PayPlanFrequency.OrdinalWeekday) {
					periodRate=APR/100/12;
				}
				else if(frequency==PayPlanFrequency.Monthly) {
					periodRate=APR/100/12;
				}
				else {//quarterly
					periodRate=APR/100/4;
				}
			}
			return periodRate;
		}

		///<summary>periodNum is zero-based.</summary>
		public static DateTime CalcNextPeriodDate(DateTime firstDate,int periodNum,PayPlanFrequency frequency) {
			DateTime retVal=DateTime.Today;
			if(frequency==PayPlanFrequency.Weekly) {
				retVal=firstDate.AddDays(7*periodNum);
			}
			else if(frequency==PayPlanFrequency.EveryOtherWeek) {
				retVal=firstDate.AddDays(14*periodNum);
			}
			else if(frequency==PayPlanFrequency.OrdinalWeekday) {//First/second/etc Mon/Tue/etc of month
				DateTime roughMonth=firstDate.AddMonths(1*periodNum);//this just gets us into the correct month and year
				DayOfWeek dayOfWeekFirstDate=firstDate.DayOfWeek;
				//find the starting point for the given month: the first day that matches day of week
				DayOfWeek dayOfWeekFirstMonth=(new DateTime(roughMonth.Year,roughMonth.Month,1)).DayOfWeek;
				if(dayOfWeekFirstMonth==dayOfWeekFirstDate) {//1st is the proper day of the week
					retVal=new DateTime(roughMonth.Year,roughMonth.Month,1);
				}
				else if(dayOfWeekFirstMonth<dayOfWeekFirstDate) {//Example, 1st is a Tues (2), but we need to start on a Thursday (4)
					retVal=new DateTime(roughMonth.Year,roughMonth.Month,dayOfWeekFirstDate-dayOfWeekFirstMonth+1);//4-2+1=3.  The 3rd is a Thursday
				}
				else {//Example, 1st is a Thursday (4), but we need to start on a Monday (1) 
					retVal=new DateTime(roughMonth.Year,roughMonth.Month,7-(dayOfWeekFirstMonth-dayOfWeekFirstDate)+1);//7-(4-1)+1=5.  The 5th is a Monday
				}
				int ordinalOfMonth=GetOrdinalOfMonth(firstDate);//for example 3 if it's supposed to be the 3rd Friday of each month
				retVal=retVal.AddDays(7*(ordinalOfMonth-1));//to get to the 3rd Friday, and starting from the 1st Friday, we add 2 weeks.
			}
			else if(frequency==PayPlanFrequency.Monthly) {
				retVal=firstDate.AddMonths(1*periodNum);
			}
			else {//quarterly
				retVal=firstDate.AddMonths(3*periodNum);
			}
			return retVal;
		}

		///<summary>For example, date is the 3rd Friday of the month, then this returns 3.</summary>
		private static int GetOrdinalOfMonth(DateTime date) {
			if(date.AddDays(-28).Month==date.Month) {
				return 4;//treat a 5 like a 4
			}
			else if(date.AddDays(-21).Month==date.Month) {//4
				return 4;
			}
			else if(date.AddDays(-14).Month==date.Month) {
				return 3;
			}
			if(date.AddDays(-7).Month==date.Month) {
				return 2;
			}
			return 1;
		}

		///<summary>Creates charge rows for display on the form from the data table input. Similar to CreateRowForPayPlanCharge but for use with sheets/datatables.</summary>
		public static DataRow CreateRowForPayPlanChargeDT(DataTable table,PayPlanCharge payPlanCharge,int payPlanChargeOrdinal,bool isDynamic) {
			DataRow retVal=table.NewRow();
			string descript = "#"+(payPlanChargeOrdinal+1);
			if(isDynamic && payPlanCharge.LinkType==PayPlanLinkType.Procedure) {
				Procedure curProc=Procedures.GetOneProc(payPlanCharge.FKey,false);
				if(curProc!=null) {
					ProcedureCode curProcCode=ProcedureCodes.GetProcCodeFromDb(curProc.CodeNum);
					if(curProcCode!=null) {
						descript+=" "+curProcCode.ProcCode;
						if(curProcCode.AbbrDesc!="") {
							descript+=" - "+curProcCode.AbbrDesc;
						}
					}
				}
			}
			if(isDynamic && payPlanCharge.LinkType==PayPlanLinkType.Adjustment) {
				descript+=" - "+Lans.g("Pay Plan Edit","Adjustment");
       }
			if(payPlanCharge.Note!="") {
				descript+=" "+payPlanCharge.Note;
				//Don't add a # if it's a recalculated charge because they aren't "true" payplan charges.
				if(payPlanCharge.Note.Trim().ToLower().Contains("recalculated based on")) {
					descript=payPlanCharge.Note;
				}
			}
			retVal["ChargeDate"]=payPlanCharge.ChargeDate.ToShortDateString();//0 Date
			retVal["Provider"]=Providers.GetAbbr(payPlanCharge.ProvNum);//1 Prov Abbr
			retVal["Description"]=descript;//2 Descript
			if(payPlanCharge.Principal<0) { //it's a patient payment plan adjustment (non-dynamic)
				retVal["Principal"]="";
				retVal["Interest"]="";//4 Interest
				retVal["Due"]="";//5 Due
				retVal["Payment"]="";//6 Payment (filled later)
				if(!isDynamic) { 
					retVal["Adjustment"]=payPlanCharge.Principal.ToString("f");//7
				}
			}
			else {
				retVal["Principal"]=payPlanCharge.Principal.ToString("f");//3 Principal
				retVal["Interest"]=payPlanCharge.Interest.ToString("f");//4 Interest
				retVal["Due"]=(payPlanCharge.Principal+payPlanCharge.Interest).ToString("n");//5 Due
				retVal["Payment"]="";//6 Payment (filled later)
				if(!isDynamic) {
					retVal["Adjustment"]=0.ToString("f");//7
				}
			}
			retVal["Balance"]="";//8 Balance (filled later)
			if(payPlanCharge.Principal<0) {
				retVal["Type"]=PayPlanEntryType.adjustment.ToString();
			}
			else {
				retVal["Type"]=PayPlanEntryType.charge.ToString();
			}
			return retVal;
		}

		///<summary>Creates pay plan rows for display on the form from the data table input.</summary>
		public static DataRow CreateRowForPayPlanListDT(DataTable table,DataRow payPlanList,int payPlanListOrdinal,bool isDynamic) {
			DataRow retVal=table.NewRow();
			string descript;
			if(payPlanList[2].ToString().Trim().ToLower().Contains("downpayment")) {//description
				descript=payPlanList[2].ToString();
			}
			else if(payPlanList[2].ToString().Trim().ToLower().Contains("increased interest:")) {//description
				descript="#"+(payPlanListOrdinal+1)+" Increased Interest:";
			}
			else if(PIn.Double(payPlanList[6].ToString())>0) {//payment
				descript=payPlanList[2].ToString();
			}
			else if(payPlanList[2].ToString().StartsWith("#")) {//only pay plan charges start with #
				StringBuilder strBuilder=new StringBuilder(payPlanList[2].ToString());
				StringTools.RegReplace(strBuilder,"^#[0-9]+","");
				descript="#"+(payPlanListOrdinal+1)+strBuilder;
			}
			else {
				descript="#"+(payPlanListOrdinal+1);
			}
			retVal[0]=payPlanList[0].ToString();//0 Date
			retVal[1]=payPlanList[1].ToString(); //1 Prov Abbr
			retVal[2]=descript;//2 Descript
			retVal[3]=payPlanList[3].ToString();//3 Principal
			retVal[4]=payPlanList[4].ToString();//4 Interest
			retVal[5]=payPlanList[5].ToString();//5 Due
			retVal[6]=payPlanList[6].ToString();//6 Payment (filled later)
			if(!isDynamic) {
				retVal[7]=payPlanList[7].ToString();//7 adjustment
			}
			retVal[8]=payPlanList[8].ToString();//8 Balance (filled later)
			return retVal;
		}

		///<summary>Creates pay plan split rows for display on the form from the data table input. Similar to CreateRowForPayPlanSplit but for use with sheets/datatables.</summary>
		public static DataRow CreateRowForPayPlanSplitDT(DataTable table,PaySplit payPlanSplit,DataRow rowBundlePayment,bool isDynamic) {
			DataRow retVal=table.NewRow();
			string descript=Defs.GetName(DefCat.PaymentTypes,PIn.Long(rowBundlePayment["PayType"].ToString()));
			if(rowBundlePayment["CheckNum"].ToString()!="") {
				descript+=" #"+rowBundlePayment["CheckNum"].ToString();
			}
			descript+=" "+payPlanSplit.SplitAmt.ToString("c");
			if(PIn.Double(rowBundlePayment["PayAmt"].ToString())!=payPlanSplit.SplitAmt) {
				descript+=Lans.g("FormPayPlan","(split)");
			}
			retVal["ChargeDate"]=payPlanSplit.DatePay.ToShortDateString();//0 Date
			retVal["Provider"]=Providers.GetAbbr(PIn.Long(rowBundlePayment["ProvNum"].ToString()));//1 Prov Abbr
			retVal["Description"]=descript;//2 Descript
			retVal["Principal"]=0.ToString("f");//3 Principal
			retVal["Interest"]=0.ToString("f");//4 Interest 
			retVal["Due"]=0.ToString("f");//5 Due (filled later)
			retVal["Payment"]=payPlanSplit.SplitAmt.ToString("f");// Payment
			if(!isDynamic) {
				retVal["Adjustment"]=0.ToString("f");//7 Adjustment
			}
			retVal["Balance"]=("");//8 Balance (filled later)
			retVal["Type"]=PayPlanEntryType.pay.ToString();
			return retVal;
		}

		public enum PayPlanEntryType {
			charge,
			adjustment,
			pay
		}

		///<summary>Creates pay plan split rows for display on the form from the data table input. Similar to CreateRowForClaimProcs but for use with sheets/datatables.</summary>
		public static DataRow CreateRowForClaimProcsDT(DataTable table,DataRow rowBundleClaimProc,bool isDynamic) {//Either a claimpayment or a bundle of claimprocs with no claimpayment that were on the same date.
			DataRow retVal=table.NewRow();
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
					descript+=" "+Lans.g("FormPayPlan","(split)");
				}
			}
			retVal["ChargeDate"]=PIn.DateT(rowBundleClaimProc["DateCP"].ToString()).ToShortDateString();//0 Date
			retVal["Provider"]=Providers.GetLName(PIn.Long(rowBundleClaimProc["ProvNum"].ToString()));//1 Prov Abbr
			retVal["Description"]=descript;//2 Descript
			retVal["Principal"]="";//3 Principal
			retVal["Interest"]="";//4 Interest
			retVal["Due"]="";//5 Due
			retVal["Payment"]=PIn.Double(rowBundleClaimProc["InsPayAmt"].ToString()).ToString("n");// Payment
			if(!isDynamic) {
				retVal["Adjustment"]="";//7
			}
			retVal["Balance"]=("");//8 Balance (filled later)
			retVal["Type"]=PayPlanEntryType.pay.ToString();
			return retVal;
		}

		///<summary>Performs same function as ComparePayPlanRows but for use with DataTables/Sheets.</summary>
		public static int ComparePayPlanRowsDT(DataRow x,DataRow y) {
			DateTime dateTimeX=PIn.Date(x["ChargeDate"].ToString());
			DateTime dateTimeY=PIn.Date(y["ChargeDate"].ToString()); 
			if(dateTimeX.Date!=dateTimeY.Date) {
				return dateTimeX.CompareTo(dateTimeY);// sort by date
			}
			bool xIsRecalc=x["Description"].ToString().ToLower().Contains("recalculated based on");
			bool yIsRecalc=y["Description"].ToString().ToLower().Contains("recalculated based on");
			if(xIsRecalc!=yIsRecalc) {
				return xIsRecalc.CompareTo(yIsRecalc);// recalculated charges to the bottom of the current date.
			}
			if(xIsRecalc && yIsRecalc) { 
				return PIn.Double(x["Principal"].ToString()).CompareTo(PIn.Double(y["Principal"].ToString()));// sort by principal amounts if both are recalculated charges
			}
			if(x["Type"].ToString()!=y["Type"].ToString()) {
				return x["Type"].ToString().CompareTo(y["Type"].ToString());//charges first; I.e. "charge".CompareTo("pay") will return charge first
			}
			return x["Description"].ToString().CompareTo(y["Description"].ToString());//Sort by description. 
		}

		///<summary>Creates one PayPlanCharge debit whose principal is the sum of all credits minus the sum of all past debits.</summary>
		public static PayPlanCharge CloseOutPatPayPlan(List<PayPlanCharge> listPayPlanCharges,PayPlan payPlan,DateTime dateToday) {
			double sumPastDebits = listPayPlanCharges
				.Where(x => x.ChargeType==PayPlanChargeType.Debit)
				.Where(x => x.ChargeDate <= dateToday.Date)
				.Sum(x => x.Principal);
			double sumCredits = listPayPlanCharges
				.Where(x => x.ChargeType==PayPlanChargeType.Credit)
				.Where(x => x.ChargeDate != DateTime.MaxValue.Date) //only count non-TP credits
				.Sum(x => x.Principal);
			PayPlanCharge closeoutCharge=new PayPlanCharge() {
				PayPlanNum=payPlan.PayPlanNum,
				Guarantor=payPlan.PatNum, //the closeout charge should always appear on the patient of the payment plan.
				PatNum=payPlan.PatNum,
				ChargeDate=dateToday,
				Interest=0,
				Principal=sumCredits-sumPastDebits,
				Note=Lans.g("FormPayPlan","Close Out Charge"),
				ChargeType=PayPlanChargeType.Debit,
			};
			return closeoutCharge;
		}

		public static void CreateScheduleCharges(PayPlanTerms terms,PayPlan payPlan,Family fam,long provNum,long clinicNum,
			List<PayPlanCharge> listPayPlanCharges)
		{
			//Set variables
			const int payPlanChargesCeiling=2000;//This is the maximum number of payplan charges allowed to be made plus 1.
			double periodRate=CalcPeriodRate(terms.APR,terms.Frequency);
			decimal principalDecrementingAmt=(decimal)(terms.PrincipalAmount-terms.DownPayment);
			decimal previousPrincipalDecrementingAmt=principalDecrementingAmt;
			int chargesCount=0;//Not the same as listPayPlanCharges.Count
			//Add charge for down payment
			if(terms.DownPayment!=0) {
				listPayPlanCharges.Add(PayPlanEdit.CreateDebitCharge(payPlan,fam,provNum,clinicNum,terms.DownPayment,0,DateTime.Today
					,Lans.g("PayPlanEdit","Downpayment")));
			}
			//Add charges
			while(CompareDecimal.IsGreaterThanZero(principalDecrementingAmt) && chargesCount < payPlanChargesCeiling) {//the ceiling prevents infinite loop
				//If principal is increasing, terms are invalid, and we must clear the list of charges and return.
				if(decimal.Compare(principalDecrementingAmt,previousPrincipalDecrementingAmt) > 0) {
					listPayPlanCharges.RemoveAll(x => x.ChargeType==PayPlanChargeType.Debit);
					terms.AreTermsValid=false;
					return;
				}
				previousPrincipalDecrementingAmt=principalDecrementingAmt;
				PayPlanCharge ppCharge=
					CreateDebitCharge(payPlan,fam,provNum,clinicNum,0,0,CalcNextPeriodDate(terms.DateFirstPayment,chargesCount,terms.Frequency),"");
				if(ppCharge.ChargeDate.Date>=terms.DateInterestStart.Date) {
					ppCharge.Interest=Math.Round(((double)principalDecrementingAmt*periodRate),terms.RoundDec);//2 decimals
				}
				ppCharge.Principal=(double)terms.PeriodPayment-ppCharge.Interest;
				if((terms.PayCount>0 && chargesCount==(terms.PayCount-1)) || (principalDecrementingAmt+(decimal)ppCharge.Interest<=terms.PeriodPayment)) {
					ppCharge.Principal=(double)principalDecrementingAmt;
				}
				principalDecrementingAmt-=(decimal)ppCharge.Principal;
				listPayPlanCharges.Add(ppCharge);
				chargesCount++;
			}
		}

		#region Recalculate Schedule
		///<summary>Recalculates an existing amortization schedule. This may alter total interest amounts based on over/under payment.</summary>
		public static void RecalculateScheduleCharges(PayPlanTerms terms,PayPlanRecalculationData recalcData) {
			recalcData.OverPaidAmt=CreateChargeMatchOverpayment(recalcData);
			//if overpaid amount is being paid towards principal, we subtract from principal and set to zero.
			if(!recalcData.IsPrepay && recalcData.OverPaidAmt>0) {
				recalcData.PrincipalRemaining-=(double)recalcData.OverPaidAmt;
				recalcData.OverPaidAmt=0;
			}
			recalcData.OverPaidDecrementingAmt=(decimal)recalcData.OverPaidAmt;
			recalcData.PrincipalDecrementingAmt=(decimal)recalcData.PrincipalRemaining;
			const int payPlanChargesCeiling=2000;//This is the maximum number of payplan charges allowed to be made plus 1. To prevent infinite loop.
			while(CompareDecimal.IsGreaterThanZero(recalcData.PrincipalDecrementingAmt) && recalcData.ListChargesToAdd.Count()<payPlanChargesCeiling) {
				PayPlanCharge ppCharge=CreateDebitCharge(recalcData.PayPlan,recalcData.Fam,recalcData.ProvNum,recalcData.ClinicNum,0,0,
					CalcNextPeriodDate(recalcData.FirstFutureChargeDate,recalcData.ListChargesToAdd.Count(),terms.Frequency),"");
				if(ppCharge.ChargeDate.Date>=terms.DateInterestStart.Date) {
					ppCharge.Interest=CalcInterestForRecalc(recalcData,terms.RoundDec);
				}
				ppCharge.Principal=CalcPrincipalForRecalc(recalcData,ppCharge.Interest);
				decimal overPaidAmtApplied=ApplyPrepaymentForRecalc(recalcData,ppCharge,terms);
				//Interest can be zero if there is no APR, the charge is fully prepaid, or charge date is less than interest start date.
				if(ppCharge.Interest==0) {
					recalcData.ChargesWithInterestCount--;//Decrement so interest gets divided correctly for remaining charges when not recalculating interest.
				}
				HandleLastCharge(recalcData,ppCharge,terms);
				recalcData.OverPaidDecrementingAmt-=overPaidAmtApplied;
				recalcData.PrincipalDecrementingAmt-=overPaidAmtApplied;
				if(recalcData.PrincipalDecrementingAmt<0) {//If overpayment is greater than principal, a negative charge must be made to offset this.
					ppCharge.Principal=(double)(0-recalcData.OverPaidDecrementingAmt+recalcData.PrincipalDecrementingAmt);
					ppCharge.Interest=0;
					recalcData.PrincipalDecrementingAmt=0;
				}
				else {
					recalcData.PrincipalDecrementingAmt-=(decimal)ppCharge.Principal;
					recalcData.InterestUnpaidDecrementingAmt-=(decimal)ppCharge.Interest;
				}
				recalcData.ListChargesToAdd.Add(ppCharge);
			}
			HandleIncreasedInterest(recalcData,terms);
			//Remove old future charges and add new recalculated ones.
			recalcData.ListPayPlanCharges.RemoveAll(x => recalcData.ListFutureDebits.Contains(x));
			recalcData.ListPayPlanCharges.AddRange(recalcData.ListChargesToAdd);
		}

		///<summary>If past payments total is greater than past charges total,
		///adds a charge to listPayPlanCharges for the overpayment amount and returns this amount.</summary>
		private static double CreateChargeMatchOverpayment(PayPlanRecalculationData recalcData) 
		{
			double pastSplitsTotal=recalcData.ListPaySplits.Where(x => x.DatePay<=DateTime.Today).Sum(x => x.SplitAmt);
			double pastDebitsTotal=recalcData.ListPastDebits.Sum(x => x.Principal+x.Interest);
			double overPaidAmount=pastSplitsTotal-pastDebitsTotal;
			if(CompareDecimal.IsGreaterThanZero(overPaidAmount)) {
				string note="Recalculated based on pay on principal";
				if(recalcData.IsPrepay) {
					note="Recalculated based on prepayment";
				}
				recalcData.ListPayPlanCharges.Add(CreateDebitCharge(recalcData.PayPlan,recalcData.Fam,recalcData.ProvNum,recalcData.ClinicNum,overPaidAmount,
					0,DateTime.Today,Lans.g("PayPlanEdit",note)));
			}
			return overPaidAmount;
		}

		///<summary>Gets the date of the next future debit or the next period date after the last debit if no future debits exist.</summary>
		private static DateTime CalcNextChargeDateForRecalc(PayPlanFrequency frequency,PayPlanRecalculationData recalcData)
		{
			if(recalcData.ListFutureDebits.Count>0) {
				return recalcData.ListFutureDebits.Min(x => x.ChargeDate);
			}
			else {//If we have no future charges, next charge date will be calculated from last charge on or before today.
				if(recalcData.ListPastDebits.Count>0) {
					return CalcNextPeriodDate(recalcData.ListPastDebits.Max(x => x.ChargeDate),1,frequency);
				}
				else {
					return CalcNextPeriodDate(DateTime.Today,1,frequency);
				}
			}
		}

		///<summary>Calculates the period payment based on plan terms and selected recalculation options.</summary>
		private static decimal CalcPeriodPaymentForRecalc(PayPlanTerms terms,PayPlanRecalculationData recalcData)
		{
			if(terms.PayCount==0) {
				return terms.PeriodPayment;
			}
			double periodExactAmt=recalcData.PrincipalRemaining+recalcData.InterestFutureUnpaidAmt;
			if(recalcData.PaymentRemainingCount>0) {
				if(terms.APR==0) {
					periodExactAmt=recalcData.PrincipalRemaining/recalcData.PaymentRemainingCount;
				}
				else if(!recalcData.IsRecalculateInterest) {//APR applies, but the user wants to keep the old interest.
					periodExactAmt=(recalcData.PrincipalRemaining+recalcData.InterestFutureUnpaidAmt)/recalcData.PaymentRemainingCount;
				}
				else {//APR applies and the user wants to recalculate the interest.
					periodExactAmt=recalcData.PrincipalRemaining*recalcData.PeriodRate/(1-Math.Pow(1+recalcData.PeriodRate,-recalcData.PaymentRemainingCount));
				}
			}
			//Round up to the nearest penny (or international equivalent).  
			//This causes the principal on the last payment to be less than or equal to the other principal amounts.
			return (decimal)(Math.Ceiling(periodExactAmt*Math.Pow(10,terms.RoundDec))/Math.Pow(10,terms.RoundDec));
		}

		///<summary>Calculates interest for the current charge being recalculated.</summary>
		private static double CalcInterestForRecalc(PayPlanRecalculationData recalcData,int roundDec)
		{
			if(recalcData.IsRecalculateInterest) {//If recalculating interest, calculate as normal
				return Math.Round((double)recalcData.PrincipalDecrementingAmt*recalcData.PeriodRate,roundDec);
			}
			//Otherwise, divide the remaining interest amongst all future payments by payment count or period payment amount
			if(recalcData.PaymentRemainingCount>0) {
				//Fully prepaid charges will have no interest
				return Math.Round(recalcData.InterestFutureUnpaidAmt/recalcData.ChargesWithInterestCount,roundDec);
			}
			return Math.Round(recalcData.InterestFutureUnpaidAmt/
				((recalcData.PrincipalRemaining+recalcData.InterestFutureUnpaidAmt)/(double)recalcData.PeriodPaymentAmt),roundDec);
		}

		///<summary>Calculates principal for current charge being recalculated.</summary>
		private static double CalcPrincipalForRecalc(PayPlanRecalculationData recalcData,double interest) {
			if(recalcData.PrincipalDecrementingAmt<recalcData.PeriodPaymentAmt-(decimal)interest) {
				return (double)recalcData.PrincipalDecrementingAmt;
			}
			return (double)recalcData.PeriodPaymentAmt-interest;
		}

		///<summary>Creates a zero charge if current charge being recalculated is fully paid,
		///otherwise interest and principal are adjusted accordingly</summary>
		private static decimal ApplyPrepaymentForRecalc(PayPlanRecalculationData recalcData,PayPlanCharge ppCharge,PayPlanTerms terms) 
		{
			decimal retVal=0;
			if(recalcData.OverPaidDecrementingAmt>0) {
				//Set to zero if full charge is prepaid
				if((double)recalcData.OverPaidDecrementingAmt>=ppCharge.Principal+ppCharge.Interest) {//principal+interest always equals period pay amount.
					retVal=(decimal)(ppCharge.Principal+ppCharge.Interest);
					ppCharge.Principal=0;
					ppCharge.Interest=0;
					ppCharge.Note=Lans.g("PayPlanEdit","Prepaid");
				}
				else {
					ppCharge.Principal-=(double)recalcData.OverPaidDecrementingAmt;
					if(recalcData.IsRecalculateInterest && ppCharge.ChargeDate.Date>=terms.DateInterestStart.Date) {
						ppCharge.Interest=
							Math.Round(((double)(recalcData.PrincipalDecrementingAmt-recalcData.OverPaidDecrementingAmt)*recalcData.PeriodRate),terms.RoundDec);
					}
					retVal=recalcData.OverPaidDecrementingAmt;
				}
			}
			return retVal;
		}

		///<summary>Principal and Interest for last charge may need to be adjusted so that full amount of interest and principal remaining
		///are used up.</summary>
		private static void HandleLastCharge(PayPlanRecalculationData recalcData, PayPlanCharge ppCharge,PayPlanTerms terms)
		{
			//check to see if this charge is the last one on the amortization schedule
			if(recalcData.PaymentRemainingCount>0 && recalcData.ListChargesToAdd.Count()==recalcData.PaymentRemainingCount-1
				|| recalcData.PrincipalDecrementingAmt+(decimal)ppCharge.Interest<=recalcData.PeriodPaymentAmt)
			{
				ppCharge.Principal=(double)recalcData.PrincipalDecrementingAmt;
				if(!recalcData.IsRecalculateInterest && ppCharge.ChargeDate.Date>=terms.DateInterestStart.Date) {
					ppCharge.Interest=(double)recalcData.InterestUnpaidDecrementingAmt;
				}
			}
		}

		///<summary>If interest for next charge was changed, adjust note. If we have reached the end of the schedule, a new charge may need to be added
		///if the plan is not paid off.</summary>
		private static void HandleIncreasedInterest(PayPlanRecalculationData recalcData,PayPlanTerms terms) {
			if(CompareDouble.IsLessThanZero(recalcData.OverPaidAmt)) {
				double interest=((recalcData.PrincipalRemaining+Math.Abs(recalcData.OverPaidAmt))*recalcData.PeriodRate);
				if(recalcData.ListChargesToAdd.Count!=0 && recalcData.ListChargesToAdd[0].ChargeDate>=terms.DateInterestStart) {
					double increasedInterestAmt=interest-recalcData.ListChargesToAdd[0].Interest;
					recalcData.ListChargesToAdd[0].Interest=interest;
					recalcData.ListChargesToAdd[0].Note=Lans.g("PayPlanEdit","Increased interest")+": "+increasedInterestAmt.ToString("c");
				}
				else if(recalcData.ListChargesToAdd.Count==0 
					&& recalcData.FirstFutureChargeDate.Date>=terms.DateInterestStart.Date
					&& recalcData.ListPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit).Count==recalcData.ListPastDebits.Count) 
				{
					recalcData.ListPayPlanCharges.Add(CreateDebitCharge(recalcData.PayPlan,recalcData.Fam,recalcData.ProvNum,recalcData.ClinicNum,
						0,interest,recalcData.FirstFutureChargeDate,Lans.g("PayPlanEdit","Increased interest")+": "+interest.ToString("c")));
				}
			}
		}
		#endregion

		#endregion
		#region PayPlanCredits
		public static PayPlanCreditLoadData GetLoadDataForPayPlanCredits(long patCurNum,long payPlanCurNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PayPlanCreditLoadData>(MethodBase.GetCurrentMethod(),patCurNum,payPlanCurNum);
			}
			PayPlanCreditLoadData creditData=new PayPlanCreditLoadData();
			List<long> listPatNums=new List<long> { patCurNum };
			creditData.ListAdjustments=Adjustments.GetAdjustForPats(listPatNums);
			creditData.ListProcs=Procedures.GetCompAndTpForPats(listPatNums);
			creditData.ListPayPlanLinksForProcs=PayPlanLinks.GetForFKeysAndLinkType(creditData.ListProcs.Select(x => x.ProcNum).ToList()
				,PayPlanLinkType.Procedure);
			//Used to figure out how much we need to pay off procs with, also contains insurance payplans.
			List<PayPlan> listPayPlans=PayPlans.GetForPats(listPatNums,patCurNum);
			if(listPayPlans.Count>0) {
				//get all current payplan charges for plans already on the patient, excluding the current one and do not get credits for current payplan
				creditData.ListPayPlanCharges=PayPlanCharges.GetDueForPayPlans(listPayPlans,patCurNum)//Does not get charges for the future.
					.Where(x =>  x.PatNum==patCurNum && !(x.PayPlanNum==payPlanCurNum && x.ChargeType==PayPlanChargeType.Credit)).ToList();
			}
			List<PaySplit> listPaySplits=PaySplits.GetForPats(listPatNums);//Might contain payplan payments
			listPaySplits.AddRange(PaySplits.GetForAdjustments(creditData.ListAdjustments.Select(x => x.AdjNum).ToList()));
			listPaySplits.AddRange(PaySplits.GetForProcs(creditData.ListProcs.Select(x => x.ProcNum).ToList()));
			creditData.ListTempPaySplit=listPaySplits.DistinctBy(x => x.SplitNum)
				.Where(x => x.UnearnedType == 0)//Do not include unearned.
				.Where(x => x.PayPlanNum==0 || listPayPlans.Exists(y => y.PayPlanNum==x.PayPlanNum)).ToList();
			creditData.ListPayments=Payments.GetNonSplitForPats(listPatNums);
			creditData.ListInsPayAsTotal=ClaimProcs.GetByTotForPats(listPatNums);//Claimprocs paid as total, might contain ins payplan payments.
			creditData.ListClaimProcs=ClaimProcs.GetForProcs(creditData.ListProcs.Select(x => x.ProcNum).ToList());
			return creditData;
		}

		///<summary>Gets all credits for the current patient.  Returns the summed credits as a decimal. Explicitly attached credits will be taken
		///care of later when linking.</summary>
		public static decimal GetAccountCredits(List<Adjustment> listAdjustments,List<PaySplit> listPaySplits,List<Payment> listPayments
			,List<ClaimProc> listInsPayAsTotal,List<PayPlanCharge> listPayPlanCharges) 
		{
			//No remoting role check; no call to db
			//Getting a date-sorted list of all credits that haven't been attributed to anything.
			decimal creditTotal=0;
			for(int i=0;i<listAdjustments.Count;i++) {
				if(listAdjustments[i].AdjAmt<0) {
					creditTotal-=(decimal)listAdjustments[i].AdjAmt;
				}
			}
			for(int i=0;i<listPaySplits.Count;i++) {
				creditTotal+=(decimal)listPaySplits[i].SplitAmt;
			}
			for(int i=0;i<listPayments.Count;i++) {
				creditTotal+=(decimal)listPayments[i].PayAmt;
			}
			for(int i=0;i<listInsPayAsTotal.Count;i++) {
				creditTotal+=(decimal)listInsPayAsTotal[i].InsPayAmt;
			}
			for(int i=0;i<listPayPlanCharges.Count;i++) {
				if(listPayPlanCharges[i].ChargeType==PayPlanChargeType.Credit) {
					creditTotal+=(decimal)listPayPlanCharges[i].Principal;
				}
			}
			//don't need to to take insurance estimates and estimated writeoffs into account as they are always explicitly linked to procedures.
			return creditTotal;
		}

		public static List<PayPlanEntry> CreatePayPlanEntriesForAccountCharges(List<AccountEntry> listAccountCharges,Patient patCur) {
			//No remoting role check; no call to db
			List<PayPlanEntry>listPayPlanEntries=new List<PayPlanEntry>();
			foreach(AccountEntry entryCharge in listAccountCharges) {
				//for all account charges
				if(entryCharge.AmountEnd==0
					|| entryCharge.GetType()==typeof(Adjustment)
					|| entryCharge.GetType()==typeof(PayPlanCharge)
					|| entryCharge.PatNum!=patCur.PatNum) {
					continue;
				}
				//if it's a procedure..
				Procedure procCur=(Procedure)entryCharge.Tag;
				listPayPlanEntries.Add(new PayPlanEntry() {
					ProcNumOrd=procCur.ProcNum,
					AmtOrd=(double)entryCharge.AmountOriginal,
					DateOrd=procCur.ProcDate,
					ProcStatOrd=procCur.ProcStatus,
					IsChargeOrd=false,
					DateStr=procCur.ProcDate.ToShortDateString(),
					PatStr=patCur.FName,
					StatStr=procCur.ProcStatus.ToString(),
					ProcStr=ProcedureCodes.GetStringProcCode(procCur.CodeNum),
					FeeStr=procCur.ProcFeeTotal.ToString("f"),
					RemBefStr=entryCharge.AmountAvailable.ToString("f"),
					Proc=procCur,
					ProvAbbr=Providers.GetAbbr(procCur.ProvNum),
					//everything else blank
				});
			}
			return listPayPlanEntries; 
		}

		public static List<PayPlanEntry> CreatePayPlanEntriesForPayPlanCredits(List<AccountEntry> listAccountCharges,
			List<PayPlanCharge> listPayPlanCreditsCur) 
		{
			//No remoting role check; no call to db
			List<PayPlanEntry> listPayPlanEntries=new List<PayPlanEntry>();
			List<PayPlanCharge> listCreditsApplied=new List<PayPlanCharge>();
			foreach(PayPlanCharge credCur in listPayPlanCreditsCur) {
				AccountEntry entryCur=listAccountCharges.Where(x => x.PriKey==credCur.ProcNum && x.GetType() == typeof(Procedure)).FirstOrDefault();
				Procedure procCur=null;
				if(entryCur!=null) {
					procCur=(Procedure)entryCur.Tag;
				}
				decimal fee=0;
				if(entryCur!=null) {
					fee=entryCur.AmountAvailable;
				}
				listCreditsApplied.Add(credCur);
				decimal remAfter=fee-listCreditsApplied.Where(x => x.ProcNum==credCur.ProcNum).Sum(x => (decimal)x.Principal);
				PayPlanEntry addEntry=new PayPlanEntry();
				addEntry.ProcNumOrd=credCur.ProcNum;
				addEntry.AmtOrd=credCur.Principal;
				addEntry.DateOrd=credCur.ChargeDate;
				addEntry.ProcStatOrd=ProcStat.TP;
				if(procCur!=null) {
					addEntry.ProcStatOrd=procCur.ProcStatus;
				}
				addEntry.IsChargeOrd=true;
				if(procCur!=null && procCur.ProcStatus==ProcStat.TP) {
					addEntry.CredDateStr=Lans.g("PayPlanEdit","None");
				}
				else {
					addEntry.CredDateStr=credCur.ChargeDate.ToShortDateString();
				}
				addEntry.AmtStr=credCur.Principal.ToString("f");
				addEntry.RemAftStr="";
				if(entryCur!=null) {
					addEntry.RemAftStr=remAfter.ToString("f");
				}
				addEntry.NoteStr=credCur.Note;
				addEntry.Proc=procCur;
				addEntry.Charge=credCur;
				//specifically to get the provider that did the work for this procedure to see if the charge has the same prov
				addEntry.ProvNum=procCur?.ProvNum??0;
				//everything else blank
				listPayPlanEntries.Add(addEntry);
			}
			return listPayPlanEntries;
		}

		///<summary>Creates a single PayPlanEntry to represent credits not attached to a procedure.</summary>
		public static PayPlanEntry CreatePayPlanEntryForUnattachedProcs(List<PayPlanCharge> listPayPlanCreditsCur,string patFName) {
			//No remoting role check; no call to db
			PayPlanEntry payPlanEntry=new PayPlanEntry();
			//add another procedure row showing all unattached credits if at least one charge with a procnum of 0 exists.
			payPlanEntry.AmtOrd=0;
			payPlanEntry.DateOrd=DateTime.MinValue;
			payPlanEntry.ProcStatOrd=ProcStat.TP; //for ordering purposes, since we want unattached to always show up last.
			payPlanEntry.IsChargeOrd=false;
			payPlanEntry.ProcNumOrd=0;
			payPlanEntry.DateStr=Lans.g("PayPlanEdit","Unattached");
			payPlanEntry.PatStr=patFName;
			payPlanEntry.StatStr="";
			payPlanEntry.ProcStr="";
			payPlanEntry.FeeStr="";
			payPlanEntry.RemBefStr="";
			payPlanEntry.ProvAbbr="";//credit will go to prov on payment plan
			return payPlanEntry;
		}

		///<summary>Creats a single PayPlanCharge credit that is unattached to a procedure.</summary>
		public static PayPlanCharge CreateUnattachedCredit(string textDate,long patNum,string textNote,long payPlanNum,double amt) {
			//No remoting role check; no call to db
			PayPlanCharge addCharge=new PayPlanCharge() {
					ChargeDate=PIn.Date(textDate),
					ChargeType=PayPlanChargeType.Credit,
					Guarantor=patNum,//credits should always appear on the patient of the payment plan.
					Note=PIn.String(textNote),
					PatNum=patNum,
					PayPlanNum=payPlanNum,
					Principal=amt,
					ProcNum=0,
					//provider/clinic will be set when the amortization schedule is saved. FormPayPlan.SaveData()
					//ClinicNum=0,
					//ProvNum=0,
				};
			return addCharge;
		}

		public static List<PayPlanCharge> CreateOrUpdateChargeForSelectedEntry(PayPlanEntry selectedEntry,List<PayPlanCharge> listPayPlanCreditsCur,
			double amt,string textNote,string textDate,long patNum,long payPlanNum,PayPlanCharge selectedCharge) 
		{
			//No remoting role check; no call to db
			selectedCharge.Principal=amt;
			selectedCharge.Note=PIn.String(textNote);
			if(selectedEntry.ProcStatOrd==ProcStat.TP && selectedEntry.ProcNumOrd!=0) { //if it's treatment planned, save the date as maxvalue so it will not show up in the ledger.
				//if it doesn't have a procnum, then we are editing an unattached row.
				selectedCharge.ChargeDate=DateTime.MaxValue;
			}
			else {
				selectedCharge.ChargeDate=PIn.Date(textDate);
			}
			if(!selectedEntry.IsChargeOrd) { //if it's a procedure
					//add a charge for the selected procedure. get info from text boxes.
					selectedCharge.ChargeType=PayPlanChargeType.Credit;
					selectedCharge.Guarantor=patNum;//credits should always appear on the patient of the payment plan.
					selectedCharge.PatNum=patNum;
					selectedCharge.PayPlanNum=payPlanNum;
					selectedCharge.ProcNum=selectedEntry.ProcNumOrd;
					//provider/clinic will be set when the amortization schedule is saved. FormPayPlan.SaveData()
					//ClinicNum=0,
					//ProvNum=0,
					listPayPlanCreditsCur.Add(selectedCharge);
				}
			return listPayPlanCreditsCur;
		}

		///<summary>Creates a PayPlanCharge credit for each entry in listSelectedProcs.</summary>
		public static List<PayPlanCharge> CreateCreditsForAllSelectedEntries(List<PayPlanEntry> listSelectedProcs,List<PayPlanEntry> listPayPlanEntries,
			DateTime date,long patNum,long payPlanNum,List<PayPlanCharge> listPayPlanCreditsCur) 
		{
			//No remoting role check; no call to db
			//add a charge for every selected procedure for the amount remaining.
			//don't allow adding $0.00 credits.
			foreach(PayPlanEntry entryProcCur in listSelectedProcs) {
				List<PayPlanEntry> listEntriesForProc=listPayPlanEntries
					.Where(x => x.ProcNumOrd==entryProcCur.ProcNumOrd)
					.Where(x => x.IsChargeOrd==true).ToList();
				PayPlanCharge addCharge=new PayPlanCharge();
				if(entryProcCur.ProcStatOrd==ProcStat.TP) {//If tp, maxvalue.
					addCharge.ChargeDate=DateTime.MaxValue;
				}
				else {
					addCharge.ChargeDate=date;
				}
				addCharge.ChargeType=PayPlanChargeType.Credit;
				addCharge.Guarantor=patNum;//credits should always appear on the patient of the payment plan.
				addCharge.Note=ProcedureCodes.GetStringProcCode(entryProcCur.Proc.CodeNum)+": "+Procedures.GetDescription(entryProcCur.Proc);
				addCharge.PatNum=patNum;
				addCharge.PayPlanNum=payPlanNum;
				addCharge.Principal=PIn.Double(entryProcCur.RemBefStr);
				if(listEntriesForProc.Count!=0) {
					addCharge.Principal=listEntriesForProc.Min(x => PIn.Double(x.RemAftStr));
				}
				addCharge.ProcNum=entryProcCur.ProcNumOrd;
				//provider/clinic will be set when the amortization schedule is saved. FormPayPlan.SaveData()
				//ClinicNum=0,
				//ProvNum=0,
				if(CompareDouble.IsGreaterThan(addCharge.Principal,0)) {
					listPayPlanCreditsCur.Add(addCharge);
				}
			}
			return listPayPlanCreditsCur;
		}

		///<summary>Links charges to credits explicitly based on FKs first, then implicitly based on Date.</summary>
		public static PayPlanLinked LinkChargesToCredits(List<AccountEntry> listAccountCharges,List<PaySplit> listPaySplits
			,List<Adjustment> listAdjustments,List<PayPlanCharge> listPayPlanCharges,List<ClaimProc> listClaimProcs,long payPlanNum,decimal accountCredits,
			List<Procedure> listProcedures) 
		{
			//No remoting role check; no call to db
			PayPlanLinked retVal=new PayPlanLinked();
			#region Explicit
			for(int i=0;i<listAccountCharges.Count;i++) {
				AccountEntry charge=listAccountCharges[i];
				if(charge.GetType()==typeof(Procedure)) {
					Procedure procedure=listProcedures.Find(x => x.ProcNum==charge.ProcNum);
					if(procedure!=null && procedure.ProcStatus!=ProcStat.C) {
						charge.AmountEnd-=(decimal)procedure.DiscountPlanAmt; //This might not actually be used, but for consistancy.
						charge.AmountAvailable-=(decimal)procedure.DiscountPlanAmt;
					}
				}
				for(int j=0;j<listPaySplits.Count;j++) {
					PaySplit paySplit=listPaySplits[j];
					decimal paySplitAmt=(decimal)paySplit.SplitAmt;
					//Procedures that were being paid on through this payment plan should not get removed from this grid, even if they are fully paid off.
					if(charge.GetType()==typeof(Procedure)
						&& paySplit.ProcNum==charge.PriKey
						&& (paySplit.PayPlanNum == 0 || paySplit.PayPlanNum != payPlanNum)) 
					{
						charge.SplitCollection.Add(paySplit);
						charge.AmountEnd-=paySplitAmt;
						accountCredits-=paySplitAmt;
						charge.AmountAvailable-=paySplitAmt;
					}
					else if(charge.GetType()==typeof(PayPlanCharge) && ((PayPlanCharge)charge.Tag).PayPlanNum==paySplit.PayPlanNum 
						&& charge.AmountEnd>0 && paySplit.SplitAmt>0) 
					{
						charge.AmountEnd-=paySplitAmt;
						accountCredits-=paySplitAmt;
					}
				}
				for(int j=0;j<listAdjustments.Count;j++) {
					Adjustment adjustment=listAdjustments[j];
					decimal adjustmentAmt=(decimal)adjustment.AdjAmt;
					if(charge.GetType()==typeof(Procedure) && adjustment.ProcNum==charge.PriKey) {
						charge.AmountEnd+=adjustmentAmt;
						if(adjustment.AdjAmt<0) {
							accountCredits+=adjustmentAmt;
						}
						charge.AmountAvailable+=adjustmentAmt;
						//If the adjustment is attached to a procedure decrease the procedure's amountoriginal so we know what it was just prior to autosplitting.
					}
				}
				for(int j=0;j < listPayPlanCharges.Count;j++) {
					PayPlanCharge payPlanCharge=listPayPlanCharges[j];
					if(charge.GetType()==typeof(Procedure) && payPlanCharge.ProcNum == charge.PriKey) //payPlanCharge.ProcNum will only be set for credits.
						{
						charge.AmountEnd-=(decimal)payPlanCharge.Principal;
						charge.AmountAvailable-=(decimal)payPlanCharge.Principal;
						accountCredits-=(decimal)payPlanCharge.Principal;
					}
				}
				//claimprocs explicitly linked to the procedures for this patient.
				for(int j=0;j < listClaimProcs.Count;j++) {
					ClaimProc claimProcCur=listClaimProcs[j];
					if(charge.GetType()!=typeof(Procedure) || claimProcCur.ProcNum!=charge.PriKey) {
						continue;
					}
					decimal amt=0;
					if((claimProcCur.Status==ClaimProcStatus.Estimate || claimProcCur.Status==ClaimProcStatus.NotReceived)) {
						//Estimated Payment
						amt=(decimal)claimProcCur.InsEstTotal;
						if(claimProcCur.InsEstTotalOverride!=-1) {
							amt=(decimal)claimProcCur.InsEstTotalOverride;
						}
						charge.AmountEnd-=amt;
						charge.AmountAvailable-=amt;
						//Estimated Writeoff
						amt=0;
						if(claimProcCur.WriteOffEstOverride!=-1) {
							amt=(decimal)claimProcCur.WriteOffEstOverride;
						}
						else if(claimProcCur.WriteOffEst!=-1) {
							amt=(decimal)claimProcCur.WriteOffEst;
						}
						charge.AmountEnd-=amt;
						charge.AmountAvailable-=amt;
					}
					else if(claimProcCur.Status==ClaimProcStatus.Received || claimProcCur.Status==ClaimProcStatus.Supplemental
						|| claimProcCur.Status==ClaimProcStatus.CapClaim || claimProcCur.Status==ClaimProcStatus.CapComplete) 
					{
						//actual payment and actual writeoff.
						amt=(decimal)claimProcCur.InsPayAmt+(decimal)claimProcCur.WriteOff;
						charge.AmountEnd-=amt;
						charge.AmountAvailable-=amt;
					}
				}
			}
			#endregion
			#region apply negative charges
			//Apply negative charges as if they're credits.
			for(int i=0;i < listAccountCharges.Count;i++) {
				AccountEntry entryCharge=listAccountCharges[i];
				if(entryCharge.AmountEnd<0) {
					accountCredits-=entryCharge.AmountEnd;
					entryCharge.AmountEnd=0;
				}
			}
			#endregion
			#region Implicit
			//Now we have a date-sorted list of all the unpaid charges as well as all non-attributed credits.  
			//We need to go through each and pay them off in order until all we have left is the most recent unpaid charges.
			for(int i=0;i<listAccountCharges.Count && accountCredits>0;i++) {
				if(listAccountCharges[i].Tag.GetType()==typeof(Procedure) && ((Procedure)listAccountCharges[i].Tag).ProcStatus==ProcStat.TP) {
					continue;//do not implicitly 'pay off' TP procs. They should always show. 
				}
				AccountEntry charge=listAccountCharges[i];
				decimal amt=Math.Min(charge.AmountEnd,accountCredits);
				charge.AmountEnd-=amt;
				accountCredits-=amt;
				charge.AmountAvailable-=amt;//Decrease amount original for the charge so we know what it was just prior to when the autosplits were made.
			}
			#endregion
			retVal.ListAccountCharges=listAccountCharges;
			retVal.AccountCredits=accountCredits;
			return retVal;
		}
		#endregion
		#region Dynamic Payment Plans
		public static List<PayPlanCharge> GetListExpectedCharges(List<PayPlanCharge> listChargesInDB,PayPlanTerms terms,Family famCur
			,List<PayPlanLink> listPayPlanLinks,PayPlan payplan,bool isNextPeriodOnly,bool isForDownPaymentCharge=false,List<PaySplit> listPaySplits=null
			,List<PayPlanCharge> listExpectedChargesDownPayment=null)
		{
			//no remoting role check; no call to db
			if(listPaySplits==null) {
				listPaySplits=PaySplits.GetForPayPlans(new List<long>(){payplan.PayPlanNum});
			}
			int chargesCount=listChargesInDB.Count; 
			int periodCount=listChargesInDB.DistinctBy(x => x.ChargeDate.Date).Count();
			if(terms.DownPayment!=0 && listChargesInDB.Count!=0 && terms.DateFirstPayment.Date!=listChargesInDB[0].ChargeDate.Date) {
				periodCount--;//down payment does not count towards the period count since it was made before the start date.
			}
			return GetListExpectedCharges(listChargesInDB,terms,famCur,listPayPlanLinks,listPaySplits,payplan,isNextPeriodOnly,chargesCount,periodCount
				,isForDownPaymentCharge,listExpectedChargesDownPayment);
		}

		///<summary>Purpose is to calculate expected charges that have not come due yet, based on current terms. Does not include down payment. </summary>
		public static List<PayPlanCharge> GetListExpectedCharges(List<PayPlanCharge> listChargesInDB,PayPlanTerms terms,Family famCur
			,List<PayPlanLink> listPayPlanLinks,List<PaySplit> listPaySplits,PayPlan payplan,bool isNextPeriodOnly,int chargesCount,int periodCount
			,bool isForDownPaymentCharge,List<PayPlanCharge> listExpectedChargesDownPayment=null)
		{
			//no remoting role check; no call to db
			List<PayPlanCharge> listExpectedCharges=new List<PayPlanCharge>();
			//Get production attached to credits attached to the payment plan (what we will be making charges for)
			List<PayPlanProductionEntry> listCreditsAndProduction=PayPlanProductionEntry.GetWithAmountRemaining(listPayPlanLinks,listChargesInDB);
			if(listExpectedChargesDownPayment!=null) {
				//Charges for down payment are created but not in DB yet, so we must subtract these charges from the remaining amounts of production items.
				foreach(PayPlanProductionEntry productionEntry in listCreditsAndProduction) {
					productionEntry.AmountRemaining-=(decimal)listExpectedChargesDownPayment
						.Where(x => x.FKey==productionEntry.PriKey && x.LinkType==productionEntry.LinkType).Sum(x => x.Principal);
				}
			}
			decimal sumPrincipalNotYetCharged=(decimal)terms.PrincipalAmount-(decimal)listChargesInDB.Sum(x => x.Principal);
			if(terms.DynamicPayPlanTPOption==DynamicPayPlanTPOptions.AwaitComplete && !isForDownPaymentCharge) {
				List<PayPlanProductionEntry> payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
				for(int i = 0;i<payPlanProductionEntries.Count;i++) {
					Procedure proc=null;
					if(payPlanProductionEntries[i].LinkType==PayPlanLinkType.Procedure) {
						proc=Procedures.GetOneProc((long)listPayPlanLinks[i].FKey,false);
					}
					if(proc!=null && proc.ProcStatus!=ProcStat.C) {//TP procs don't really have value yet until they're complete
						sumPrincipalNotYetCharged-=payPlanProductionEntries[i].AmountOriginal;
					}
				}
			}
			//If plan has no charges in DB yet, and method isn't being called to create down payment charges,
			//we must subtract the down payment from the principal so it is not considered in interest calculations.
			if(listChargesInDB.Count==0 && !isForDownPaymentCharge) {
				sumPrincipalNotYetCharged-=(decimal)terms.DownPayment;
			}
			//specifically to make a check more accurate where determining if payplan will ever be paid off
			decimal previousSumPrincipalNotYetCharged=sumPrincipalNotYetCharged;
			int maxPayPlanCharges=2000;//ceiling of payplan charges should not go beyond 2000
			while(sumPrincipalNotYetCharged > 0 && chargesCount < maxPayPlanCharges) {
				DateTime periodDate=CalcNextPeriodDate(terms.DateFirstPayment,periodCount,terms.Frequency);
				double interestForPeriod=CalculateInterestAmountForPeriod(sumPrincipalNotYetCharged,terms,periodDate,isNextPeriodOnly,listPaySplits
					,listChargesInDB,listExpectedCharges);
				double principalForPeriod=CalculatePrincipalAmountForPeriod(sumPrincipalNotYetCharged,terms,interestForPeriod);
				if(CompareDouble.IsEqual(principalForPeriod,0)) {
					break;
				}
				if((decimal.Compare(sumPrincipalNotYetCharged,previousSumPrincipalNotYetCharged) > 0 && interestForPeriod > 0)) {
					//The principal is actually increasing or staying the same with each payment.
					terms.AreTermsValid=false;
					listExpectedCharges.Clear();
					return listExpectedCharges;//return nothing. We don't want to leave charges in this list since we're stopping our calculations.
				}
				previousSumPrincipalNotYetCharged=sumPrincipalNotYetCharged;
				decimal curPayAmount=0;
				#region Generate Charges For Period
				while(!CompareDecimal.IsEqual(curPayAmount,terms.PeriodPayment) && !CompareDouble.IsZero(principalForPeriod) && chargesCount < maxPayPlanCharges) {
					PayPlanCharge chargeCur=new PayPlanCharge();
					PayPlanProductionEntry entry=listCreditsAndProduction.FirstOrDefault(x => CompareDecimal.IsGreaterThanZero(x.AmountRemaining));
					if(entry==null) {
						return listExpectedCharges;
					}
					Procedure proc=null;
					if(entry.LinkType==PayPlanLinkType.Procedure) {
						proc=Procedures.GetOneProc(entry.PriKey,false);
					}
					//Used for handling 'Await Complete' option in dynamic payment plans. skips over adding a charge for an incomplete proc.
					if(proc!=null && proc.ProcStatus==ProcStat.TP && terms.DynamicPayPlanTPOption==DynamicPayPlanTPOptions.AwaitComplete && !isForDownPaymentCharge) {
						entry.AmountRemaining=0;
						continue;
					}
					//compare amount remaining on attached production with the period payment amount.
					double principalForCharge=Math.Min(principalForPeriod,(double)entry.AmountRemaining);
					if(curPayAmount>0) {
						interestForPeriod=0;//only apply interest to the first charge for the period. 
					}
					chargeCur=CreateDebitChargeDynamic(payplan,famCur,entry.ProvNum,entry.ClinicNum,principalForCharge,interestForPeriod
						,periodDate,"",entry.PriKey,entry.LinkType);
					entry.AmountRemaining-=(decimal)principalForCharge;
					sumPrincipalNotYetCharged-=(decimal)chargeCur.Principal;
					principalForPeriod-=chargeCur.Principal;
					curPayAmount+=(decimal)chargeCur.Principal;
					listExpectedCharges.Add(chargeCur);
					chargesCount++;
				}
				#endregion
				periodCount++;
				if(isNextPeriodOnly && listExpectedCharges.Count > 0) {
					return listExpectedCharges;//only returns charges made for this next period. 
				}
			}
			return listExpectedCharges;//returns all expected for the entire life of the payment plan. 
		}

		///<summary>Returns the difference between the principal and sum of pay splits that apply to the principal.
		///If the sum of pay splits that apply to the principal is less than the downpayment amount, 
		///then the difference between the principal and the down payment amount is returned instead.</summary>
		public static double CalculatePrincipalRemaining(double principalAmount,double downPayment,List<PaySplit> listPaySplitsForPayPlan
			,List<PayPlanCharge> listChargesInDB,List<PayPlanCharge> listExpectedCharges)
		{
			//Total up all of the PaySplits attached to the payment plan.
			double amtToApply=listPaySplitsForPayPlan.Sum(x => x.SplitAmt);
			//Always "apply" PaySplits towards interest charges first.
			//This can cause future interest charges to fluctuate sometimes (typically will increase).
			//There is currently no way to explicitly specify payments as allocated to principal only or interest only.
			//This code will need to change once that feature has been introduced.  Old code used PayPlanChargeNum to achieve this.
			//It would be best to have an official flag that designates amounts towards principal and interest.
			//This is mainly because PayPlanCharge entries hold value for both interest and principal and we need to support partials.
			double sumInterestCharged=listChargesInDB.Sum(x => x.Interest)+listExpectedCharges.Sum(x => x.Interest);
			amtToApply-=Math.Min(amtToApply,sumInterestCharged);
			//Figure out the amount of principal remaining by subtracting the overall principal from the remainder of PaySplits on the plan after interest.
			//This may be negative if the entire plan is overpaid. We must subtract the down payment amount at minimum as we never charge interest on it.
			//The result is the total amount of principal left on the payment plan that can be used to calculate future interest charges.
			double planTotalPrincipalRemaining=(principalAmount-Math.Max(downPayment,amtToApply));
			//Return zero if the total principal balance is negative because we can't use a negative balance in interest calculations.
			return Math.Max(0,planTotalPrincipalRemaining);
		}

		public static decimal CalculatePeriodPayment(double apr,PayPlanFrequency frequency,decimal periodPayment,int payCount,int roundDec
			,double principalAmount,double downPayment) 
		{
			double periodRate=CalcPeriodRate(apr,frequency);
			return CalculatePeriodPayment(payCount,apr,periodPayment,roundDec,principalAmount-downPayment,periodRate);
		}

		public static decimal CalculatePeriodPayment(int payCount,double apr,decimal periodPayment,int roundDec,double principalAmount,double periodRate) {
			decimal periodPaymentAmt=0;
			if(payCount==0) {
				periodPaymentAmt=periodPayment;
			}
			else {//will need to save what this initial periodPaymentAmt is and use it for the future
				double periodExactAmt=0;
				if(apr==0) {
					periodExactAmt=principalAmount/payCount;
				}
				else {
					periodExactAmt=principalAmount*periodRate/(1-Math.Pow(1+periodRate,-payCount));
				}
				//Round up to the nearest penny (or international equivalent).
				//This causes the principal on the last payment to be less than or equal to the other principal amounts.
				periodPaymentAmt=(decimal)(Math.Ceiling(periodExactAmt*Math.Pow(10,roundDec))/Math.Pow(10,roundDec));
			}
			return periodPaymentAmt;
		}

		private static double CalculatePrincipalAmountForPeriod(decimal sumPrincipalNotYetCharged,PayPlanTerms terms,double interest) {
			//sumPrincipalNotCharged is the sum of the principal that is expected to be charged out in the future
			double principal;
			if(sumPrincipalNotYetCharged < (terms.PeriodPayment - (decimal)interest)) {
				principal=(double)sumPrincipalNotYetCharged;
			}
			else {
				principal=(double)terms.PeriodPayment-interest;
			}
			if(sumPrincipalNotYetCharged+(decimal)interest <= terms.PeriodPayment) {
				principal=(double)sumPrincipalNotYetCharged;//all remaining principal.
			}
			return principal;
		}

		///<summary>Calculates how much will be charged for interest for the given periodDate.</summary>
		private static double CalculateInterestAmountForPeriod(decimal sumPrincipalNotYetCharged,PayPlanTerms terms,DateTime periodDate
			,bool isNextPeriodOnly,List<PaySplit> listPaySplits,List<PayPlanCharge> listChargesInDB,List<PayPlanCharge> listExpectedCharges)
		{
			double interest=0;
			if(periodDate.Date>=terms.DateInterestStart.Date) {
				double periodRate=CalcPeriodRate(terms.APR,terms.Frequency);
				double sumChargesAllPrincipal=listChargesInDB.Sum(x => x.Principal)+listExpectedCharges.Sum(x => x.Principal);
				double sumChargesAllPrincipalAndInterest=sumChargesAllPrincipal+listChargesInDB.Sum(x => x.Interest)+listExpectedCharges.Sum(x => x.Interest);
				bool isPlanOverpaid=CompareDouble.IsGreaterThan(listPaySplits.Sum(x => x.SplitAmt),sumChargesAllPrincipalAndInterest);
				decimal principalAmountUsedForCalculation=sumPrincipalNotYetCharged;
				//If we are creating a charge, or the payments made towards the plan have covered all posted charges and all expected charges generated thus
				//far, we must calculate interest using the amount of principal that has not been covered yet. 
				//This means that when an overpayment has been made, the amortization schedule may show increasing interest on expected charges that
				//are covered by the overpayment because as each one is generated some of the overpayment will be pulled away from crediting principal toward
				//the interest of the charge being generated.
				if(isNextPeriodOnly || isPlanOverpaid) {
					//Figure out the total principal value for the plan by adding up the principal on the charges in the database, the 'future' charges that 
					//have been calculated, and the amount of principal that has yet to be considered (via the calling method).
					double principalAmount=sumChargesAllPrincipal+(double)sumPrincipalNotYetCharged;
					principalAmountUsedForCalculation=(decimal)CalculatePrincipalRemaining(principalAmount,terms.DownPayment,listPaySplits,listChargesInDB
						,listExpectedCharges);
				}
				interest=Math.Round(((double)principalAmountUsedForCalculation*periodRate),terms.RoundDec);
			}
			return interest;
		}

		public static PaymentSchedule GetPayScheduleFromFrequency(PayPlanFrequency frequency) {
			switch(frequency) {
				case PayPlanFrequency.Weekly:
					return PaymentSchedule.Weekly;
				case PayPlanFrequency.EveryOtherWeek:
					return PaymentSchedule.BiWeekly;
				case PayPlanFrequency.OrdinalWeekday:
					return PaymentSchedule.MonthlyDayOfWeek;
				case PayPlanFrequency.Monthly:
					return PaymentSchedule.Monthly;
				case PayPlanFrequency.Quarterly:
					return PaymentSchedule.Quarterly;
				default:
					return PaymentSchedule.Monthly;//most common
			}
		}

		///<summary>Returns the terms for the payment plan from the fields on the object.</summary>
		public static PayPlanTerms GetPayPlanTerms(PayPlan payPlan,List<PayPlanLink> listLinksForPayPlan) {
			PayPlanTerms terms=new PayPlanTerms();
			terms.APR=payPlan.APR;
			terms.DateInterestStart=payPlan.DateInterestStart;
			terms.DateAgreement=payPlan.PayPlanDate;
			terms.DateFirstPayment=payPlan.DatePayPlanStart;
			terms.DownPayment=payPlan.DownPayment;
			terms.Frequency=payPlan.ChargeFrequency;
			terms.PaySchedule=payPlan.PaySchedule;
			//payCount is excluded because it does not apply to dynamic payment plans
			terms.PeriodPayment=(decimal)payPlan.PayAmt;
			terms.DynamicPayPlanTPOption=payPlan.DynamicPayPlanTPOption;
			terms.PrincipalAmount=(double)PayPlanProductionEntry.GetProductionForLinks(listLinksForPayPlan)
				.Sum(x => x.AmountOverride==0?x.AmountOriginal:x.AmountOverride);
			terms.RoundDec=CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
			return terms;
		}
		#endregion
		#region Data Classes
		[Serializable]
		public class PayPlanCreditLoadData {
			public List<Adjustment> ListAdjustments=new List<Adjustment>();
			public List<Procedure> ListProcs=new List<Procedure>();
			public List<PayPlanCharge> ListPayPlanCharges=new List<PayPlanCharge>();
			public List<PaySplit> ListTempPaySplit=new List<PaySplit>();
			public List<Payment> ListPayments=new List<Payment>();
			public List<ClaimProc> ListInsPayAsTotal=new List<ClaimProc>();
			public List<ClaimProc> ListClaimProcs=new List<ClaimProc>();
			public List<PayPlanLink> ListPayPlanLinksForProcs=new List<PayPlanLink>();
		}

		public class PayPlanEntriesAndCharges {
			public List<PayPlanEntry> ListPayPlanEntries=new List<PayPlanEntry>();
			public List<PayPlanCharge> ListPayPlanCharges=new List<PayPlanCharge>();
		}

		public class PayPlanLinked {
			public List<AccountEntry> ListAccountCharges=new List<AccountEntry>();
			public decimal AccountCredits;
		}

		///<summary>Class for ordering and displaying line items in FormPayPlanCredits.</summary>
		public class PayPlanEntry {
			//ordering fields
			public long ProcNumOrd;
			public DateTime DateOrd;
			public double AmtOrd;
			public bool IsChargeOrd;//true if payplancharge, false if procedure
			public ProcStat ProcStatOrd;
			//visible fields
			public string DateStr="";
			public string PatStr="";
			public string StatStr="";
			public string ProcStr="";
			public string FeeStr="";
			public string RemBefStr="";
			public string CredDateStr="";
			public string AmtStr="";
			public string RemAftStr="";
			public string NoteStr="";
			public string ProvAbbr="";
			//other fields
			///<summary>Stores the procedure associated to the payplanentry. Null if none.</summary>
			public Procedure Proc;
			///<summary>If a charge, stores the payplancharge associated. Null if a procedure.</summary>
			public PayPlanCharge Charge;
			///<summary>Informational field to determine the provider on the procedure for that credit.</summary>
			public long	ProvNum;
		}

		///<summary>Helper class to store all data needed for recalculating an existing amortization schedule.</summary>
		public class PayPlanRecalculationData {
			///<summary>The pay plan for which the amortization schedule is being recalculated.</summary>
			public PayPlan PayPlan=new PayPlan();
			///<summary>Family of the patient to which the pay plan belongs.</summary>
			public Family Fam=new Family();
			///<summary>ProvNum associated to the pay plan.</summary>
			public long ProvNum;
			///<summary>ClinicNum associated to the pay plan.</summary>
			public long ClinicNum;
			///<summary>All pay plan charges for the pay plan.</summary>
			public List<PayPlanCharge> ListPayPlanCharges=new List<PayPlanCharge>();
			///<summary>All pay plan charges of type debit dated on or before today.</summary>
			public List<PayPlanCharge> ListPastDebits=new List<PayPlanCharge>();
			///<summary>All pay plan charges of type debit dated after today.</summary>
			public List<PayPlanCharge> ListFutureDebits=new List<PayPlanCharge>();
			///<summary>All recalculated charges that will be added to ListPayPlanCharges.</summary>
			public List<PayPlanCharge> ListChargesToAdd=new List<PayPlanCharge>();
			///<summary>Pay plans payment count minus count of past debits. Down payment charges and charges created due to previous recalculations are
			///not included in the count of past debits subtracted.</summary>
			public int PaymentRemainingCount;
			///<summary>List of pay splits associated to the pay plan.</summary>
			public List<PaySplit> ListPaySplits=new List<PaySplit>();
			///<summary>Indicates whether the prepay or pay on principal option was selected on the Payment Plan Recalculate window.</summary>
			public bool IsPrepay;
			///<summary>Indicates whether the recalculate interest box was checked on the Payment Plan Recalculate window.</summary>
			public bool IsRecalculateInterest;
			///<summary>Principal that is yet to be charged. Equals total principal minus the sum of principal for past debits.</summary>
			public double PrincipalRemaining;
			///<summary>Sum of interest for future debits.</summary>
			public double InterestFutureUnpaidAmt;
			///<summary>Interest rate calculated from APR and pay period frequency.</summary>
			public double PeriodRate;
			///<summary>Date of next pay period.</summary>
			public DateTime FirstFutureChargeDate;
			///<summary>Sum of pay splits dated on or before today minus the sum of past debits. May be negative if patient has under-paid.</summary>
			public double OverPaidAmt;
			///<summary>Amount due each pay period. Sum of principal and interest for each pay period may not sum to this amount for all pay periods
			///after recalculation.</summary>
			public decimal PeriodPaymentAmt;
			///<summary>Amount to subtract principal of charges from during recalculation. Recalculation will be complete when this reaches zero.</summary>
			public decimal PrincipalDecrementingAmt;
			///<summary>Only used when not recalculating interest. Amount to decrement as interest is distributed to recalculated charges.</summary>
			public decimal InterestUnpaidDecrementingAmt;
			///<summary>Only used when prepay option is selected. Decremented as prepayments are distributed to recalculated charges.</summary>
			public decimal OverPaidDecrementingAmt;
			///<summary>Only used when not recalculating interest. Holds the count of charges that interest will be distributed amongst.</summary>
			public int ChargesWithInterestCount;

			///<summary>Calculates and holds variables needed for running recalculation of existing amortization schedule.</summary>
			public static PayPlanRecalculationData CreateRecalculationData(PayPlanTerms terms,PayPlan payPlan,Family fam,long provNum,long clinicNum,
				List<PayPlanCharge> listPayPlanCharges,List<PaySplit> listPaySplits,bool isPrepay,bool isRecalculateInterest) 
			{
				PayPlanRecalculationData recalcData=new PayPlanRecalculationData();
				recalcData.PayPlan=payPlan;
				recalcData.Fam=fam;
				recalcData.ProvNum=provNum;
				recalcData.ClinicNum=clinicNum;
				recalcData.ListPayPlanCharges=listPayPlanCharges;
				recalcData.ListPastDebits=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.ChargeDate <= DateTime.Today);
				recalcData.ListFutureDebits=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.ChargeDate > DateTime.Today);
				recalcData.ListChargesToAdd=new List<PayPlanCharge>();
				recalcData.PaymentRemainingCount=terms.PayCount-recalcData.ListPastDebits
					.Where(x => !x.Note.Trim().ToLower().Contains("recalculated based on") && !x.Note.Trim().ToLower().Contains("downpayment")).Count();
				recalcData.ChargesWithInterestCount=recalcData.PaymentRemainingCount;
				recalcData.ListPaySplits=listPaySplits;
				recalcData.IsPrepay=isPrepay;
				recalcData.IsRecalculateInterest=isRecalculateInterest;
				recalcData.InterestFutureUnpaidAmt=recalcData.ListFutureDebits.Sum(x => x.Interest);
				recalcData.InterestUnpaidDecrementingAmt=(decimal)recalcData.InterestFutureUnpaidAmt;
				recalcData.PeriodRate=CalcPeriodRate(terms.APR,terms.Frequency);
				recalcData.FirstFutureChargeDate=CalcNextChargeDateForRecalc(terms.Frequency,recalcData);
				recalcData.PrincipalRemaining=terms.PrincipalAmount-recalcData.ListPastDebits.Sum(x => x.Principal);
				recalcData.PeriodPaymentAmt=CalcPeriodPaymentForRecalc(terms,recalcData);
				//Below properties are set in PayPlanEdit.RecalculateScheduleCharges() after matching overpayment charge is created.
				//recalcData.OverPaidAmt;
				//recalcData.PrincipalDecrementingAmt;
				//recalcData.OverPaidDecrementingAmt;
				return recalcData;
			}
		}
		#endregion
	}

	///<summary>Helper class to store all of the current terms for the payment plan. Used to calculate planned future charges.</summary>
	public class PayPlanTerms {
		public double PrincipalAmount;
		public double APR;
		public DateTime DateInterestStart;
		public decimal PeriodPayment;
		public int PayCount;
		public DateTime DateFirstPayment;
		public int RoundDec;
		public PayPlanFrequency Frequency;
		///<summary>Defaults to true.  Gets set to false once determined that the payamt and interest cannot be paid off with current values.</summary>
		public bool AreTermsValid=true;
		public DateTime DateAgreement;
		public double DownPayment;
		public PaymentSchedule PaySchedule;
		public DynamicPayPlanTPOptions DynamicPayPlanTPOption;
	}
}
