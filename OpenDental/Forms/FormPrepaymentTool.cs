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
		public Payment ReturnPayment;
		private Patient _patCur;
		///<summary>A constant list of procedure codes which are pulled from the AvaTax bridge property Prepay Proc Codes.</summary>
		private List<ProcedureCode> _listPrePaySupportCodes;
		private List<ProcedureCode> _listPrepaymentCodes;
		///<summary>This will hold ProcCodes along with the fee associated to the code.</summary>
		private List<ProcedureCharge> _listProcedureCharge;
		///<summary>Get all repeating charges for patient on load.</summary>
		private List<RepeatCharge> _listRcForPat;
		private double _chargeTotal;
		private double _discountTotal;
		private double _estTaxTotal;
		private double _total;
		///<summary>Number of rows which used to be in the payment grid.  Used when you add a row to make it easier to highlight new rows.</summary>
		private int _prevPaymentRowCount;
		///<summary>List of procedures returned from FormProcSelect and sent to AvaTax API to prevent double billing.</summary>
		private List<Procedure> _listCompletedProcs;
		///<summary>List of procedurecharges returned from FormProcSelect and used to fill gridCompletedProcs.</summary>
		private List<ProcedureCharge> _listPreviouslyCompProcedureCharges;
		private string _prepaidThroughNote;
		private DateTime _datePrepayThrough;
		/// <summary>Initialized to mindate, set in ZeroOutRepeatingCharge().  Used for building the billingnote on generated procedures.</summary>
		public DateTime DatePrepaidThrough {
			get {
				return _datePrepayThrough;
			}
			private set {
				_datePrepayThrough=value;
				_prepaidThroughNote=$"Prepaid through: {_datePrepayThrough.Date:MM/dd/yyyy}";
			}
		}

		public FormPrepaymentTool(Patient PatCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=PatCur;
		}

		///<summary>This class lets us track a single procedurecode in the prepayment grid along with the any charge, discount or est tax values
		///associated with that procedure.</summary>
		private class ProcedureCharge {
			public ProcedureCode ProcCode;
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

			public ProcedureCharge(ProcedureCode code,double charge) {
				ProcCode=code;
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
			_listRcForPat=RepeatCharges.Refresh(_patCur.PatNum).ToList();
			_listProcedureCharge=new List<ProcedureCharge>();
			_listCompletedProcs=new List<Procedure>();
			_listPreviouslyCompProcedureCharges=new List<ProcedureCharge>();
			_prevPaymentRowCount=0;
			_listPrepaymentCodes=new List<ProcedureCode>();
			//Load procedurecode list.  HQ internal codes.
			_listPrePaySupportCodes=AvaTax.ListPrePayProcCodes;
			DatePrepaidThrough=DateTime.MinValue;
			FillGridCodes();
			FillGridPrepayment();	
		}

		private void FillGridCodes() {
			GridRow row;
			GridCodes.BeginUpdate();
			GridCodes.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Code"),50);
			GridCodes.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),100){ IsWidthDynamic=true };
			GridCodes.ListGridColumns.Add(col);
			GridCodes.ListGridRows.Clear();
			for(int i=0;i<_listPrePaySupportCodes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listPrePaySupportCodes[i].ProcCode);
				row.Cells.Add(_listPrePaySupportCodes[i].Descript);
				row.Tag=_listPrePaySupportCodes[i];
				GridCodes.ListGridRows.Add(row);
			}
			GridCodes.EndUpdate();
		}

		private void FillGridCompletedProcs() {
			GetTaxEstimates(_listPreviouslyCompProcedureCharges,false);
			GridCompletedProcs.BeginUpdate();
			GridCompletedProcs.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Code"),50,HorizontalAlignment.Center);
			GridCompletedProcs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),100,HorizontalAlignment.Left){ IsWidthDynamic=true };
			GridCompletedProcs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Units"),30,HorizontalAlignment.Right);
			GridCompletedProcs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Base"),50,HorizontalAlignment.Right);
			GridCompletedProcs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Charge"),75,HorizontalAlignment.Right);
			GridCompletedProcs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Discount"),75,HorizontalAlignment.Right);
			GridCompletedProcs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Est Tax"),75,HorizontalAlignment.Right);
			GridCompletedProcs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),75,HorizontalAlignment.Right);
			GridCompletedProcs.ListGridColumns.Add(col);
			GridCompletedProcs.ListGridRows.Clear();
			foreach(ProcedureCharge proc in _listPreviouslyCompProcedureCharges) {
				GridRow row=new GridRow();
				row.Cells.Add(proc.ProcCode.ProcCode);
				row.Cells.Add(proc.ProcCode.Descript);
				row.Cells.Add(proc.ProcCount.ToString());
				row.Cells.Add(proc.BaseFee.ToString("f"));
				row.Cells.Add(proc.ProcFee.ToString("f"));
				row.Cells.Add(proc.ProcDiscount.ToString("f"));
				row.Cells.Add(proc.EstTax.ToString("f"));
				row.Cells.Add(proc.ProcTotal.ToString("f"));
				GridCompletedProcs.ListGridRows.Add(row);
				_chargeTotal+=proc.ProcFee;
				_discountTotal+=proc.ProcDiscount;
				_estTaxTotal+=proc.EstTax;
				_total+=proc.ProcTotal;
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
			GetTaxEstimates(_listProcedureCharge);
			GridPrepayment.BeginUpdate();
			GridPrepayment.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Code"),50,HorizontalAlignment.Center);
			GridPrepayment.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),100,HorizontalAlignment.Left){ IsWidthDynamic=true };
			GridPrepayment.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Units"),30,HorizontalAlignment.Right);
			GridPrepayment.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Base"),50,HorizontalAlignment.Right);
			GridPrepayment.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Charge"),75,HorizontalAlignment.Right);
			GridPrepayment.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Discount"),75,HorizontalAlignment.Right);
			GridPrepayment.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Est Tax"),75,HorizontalAlignment.Right);
			GridPrepayment.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),75,HorizontalAlignment.Right);
			GridPrepayment.ListGridColumns.Add(col);
			GridPrepayment.ListGridRows.Clear();
			foreach(ProcedureCharge proc in _listProcedureCharge) {
				GridRow row=new GridRow();
				row.Cells.Add(proc.ProcCode.ProcCode);
				row.Cells.Add(proc.ProcCode.Descript);
				row.Cells.Add(proc.ProcCount.ToString());
				row.Cells.Add(proc.BaseFee.ToString("f"));
				row.Cells.Add(proc.ProcFee.ToString("f"));
				row.Cells.Add(proc.ProcDiscount.ToString("f"));
				row.Cells.Add(proc.EstTax.ToString("f"));
				row.Cells.Add(proc.ProcTotal.ToString("f"));
				GridPrepayment.ListGridRows.Add(row);
				_chargeTotal+=proc.ProcFee;
				_discountTotal+=proc.ProcDiscount;
				_estTaxTotal+=proc.EstTax;
				_total+=proc.ProcTotal;
			}
			GridPrepayment.EndUpdate();
			for(int i=_prevPaymentRowCount;i<_listProcedureCharge.Count;i++) {
					GridPrepayment.SetSelected(i,true);
			}
			//Fill completed proc grid so the total row at the bottom is accurate.
			FillGridCompletedProcs();
			_prevPaymentRowCount=_listProcedureCharge.Count;
			textOrigSub.Text=_chargeTotal.ToString("f");
			textDiscount.Text=_discountTotal.ToString("f");
			textTaxSub.Text=_estTaxTotal.ToString("f");
			textTotal.Text=_total.ToString("f");
		}

		private void GridPrepayment_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormPrepaymentEdit FormPE=new FormPrepaymentEdit();
			FormPE.CountCur=_listProcedureCharge[e.Row].ProcCount;
			if(FormPE.ShowDialog()==DialogResult.OK) {
				_listProcedureCharge[e.Row].ProcCount=FormPE.CountCur;
				_listProcedureCharge[e.Row].Calc();
				FillGridPrepayment();
			}
		}

		///<summary>Calls Avalara to get an estimated sales tax for each charge.</summary>
		private void GetTaxEstimates(List<ProcedureCharge> listCharges,bool isPrepaymentList=true) {
			foreach(ProcedureCharge procCharge in listCharges) {
				//Prevents getting tax estimates on previously completed procedures that were not taxed in the first place.
				if(isPrepaymentList || procCharge.HasTaxAdjustment) {
					procCharge.EstTax=(double)AvaTax.GetEstimate(procCharge.ProcCode.CodeNum,_patCur.PatNum,procCharge.ProcFee-procCharge.ProcDiscount);
					procCharge.Calc();
				}
				
			}
		}

		///<summary>Looks in the patient's repeating charges for a rate to use.  If the patient does not have a repeat charge setup
		///for the procedurecode that is being added then a procedurecharge will not be created.
		///This is then used to create one or more procedurecharges.</summary>
		private void CreateProcedureCharge(ProcedureCode ProcCode,int count,bool isNew=true) {
			List<RepeatCharge> listRcForProc=_listRcForPat.FindAll(x => x.ProcCode==ProcCode.ProcCode && x.IsEnabled);
			List<ProcedureCharge> listProcChargeInGrid=_listProcedureCharge.FindAll(x => x.ProcCode==ProcCode);
			if(listRcForProc.Count==1) {
				//Discussed with accounting dept.  If there is only one repeat charge for a proc on an account it should never have a stop date.
				//If for some reason there is a stop date, it is a very special case that we can't currently handle with the prepayment tool.
				int index=_listProcedureCharge.FindIndex(x => x.BaseFee==listRcForProc[0].ChargeAmt && x.ProcCode==ProcCode);
				//Does not exist in our list, make a new one.
				if(index==-1) {
					ProcedureCharge procCharge=new ProcedureCharge(ProcCode,listRcForProc.FirstOrDefault().ChargeAmt);
					procCharge.ProcCount=count;
					procCharge.Calc();
					_listProcedureCharge.Add(procCharge);
				}
				else {
					_listProcedureCharge[index].ProcCount+=count;
				}
			}
			else if(listRcForProc.Count>=2) {
				int remainder=count;
				foreach(RepeatCharge charge in listRcForProc) {
					if(remainder<=0) {//Nothing left to add, kick out of loop.
						break;
					}
					if(charge.DateStop.Year>1880 && charge.DateStop<DateTime.Today) {
						continue;//The charge has a stop date in the past (has been disabled).
					}
					//Look for existing procedurecharge.
					int index=_listProcedureCharge.FindIndex(x => x.BaseFee==charge.ChargeAmt && x.ProcCode==ProcCode && x.HasReachedStopDate==false);
					//Does not exist in our list, make a new one.
					if(index==-1) {
						ProcedureCharge procCharge=new ProcedureCharge(ProcCode,charge.ChargeAmt);
						//See if the months we are adding extends beyond this repeat charge's stop date.
						DateTime dateToUse=charge.DateStart;
						//Check if we should use start date or today.  Prevent old charges.
						if(DateTime.Today>charge.DateStart) {
							dateToUse=DateTime.Today;
						}
						if(dateToUse.AddMonths(remainder)>charge.DateStop && charge.DateStop.Year>1880) {
							procCharge.HasReachedStopDate=true;
							//Difference is number of months between today and the stop date, this will become the count for our current procedurecharge
							int difference=((charge.DateStop.Year-dateToUse.Year)*12)+(charge.DateStop.Month-dateToUse.Month);
							if(DateTime.Today<DateTimeOD.GetMostRecentValidDate(dateToUse.Year,dateToUse.Month,_patCur.BillingCycleDay)) {
								difference+=1;
							}
							procCharge.ProcCount=difference;
							remainder=remainder-difference;
						}
						//Stop date was not reached
						else {
							procCharge.ProcCount=remainder;
							remainder=0;
						}
						procCharge.Calc();
						_listProcedureCharge.Add(procCharge);
					}
					//Found a matching procedurecharge.
					else {
						if(_listProcedureCharge[index].HasReachedStopDate) {
							//Already hit stopdate for this repeat charge so continue on to the next one.
							continue;
						}
						//See if stop date is mindate, if it is then we can add to the count and exit the loop.
						if(charge.DateStop.Year<1880) {
							_listProcedureCharge[index].ProcCount+=remainder;
							remainder=0;
							continue;
						}
						//Has a stop date, see if we hit it.
						else {
							//See if we should use today or the start date.
							if(DateTime.Today>charge.DateStart) {
								//See if the count exceeds the difference between today and stop date of this repeat charge.
								int difference=((charge.DateStop.Year-DateTime.Today.Year)*12)+(charge.DateStop.Month-DateTime.Today.Month);
								if(DateTime.Today.Day<_patCur.BillingCycleDay) {
									difference+=1;
								}
								int combinedCount=_listProcedureCharge[index].ProcCount+remainder;
								if(DateTime.Today.AddMonths(combinedCount)>=charge.DateStop) {
									//Hit the stop date.
									_listProcedureCharge[index].HasReachedStopDate=true;
									remainder=combinedCount-difference;
									_listProcedureCharge[index].ProcCount=difference;
								}
								else {
									_listProcedureCharge[index].ProcCount+=remainder;
									remainder=0;
								}
							}
							else {//Start date in future.
								//See if the count exceeds the difference between the start and stop date of this repeat charge.
								int difference=((charge.DateStop.Year-charge.DateStart.Year)*12)+(charge.DateStop.Month-charge.DateStart.Month);
								int combinedCount=_listProcedureCharge[index].ProcCount+remainder;
								if(combinedCount>difference) {
									//Hit the stop date.
									_listProcedureCharge[index].HasReachedStopDate=true;
									remainder=combinedCount-difference;
									_listProcedureCharge[index].ProcCount=difference;
								}
								else {
									_listProcedureCharge[index].ProcCount+=remainder;
									remainder=0;
								}
							}
						}
					}
				}
			}
		}

		///<summary>Go through the transaction dictionary created in CreateProcedureLogs() to edit repeat charges as needed.  
		///Returns the note for the newly generated repeat charge.</summary>
		private void ZeroOutRepeatingCharge(ProcedureCode procCur,List<AvaTax.TransQtyAmt> listCurTrans) {
			Commlog prepaymentCommlog=new Commlog();
			prepaymentCommlog.PatNum=_patCur.PatNum;
			prepaymentCommlog.SentOrReceived=CommSentOrReceived.Received;
			prepaymentCommlog.CommDateTime=DateTime.Now;
			prepaymentCommlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.FIN);
			prepaymentCommlog.Mode_=CommItemMode.None;
			prepaymentCommlog.Note="";//Appended to below.
			prepaymentCommlog.UserNum=Security.CurUser.UserNum;
			string note="From PrepaymentTool: \r\n";
			bool hasBeenBilledThisMonth=(DateTime.Today.Day>=_patCur.BillingCycleDay);
			//Get all applicable repeat charges.
			List<RepeatCharge> listRcForProc=_listRcForPat.FindAll(x => x.ProcCode==procCur.ProcCode && x.IsEnabled);
			//Get number of months new repeat charge will be for.
			int numMonths=listCurTrans.Sum(x => x.qty);
			//Create repeat charge, taken from ContrAccount.cs
			RepeatCharge rcNew=new RepeatCharge();
			rcNew.PatNum=_patCur.PatNum;
			rcNew.ProcCode=procCur.ProcCode;
			rcNew.ChargeAmt=0;
			rcNew.IsEnabled=true;
			rcNew.CopyNoteToProc=true;
			//Build dates using billing day so the patient doesn't have gaps in their repeat charges.
			DateTime dateBillThisMonth=DateTimeOD.GetMostRecentValidDate(DateTime.Today.Year,DateTime.Today.Month,_patCur.BillingCycleDay);
			if(hasBeenBilledThisMonth) {
				//Current month has been billed, push new repeat charge out a month.
				rcNew.DateStart=dateBillThisMonth.AddMonths(1);
				rcNew.DateStop=dateBillThisMonth.AddMonths(numMonths);
			}
			else {
				//Current month has not been billed yet, include on this repeat charge.
				rcNew.DateStart=dateBillThisMonth;
				rcNew.DateStop=dateBillThisMonth.AddMonths(numMonths-1);
			}
			//Use the stop date to update the Note as requested by Accounting.
			DatePrepaidThrough=rcNew.DateStop.AddMonths(1).AddDays(-1);
			rcNew.Note=_prepaidThroughNote;
			Patient oldPat;
			//Edit exisiting repeat charge start/stop dates.
			foreach(RepeatCharge rcExisting in listRcForProc) {
				RepeatCharge oldCharge=rcExisting.Copy();
				oldPat=Patients.GetPat(oldCharge.PatNum);
				if(rcExisting.DateStop.Year>1880 && rcExisting.DateStop<DateTime.Today) {
					continue;//The charge has a stop date in the past (has been disabled).
				}
				if(rcExisting.DateStop.Year>1880 && rcExisting.DateStop<=DateTime.Today.AddMonths(numMonths)) {
					rcExisting.DateStop=DateTime.Today;
					rcExisting.IsEnabled=false;
					//This repeat charge will never be used again due to the prepayment we are creating right now.  Disable and add note to commlog for history.
					note+="Disabled repeat charge with Rate: "+POut.Double(rcExisting.ChargeAmt)+" for Code: "+POut.String(rcExisting.ProcCode)
						+" Start Date: "+POut.Date(rcExisting.DateStart)+" Stop Date: "+POut.Date(rcExisting.DateStop)+"\r\n";
					RepeatCharges.Update(rcExisting);
					RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(oldCharge,Permissions.RepeatChargeUpdate,oldPat,newCharge:rcExisting,isAutomated:false);
					continue;
				}
				//Need to push start date of existing repeat charge forward one month past the new repeat charge (if charge months overlap).
				DateTime dateNext=rcNew.DateStop.AddMonths(1);
				if(dateNext > rcExisting.DateStart) {//Only change if needed.
					note+="Edited Start Date for repeat charge from: "+POut.Date(rcExisting.DateStart)+" to: "+POut.Date(dateNext)+
						" Code: "+POut.String(rcExisting.ProcCode)+" Rate: "+POut.Double(rcExisting.ChargeAmt)+"\r\n";
					//Change to billing day to make sure it matches other repeat charges.
					rcExisting.DateStart=dateNext;
					RepeatCharges.Update(rcExisting);
					RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(oldCharge,Permissions.RepeatChargeUpdate,oldPat,newCharge:rcExisting,isAutomated:false);
				}
			}
			//Insert the new repeat charge.
			prepaymentCommlog.Note=note;
			Commlogs.Insert(prepaymentCommlog);
			oldPat=Patients.GetPat(rcNew.PatNum);
			rcNew.RepeatChargeNum=RepeatCharges.Insert(rcNew);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(rcNew,Permissions.RepeatChargeCreate,oldPat,isAutomated:false);
		}

		///<summary>Adds all selected rows in the support grid to the prepayment grid.</summary>
		private void AddSelectedProcs(int count) {
			foreach(GridRow row in GridCodes.SelectedGridRows) {
				CreateProcedureCharge((ProcedureCode)row.Tag,count);
			}
		}

		///<summary>Removes any selected rows from the prepayments grid.</summary>
		private void RemoveSelectedProcs() {
			foreach(int index in GridPrepayment.SelectedIndices.OrderByDescending(x => x)) {//Need to flip the list to prevent UEs.
				_listProcedureCharge.RemoveAt(index);
			}
			foreach(int index in GridCompletedProcs.SelectedIndices.OrderByDescending(x => x)) {
				_listPreviouslyCompProcedureCharges.RemoveAt(index);
				_listCompletedProcs.RemoveAt(index);
			}
		}

		///<summary>Looks at the procedures in the prepayment grid and determines if a discount should be applied.</summary>
		private void DiscountTest() {
			List<ProcedureCode> listDiscountCodes=AvaTax.ListDiscountProcCodes;
			//Only 4 proccodes are eligible for discount: 001,008,017,030.
			foreach(ProcedureCode code in listDiscountCodes) {
				int countSum=0;
				//Prepayment grid.
				List<ProcedureCharge> listProcChargeInGrid=_listProcedureCharge.FindAll(x => x.ProcCode.ProcCode==code.ProcCode);
				if(listProcChargeInGrid.Count>0) {
					countSum+=listProcChargeInGrid.Sum(x => x.ProcCount);	
				}
				//Completed procedures grid.
				List<ProcedureCharge> listCompletedProcCharges=_listPreviouslyCompProcedureCharges.FindAll(x => x.ProcCode.ProcCode==code.ProcCode);
				if(listCompletedProcCharges.Count>0) {
					countSum+=listCompletedProcCharges.Sum(x => x.ProcCount);
				}
				if(countSum>5) {//Discounts are only provided if there are at least 6 months for the code.  See CreateDiscount() for details.
					foreach(ProcedureCharge proc in listProcChargeInGrid) {
						CreateDiscount(proc,countSum);
					}
					foreach(ProcedureCharge proc in listCompletedProcCharges) {
						CreateDiscount(proc,countSum);
					}
				}
				else {//If changed to an count less than 6 months, then remove discounts in case they had some, since the discounts no longer apply.
					RemoveDiscount(code.ProcCode);
				}
			}
		}

		///<summary>Remove discounts from procedurecharges.</summary>
		private void RemoveDiscount(string procCode) {
			//Prepayment grid.
			foreach(ProcedureCharge procCharge in _listProcedureCharge.Where(x => x.ProcCode.ProcCode==procCode)) {
				procCharge.ProcDiscount=0;
				procCharge.Calc();
			}
			//Completed procedures grid.
			foreach(ProcedureCharge procCharge in _listPreviouslyCompProcedureCharges.Where(x => x.ProcCode.ProcCode==procCode)) {
				procCharge.ProcDiscount=0;
				procCharge.Calc();
			}
		}

		///<summary>Calculates the discount for a procedurecharge.  The count must be greater than 5.</summary>
		private void CreateDiscount(ProcedureCharge proc,int count) {
			//6-11 months gets a 5% discount.
			if(count<12) {//We already know it is greater than 5
				proc.ProcDiscount=.05*proc.ProcFee;
			}
			//12-23 months gets a 10% discount.
			else if(count<24) {
				proc.ProcDiscount=.1*proc.ProcFee;
			}
			//24+ months gets a 15% discount.
			else if(count>=24) {
				proc.ProcDiscount=.15*proc.ProcFee;
			}
			proc.ProcDiscount=Math.Round(proc.ProcDiscount,2,MidpointRounding.AwayFromZero);
			proc.Calc();
		}

		///<summary>Create completed ProcedureLogs from prepayment grid.</summary>
		private void CreateProcedureLogs() {
			Dictionary<ProcedureCode,List<AvaTax.TransQtyAmt>> dictTransactions=new Dictionary<ProcedureCode,List<AvaTax.TransQtyAmt>>();
			List<ProcedureCharge> listMatchingProcCharges=new List<ProcedureCharge>();
			//Go through our list of support codes and get all procedurecharges relating to the given code.
			foreach(ProcedureCode ProcCode in _listPrePaySupportCodes) {
				listMatchingProcCharges=_listProcedureCharge.FindAll(x => x.ProcCode==ProcCode);
				if(listMatchingProcCharges.Count==0) {
					continue;
				}
				List<AvaTax.TransQtyAmt> listTransQtyAmt=new List<AvaTax.TransQtyAmt>();
				foreach(ProcedureCharge procCharge in listMatchingProcCharges) {
					listTransQtyAmt.Add(new AvaTax.TransQtyAmt(procCharge.ProcCount,procCharge.BaseFee));
				}
				dictTransactions.Add(ProcCode,listTransQtyAmt);
				//Zero out related repeat charges.
				ZeroOutRepeatingCharge(ProcCode,listTransQtyAmt);
				//Create procedures and adjustments without calling AvaTax API if the current patient is not taxable.
				if(!AvaTax.IsTaxable(_patCur.PatNum)) {
					Procedure procCur=new Procedure() {
						PatNum=_patCur.PatNum,
						ProvNum=7,//HQ only this is jordan's provNum.
						ProcDate=DateTime.Today,
						DateEntryC=DateTime.Now,
						DateComplete=DateTime.Now,
						CodeNum=ProcCode.CodeNum,
						ProcStatus=ProcStat.C
					};
					foreach(ProcedureCharge procCharge in listMatchingProcCharges) {
						procCur.BillingNote+=$"Rate: ${POut.Double(procCharge.BaseFee)} Months: {POut.Int(procCharge.ProcCount)}\r\n";
					}
					procCur.BillingNote+=_prepaidThroughNote;
					List<Procedure> listMatchingCompletedProcs=_listCompletedProcs.FindAll(x => x.CodeNum==ProcCode.CodeNum);
					int monthsPrepayed=listMatchingProcCharges.Sum(x => x.ProcCount)+listMatchingCompletedProcs.Count();
					procCur.ProcFee=listMatchingProcCharges.Sum(x => x.ProcFee);
					procCur.ProcNum=Procedures.Insert(procCur);
					if(monthsPrepayed>=6 && AvaTax.ListDiscountProcCodes.Exists(x => x.ProcCode==ProcedureCodes.GetProcCode(procCur.CodeNum).ProcCode)) {
						//Create a discount adjustment for prepayments.
						if(monthsPrepayed>=6 && monthsPrepayed<=11) {
							AvaTax.CreateDiscountAdjustment(procCur,.05,255);//5% discount.  Hard coded ODHQ defnum.
						}
						else if(monthsPrepayed>=12 && monthsPrepayed<=23) {
							AvaTax.CreateDiscountAdjustment(procCur,.10,206);//10% discount.  Hard coded ODHQ defnum.
						}
						else if(monthsPrepayed>=24) {
							AvaTax.CreateDiscountAdjustment(procCur,.15,229);//15% discount.  Hard coded ODHQ defnum.
						}
						//Create adjustments for the previously completed procedures.
						foreach(Procedure proc in listMatchingCompletedProcs) {
							if(monthsPrepayed>=6 && monthsPrepayed<=11) {
								AvaTax.CreateDiscountAdjustment(proc,.05,255);//5% discount.  Hard coded ODHQ defnum.
							}
							else if(monthsPrepayed>=12 && monthsPrepayed<=23) {
								AvaTax.CreateDiscountAdjustment(proc,.10,206);//10% discount.  Hard coded ODHQ defnum.
							}
							else if(monthsPrepayed>=24) {
								AvaTax.CreateDiscountAdjustment(proc,.15,229);//15% discount.  Hard coded ODHQ defnum.
							}
						}
					}
				}
			}
			if(AvaTax.IsTaxable(_patCur.PatNum)) {
				try {
					AvaTax.CreatePrepaymentTransaction(dictTransactions,_patCur,_listCompletedProcs);
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Error sending procedures to AvaTax.")+"\r\n"+ex.ToString());
				}
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
			using FormProcSelect FormPS=new FormProcSelect(_patCur.PatNum,false);
			if(FormPS.ShowDialog()!=DialogResult.OK) {
				return;
			}
			//isMultiSelect is set to false so should only ever be one procedure, constructing in a way to handle multiple in case we ever want to change
			//isMultiSelect to be true.
			foreach(Procedure proc in FormPS.ListSelectedProcs) {
				//Use our list of ProcedureCodes so irrelevant codes result in a new ProcedureCode with blank values.
				ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum,_listPrePaySupportCodes);
				//Didn't exist in our list of ProcedureCodes, tell user it won't be added.
				if(procCode.CodeNum==0) {
					//Did not translate because HQ only.
					MsgBox.Show(this,"Selected procedure cannot be used with the Prepayment Tool.");
					continue;
				}
				ProcedureCharge procCharge=new ProcedureCharge(procCode,proc.ProcFee);
				Adjustment taxAdjustment=Adjustments.GetSalesTaxForProc(proc.ProcNum);
				if(taxAdjustment!=null) {
					procCharge.EstTax=taxAdjustment.AdjAmt;
					procCharge.HasTaxAdjustment=true;
				}
				else {
					procCharge.EstTax=0;
				}
				procCharge.Calc();
				_listPreviouslyCompProcedureCharges.Add(procCharge);
				_listCompletedProcs.Add(proc);
			}
			//This calls FillGridCompletedProcs()
			FillGridPrepayment();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			RemoveSelectedProcs();
			FillGridPrepayment();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_listProcedureCharge.Count==0) {
				MessageBox.Show(Lan.g("Prepayment Tool","No procedures selected."));
				return;
			}
			ReturnPayment=new Payment();
			ReturnPayment.PayDate=DateTime.Today;
			ReturnPayment.PatNum=_patCur.PatNum;
			ReturnPayment.ClinicNum=0;
			ReturnPayment.PayAmt=PIn.Double(textTotal.Text);
			ReturnPayment.DateEntry=DateTime.Today; List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			if(listDefs.Count>0) {
				ReturnPayment.PayType=listDefs[0].DefNum;
			}
			CreateProcedureLogs();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion
	}
}