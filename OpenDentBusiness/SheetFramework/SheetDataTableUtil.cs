using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;
using CodeBase;

namespace OpenDentBusiness.SheetFramework {
	public class SheetDataTableUtil {
		private static List<MedLabResult> _listResults;
		///<Summary>DataSet should be prefilled with AccountModules.GetAccount() before calling this method if getting a table for a statement.</Summary>
		public static DataTable GetDataTableForGridType(Sheet sheet,DataSet dataSet,string gridType,Statement stmt,MedLab medLab,Patient patGuar = null) {
			DataTable retVal=new DataTable();
			switch(gridType) {
				case "StatementMain":
					retVal=GetTable_StatementMain(dataSet,stmt);
					break;
				case "StatementAging":
					retVal=GetTable_StatementAging(stmt,patGuar);
					break;
				case "StatementPayPlan":
					retVal=GetTable_StatementPayPlan(dataSet,false);
					break;
				case "StatementDynamicPayPlan":
					retVal=GetTable_StatementPayPlan(dataSet,true);
					break;
				case "StatementEnclosed":
					retVal=GetTable_StatementEnclosed(dataSet,stmt,patGuar);
					break;
				case "StatementInvoicePayment":
					//payment info we need is not in dataSet, so don't even bother passing it in.
					retVal=GetTable_StatementInvoicePayment(stmt);
					break;
				case "MedLabResults":
					retVal=GetTable_MedLabResults(medLab);
					break;
				case "PayPlanMain":
					retVal=GetTable_PayPlanMain(sheet);
					break;
				case "TreatPlanMain":
					retVal=GetTable_TreatPlanMain(sheet);
					break;
				case "TreatPlanBenefitsFamily":
					retVal=GetTable_TreatPlanBenefitsFamily(sheet);
					break;
				case "TreatPlanBenefitsIndividual":
					retVal=GetTable_TreatPlanBenefitsIndividual(sheet);
					break;
				case "EraClaimsPaid":
					retVal=GetTable_EraClaimsPaid(sheet);
					break;
				case "EraClaimsPaidProcRemarks":
					retVal=GetTable_EraClaimsPaidProcRemarks(sheet);
					break;
				case "ReferralLetterProceduresCompleted":
					retVal=GetTable_ReferralLetterProceduresCompleted(sheet);
					break;
				default:
					break;
			}
			return retVal;
		}

		///<summary>Gets account tables by calling AccountModules.GetAccount and then appends dataRows together into a single table.
		///DataSet should be prefilled with AccountModules.GetAccount() before calling this method.</summary>
		private static DataTable GetTable_StatementMain(DataSet dataSet,Statement stmt) {
			DataTable retVal=null;
			foreach(DataTable t in dataSet.Tables) {
				if(!t.TableName.StartsWith("account")) {
					continue;
				}
				if(retVal==null) {//first pass
					retVal=t.Clone();
				}
				foreach(DataRow r in t.Rows) {
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && stmt.IsReceipt) {//Canadian. en-CA or fr-CA
						if(r["StatementNum"].ToString()!="0") {//Hide statement rows for Canadian receipts.
							continue;
						}
						if(r["ClaimNum"].ToString()!="0") {//Hide claim rows and claim payment rows for Canadian receipts.
							continue;
						}
						if(PIn.Long(r["ProcNum"].ToString())!=0) {
							r["description"]="";//Description: blank in Canada normally because this information is used on taxes and is considered a security concern.
						}
						r["ProcCode"]="";//Code: blank in Canada normally because this information is used on taxes and is considered a security concern.
						r["tth"]="";//Tooth: blank in Canada normally because this information is used on taxes and is considered a security concern.
					}
					if(CultureInfo.CurrentCulture.Name=="en-US" && stmt.IsReceipt && r["PayNum"].ToString()=="0") {//Hide everything except patient payments
						continue;
						//js Some additional features would be nice for receipts, such as hiding the bal column, the aging, and the amount due sections.
					}
					//The old way of printing "Single patient only" receipts would simply show all rows from the "account" table in one grid for foreign users.
					//In order to keep this functionality for "Statements use Sheets" we need to force all rows to be associated to the stmt.PatNum.
					if(CultureInfo.CurrentCulture.Name!="en-US"
						&& !CultureInfo.CurrentCulture.Name.EndsWith("CA")
						&& stmt.IsReceipt
						&& stmt.SinglePatient) {
						long patNumCur=PIn.Long(r["PatNum"].ToString());
						//If the PatNum column is valid and is for a different patient then force it to be for this patient so that it shows up in the same grid.
						if(patNumCur > 0 && patNumCur!=stmt.PatNum) {
							r["PatNum"]=POut.Long(stmt.PatNum);
						}
					}
					//GetTable_StatementMain() gets called every time FormSheetFillEdit needs to redraw. That happens when we preview or click on the statement
					//Thus, we would be prepending the provider into the description every call. This bool prevents that from happening if we manually added the provider in the description.
					//This only concerns English (Australia).
					bool hasProvInDescription=PIn.String(r["Description"].ToString()).StartsWith(PIn.String(r["prov"].ToString())+" - ");
					if(CultureInfo.CurrentCulture.Name=="en-AU" && r["prov"].ToString().Trim()!="" && !hasProvInDescription) {
						r["description"]=r["prov"]+" - "+r["description"];
					}
					retVal.ImportRow(r);
				}
				if(t.Rows.Count==0) {
					Patient p=Patients.GetPat(PIn.Long(t.TableName.Replace("account","")))??Patients.GetPat(stmt.PatNum);
					retVal.Rows.Add(
						0,//"AdjNum"
						"",//"AbbrDesc"
						"",//"balance"
						0,//"balanceDouble"
						"",//"charges"
						0,//"chargesDouble"
						0,//"ClaimNum"
						"",//"ClaimPaymentNum"
						"",//"clinic"
						0,//"ClinicNum"
						"",//"colorText"
						"",//"credits"
						0,//"creditsDouble"
						DateTime.Today.ToShortDateString(),//"date"
						DateTime.Today,//"DateTime"
						DateTime.MinValue,//dateTimeSort
						Lans.g("Statements","No Account Activity"),//"description"
						"", //"InvoiceNum"
						0,//IsTransfer
						p.FName,//"patient"
						p.PatNum,//"PatNum"
						"",//"paymentsOnObj"
						0,//"PayNum"
						0,//"PayPlanNum"
						0,//"PayPlanChargeNum"
						"",//"ProcCode"
						"0",//"ProcNum"
						0,//"ProcNumLab"
						0,//"procsOnObj"
						"",//adjustsOnObj
						0,//"prov"
						"",//"signed"
						0,//"StatementNum"
						"",//"ToothNum"
						"",//"ToothRange"
						"",//"tth"
						0//SuperFamily
						);
				}
			}
			return retVal;
		}

		private static DataTable GetTable_StatementAging(Statement stmt,Patient patGuar) {
			DataTable retVal=new DataTable();
			if(stmt.SuperFamily!=0) {
				retVal.Columns.Add(new DataColumn("PatNum"));
				retVal.Columns.Add(new DataColumn("Account"));
			}
			retVal.Columns.Add(new DataColumn("Age00to30"));
			retVal.Columns.Add(new DataColumn("Age31to60"));
			retVal.Columns.Add(new DataColumn("Age61to90"));
			retVal.Columns.Add(new DataColumn("Age90plus"));
			if(stmt.SuperFamily!=0) {
				retVal.Columns.Add(new DataColumn("AcctTotal"));
			}
			DataRow row;
			if(stmt.SuperFamily!=0) {//Superfamily statement
				List<Patient> listSuperfamGuars=Patients.GetSuperFamilyGuarantors(stmt.SuperFamily)
					.FindAll(x => x.HasSuperBilling && new[] { x.Bal_0_30,x.Bal_31_60,x.Bal_61_90,x.BalOver90,x.BalTotal }.Any(y => y>0));
				foreach(Patient guarantor in listSuperfamGuars) {//seperate rows instead of summed into a single row for all families.
					row=retVal.NewRow();
					row[0]=guarantor.PatNum;
					row[1]=guarantor.GetNameFL();
					row[2]=guarantor.Bal_0_30.ToString("F");
					row[3]=guarantor.Bal_31_60.ToString("F");
					row[4]=guarantor.Bal_61_90.ToString("F");
					row[5]=guarantor.BalOver90.ToString("F");
					row[6]=guarantor.BalTotal.ToString("F");
					retVal.Rows.Add(row);
				}
			}
			else {
				row=retVal.NewRow();
				patGuar=patGuar??Patients.GetFamily(stmt.PatNum).Guarantor;
				row[0]=patGuar.Bal_0_30.ToString("F");
				row[1]=patGuar.Bal_31_60.ToString("F");
				row[2]=patGuar.Bal_61_90.ToString("F");
				row[3]=patGuar.BalOver90.ToString("F");
				retVal.Rows.Add(row);
			}
			return retVal;
		}
		///<Summary>DataSet should be prefilled with AccountModules.GetAccount() before calling this method.</Summary>
		private static DataTable GetTable_StatementPayPlan(DataSet dataSet,bool isDynamicPaymentPlan) {
			DataTable retVal=new DataTable();
			foreach(DataTable t in dataSet.Tables) {
				if(!isDynamicPaymentPlan && !t.TableName.StartsWith("payplan")) {
					continue;
				}
				if(isDynamicPaymentPlan && !t.TableName.StartsWith("dynamicPayPlan")) {
					continue;
				}
				retVal=t.Clone();
				foreach(DataRow r in t.Rows) {
					retVal.ImportRow(r);
				}
			}
			return retVal;
		}

		///<Summary>DataSet should be prefilled with AccountModules.GetAccount() before calling this method.</Summary>
		private static DataTable GetTable_StatementEnclosed(DataSet dataSet,Statement stmt,Patient patGuar = null) {
			int payPlanVersionCur=PrefC.GetInt(PrefName.PayPlansVersion);
			DataTable tableMisc=dataSet.Tables["misc"];
			string text="";
			DataTable table=new DataTable();
			table.Columns.Add(new DataColumn("AmountDue"));
			table.Columns.Add(new DataColumn("DateDue"));
			table.Columns.Add(new DataColumn("AmountEnclosed"));
			DataRow row=table.NewRow();
			#region Statement Type NotSet
			if(stmt.StatementType!=StmtType.LimitedStatement) {
				List<Patient> listSuperFamGuars=new List<Patient>();
				if(stmt.SuperFamily!=0) {//Superfamily statement
					patGuar=patGuar??Patients.GetPat(Patients.GetPat(stmt.SuperFamily).Guarantor);
					listSuperFamGuars=Patients.GetSuperFamilyGuarantors(patGuar.SuperFamily).FindAll(x => x.HasSuperBilling);
				}
				else {
					patGuar=patGuar??Patients.GetPat(Patients.GetPat(stmt.PatNum).Guarantor);
				}
				double balTotal=0;
				if(stmt.SuperFamily!=0) {//Superfamily statement
					double balCur;
					foreach(Patient guarantor in listSuperFamGuars) {
						balCur=guarantor.BalTotal;
						if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
							balCur-=guarantor.InsEst;
						}
						if(balCur<=0) {//if this guarantor has a negative balance, don't subtract from the super statement amount due (Ryan says so)
							continue;
						}
						balTotal+=balCur;
					}
				}
				else {
					balTotal=patGuar.BalTotal;
					if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
						balTotal-=patGuar.InsEst;
					}
				}
				for(int m = 0;m<tableMisc.Rows.Count;m++) {
					//only add payplandue value to total balance in version 1 (version 2+ already account for it when calculating aging)
					if(tableMisc.Rows[m]["descript"].ToString()=="payPlanDue" && payPlanVersionCur==1) {
						balTotal+=PIn.Double(tableMisc.Rows[m]["value"].ToString());
						//payPlanDue;//PatGuar.PayPlanDue;
					}
				}
				if(stmt.SuperFamily!=0) {//Superfamily statement
					List<InstallmentPlan> listInstallPlans=stmt.ListInstallmentPlans??InstallmentPlans.GetForSuperFam(patGuar.SuperFamily);
					if(listInstallPlans.Count>0) {
						double installPlanTotal=0;
						foreach(InstallmentPlan plan in listInstallPlans) {
							installPlanTotal+=plan.MonthlyPayment;
						}
						if(installPlanTotal < balTotal) {
							text=installPlanTotal.ToString("F");
						}
						else {
							text=balTotal.ToString("F");
						}
					}
					else {//No installment plans
						text=balTotal.ToString("F");
					}
				}
				else {
					InstallmentPlan installPlan;
					if(stmt.ListInstallmentPlans==null) {
						installPlan=InstallmentPlans.GetOneForFam(patGuar.PatNum);
					}
					else {
						installPlan=stmt.ListInstallmentPlans.FirstOrDefault();
					}
					if(installPlan!=null) {
						//show lesser of normal total balance or the monthly payment amount.
						if(installPlan.MonthlyPayment < balTotal) {
							text=installPlan.MonthlyPayment.ToString("F");
						}
						else {
							text=balTotal.ToString("F");
						}
					}
					else {//no installmentplan
						text=balTotal.ToString("F");
					}
				}
			}
			#endregion Statement Type NotSet
			#region Statement Type LimitedStatement
			else {
				double statementTotal=dataSet.Tables.OfType<DataTable>().Where(x => x.TableName.StartsWith("account"))
					.SelectMany(x => x.Rows.OfType<DataRow>())
					.Where(x => x["AdjNum"].ToString()!="0"//adjustments, may be charges or credits
						|| x["ProcNum"].ToString()!="0"//procs, will be charges with credits==0
						|| x["PayNum"].ToString()!="0"//patient payments, will be credits with charges==0
						|| x["ClaimPaymentNum"].ToString()!="0").ToList()//claimproc payments+writeoffs, will be credits with charges==0
					.Sum(x => PIn.Double(x["chargesDouble"].ToString())-PIn.Double(x["creditsDouble"].ToString()));//add charges-credits
				if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
					text=statementTotal.ToString("c");
				}
				else {
					double patInsEst=PIn.Double(tableMisc.Rows.OfType<DataRow>()
						.Where(x => x["descript"].ToString()=="patInsEst")
						.Select(x => x["value"].ToString()).FirstOrDefault());//safe, if string is blank or null PIn.Double will return 0
					text=(statementTotal-patInsEst).ToString("c");
				}
			}
			#endregion Statement Type LimitedStatement
			row[0]=text;
			if(PrefC.GetLong(PrefName.StatementsCalcDueDate)==-1) {
				text=Lans.g("Statements","Upon Receipt");
			}
			else {
				text=DateTime.Today.AddDays(PrefC.GetLong(PrefName.StatementsCalcDueDate)).ToShortDateString();
			}
			row[1]=text;
			row[2]="";
			table.Rows.Add(row);
			return table;
		}

		///<summary>DataSet should be prefilled with AccountModules.GetAccount() before calling this method.  Only returns results for invoices.</summary>
		private static DataTable GetTable_StatementInvoicePayment(Statement stmt) {
			int payPlanVersionCur=PrefC.GetInt(PrefName.PayPlansVersion);
			DataTable table=new DataTable(); //not sure what other codes we should add.
			table.Columns.Add(new DataColumn("date"));
			table.Columns.Add(new DataColumn("prov"));
			table.Columns.Add(new DataColumn("patient"));
			table.Columns.Add(new DataColumn("type"));
			table.Columns.Add(new DataColumn("ProcCode"));
			table.Columns.Add(new DataColumn("description"));
			table.Columns.Add(new DataColumn("amt"));
			//only for invoices
			if(!stmt.IsInvoice) {
				return table;
			}
			Family famCur = Patients.GetFamily(stmt.PatNum);
			Patient patGuar = Patients.GetPat(Patients.GetPat(stmt.PatNum).Guarantor);
			//the paysplit information we need is not in the dataSet that we have access to.
			DataTable tablePaySplits = AccountModules.GetPaymentsForInvoice(famCur.ListPats.Select(x => x.PatNum).ToList(),stmt.ListProcNums,stmt.DateSent);
			DataRow row;
			for(int i = 0;i < tablePaySplits.Rows.Count;i++) {
				row=table.NewRow();
				row["date"] = PIn.Date(tablePaySplits.Rows[i]["Date"].ToString()).ToShortDateString();
				row["prov"] = tablePaySplits.Rows[i]["Provider"].ToString();
				row["patient"] = tablePaySplits.Rows[i]["FName"].ToString();
				row["type"] = tablePaySplits.Rows[i]["TranType"].ToString();
				row["ProcCode"] = tablePaySplits.Rows[i]["ProcCode"].ToString();
				row["description"] = tablePaySplits.Rows[i]["Descript"].ToString();
				row["amt"] = PIn.Double(tablePaySplits.Rows[i]["Amt"].ToString()).ToString("f");
				table.Rows.Add(row);
			}
			return table;
		}

		private static DataTable GetTable_MedLabResults(MedLab medLab) {
			DataTable retval=new DataTable();
			retval.Columns.Add(new DataColumn("obsIDValue"));
			retval.Columns.Add(new DataColumn("obsAbnormalFlag"));
			retval.Columns.Add(new DataColumn("obsUnits"));
			retval.Columns.Add(new DataColumn("obsRefRange"));
			retval.Columns.Add(new DataColumn("facilityID"));
			List<MedLab> listMedLabs=MedLabs.GetForPatAndSpecimen(medLab.PatNum,medLab.SpecimenID,medLab.SpecimenIDFiller);//should always be at least one MedLab
			MedLabFacilities.GetFacilityList(listMedLabs,out _listResults);//refreshes and sorts the classwide _listResults variable
			string obsDescriptPrev="";
			for(int i = 0;i<_listResults.Count;i++) {
				//LabCorp requested that these non-performance results not be displayed on the report
				if((_listResults[i].ResultStatus==ResultStatus.F || _listResults[i].ResultStatus==ResultStatus.X)
					&& _listResults[i].ObsValue==""
					&& _listResults[i].Note=="") {
					continue;
				}
				string obsDescript="";
				MedLab medLabCur=listMedLabs.FirstOrDefault(x => x.MedLabNum==_listResults[i].MedLabNum);
				if(i==0 || _listResults[i].MedLabNum!=_listResults[i-1].MedLabNum) {
					if(medLabCur!=null && medLabCur.ActionCode!=ResultAction.G) {
						if(obsDescriptPrev==medLabCur.ObsTestDescript) {
							obsDescript=".";
						}
						else {
							obsDescript=medLabCur.ObsTestDescript;
							obsDescriptPrev=obsDescript;
						}
					}
				}
				DataRow row=retval.NewRow();
				string spaces="  ";
				string spaces2="    ";
				string obsVal="";
				int padR=38;
				string newLine="";
				if(obsDescript!="") {
					if(obsDescript==_listResults[i].ObsText) {
						spaces="";
						spaces2="  ";
						padR=40;
					}
					else {
						obsVal+=obsDescript+"\r\n";
						newLine+="\r\n";
					}
				}
				if(_listResults[i].ObsValue=="Test Not Performed") {
					obsVal+=spaces+_listResults[i].ObsText;
				}
				else if(_listResults[i].ObsText=="."
					|| _listResults[i].ObsValue.Contains(":")
					|| _listResults[i].ObsValue.Length>20
					|| (medLabCur!=null && medLabCur.ActionCode==ResultAction.G)) {
					obsVal+=spaces+_listResults[i].ObsText+"\r\n"+spaces2+_listResults[i].ObsValue.Replace("\r\n","\r\n"+spaces2);
					newLine+="\r\n";
				}
				else {
					obsVal+=spaces+_listResults[i].ObsText.PadRight(padR,' ')+_listResults[i].ObsValue;
				}
				if(_listResults[i].Note!="") {
					obsVal+="\r\n"+spaces2+_listResults[i].Note.Replace("\r\n","\r\n"+spaces2);
				}
				row["obsIDValue"]=obsVal;
				row["obsAbnormalFlag"]=newLine+MedLabResults.GetAbnormalFlagDescript(_listResults[i].AbnormalFlag);
				row["obsUnits"]=newLine+_listResults[i].ObsUnits;
				row["obsRefRange"]=newLine+_listResults[i].ReferenceRange;
				row["facilityID"]=newLine+_listResults[i].FacilityID;
				retval.Rows.Add(row);
			}
			return retval;
		}

		private static DataTable GetTable_PayPlanMain(Sheet sheet) {
			PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(sheet.Parameters,"payplan").ParamValue;
			//Construct empty Data table ===============================================================================
			DataTable retVal=new DataTable();
			retVal.Columns.AddRange(new[] {
				new DataColumn("ChargeDate",typeof(string)),
				new DataColumn("Provider",typeof(string)),
				new DataColumn("Description",typeof(string)),
				new DataColumn("Principal",typeof(string)),
				new DataColumn("Interest",typeof(string)),
				new DataColumn("Due",typeof(string)),
				new DataColumn("Payment",typeof(string)),
			});
			if(!payPlan.IsDynamic) {
				retVal.Columns.Add(new DataColumn("Adjustment",typeof(string)));
			}
			retVal.Columns.AddRange(new[] {
				new DataColumn("Balance",typeof(string)),
				new DataColumn("Type",typeof(string)),
				new DataColumn("paramIsBold",typeof(bool)),
			});
			Patient patCur=Patients.GetPat(payPlan.PatNum);
			if(payPlan.PatNum==0 || patCur==null) {//Pay plan should never exist without a patnum or be null.
				return retVal;//return an empty data table that has the correct format.
			}
			//Fill data table if neccessary ===============================================================================
			List<PayPlanCharge> payPlanChargeList=PayPlanCharges.GetForPayPlan(payPlan.PayPlanNum);
			if(payPlan.IsDynamic) {
				List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetListForPayplan(payPlan.PayPlanNum);
				PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(payPlan,listPayPlanLinks);
				List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(payPlanChargeList
					,terms,Patients.GetFamily(payPlan.PatNum),listPayPlanLinks,payPlan,false);
				if(payPlan.DynamicPayPlanTPOption==DynamicPayPlanTPOptions.AwaitComplete) {
					listExpectedCharges.AddRange(PayPlanEdit.GetListExpectedChargesAwaitingCompletion(payPlanChargeList
					,terms,Patients.GetFamily(payPlan.PatNum),listPayPlanLinks,payPlan,false));
				}
				payPlanChargeList.AddRange(listExpectedCharges);
			}
			DataTable bundledPayments=PaySplits.GetForPayPlan(payPlan.PayPlanNum);
			List<PaySplit> payPlanSplitList=PaySplits.GetFromBundled(bundledPayments);
			DataTable bundledClaimProcs=ClaimProcs.GetBundlesForPayPlan(payPlan.PayPlanNum);
			int count=0;//used to set Note decription
						//===payplan charges===
			DateTime prevPeriod=DateTime.MinValue;
			int periodNum=-1;//Negative to offset the original functionality
			for(int i = 0;i<payPlanChargeList.Count;i++) {
				if(payPlanChargeList[i].Note.Trim().ToLower().Contains("recalculated based on")
					|| payPlanChargeList[i].Note.Trim().ToLower().Contains("expected")) //from clasic pay plan print outs.
				{
					count++;
					continue;//Attempt to show only the real payment plan charge 
				}
				if(payPlanChargeList[i].ChargeType==PayPlanChargeType.Credit) {
					count++;
					continue;//hide credits from the amortization grid.
				}
        if(payPlan.IsDynamic) {
					if(prevPeriod!=payPlanChargeList[i].ChargeDate) {
						prevPeriod=payPlanChargeList[i].ChargeDate;
						periodNum++;
          }
					retVal.Rows.Add(PayPlanEdit.CreateRowForPayPlanChargeDT(retVal,payPlanChargeList[i],periodNum,payPlan.IsDynamic));
				} 
				else {
					retVal.Rows.Add(PayPlanEdit.CreateRowForPayPlanChargeDT(retVal,payPlanChargeList[i],i-count,payPlan.IsDynamic));
				}
			}
			//===payplan payments===
			if(payPlan.PlanNum==0) {//===normal payplan===
				for(int i = 0;i<payPlanSplitList.Count;i++) {
					retVal.Rows.Add(PayPlanEdit.CreateRowForPayPlanSplitDT(retVal,payPlanSplitList[i],bundledPayments.Rows[i],payPlan.IsDynamic));
				}
			}
			else {//===insurance payplan===
				for(int i = 0;i<bundledClaimProcs.Rows.Count;i++) {
					retVal.Rows.Add(PayPlanEdit.CreateRowForClaimProcsDT(retVal,bundledClaimProcs.Rows[i],payPlan.IsDynamic));
				}
			}
			count=0;//reset to zero for the next loop.
			//Sort rows based on date ===============================================================================
			List<DataRow> payPlanList=new List<DataRow>(retVal.Select());
			payPlanList.Sort(PayPlanEdit.ComparePayPlanRowsDT);
			//Move TBD rows to the bottom of the list ===============================================================================
			payPlanList=payPlanList.OrderBy(x=>x["ChargeDate"].ToString()=="TBD").ToList();
			//Fill sorted data rows to sortRetVal DataTable ===============================================================================
			DataTable sortRetVal=retVal.Clone();
			prevPeriod=DateTime.MinValue; //Only for dynamic pay plan.
			periodNum=-1;//Negative to offset the original functionality. Only for dynamic pay plan.
			for(int i = 0;i<payPlanList.Count;i++) {
				if(payPlanList[i][2].ToString().Trim().ToLower().Contains("downPayment")) {//description
					count++;
				}
				if(PIn.Double(payPlanList[i][6].ToString()) > 0) {//payment
					count++;
				}
        if(payPlan.IsDynamic) {
					//Since the entries are sorted by date, period will only increase if the previous period was a different date. 
					PayPlanEdit.PayPlanEntryType entryType=(PayPlanEdit.PayPlanEntryType)Enum.Parse(typeof(PayPlanEdit.PayPlanEntryType),PIn.String(payPlanList[i][8].ToString()));
					if(prevPeriod!=PIn.Date(payPlanList[i][0].ToString()) && entryType!=PayPlanEdit.PayPlanEntryType.pay) {
						prevPeriod=PIn.Date(payPlanList[i][0].ToString());
						periodNum++;
					}
					sortRetVal.Rows.Add(PayPlanEdit.CreateRowForPayPlanListDT(sortRetVal,payPlanList[i],periodNum,payPlan.IsDynamic));
				} 
				else {
					sortRetVal.Rows.Add(PayPlanEdit.CreateRowForPayPlanListDT(sortRetVal,payPlanList[i],i-count,payPlan.IsDynamic));
				}
			}
			//Calculate running totals and add to sortRetVal DataTable ===============================================================================
			double totPrincipal=0;
			double totInterest=0;
			double totDue=0;
			double totPayment=0;
			double runningBalance=0;
			double totAdjustment=0;
			for(int i = 0;i<sortRetVal.Rows.Count;i++) {
				DataRow rowTemp=sortRetVal.Rows[i];
				double rowPrincipal=PIn.Double(rowTemp["Principal"].ToString());
				double rowInterest=PIn.Double(rowTemp["Interest"].ToString());
				double rowDue=PIn.Double(rowTemp["Due"].ToString());
				double rowPayment=PIn.Double(rowTemp["Payment"].ToString());
				double rowAdjustment=0;
				if(!payPlan.IsDynamic) {
					rowAdjustment=PIn.Double(rowTemp["Adjustment"].ToString());
				}
				totPrincipal+=rowPrincipal;
				totInterest+=rowInterest;
				totDue+=rowDue;
				totPayment+=rowPayment;
				totAdjustment+=rowAdjustment;
				if(rowDue>0) {
					runningBalance+=rowDue;
				}
				if(rowAdjustment<0) {
					runningBalance+=rowAdjustment;
				}
				if(rowPayment==0) {
					rowTemp["Payment"]=0.ToString("n");
				}
				else {
					runningBalance-=rowPayment;
				}
				rowTemp["Balance"]=runningBalance.ToString("n");
				rowTemp["paramIsBold"]=false;
			}
			DataRow totalRow=sortRetVal.NewRow();
			//Fill payment and balance columns/cells to sortRetVal ===============================================================================
			totalRow["Principal"]=totPrincipal.ToString("n");
			totalRow["Interest"]=totInterest.ToString("n");
			totalRow["Due"]=totDue.ToString("n");
			totalRow["Payment"]=totPayment.ToString("n");
			if(!payPlan.IsDynamic) {
				totalRow["Adjustment"]=totAdjustment.ToString("n");
			}
			totalRow["paramIsBold"]=true;
			sortRetVal.Rows.Add(totalRow);
			return sortRetVal;
		}

		private static DataTable GetTable_TreatPlanMain(Sheet sheet) {
			TreatPlan treatPlan=(TreatPlan)SheetParameter.GetParamByName(sheet.Parameters,"TreatPlan").ParamValue;
			bool checkShowSubtotals=(bool)SheetParameter.GetParamByName(sheet.Parameters,"checkShowSubTotals").ParamValue;
			bool checkShowTotals=(bool)SheetParameter.GetParamByName(sheet.Parameters,"checkShowTotals").ParamValue;
			//Note: this logic was ported from ContrTreat.cs
			//Construct empty Data table ===============================================================================
			DataTable retVal=new DataTable();
			retVal.Columns.AddRange(new[] {
				new DataColumn("Done",typeof(string)),
				new DataColumn("Priority",typeof(string)),
				new DataColumn("Tth",typeof(string)),
				new DataColumn("Surf",typeof(string)),
				new DataColumn("Code",typeof(string)),
				new DataColumn("Sub",typeof(string)),
				new DataColumn("Description",typeof(string)),
				new DataColumn("Fee",typeof(string)),
				new DataColumn("Pri Ins",typeof(string)),
				new DataColumn("Sec Ins",typeof(string)),
				new DataColumn("Discount",typeof(string)),
				new DataColumn("Pat",typeof(string)),
				new DataColumn("Prognosis",typeof(string)),
				new DataColumn("Dx",typeof(string)),
				new DataColumn("Abbr",typeof(string)),
				new DataColumn("Allowed",typeof(string)),
				new DataColumn("paramTextColor",typeof(int)),//Name. EG "Black" or "ff0000d7"
				new DataColumn("paramIsBold",typeof(bool)),
				new DataColumn("paramIsBorderBoldBottom",typeof(bool)),
				new DataColumn("paramTreatPlanType",typeof(int)),
				new DataColumn("Tax Est",typeof(decimal)),
				new DataColumn("ProvNum",typeof(long)),
				new DataColumn("DateTP",typeof(DateTime)),
				new DataColumn("ClinicNum",typeof(long)),
				new DataColumn(DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR,typeof(string)),
			});
			Patient patCur=Patients.GetPat(treatPlan.PatNum);
			if(treatPlan.PatNum==0 || patCur==null) {
				return retVal;//return an empty data table that has the correct format.
			}
			//Fill data table if neccessary ===============================================================================
			List<PatPlan> patPlanList=PatPlans.Refresh(patCur.PatNum);
			if(!PatPlans.IsPatPlanListValid(patPlanList)) {
				patPlanList=PatPlans.Refresh(patCur.PatNum);
			}
			List<PatPlan> listPatPlans=PatPlans.Refresh(patCur.PatNum);
			List<InsSub> listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);
			List<Procedure> procList=Procedures.Refresh(patCur.PatNum);
			decimal subfee=0;
			decimal subpriIns=0;
			decimal subsecIns=0;
			decimal subdiscount=0;
			decimal subpat=0;
			decimal subfeeallowed=0;
			decimal subTaxEst=0;
			decimal subCatPercUCR=0;
			decimal totFee=0;
			decimal totPriIns=0;
			decimal totSecIns=0;
			decimal totDiscount=0;
			decimal totPat=0;
			decimal totFeeAllowed=0;
			decimal totTaxEst=0;
			decimal totCatPercUCR=0;
			List<TpRow> rowsMain=new List<TpRow>();
			TpRow row;
			#region AnyTP
			//else {//any except current tp selected
			//ProcTP[] ProcTPSelectList=ProcTPs.GetListForTP(treatPlan.TreatPlanNum,procTPList);
			bool isDone;
			for(int i = 0;i<treatPlan.ListProcTPs.Count;i++) {
				row=new TpRow();
				isDone=false;
				for(int j = 0;j<procList.Count;j++) {
					if(procList[j].ProcNum==treatPlan.ListProcTPs[i].ProcNumOrig) {
						if(procList[j].ProcStatus==ProcStat.C) {
							isDone=true;
						}
					}
				}
				if(isDone) {
					row.Done="X";
				}
				ProcedureCode procCode=ProcedureCodes.GetProcCode(treatPlan.ListProcTPs[i].ProcCode);
				row.Priority=Defs.GetName(DefCat.TxPriorities,treatPlan.ListProcTPs[i].Priority);
				row.Tth=treatPlan.ListProcTPs[i].ToothNumTP;
				row.Surf=treatPlan.ListProcTPs[i].Surf;
				row.Code=treatPlan.ListProcTPs[i].ProcCode;
				row.Description=treatPlan.ListProcTPs[i].Descript;
				row.Fee=(decimal)treatPlan.ListProcTPs[i].FeeAmt;//Fee
				subfee+=(decimal)treatPlan.ListProcTPs[i].FeeAmt;
				totFee+=(decimal)treatPlan.ListProcTPs[i].FeeAmt;
				row.PriIns=(decimal)treatPlan.ListProcTPs[i].PriInsAmt;//PriIns
				subpriIns+=(decimal)treatPlan.ListProcTPs[i].PriInsAmt;
				totPriIns+=(decimal)treatPlan.ListProcTPs[i].PriInsAmt;
				row.SecIns=(decimal)treatPlan.ListProcTPs[i].SecInsAmt;//SecIns
				subsecIns+=(decimal)treatPlan.ListProcTPs[i].SecInsAmt;
				totSecIns+=(decimal)treatPlan.ListProcTPs[i].SecInsAmt;
				row.Discount=(decimal)treatPlan.ListProcTPs[i].Discount;//Discount
				subdiscount+=(decimal)treatPlan.ListProcTPs[i].Discount;
				totDiscount+=(decimal)treatPlan.ListProcTPs[i].Discount;
				row.Pat=(decimal)treatPlan.ListProcTPs[i].PatAmt;//Pat
				subpat+=(decimal)treatPlan.ListProcTPs[i].PatAmt;
				totPat+=(decimal)treatPlan.ListProcTPs[i].PatAmt;
				row.Prognosis=treatPlan.ListProcTPs[i].Prognosis;//Prognosis
				row.ProcAbbr=treatPlan.ListProcTPs[i].ProcAbbr;//Abbr
				row.FeeAllowed=(decimal)treatPlan.ListProcTPs[i].FeeAllowed;//FeeAllowed
				if(CompareDecimal.IsGreaterThan(row.FeeAllowed,-1)) {
					subfeeallowed+=(decimal)treatPlan.ListProcTPs[i].FeeAllowed;
					totFeeAllowed+=(decimal)treatPlan.ListProcTPs[i].FeeAllowed;
				}
				row.TaxEst=(decimal)treatPlan.ListProcTPs[i].TaxAmt;//TaxAmt
				subTaxEst+=(decimal)treatPlan.ListProcTPs[i].TaxAmt;
				totTaxEst+=(decimal)treatPlan.ListProcTPs[i].TaxAmt;
				row.CatPercUCR=(decimal)treatPlan.ListProcTPs[i].CatPercUCR;//CatPercUCR
				subCatPercUCR+=(decimal)treatPlan.ListProcTPs[i].CatPercUCR;
				totCatPercUCR+=(decimal)treatPlan.ListProcTPs[i].CatPercUCR;
				row.Dx=treatPlan.ListProcTPs[i].Dx;
				row.ColorText=Defs.GetColor(DefCat.TxPriorities,treatPlan.ListProcTPs[i].Priority);
				if(row.ColorText==System.Drawing.Color.White) {
					row.ColorText=System.Drawing.Color.Black;
				}
				row.ProvNum=treatPlan.ListProcTPs[i].ProvNum;
				row.DateTP=treatPlan.ListProcTPs[i].DateTP;
				row.ClinicNum=treatPlan.ListProcTPs[i].ClinicNum;
				row.Tag=treatPlan.ListProcTPs[i].Copy();
				rowsMain.Add(row);
				#region subtotal
				if(checkShowSubtotals &&
					(i==treatPlan.ListProcTPs.Count-1 || treatPlan.ListProcTPs[i+1].Priority != treatPlan.ListProcTPs[i].Priority)) {
					row=new TpRow();
					row.Description=Lans.g("TableTP","Subtotal");
					row.Fee=subfee;
					row.PriIns=subpriIns;
					row.SecIns=subsecIns;
					row.Discount=subdiscount;
					row.Pat=subpat;
					row.FeeAllowed=subfeeallowed;
					row.TaxEst=subTaxEst;
					row.CatPercUCR=subCatPercUCR;
					row.ColorText=Defs.GetColor(DefCat.TxPriorities,treatPlan.ListProcTPs[i].Priority);
					if(row.ColorText==System.Drawing.Color.White) {
						row.ColorText=System.Drawing.Color.Black;
					}
					row.Bold=true;
					row.ColorLborder=System.Drawing.Color.Black;
					rowsMain.Add(row);
					subfee=0;
					subpriIns=0;
					subsecIns=0;
					subdiscount=0;
					subpat=0;
					subfeeallowed=0;
					subTaxEst=0;
					subCatPercUCR=0;
				}
				#endregion
			}
			#endregion AnyTP except current
			#region Totals
			if(checkShowTotals) {
				row=new TpRow();
				row.Description=Lans.g("TableTP","Total");
				row.Fee=totFee;
				row.PriIns=totPriIns;
				row.SecIns=totSecIns;
				row.Discount=totDiscount;
				row.Pat=totPat;
				row.FeeAllowed=totFeeAllowed;
				row.TaxEst=totTaxEst;
				row.CatPercUCR=totCatPercUCR;
				row.Bold=true;
				row.ColorText=System.Drawing.Color.Black;
				rowsMain.Add(row);
			}
			#endregion Totals
			foreach(TpRow tpRow in rowsMain) {
				DataRow dRow=retVal.NewRow();
				dRow["Done"]                   =tpRow.Done;
				dRow["Priority"]               =tpRow.Priority;
				dRow["Tth"]                    =tpRow.Tth;
				dRow["Surf"]                   =tpRow.Surf;
				dRow["Code"]                   =tpRow.Code;
				//If any patient insplan allows subst codes (if !plan.CodeSubstNone) and the code has a valid substitution code, then indicate the substitution.
				//If it is not a valid substitution code or if none of the plans allow substitutions, leave the it blank.
				ProcedureCode procCode=ProcedureCodes.GetProcCode(tpRow.Code);
				//If any plan allows substitution, show X
				dRow["Sub"]=SubstitutionLinks.HasSubstCodeForProcCode(procCode,tpRow.Tth,listSubLinks,listInsPlans) ? "X" : "";
				dRow["Description"]            =tpRow.Description;
				if(PrefC.GetBool(PrefName.TreatPlanItemized)
					|| tpRow.Description==Lans.g("TableTP","Subtotal") || tpRow.Description==Lans.g("TableTP","Total")) {
					dRow["Fee"]                  =tpRow.Fee.ToString("F");
					dRow["Pri Ins"]              =tpRow.PriIns.ToString("F");
					dRow["Sec Ins"]              =tpRow.SecIns.ToString("F");
					dRow["Discount"]             =tpRow.Discount.ToString("F");
					dRow["Tax Est"]              =tpRow.TaxEst.ToString("F");
					dRow["Pat"]                  =tpRow.Pat.ToString("F");
					dRow["Allowed"]              =CompareDecimal.IsGreaterThan(tpRow.FeeAllowed,-1) ? tpRow.FeeAllowed.ToString("F") : "X";//-1 means the proc is DoNotBillIns
					dRow[DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR]=tpRow.CatPercUCR.ToString("F");
				}
				dRow["Prognosis"]              =tpRow.Prognosis;
				dRow["Dx"]                     =tpRow.Dx;
				dRow["Abbr"]                   =tpRow.ProcAbbr;
				dRow["paramTextColor"]         =tpRow.ColorText.ToArgb();
				dRow["paramIsBold"]            =tpRow.Bold;
				dRow["paramIsBorderBoldBottom"]=tpRow.Bold;
				dRow["paramTreatPlanType"]       =(int)treatPlan.TPType;
				dRow["ProvNum"]                              =tpRow.ProvNum;
				dRow["DateTP"]                               =tpRow.DateTP.ToShortDateString();
				dRow["ClinicNum"]                            =tpRow.ClinicNum;
				retVal.Rows.Add(dRow);
			}
			return retVal;
		}

		private static DataTable GetTable_TreatPlanBenefitsFamily(Sheet sheet) {
			TreatPlan treatPlan=(TreatPlan)SheetParameter.GetParamByName(sheet.Parameters,"TreatPlan").ParamValue;
			bool checkShowIns=(bool)SheetParameter.GetParamByName(sheet.Parameters,"checkShowIns").ParamValue;
			//Note this logic was ported from ContrTreat.cs and is intended to emulate the way ContrTreat.CreateDocument created the insurance benefit table
			//Construct empty Data table ===============================================================================
			DataTable retVal=new DataTable();
			retVal.Columns.AddRange(new[] {
				new DataColumn("BenefitName",typeof(string)),
				new DataColumn("Primary",typeof(string)),
				new DataColumn("Secondary",typeof(string))
			});
			if(!checkShowIns) {
				return retVal;
			}
			retVal.Rows.Add("Family Maximum","","");
			retVal.Rows.Add("Family Deductible","","");
			Patient patCur=Patients.GetPat(treatPlan.PatNum);
			if(treatPlan.PatNum==0 || patCur==null) {
				return retVal;//return an empty data table that has the correct format.
			}
			//Fill data table if neccessary ===============================================================================
			List<PatPlan> patPlanList=PatPlans.Refresh(patCur.PatNum);
			if(!PatPlans.IsPatPlanListValid(patPlanList)) {
				patPlanList=PatPlans.Refresh(patCur.PatNum);
			}
			Family famCur=Patients.GetFamily(patCur.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(famCur);
			List<InsPlan> insPlanList=InsPlans.RefreshForSubList(subList);
			List<Benefit> benefitList=Benefits.Refresh(patPlanList,subList);
			for(int i = 0;i<patPlanList.Count && i<2;i++) {//limit to first 2 insplans
				InsSub subCur=InsSubs.GetSub(patPlanList[i].InsSubNum,subList);
				InsPlan planCur=InsPlans.GetPlan(subCur.PlanNum,insPlanList);
				double familyMax=Benefits.GetAnnualMaxDisplay(benefitList,planCur.PlanNum,patPlanList[i].PatPlanNum,true);
				if(!CompareDouble.IsEqual(familyMax,-1)) {
					retVal.Rows[0][i+1]=familyMax.ToString("F");
				}
				double familyDed=Benefits.GetDeductGeneralDisplay(benefitList,planCur.PlanNum,patPlanList[i].PatPlanNum,BenefitCoverageLevel.Family);
				if(!CompareDouble.IsEqual(familyDed,-1)) {
					retVal.Rows[1][i+1]=familyDed.ToString("F");
				}
			}
			return retVal;
		}

		private static DataTable GetTable_TreatPlanBenefitsIndividual(Sheet sheet) {
			TreatPlan treatPlan=(TreatPlan)SheetParameter.GetParamByName(sheet.Parameters,"TreatPlan").ParamValue;
			bool checkShowIns=(bool)SheetParameter.GetParamByName(sheet.Parameters,"checkShowIns").ParamValue;
			//Note this logic was ported from ContrTreat.cs and is intended to emulate the way ContrTreat.CreateDocument created the insurance benefit table
			//Construct empty Data table ===============================================================================
			DataTable retVal=new DataTable();
			retVal.Columns.AddRange(new[] {
				new DataColumn("BenefitName",typeof(string)),
				new DataColumn("Primary",typeof(string)),
				new DataColumn("Secondary",typeof(string))
			});
			if(!checkShowIns) {
				return retVal;
			}
			Patient patCur=Patients.GetPat(treatPlan.PatNum);
			retVal.Rows.Add("Annual Maximum","","");
			retVal.Rows.Add("Deductible","","");
			retVal.Rows.Add("Deductible Remaining","","");
			retVal.Rows.Add("Insurance Used","","");
			retVal.Rows.Add("Pending","","");
			retVal.Rows.Add("Remaining","","");
			if(treatPlan.PatNum==0 || patCur==null) {
				return retVal;//return an empty data table that has the correct format.
			}
			//Fill data table if neccessary ===============================================================================
			List<PatPlan> patPlanList=PatPlans.Refresh(patCur.PatNum);
			if(!PatPlans.IsPatPlanListValid(patPlanList)) {
				patPlanList=PatPlans.Refresh(patCur.PatNum);
			}
			Family famCur=Patients.GetFamily(patCur.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(famCur);
			List<InsPlan> insPlanList=InsPlans.RefreshForSubList(subList);
			List<Benefit> benefitList=Benefits.Refresh(patPlanList,subList);
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(patCur.PatNum,benefitList,patPlanList,insPlanList,DateTime.Today,subList);
			for(int i = 0;i<patPlanList.Count && i<2;i++) {
				InsSub subCur=InsSubs.GetSub(patPlanList[i].InsSubNum,subList);
				InsPlan planCur=InsPlans.GetPlan(subCur.PlanNum,insPlanList);
				double pend=InsPlans.GetPendingDisplay(histList,DateTime.Today,planCur,patPlanList[i].PatPlanNum,-1,patCur.PatNum,patPlanList[i].InsSubNum,benefitList);
				double used=InsPlans.GetInsUsedDisplay(histList,DateTime.Today,planCur.PlanNum,patPlanList[i].PatPlanNum,-1,insPlanList,benefitList,patCur.PatNum,patPlanList[i].InsSubNum);
				retVal.Rows[3][i+1]=used.ToString("F");
				retVal.Rows[4][i+1]=pend.ToString("F");
				double maxInd=Benefits.GetAnnualMaxDisplay(benefitList,planCur.PlanNum,patPlanList[i].PatPlanNum,false);
				if(!CompareDouble.IsEqual(maxInd,-1)) {
					double remain=maxInd-used-pend;
					if(remain<0) {
						remain=0;
					}
					retVal.Rows[0][i+1]=maxInd.ToString("F");
					retVal.Rows[5][i+1]=remain.ToString("F");
				}
				//deductible:
				double ded=Benefits.GetDeductGeneralDisplay(benefitList,planCur.PlanNum,patPlanList[i].PatPlanNum,BenefitCoverageLevel.Individual);
				double dedFam=Benefits.GetDeductGeneralDisplay(benefitList,planCur.PlanNum,patPlanList[i].PatPlanNum,BenefitCoverageLevel.Family);
				if(!CompareDouble.IsEqual(ded,-1)) {
					double dedRem=InsPlans.GetDedRemainDisplay(histList,DateTime.Today,planCur.PlanNum,patPlanList[i].PatPlanNum,-1,insPlanList,patCur.PatNum,ded,dedFam);
					retVal.Rows[1][i+1]=ded.ToString("F");
					retVal.Rows[2][i+1]=dedRem.ToString("F");
				}
			}
			return retVal;
		}

		private static DataTable GetTable_EraClaimsPaid(Sheet sheet) {
			DataTable retVal=new DataTable();
			retVal.Columns.Add(new DataColumn("ClpSegmentIndex"));
			retVal.Columns.Add(new DataColumn("ProcCode"));
			retVal.Columns.Add(new DataColumn("ProcDescript"));
			retVal.Columns.Add(new DataColumn("FeeBilled"));
			retVal.Columns.Add(new DataColumn("PatResp"));
			retVal.Columns.Add(new DataColumn("Contractual"));
			retVal.Columns.Add(new DataColumn("PayorReduct"));
			retVal.Columns.Add(new DataColumn("OtherAdjust"));
			retVal.Columns.Add(new DataColumn("InsPaid"));
			retVal.Columns.Add(new DataColumn("RemarkCodes"));
			X835 x835=(X835)SheetParameter.GetParamByName(sheet.Parameters,"ERA").ParamValue;
			foreach(Hx835_Claim claimPaid in x835.ListClaimsPaid) {
				foreach(Hx835_Proc proc in claimPaid.ListProcs) {
					DataRow row=retVal.NewRow();
					row["ClpSegmentIndex"]=claimPaid.ClpSegmentIndex;
					//Mimics FormEtrans835ClaimEdit.FillProcedureBreakdown(...)
					row["ProcCode"]=proc.ProcCodeAdjudicated;
					string procDescript="";
					if(ProcedureCodes.IsValidCode(proc.ProcCodeAdjudicated)) {
						ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.ProcCodeAdjudicated);
						procDescript=procCode.AbbrDesc;
					}
					row["ProcDescript"]=procDescript;
					row["FeeBilled"]=proc.ProcFee.ToString("f2");
					decimal patRespProc=0;
					decimal contractualForProc=0;
					decimal payorInitReductForProc=0;
					decimal otherAdjustForProc=0;
					foreach(Hx835_Adj adj in proc.ListProcAdjustments) {
						if(adj.AdjCode=="PR") {//Patient Responsibility
							patRespProc+=adj.AdjAmt;
						}
						else if(adj.AdjCode=="CO") {//Contractual Obligations
							contractualForProc+=adj.AdjAmt;
						}
						else if(adj.AdjCode=="PI") {//Payor Initiated Reductions
							payorInitReductForProc+=adj.AdjAmt;
						}
						else {//Other Adjustments
							otherAdjustForProc+=adj.AdjAmt;
						}
					}
					row["PatResp"]=patRespProc.ToString("f2");
					row["Contractual"]=contractualForProc.ToString("f2");
					row["PayorReduct"]=payorInitReductForProc.ToString("f2");
					row["OtherAdjust"]=otherAdjustForProc.ToString("f2");
					row["InsPaid"]=proc.InsPaid.ToString("f2");
					row["RemarkCodes"]=string.Join(",",proc.ListRemarks.Select(x => x.Code).ToList());
					retVal.Rows.Add(row);
				}
			}
			return retVal;
		}

		private static DataTable GetTable_EraClaimsPaidProcRemarks(Sheet sheet) {
			DataTable retVal=new DataTable();
			retVal.Columns.Add(new DataColumn("ClpSegmentIndex"));
			retVal.Columns.Add(new DataColumn("RemarkCode"));
			retVal.Columns.Add(new DataColumn("RemarkDescription"));
			X835 x835=(X835)SheetParameter.GetParamByName(sheet.Parameters,"ERA").ParamValue;
			Dictionary<int,List<string>> dictRemarks=new Dictionary<int,List<string>>();
			foreach(Hx835_Claim claimPaid in x835.ListClaimsPaid) {
				foreach(Hx835_Proc proc in claimPaid.ListProcs) {
					List<string> listCodes=proc.ListRemarks.Select(x => x.Code).ToList();
					foreach(string code in listCodes) {
						if(!dictRemarks.ContainsKey(claimPaid.ClpSegmentIndex)) {
							dictRemarks[claimPaid.ClpSegmentIndex]=new List<string>();
						}
						if(dictRemarks[claimPaid.ClpSegmentIndex].Contains(code)) {//Already added.
							continue;
						}
						dictRemarks[claimPaid.ClpSegmentIndex].Add(code);//Distinct. This inits and overrides.
						DataRow row=retVal.NewRow();
						row["ClpSegmentIndex"]=claimPaid.ClpSegmentIndex;
						row["RemarkCode"]=code;
						row["RemarkDescription"]=proc.ListRemarks.FirstOrDefault(x => x.Code==code).Value;
						retVal.Rows.Add(row);
					}
				}
			}
			return retVal;
		}

		private static DataTable GetTable_ReferralLetterProceduresCompleted(Sheet sheet) {
			long patNum=(long)SheetParameter.GetParamByName(sheet.Parameters,"PatNum").ParamValue;
			List<Procedure> listProcs=(List<Procedure>)SheetParameter.GetParamByName(sheet.Parameters,"CompletedProcs").ParamValue;
			//Construct empty Data table ===============================================================================
			DataTable retVal=new DataTable();
			retVal.Columns.AddRange(new[] {
				new DataColumn("Date",typeof(string)),
				new DataColumn("Prov",typeof(string)),
				new DataColumn("ProcCode",typeof(string)),
				new DataColumn("Tth",typeof(string)),
				new DataColumn("Surf",typeof(string)),
				new DataColumn("Description",typeof(string)),
				new DataColumn("Note",typeof(string))
			});
			List<TpRow> rowsMain=new List<TpRow>();
			for(int i = 0;i<listProcs.Count;i++) {
				DataRow dRow=retVal.NewRow();
				ProcedureCode procCode=ProcedureCodes.GetProcCode(listProcs[i].CodeNum);
				dRow["Date"]                                     =listProcs[i].ProcDate.ToShortDateString();
				dRow["Prov"]                                     =Providers.GetAbbr(listProcs[i].ProcNum);
				dRow["ProcCode"]               =procCode.ProcCode;
				dRow["Tth"]                    =Tooth.ToInternat(listProcs[i].ToothNum);
				dRow["Surf"]                   =listProcs[i].Surf;
				dRow["Description"]            =procCode.Descript;
				dRow["Note"]                                   =listProcs[i].Note;
				retVal.Rows.Add(dRow);
			}
			return retVal;
		}
	}
}
