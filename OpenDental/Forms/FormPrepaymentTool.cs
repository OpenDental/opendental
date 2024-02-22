using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	///<summary>Internal tool meant for HQ only.</summary>
	public partial class FormPrepaymentTool:FormODBase {
		public Payment PaymentReturn;
		private Patient _patient;
		///<summary>A constant list of procedure codes which are pulled from the AvaTax bridge property Prepay Proc Codes.</summary>
		private List<ProcedureCode> _listProcedureCodesPrePaySupport;
		private List<ProcedureCode> _listProcedureCodesPrepayment;
		///<summary>This will hold ProcCodes along with the fee associated to the code.</summary>
		private List<ProcedureCharge> _listProcedureCharges;
		///<summary>Get all repeating charges for patient on load.</summary>
		private List<RepeatCharge> _listRepeatChargesForPat;
		private double _chargeTotal;
		private double _discountTotal;
		private double _estTaxTotal;
		private double _total;
		///<summary>Number of rows which used to be in the payment grid.  Used when you add a row to make it easier to highlight new rows.</summary>
		private int _prevPaymentRowCount;
		///<summary>List of procedures returned from FormProcSelect and sent to AvaTax API to prevent double billing.</summary>
		private List<Procedure> _listProceduresCompleted;
		///<summary>List of procedurecharges returned from FormProcSelect and used to fill gridCompletedProcs.</summary>
		private List<ProcedureCharge> _listProcedureChargesPreviouslyComp;
		private string _prepaidThroughNote;
		/// <summary>Initialized to mindate, set in ZeroOutRepeatingCharge().  Used for building the billingnote on generated procedures.</summary>
		private DateTime _datePrepayThrough;

		public FormPrepaymentTool(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
		}

		///<summary>This class lets us track a single procedurecode in the prepayment grid along with the any charge, discount or est tax values
		///associated with that procedure.</summary>
		private class ProcedureCharge {
			public ProcedureCode ProcedureCodeCur;
			public double ProcFee;
			public double BaseFee;
			public int ProcCount;//Number units.
			//TODO: see if this can be removed, currently is only set and never referenced.
			public int GridCount;//Total number units for all ProcCharges of same procedurecode.
			public double ProcDiscount;
			public double EstTax;
			public double ProcTotal;
			public bool HasReachedStopDate;
			public bool HasTaxAdjustment;
			public bool IsDiscountOverridden;

			public ProcedureCharge(ProcedureCode procedureCode,double charge) {
				ProcedureCodeCur=procedureCode;
				ProcCount=1;
				GridCount=1;
				ProcDiscount=0;
				BaseFee=charge;
				ProcFee=BaseFee*ProcCount;
				EstTax=0;
				HasReachedStopDate=false;
				HasTaxAdjustment=false;
				IsDiscountOverridden=false;
				Calc();
			}

			///<summary>Refreshes the data in the ProcedureCharge.</summary>
			public void Calc() {
				ProcFee=BaseFee*ProcCount;
				ProcTotal=ProcFee-ProcDiscount+EstTax;
			}
		}

		private void FormPrepaymentTool_Load(object sender,EventArgs e) {
			_listRepeatChargesForPat=RepeatCharges.Refresh(_patient.PatNum).ToList();
			_listProcedureCharges=new List<ProcedureCharge>();
			_listProceduresCompleted=new List<Procedure>();
			_listProcedureChargesPreviouslyComp=new List<ProcedureCharge>();
			_prevPaymentRowCount=0;
			_listProcedureCodesPrepayment=new List<ProcedureCode>();
			//Load procedurecode list.  HQ internal codes.
			_listProcedureCodesPrePaySupport=AvaTax.GetListProcedureCodesPrePay();
			SetDatePrepayThrough(DateTime.MinValue);
			FillGridCodes();
			FillGridPrepayment();	
		}

		private void FillGridCodes() {
			GridRow row;
			GridCodes.BeginUpdate();
			GridCodes.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Code"),50);
			GridCodes.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),100){ IsWidthDynamic=true };
			GridCodes.Columns.Add(col);
			GridCodes.ListGridRows.Clear();
			for(int i=0;i<_listProcedureCodesPrePaySupport.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listProcedureCodesPrePaySupport[i].ProcCode);
				row.Cells.Add(_listProcedureCodesPrePaySupport[i].Descript);
				row.Tag=_listProcedureCodesPrePaySupport[i];
				GridCodes.ListGridRows.Add(row);
			}
			GridCodes.EndUpdate();
		}

		private void FillGridCompletedProcs() {
			GetTaxEstimates(_listProcedureChargesPreviouslyComp,false);
			GridCompletedProcs.BeginUpdate();
			GridCompletedProcs.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Code"),50,HorizontalAlignment.Center);
			GridCompletedProcs.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),100,HorizontalAlignment.Left){ IsWidthDynamic=true };
			GridCompletedProcs.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Units"),30,HorizontalAlignment.Right);
			GridCompletedProcs.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Base"),50,HorizontalAlignment.Right);
			GridCompletedProcs.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Charge"),75,HorizontalAlignment.Right);
			GridCompletedProcs.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Discount"),75,HorizontalAlignment.Right);
			GridCompletedProcs.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Est Tax"),75,HorizontalAlignment.Right);
			GridCompletedProcs.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),75,HorizontalAlignment.Right);
			GridCompletedProcs.Columns.Add(col);
			GridCompletedProcs.ListGridRows.Clear();
			for(int i=0;i<_listProcedureChargesPreviouslyComp.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(_listProcedureChargesPreviouslyComp[i].ProcedureCodeCur.ProcCode);
				row.Cells.Add(_listProcedureChargesPreviouslyComp[i].ProcedureCodeCur.Descript);
				row.Cells.Add(_listProcedureChargesPreviouslyComp[i].ProcCount.ToString());
				row.Cells.Add(_listProcedureChargesPreviouslyComp[i].BaseFee.ToString("f"));
				row.Cells.Add(_listProcedureChargesPreviouslyComp[i].ProcFee.ToString("f"));
				row.Cells.Add(_listProcedureChargesPreviouslyComp[i].ProcDiscount.ToString("f"));
				row.Cells.Add(_listProcedureChargesPreviouslyComp[i].EstTax.ToString("f"));
				row.Cells.Add(_listProcedureChargesPreviouslyComp[i].ProcTotal.ToString("f"));
				GridCompletedProcs.ListGridRows.Add(row);
				_chargeTotal+=_listProcedureChargesPreviouslyComp[i].ProcFee;
				_discountTotal+=_listProcedureChargesPreviouslyComp[i].ProcDiscount;
				_estTaxTotal+=_listProcedureChargesPreviouslyComp[i].EstTax;
				_total+=_listProcedureChargesPreviouslyComp[i].ProcTotal;
			}
			GridCompletedProcs.EndUpdate();
		}

		///<summary>Also performs discount test.</summary>
		private void FillGridPrepayment() {
			_chargeTotal=0;
			_total=0;
			_estTaxTotal=0;
			_discountTotal=0;
			DiscountTest();
			GetTaxEstimates(_listProcedureCharges);
			GridPrepayment.BeginUpdate();
			GridPrepayment.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Code"),50,HorizontalAlignment.Center);
			GridPrepayment.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),100,HorizontalAlignment.Left){ IsWidthDynamic=true };
			GridPrepayment.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Units"),30,HorizontalAlignment.Right);
			GridPrepayment.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Base"),50,HorizontalAlignment.Right);
			GridPrepayment.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Charge"),75,HorizontalAlignment.Right);
			GridPrepayment.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Discount"),75,HorizontalAlignment.Right);
			GridPrepayment.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Est Tax"),75,HorizontalAlignment.Right);
			GridPrepayment.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),75,HorizontalAlignment.Right);
			GridPrepayment.Columns.Add(col);
			GridPrepayment.ListGridRows.Clear();
			for(int i=0;i<_listProcedureCharges.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(_listProcedureCharges[i].ProcedureCodeCur.ProcCode);
				row.Cells.Add(_listProcedureCharges[i].ProcedureCodeCur.Descript);
				row.Cells.Add(_listProcedureCharges[i].ProcCount.ToString());
				row.Cells.Add(_listProcedureCharges[i].BaseFee.ToString("f"));
				row.Cells.Add(_listProcedureCharges[i].ProcFee.ToString("f"));
				row.Cells.Add(_listProcedureCharges[i].ProcDiscount.ToString("f"));
				row.Cells.Add(_listProcedureCharges[i].EstTax.ToString("f"));
				row.Cells.Add(_listProcedureCharges[i].ProcTotal.ToString("f"));
				GridPrepayment.ListGridRows.Add(row);
				_chargeTotal+=_listProcedureCharges[i].ProcFee;
				_discountTotal+=_listProcedureCharges[i].ProcDiscount;
				_estTaxTotal+=_listProcedureCharges[i].EstTax;
				_total+=_listProcedureCharges[i].ProcTotal;
			}
			GridPrepayment.EndUpdate();
			for(int i=_prevPaymentRowCount;i<_listProcedureCharges.Count;i++) {
					GridPrepayment.SetSelected(i,true);
			}
			//Fill completed proc grid so the total row at the bottom is accurate.
			FillGridCompletedProcs();
			_prevPaymentRowCount=_listProcedureCharges.Count;
			textOrigSub.Text=_chargeTotal.ToString("f");
			textDiscount.Text=_discountTotal.ToString("f");
			textTaxSub.Text=_estTaxTotal.ToString("f");
			textTotal.Text=_total.ToString("f");
		}

		private void GridPrepayment_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormPrepaymentEdit formPrepaymentEdit=new FormPrepaymentEdit();
			formPrepaymentEdit.CountCur=_listProcedureCharges[e.Row].ProcCount;
			if(formPrepaymentEdit.ShowDialog()==DialogResult.OK) {
				_listProcedureCharges[e.Row].ProcCount=formPrepaymentEdit.CountCur;
				_listProcedureCharges[e.Row].Calc();
				FillGridPrepayment();
			}
		}

		private void SetDatePrepayThrough(DateTime datePrepayThrough) {
			_datePrepayThrough=datePrepayThrough;
			_prepaidThroughNote=$"Prepaid through: {_datePrepayThrough.Date:MM/dd/yyyy}";
		}

		///<summary>Calls Avalara to get an estimated sales tax for each charge.</summary>
		private void GetTaxEstimates(List<ProcedureCharge> listProcedureCharges,bool isPrepaymentList=true) {
			for(int i=0;i<listProcedureCharges.Count;i++) {
				//Prevents getting tax estimates on previously completed procedures that were not taxed in the first place.
				if(!isPrepaymentList && !listProcedureCharges[i].HasTaxAdjustment) {
					continue;
				}
				listProcedureCharges[i].EstTax=(double)AvaTax.GetEstimate(listProcedureCharges[i].ProcedureCodeCur.CodeNum,_patient.PatNum,listProcedureCharges[i].ProcFee-listProcedureCharges[i].ProcDiscount);
				listProcedureCharges[i].Calc();
			}
		}

		///<summary>Looks in the patient's repeating charges for a rate to use.  If the patient does not have a repeat charge setup
		///for the procedurecode that is being added then a procedurecharge will not be created.
		///This is then used to create one or more procedurecharges.</summary>
		private void CreateProcedureCharge(ProcedureCode procedureCode,int count,bool isNew=true) {
			List<RepeatCharge> listRepeatChargesForProc=_listRepeatChargesForPat.FindAll(x => x.ProcCode==procedureCode.ProcCode && x.IsEnabled);
			List<ProcedureCharge> listProcureChargesInGrid=_listProcedureCharges.FindAll(x => x.ProcedureCodeCur==procedureCode);
			if(listRepeatChargesForProc.Count==0) {
				return;
			}
			if(listRepeatChargesForProc.Count==1) {
				//Discussed with accounting dept.  If there is only one repeat charge for a proc on an account it should never have a stop date.
				//If for some reason there is a stop date, it is a very special case that we can't currently handle with the prepayment tool.
				int index=_listProcedureCharges.FindIndex(x => x.BaseFee==listRepeatChargesForProc[0].ChargeAmt && x.ProcedureCodeCur==procedureCode);
				//Does not exist in our list, make a new one.
				if(index==-1) {
					ProcedureCharge procedureCharge=new ProcedureCharge(procedureCode,listRepeatChargesForProc.FirstOrDefault().ChargeAmt);
					procedureCharge.ProcCount=count;
					procedureCharge.Calc();
					_listProcedureCharges.Add(procedureCharge);
				}
				else {
					_listProcedureCharges[index].ProcCount+=count;
					_listProcedureCharges[index].Calc();
				}
				return;
			}
			//Below here, listRepeatChargesForProc.Count>1
			int remainder=count;
			for(int i=0;i<listRepeatChargesForProc.Count;i++) {
				if(remainder<=0) {//Nothing left to add, kick out of loop.
					break;
				}
				if(listRepeatChargesForProc[i].DateStop.Year>1880 && listRepeatChargesForProc[i].DateStop<DateTime.Today) {
					continue;//The charge has a stop date in the past (has been disabled).
				}
				//Look for existing procedurecharge.
				int index=_listProcedureCharges.FindIndex(x => x.BaseFee==listRepeatChargesForProc[i].ChargeAmt && x.ProcedureCodeCur==procedureCode && x.HasReachedStopDate==false);
				//Does not exist in our list, make a new one.
				int difference;
				if(index==-1) {
					ProcedureCharge procedureCharge=new ProcedureCharge(procedureCode,listRepeatChargesForProc[i].ChargeAmt);
					//See if the months we are adding extends beyond this repeat charge's stop date.
					DateTime dateToUse=listRepeatChargesForProc[i].DateStart;
					//Check if we should use start date or today.  Prevent old charges.
					if(DateTime.Today>listRepeatChargesForProc[i].DateStart) {
						dateToUse=DateTime.Today;
					}
					if(dateToUse.AddMonths(remainder)>listRepeatChargesForProc[i].DateStop && listRepeatChargesForProc[i].DateStop.Year>1880) {
						procedureCharge.HasReachedStopDate=true;
						//Difference is number of months between today and the stop date, this will become the count for our current procedurecharge
						difference=((listRepeatChargesForProc[i].DateStop.Year-dateToUse.Year)*12)+(listRepeatChargesForProc[i].DateStop.Month-dateToUse.Month);
						if(DateTime.Today<DateTimeOD.GetMostRecentValidDate(dateToUse.Year,dateToUse.Month,_patient.BillingCycleDay)) {
							difference+=1;
						}
						procedureCharge.ProcCount=difference;
						remainder=remainder-difference;
					}
					//Stop date was not reached
					else {
						procedureCharge.ProcCount=remainder;
						remainder=0;
					}
					procedureCharge.Calc();
					_listProcedureCharges.Add(procedureCharge);
					continue;
				}
				//Found a matching procedurecharge.
				if(_listProcedureCharges[index].HasReachedStopDate) {
					//Already hit stopdate for this repeat charge so continue on to the next one.
					continue;
				}
				//See if stop date is mindate, if it is then we can add to the count and exit the loop.
				if(listRepeatChargesForProc[i].DateStop.Year<1880) {
					_listProcedureCharges[index].ProcCount+=remainder;
					remainder=0;
					_listProcedureCharges[index].Calc();
					continue;
				}
				//Has a stop date, see if we hit it.
				int combinedCount;
				//See if we should use today or the start date.
				if(DateTime.Today>listRepeatChargesForProc[i].DateStart) {
					//See if the count exceeds the difference between today and stop date of this repeat charge.
					difference=((listRepeatChargesForProc[i].DateStop.Year-DateTime.Today.Year)*12)+(listRepeatChargesForProc[i].DateStop.Month-DateTime.Today.Month);
					if(DateTime.Today.Day<_patient.BillingCycleDay) {
						difference+=1;
					}
					combinedCount=_listProcedureCharges[index].ProcCount+remainder;
					if(DateTime.Today.AddMonths(combinedCount)>=listRepeatChargesForProc[i].DateStop) {
						//Hit the stop date.
						_listProcedureCharges[index].HasReachedStopDate=true;
						remainder=combinedCount-difference;
						_listProcedureCharges[index].ProcCount=difference;
						_listProcedureCharges[index].Calc();
						continue;
					}
					_listProcedureCharges[index].ProcCount+=remainder;
					remainder=0;
					_listProcedureCharges[index].Calc();
					continue;
				}
				//Start date in future.
				//See if the count exceeds the difference between the start and stop date of this repeat charge.
				difference=((listRepeatChargesForProc[i].DateStop.Year-listRepeatChargesForProc[i].DateStart.Year)*12)+(listRepeatChargesForProc[i].DateStop.Month-listRepeatChargesForProc[i].DateStart.Month);
				combinedCount=_listProcedureCharges[index].ProcCount+remainder;
				if(combinedCount>difference) {
					//Hit the stop date.
					_listProcedureCharges[index].HasReachedStopDate=true;
					remainder=combinedCount-difference;
					_listProcedureCharges[index].ProcCount=difference;
					_listProcedureCharges[index].Calc();
					continue;
				}
				_listProcedureCharges[index].ProcCount+=remainder;
				remainder=0;
				_listProcedureCharges[index].Calc();
			}
		}

		///<summary>Go through the transaction dictionary created in CreateProcedureLogs() to edit repeat charges as needed.  
		///Returns the note for the newly generated repeat charge.</summary>
		private void ZeroOutRepeatingCharge(ProcedureCode procedureCode,List<AvaTax.TransQtyAmt> listTransQtyAmt) {
			Commlog commlogPrepayment=new Commlog();
			commlogPrepayment.PatNum=_patient.PatNum;
			commlogPrepayment.SentOrReceived=CommSentOrReceived.Received;
			commlogPrepayment.CommDateTime=DateTime.Now;
			commlogPrepayment.DateTimeEnd=DateTime.Now;
			commlogPrepayment.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.FIN);
			commlogPrepayment.Mode_=CommItemMode.None;
			commlogPrepayment.Note="";//Appended to below.
			commlogPrepayment.UserNum=Security.CurUser.UserNum;
			string note="From PrepaymentTool: \r\n";
			bool hasBeenBilledThisMonth=(DateTime.Today.Day>=_patient.BillingCycleDay);
			//Get all applicable repeat charges.
			List<RepeatCharge> listRepeatChargesForProc=_listRepeatChargesForPat.FindAll(x => x.ProcCode==procedureCode.ProcCode && x.IsEnabled);
			//Get number of months new repeat charge will be for.
			int numMonths=listTransQtyAmt.Sum(x => x.Qty);
			//Create repeat charge, taken from ContrAccount.cs
			RepeatCharge repeatChargeNew=new RepeatCharge();
			repeatChargeNew.PatNum=_patient.PatNum;
			repeatChargeNew.ProcCode=procedureCode.ProcCode;
			repeatChargeNew.ChargeAmt=0;
			repeatChargeNew.IsEnabled=true;
			repeatChargeNew.CopyNoteToProc=true;
			//Build dates using billing day so the patient doesn't have gaps in their repeat charges.
			DateTime dateBillThisMonth=DateTimeOD.GetMostRecentValidDate(DateTime.Today.Year,DateTime.Today.Month,_patient.BillingCycleDay);
			if(hasBeenBilledThisMonth) {
				//Current month has been billed, push new repeat charge out a month.
				repeatChargeNew.DateStart=dateBillThisMonth.AddMonths(1);
				repeatChargeNew.DateStop=dateBillThisMonth.AddMonths(numMonths);
			}
			else {
				//Current month has not been billed yet, include on this repeat charge.
				repeatChargeNew.DateStart=dateBillThisMonth;
				repeatChargeNew.DateStop=dateBillThisMonth.AddMonths(numMonths-1);
			}
			//Use the stop date to update the Note as requested by Accounting.
			SetDatePrepayThrough(repeatChargeNew.DateStop.AddMonths(1).AddDays(-1));
			repeatChargeNew.Note=_prepaidThroughNote;
			Patient patientOld;
			//Edit exisiting repeat charge start/stop dates.
			for(int i=0;i<listRepeatChargesForProc.Count;i++) {
				RepeatCharge repeatChargeOld=listRepeatChargesForProc[i].Copy();
				patientOld=Patients.GetPat(repeatChargeOld.PatNum);
				if(listRepeatChargesForProc[i].DateStop.Year>1880 && listRepeatChargesForProc[i].DateStop<DateTime.Today) {
					continue;//The charge has a stop date in the past (has been disabled).
				}
				if(listRepeatChargesForProc[i].DateStop.Year>1880 && listRepeatChargesForProc[i].DateStop<=DateTime.Today.AddMonths(numMonths)) {
					listRepeatChargesForProc[i].DateStop=DateTime.Today;
					listRepeatChargesForProc[i].IsEnabled=false;
					//This repeat charge will never be used again due to the prepayment we are creating right now.  Disable and add note to commlog for history.
					note+="Disabled repeat charge with Rate: "+POut.Double(listRepeatChargesForProc[i].ChargeAmt)+" for Code: "+POut.String(listRepeatChargesForProc[i].ProcCode)
						+" Start Date: "+POut.Date(listRepeatChargesForProc[i].DateStart)+" Stop Date: "+POut.Date(listRepeatChargesForProc[i].DateStop)+"\r\n";
					RepeatCharges.Update(listRepeatChargesForProc[i]);
					RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatChargeOld,EnumPermType.RepeatChargeUpdate,patientOld,newCharge:listRepeatChargesForProc[i],isAutomated:false);
					continue;
				}
				//Need to push start date of existing repeat charge forward one month past the new repeat charge (if charge months overlap).
				DateTime dateNext=repeatChargeNew.DateStop.AddMonths(1);
				if(dateNext > listRepeatChargesForProc[i].DateStart) {//Only change if needed.
					note+="Edited Start Date for repeat charge from: "+POut.Date(listRepeatChargesForProc[i].DateStart)+" to: "+POut.Date(dateNext)+
						" Code: "+POut.String(listRepeatChargesForProc[i].ProcCode)+" Rate: "+POut.Double(listRepeatChargesForProc[i].ChargeAmt)+"\r\n";
					//Change to billing day to make sure it matches other repeat charges.
					listRepeatChargesForProc[i].DateStart=dateNext;
					RepeatCharges.Update(listRepeatChargesForProc[i]);
					RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatChargeOld,EnumPermType.RepeatChargeUpdate,patientOld,newCharge:listRepeatChargesForProc[i],isAutomated:false);
				}
			}
			//Insert the new repeat charge.
			commlogPrepayment.Note=note;
			Commlogs.Insert(commlogPrepayment);
			patientOld=Patients.GetPat(repeatChargeNew.PatNum);
			repeatChargeNew.RepeatChargeNum=RepeatCharges.Insert(repeatChargeNew);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatChargeNew,EnumPermType.RepeatChargeCreate,patientOld,isAutomated:false);
		}

		///<summary>Adds all selected rows in the support grid to the prepayment grid.</summary>
		private void AddSelectedProcs(int count) {
			for(int i=0;i<GridCodes.SelectedGridRows.Count();i++){
				CreateProcedureCharge((ProcedureCode)GridCodes.SelectedGridRows[i].Tag,count);
			}
		}

		///<summary>Removes any selected rows from the prepayments grid.</summary>
		private void RemoveSelectedProcs() {
			List<int> listSelectedIndices=GridPrepayment.SelectedIndices.OrderByDescending(x => x).ToList();
			for(int i=0;i<listSelectedIndices.Count;i++){
				_listProcedureCharges.RemoveAt(listSelectedIndices[i]);
			}
			listSelectedIndices=GridCompletedProcs.SelectedIndices.OrderByDescending(x => x).ToList();
			for(int i=0;i<listSelectedIndices.Count;i++){
				_listProcedureChargesPreviouslyComp.RemoveAt(listSelectedIndices[i]);
				_listProceduresCompleted.RemoveAt(listSelectedIndices[i]);
			}
		}

		///<summary>Looks at the procedures in the prepayment grid and determines if a discount should be applied.</summary>
		private void DiscountTest() {
			List<ProcedureCode> listProcedureCodesDiscount=AvaTax.GetListProcedureCodesDiscount();
			//Only 4 proccodes are eligible for discount: 001,008,017,030.
			for(int i=0;i<listProcedureCodesDiscount.Count;i++) {
				int countSum=0;
				//Prepayment grid.
				List<ProcedureCharge> listProcedureChargesInGrid=_listProcedureCharges.FindAll(x => x.ProcedureCodeCur.ProcCode==listProcedureCodesDiscount[i].ProcCode);
				if(listProcedureChargesInGrid.Count>0) {
					countSum+=listProcedureChargesInGrid.Sum(x => x.ProcCount);	
				}
				//Completed procedures grid.
				List<ProcedureCharge> listProcedureChargesCompleted=_listProcedureChargesPreviouslyComp.FindAll(x => x.ProcedureCodeCur.ProcCode==listProcedureCodesDiscount[i].ProcCode);
				if(listProcedureChargesCompleted.Count>0) {
					countSum+=listProcedureChargesCompleted.Sum(x => x.ProcCount);
				}
				if(countSum>5) {//Discounts are only provided if there are at least 6 months for the code.  See CreateDiscount() for details.
					for(int j=0;j<listProcedureChargesInGrid.Count();j++){
						CreateDiscount(listProcedureChargesInGrid[j],countSum);
					}
					for(int j=0;j<listProcedureChargesCompleted.Count();j++){
						CreateDiscount(listProcedureChargesCompleted[j],countSum);
					}
					continue;
				}
				//If changed to an count less than 6 months, then remove discounts in case they had some, since the discounts no longer apply.
				RemoveDiscount(listProcedureCodesDiscount[i].ProcCode);
			}
		}

		///<summary>Remove discounts from procedurecharges.</summary>
		private void RemoveDiscount(string procCode) {
			//Prepayment grid.
			for(int i=0;i<_listProcedureCharges.Count;i++){
				if(_listProcedureCharges[i].ProcedureCodeCur.ProcCode==procCode) {
					_listProcedureCharges[i].ProcDiscount=0;
					_listProcedureCharges[i].Calc();
				}
			}
			//Completed procedures grid.
			for(int i=0;i<_listProcedureChargesPreviouslyComp.Count;i++){
				if(_listProcedureChargesPreviouslyComp[i].ProcedureCodeCur.ProcCode!=procCode) {
					continue;
				}
				_listProcedureChargesPreviouslyComp[i].ProcDiscount=0;
				_listProcedureChargesPreviouslyComp[i].Calc();
			}
		}

		///<summary>Calculates the discount for a procedurecharge.  The count must be greater than 5.</summary>
		private void CreateDiscount(ProcedureCharge procedureCharge,int count) {
			//6-11 months gets a 5% discount.
			if(count<12) {//We already know it is greater than 5
				procedureCharge.ProcDiscount=.05*procedureCharge.ProcFee;
			}
			//12-23 months gets a 10% discount.
			else if(count<24) {
				procedureCharge.ProcDiscount=.1*procedureCharge.ProcFee;
			}
			//24+ months gets a 15% discount.
			else if(count>=24) {
				procedureCharge.ProcDiscount=.15*procedureCharge.ProcFee;
			}
			procedureCharge.ProcDiscount=Math.Round(procedureCharge.ProcDiscount,2,MidpointRounding.AwayFromZero);
			procedureCharge.Calc();
		}

		///<summary>Create completed ProcedureLogs from prepayment grid.</summary>
		private void CreateProcedureLogs() {
			Dictionary<ProcedureCode,List<AvaTax.TransQtyAmt>> dictionaryTransactions=new Dictionary<ProcedureCode,List<AvaTax.TransQtyAmt>>();
			List<ProcedureCharge> listProcedureChargesMatching=new List<ProcedureCharge>();
			//Go through our list of support codes and get all procedurecharges relating to the given code.
			for(int i=0;i<_listProcedureCodesPrePaySupport.Count();i++){
				listProcedureChargesMatching=_listProcedureCharges.FindAll(x => x.ProcedureCodeCur==_listProcedureCodesPrePaySupport[i]);
				if(listProcedureChargesMatching.Count==0) {
					continue;
				}
				List<AvaTax.TransQtyAmt> listTransQtyAmt=new List<AvaTax.TransQtyAmt>();
				for(int j=0;j<listProcedureChargesMatching.Count;j++){
					AvaTax.TransQtyAmt transQtyAmt=new AvaTax.TransQtyAmt();
					transQtyAmt.Qty=listProcedureChargesMatching[j].ProcCount;
					transQtyAmt.Rate=listProcedureChargesMatching[j].BaseFee;
					listTransQtyAmt.Add(transQtyAmt);
				}
				dictionaryTransactions.Add(_listProcedureCodesPrePaySupport[i],listTransQtyAmt);
				//Zero out related repeat charges.
				ZeroOutRepeatingCharge(_listProcedureCodesPrePaySupport[i],listTransQtyAmt);
				//Create procedures and adjustments without calling AvaTax API if the current patient is not taxable.
				if(AvaTax.IsTaxable(_patient.PatNum)) {
					continue;
				}
				Procedure procedure=new Procedure() {
					PatNum=_patient.PatNum,
					ProvNum=7,//HQ only this is jordan's provNum.
					ProcDate=DateTime.Today,
					DateEntryC=DateTime.Now,
					DateComplete=DateTime.Now,
					CodeNum=_listProcedureCodesPrePaySupport[i].CodeNum,
					ProcStatus=ProcStat.C
				};
				for(int j=0;j<listProcedureChargesMatching.Count();j++){
					procedure.BillingNote+=$"Rate: ${POut.Double(listProcedureChargesMatching[j].BaseFee)} Months: {POut.Int(listProcedureChargesMatching[j].ProcCount)}\r\n";
				}
				procedure.BillingNote+=_prepaidThroughNote;
				List<Procedure> listProcedureMatchingCompleted=_listProceduresCompleted.FindAll(x => x.CodeNum==_listProcedureCodesPrePaySupport[i].CodeNum);
				int monthsPrepayed=listProcedureChargesMatching.Sum(x => x.ProcCount)+listProcedureMatchingCompleted.Count();
				procedure.ProcFee=listProcedureChargesMatching.Sum(x => x.ProcFee);
				procedure.ProcNum=Procedures.Insert(procedure);
				if(monthsPrepayed<6 || !AvaTax.GetListProcedureCodesDiscount().Exists(x => x.ProcCode==ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode)){
					continue;
				}
				//Create a discount adjustment for prepayments.
				if(monthsPrepayed>=6 && monthsPrepayed<=11) {
					AvaTax.CreateDiscountAdjustment(procedure,.05,255);//5% discount.  Hard coded ODHQ defnum.
				}
				else if(monthsPrepayed>=12 && monthsPrepayed<=23) {
					AvaTax.CreateDiscountAdjustment(procedure,.10,206);//10% discount.  Hard coded ODHQ defnum.
				}
				else if(monthsPrepayed>=24) {
					AvaTax.CreateDiscountAdjustment(procedure,.15,229);//15% discount.  Hard coded ODHQ defnum.
				}
				//Create adjustments for the previously completed procedures.
				for(int j=0;j<listProcedureMatchingCompleted.Count();j++){
					if(monthsPrepayed>=6 && monthsPrepayed<=11) {
						AvaTax.CreateDiscountAdjustment(listProcedureMatchingCompleted[j],.05,255);//5% discount.  Hard coded ODHQ defnum.
					}
					else if(monthsPrepayed>=12 && monthsPrepayed<=23) {
						AvaTax.CreateDiscountAdjustment(listProcedureMatchingCompleted[j],.10,206);//10% discount.  Hard coded ODHQ defnum.
					}
					else if(monthsPrepayed>=24) {
						AvaTax.CreateDiscountAdjustment(listProcedureMatchingCompleted[j],.15,229);//15% discount.  Hard coded ODHQ defnum.
					}
				}
			}
			if(!AvaTax.IsTaxable(_patient.PatNum)){
				return;
			}
			try {
				AvaTax.CreatePrepaymentTransaction(dictionaryTransactions,_patient,_listProceduresCompleted);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error sending procedures to AvaTax.")+"\r\n"+ex.ToString());
			}
		}

		#region UI Methods

		private void butAddMonths_Click(object sender,EventArgs e) {
			if(string.IsNullOrWhiteSpace(textMonths.Text)){
				return;
			}
			int months=PIn.Int(textMonths.Text,false);
			if(months>100) {
				months=100;
			}
			AddSelectedProcs(months);
			FillGridPrepayment();
		}

		private void butAdd12_Click(object sender,EventArgs e) {
			AddSelectedProcs(12);
			FillGridPrepayment();
		}

		private void butAdd6_Click(object sender,EventArgs e) {
			AddSelectedProcs(6);
			FillGridPrepayment();
		}

		private void butAdd1_Click(object sender,EventArgs e) {
			AddSelectedProcs(1);
			FillGridPrepayment();
		}

		private void butPreviousProc_Click(object sender,EventArgs e) {
			using FormProcSelect formProcSelect=new FormProcSelect(_patient.PatNum,false);
			if(formProcSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			//isMultiSelect is set to false so should only ever be one procedure, constructing in a way to handle multiple in case we ever want to change
			//isMultiSelect to be true.
			for(int i=0;i<formProcSelect.ListProceduresSelected.Count();i++){
				//Use our list of ProcedureCodes so irrelevant codes result in a new ProcedureCode with blank values.
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(formProcSelect.ListProceduresSelected[i].CodeNum,_listProcedureCodesPrePaySupport);
				//Didn't exist in our list of ProcedureCodes, tell user it won't be added.
				if(procedureCode.CodeNum==0) {
					//Did not translate because HQ only.
					MsgBox.Show(this,"Selected procedure cannot be used with the Prepayment Tool.");
					continue;
				}
				ProcedureCharge procedureCharge=new ProcedureCharge(procedureCode,formProcSelect.ListProceduresSelected[i].ProcFee);
				Adjustment adjustmentTax=Adjustments.GetSalesTaxForProc(formProcSelect.ListProceduresSelected[i].ProcNum);
				if(adjustmentTax!=null) {
					procedureCharge.EstTax=adjustmentTax.AdjAmt;
					procedureCharge.HasTaxAdjustment=true;
				}
				else {
					procedureCharge.EstTax=0;
				}
				procedureCharge.Calc();
				_listProcedureChargesPreviouslyComp.Add(procedureCharge);
				_listProceduresCompleted.Add(formProcSelect.ListProceduresSelected[i]);
			}
			//This calls FillGridCompletedProcs()
			FillGridPrepayment();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			RemoveSelectedProcs();
			FillGridPrepayment();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(_listProcedureCharges.Count==0) {
				MessageBox.Show(Lan.g("Prepayment Tool","No procedures selected."));
				return;
			}
			PaymentReturn=new Payment();
			PaymentReturn.PayDate=DateTime.Today;
			PaymentReturn.PatNum=_patient.PatNum;
			PaymentReturn.ClinicNum=0;
			PaymentReturn.PayAmt=PIn.Double(textTotal.Text);
			PaymentReturn.DateEntry=DateTime.Today; List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			if(listDefs.Count>0) {
				PaymentReturn.PayType=listDefs[0].DefNum;
			}
			CreateProcedureLogs();
			DialogResult=DialogResult.OK;
		}
		#endregion

	}
}