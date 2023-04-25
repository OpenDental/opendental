 using CodeBase;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	///<summary>This window is used to set up Dyanmic Payment Plans.  These rules will be used by the OpenDentalService for making charges.</summary>
	public partial class FormPayPlanDynamic : FormODBase {
		#region Public Variables
		///<summary>Go to the specified patnum. If this number is not 0, patients.Cur will change to new patnum, and Account refreshed.</summary>
		public long PatNumGoto;
		#endregion
		#region Private Variables
		private DynamicPaymentPlanModuleData _dynamicPaymentPlanData=new DynamicPaymentPlanModuleData();
		private PayPlan _payPlanOld;
		private bool _isSigOldValid;
		private bool _isLoading=true;
		///<summary>List of procedures that should be attached to a pay plan if it is new.</summary>
		private List<Procedure>_listProceduresForNewPayPlan;
		///<summary>Will be false if pay plan was opened from another window</summary>
		bool _hasGoToButtons;
		#endregion
		#region Properties
		private decimal _sumAttachedProduction {
			get {
				return _dynamicPaymentPlanData.ListPayPlanProductionEntries.Sum(x => (x.AmountOverride==0 ? x.AmountOriginal : x.AmountOverride));
			}
		}
		#endregion

		///<summary></summary>
		public FormPayPlanDynamic(PayPlan payPlan,bool hasGoToButtons=true,List<Procedure> listProceduresForNewPayPlan=null) {
			InitializeComponent();
			InitializeLayoutManager();
			_dynamicPaymentPlanData=PayPlanEdit.GetDynamicPaymentPlanModuleData(payPlan.Copy());
			_payPlanOld=_dynamicPaymentPlanData.PayPlan.Copy();
			_hasGoToButtons=hasGoToButtons;
			_listProceduresForNewPayPlan=listProceduresForNewPayPlan;
			Lan.F(this);
		}

		private void FormPayPlanDynamic_Load(object sender,System.EventArgs e) {
			#region Set Data
			if(_dynamicPaymentPlanData.PayPlan.IsNew && !_listProceduresForNewPayPlan.IsNullOrEmpty()) {
				AddProcedures(_listProceduresForNewPayPlan);//Add procs to production list if pay plan is new.
			}
			#endregion
			#region Fill and Set UI Fields
			warningIntegrity1.SetTypeAndVisibility(EnumWarningIntegrityType.PayPlan,PayPlans.IsPayPlanHashValid(_dynamicPaymentPlanData.PayPlan));
			comboCategory.Items.AddDefNone();
			comboCategory.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PayPlanCategories,true));
			comboCategory.SetSelectedDefNum(_dynamicPaymentPlanData.PayPlan.PlanCategory); 
			textPatient.Text=Patients.GetLim(_dynamicPaymentPlanData.PayPlan.PatNum).GetNameLF();
			textGuarantor.Text=Patients.GetLim(_dynamicPaymentPlanData.PayPlan.Guarantor).GetNameLF();
			SetTermsFromDb();
			textAmtPaid.Text=_dynamicPaymentPlanData.AmountPaid.ToString("f");
			textCompletedAmt.Text=_dynamicPaymentPlanData.PayPlan.CompletedAmt.ToString("f");
			textTotalTxAmt.Text=_sumAttachedProduction.ToString("f");//possibly will not by in sync with total amount if using TP procs... 
			textNote.Text=_dynamicPaymentPlanData.PayPlan.Note;
			if(_dynamicPaymentPlanData.PayPlan.IsNew) {
				tabControl1.SelectedIndex=1;//Show the Attached Production page to the user first. 
				butDelete.Visible=false;
				butClosePlan.Visible=false;
				butUnlock.Visible=false;
				textTotalPrincipal.Text="";
				if(_sumAttachedProduction!=0) {//New plan may have attached production if created from ortho case.
					textTotalPrincipal.Text=_sumAttachedProduction.ToString("f");
				}
				butSave.Visible=false;
				butCancelTerms.Visible=false;
			}
			else {//already saved payment plan, user needs to unlock before they can edit anything.
				FillUiForSavedPayPlan();
			}
			if(_dynamicPaymentPlanData.PayPlan.IsClosed) {
				butOK.Text=Lan.g(this,"Reopen");
				butDelete.Enabled=false;
				butClosePlan.Enabled=false;
				labelClosed.Visible=true;
			}
			if(!_hasGoToButtons) {//Can't go to pat because another window (ex: OrthoCase) will still be open.
				butGoToGuar.Visible=false;
				butGoToPat.Visible=false;
			}
			checkExcludePast.Checked=PrefC.GetBool(PrefName.PayPlansExcludePastActivity);
			#endregion
			if(PrefC.GetBool(PrefName.PayPlansUseSheets)) {
				Sheet sheet=null;
				sheet=PayPlanToSheet(_dynamicPaymentPlanData.PayPlan);
				//check to see if sig box is on the sheet
				//hides butPrint and adds butSignPrint,groupbox,and sigwrapper
				for(int i = 0;i<sheet.SheetFields.Count;i++) {
					if(sheet.SheetFields[i].FieldType==SheetFieldType.SigBox) {
						butPrint.Visible=false;
						butSignPrint.Visible=true;					
					}
				}
			}
			List<PayPlan> listPayPlansOvercharged=PayPlans.GetOverChargedPayPlans(new List<long>{_dynamicPaymentPlanData.PayPlan.PayPlanNum});
			for(int i=0;i<listPayPlansOvercharged.Count;i++) {
				if(_dynamicPaymentPlanData.PayPlan.PayPlanNum==listPayPlansOvercharged[i].PayPlanNum) {
					labelOverchargedWarning.Visible=true;
				}
			}
			FillCharges();
			FillProduction();
			#region Signature
			if(_dynamicPaymentPlanData.PayPlan.Signature!="" && _dynamicPaymentPlanData.PayPlan.Signature!=null) {
				//check to see if sheet is signed before showing
				signatureBoxWrapper.Visible=true;
				groupBoxSignature.Visible=true;
				butSignPrint.Text="View && Print";
				signatureBoxWrapper.FillSignature(_dynamicPaymentPlanData.PayPlan.SigIsTopaz,PayPlans.GetKeyDataForSignature(_dynamicPaymentPlanData.PayPlan),_dynamicPaymentPlanData.PayPlan.Signature);//fill signature
			}
			if(string.IsNullOrEmpty(_dynamicPaymentPlanData.PayPlan.Signature) || !signatureBoxWrapper.IsValid || signatureBoxWrapper.SigIsBlank) {
				_isSigOldValid=false;
			}
			else {
				_isSigOldValid=true;
			}
			#endregion
			if(!Security.IsAuthorized(Permissions.PayPlanEdit)) {
				DisableAllExcept(butGoToGuar,butGoToPat,butCancel,butPrint,checkExcludePast,gridCharges,gridLinkedProduction);
				//allow grid so users can scroll, but de-register for event so charges cannot be modified. 
				this.gridCharges.CellDoubleClick-=gridCharges_CellDoubleClick;
				_isLoading=false;
				return;
			}
			ContextMenu contextMenuDynamicPaymentPlanCharges=new ContextMenu();
			contextMenuDynamicPaymentPlanCharges.MenuItems.Add(new MenuItem(Lan.g(this,"Delete"),new EventHandler(gridCharges_RightClickDelete)));
			this.gridCharges.ContextMenu=contextMenuDynamicPaymentPlanCharges;
			_isLoading=false;
		}

		private void LoadPayDataFromDB() {
			_dynamicPaymentPlanData=PayPlanEdit.GetDynamicPaymentPlanModuleData(_dynamicPaymentPlanData.PayPlan);
		}

		private void gridCharges_RightClickDelete(object sender,EventArgs e) {
			int index=gridCharges.GetSelectedIndex();
			if(index==-1) {//Should not happen, menu item is only enabled when exactly 1 row is selected.
				return;
			}
			DynamicPayPlanRowData rowData=(DynamicPayPlanRowData)gridCharges.ListGridRows[index].Tag;
			if(!rowData.IsChargeRow()) {
				return;
			}
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetMany(rowData.ListPayPlanChargeNums);
			List<long> listPayPlanChargeNums=listPayPlanCharges.Select(x => x.PayPlanChargeNum).ToList();
			if(listPayPlanChargeNums.All(x => x==0)) {
				MsgBox.Show(Lan.g(this,"The selected charge hasn't been posted yet. Only posted charges can be deleted."));
				return;
			}
			List<PaySplit> listPaySplitsForPayPlanCharges=PaySplits.GetForPayPlanCharges(listPayPlanChargeNums);
			if(listPaySplitsForPayPlanCharges.Count > 0) {
				MsgBox.Show(Lan.g(this,"The selected charge couldn't be deleted. Only a debit charge without a payment can be deleted."));
				return;
			}
			PayPlanCharges.DeleteDebitsWithoutPayments(listPayPlanCharges,doDelete:true);
			LoadPayDataFromDB();
			FillCharges();
			FillProduction();
		}

		private void FillUiForSavedPayPlan() {
			if(_dynamicPaymentPlanData.PayPlan.IsLocked) {
				butUnlock.Visible=false;
				checkProductionLock.Checked=true;
				checkProductionLock.Enabled=false;
				butChangeGuar.Visible=false;
				LockProduction();
			}
			if(_dynamicPaymentPlanData.ListPayPlanChargesDb.Count > 0) {
				textDateFirstPay.ReadOnly=true;
			}
			textDownPayment.ReadOnly=true;//users can only add or modify downpayment on new plans since they will get immediately inserted.
			groupBoxFrequency.Enabled=false;
			groupTerms.Enabled=false;
			textTotalPrincipal.Text=_sumAttachedProduction.ToString("f");
		}

		private void FillProduction() {
			gridLinkedProduction.BeginUpdate();
			gridLinkedProduction.Columns.Clear();
			int widthClinic=140;
			int widthDesc=(PrefC.HasClinicsEnabled ? 170 : 170 + widthClinic);
			GridColumn col;
			col=new GridColumn(Lan.g(gridLinkedProduction.TranslationName,"Date\r\nAdded"),70,HorizontalAlignment.Center);
			gridLinkedProduction.Columns.Add(col);
			col=new GridColumn(Lan.g(gridLinkedProduction.TranslationName,"Date"),70,HorizontalAlignment.Center);
			gridLinkedProduction.Columns.Add(col);
			col=new GridColumn(Lan.g(gridLinkedProduction.TranslationName,"Provider"),70);
			gridLinkedProduction.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(gridLinkedProduction.TranslationName,"Clinic"),widthClinic);
				gridLinkedProduction.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(gridLinkedProduction.TranslationName,"Description"),widthDesc);
			gridLinkedProduction.Columns.Add(col);
			col=new GridColumn(Lan.g(gridLinkedProduction.TranslationName,"Amount"),60,HorizontalAlignment.Right);//amount from production value.
			gridLinkedProduction.Columns.Add(col);
			col=new GridColumn(Lan.g(gridLinkedProduction.TranslationName,"Amount")+"\r\n"+Lan.g(gridLinkedProduction.TranslationName,"Override"),60,HorizontalAlignment.Right,true);
			if(checkProductionLock.Checked) {
				col.IsEditable = false;
			}
			gridLinkedProduction.Columns.Add(col);
			gridLinkedProduction.ListGridRows.Clear();
			List<GridRow> listChargeGridRow=PayPlanL.CreateGridRowsForProductionTab(_dynamicPaymentPlanData,showAttachedProductionAndIncome:checkShowAttachedPnI.Checked);
			gridLinkedProduction.ListGridRows.AddRange(listChargeGridRow);
			gridLinkedProduction.EndUpdate();
		}

		///<summary>Fills both charges that have come due (black in color) and expected charges (grey) that have not been added yet as well
		///as payments that have been made for the charges that have come due. Returns true if the method wasn't returned from early due to errors.
		///Returns false if TryGetTermsFromUI fails or terms.AreTermsValid fails.</summary>
		private bool FillCharges(bool isSilent=false) {
			if(!TryGetTermsFromUI(out PayPlanTerms terms,isSilent)) {
				return false;
			}
			gridCharges.BeginUpdate();
			gridCharges.Columns.Clear();
			GridColumn col;
			//If this column is changed from a date column then the comparer method (ComparePayPlanRows) needs to be updated.
			//If changes are made to the order of the grid, changes need to also be made for butPrint_Click
			col=new GridColumn(Lan.g("PayPlanAmortization","Date"),64,HorizontalAlignment.Center);//0
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Description"),147);//1
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Principal"),75,HorizontalAlignment.Right);//2
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Interest"),67,HorizontalAlignment.Right);//3
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Due"),75,HorizontalAlignment.Right);//4
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Payment"),75,HorizontalAlignment.Right);//5
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Balance"),70,HorizontalAlignment.Right);//6
			gridCharges.Columns.Add(col);
			gridCharges.ListGridRows.Clear();
			List<PayPlanCharge> listPayPlanChargesExpected=PayPlanEdit.GetPayPlanChargesForDynamicPaymentPlanSchedule(_dynamicPaymentPlanData.PayPlan,terms,_dynamicPaymentPlanData.ListPayPlanChargesDb,_dynamicPaymentPlanData.ListPayPlanLinks,_dynamicPaymentPlanData.ListPaySplits);
			_dynamicPaymentPlanData.ListPayPlanChargesExpected=listPayPlanChargesExpected;
			//The above method call will set the terms.AreTermsValid field.
			if(!terms.AreTermsValid) {
				if(isSilent) {
					MsgBox.Show("This payment plan will never be paid off. The interest is too high or the payment amount is too low.");
				}
				gridCharges.EndUpdate();
				return false;
			}
			DataTable tableBundledPayments=PaySplits.GetForPayPlan(_dynamicPaymentPlanData.PayPlan.PayPlanNum);
			List<GridRow> listRows=PayPlanL.CreateRowsForDynamicPayPlanCharges(listPayPlanChargesExpected, tableBundledPayments,ungrouped:checkUngrouped.Checked);
			int totalsRowIndex = -1; //if -1, then don't show a bold line as the first charge showing has not come due yet.
			#region Fill Sum Text
			double balanceAmt=0;
			double principalDue=0;
			_dynamicPaymentPlanData.TotalInterest=0;
			double totalPay=0;
			for(int i=0;i<listRows.Count;i++) {
				bool isFutureCharge=PIn.Date(listRows[i].Cells[0].Text)>DateTime.Today;
				if(!checkExcludePast.Checked || isFutureCharge) {
					//Add the row if we aren't excluding past activity or the activity is in the future.
					gridCharges.ListGridRows.Add(listRows[i]);
				}
				if(listRows[i].Cells[2].Text!="") {//Principal
					principalDue+=PIn.Double(listRows[i].Cells[2].Text);
					balanceAmt+=PIn.Double(listRows[i].Cells[2].Text);
				}
				if(listRows[i].Cells[3].Text!="") {//Interest
					_dynamicPaymentPlanData.TotalInterest+=PIn.Double(listRows[i].Cells[3].Text);
					balanceAmt+=PIn.Double(listRows[i].Cells[3].Text);
				}
				else if(listRows[i].Cells[5].Text!="") {//Payment
					totalPay+=PIn.Double(listRows[i].Cells[5].Text);
					balanceAmt-=PIn.Double(listRows[i].Cells[5].Text);
				}
				if(!checkExcludePast.Checked || isFutureCharge) {
					gridCharges.ListGridRows[gridCharges.ListGridRows.Count-1].Cells[6].Text=balanceAmt.ToString("f");
				}
				if(!isFutureCharge) {
					textPrincipalSum.Text=principalDue.ToString("f");
					textInterestSum.Text=_dynamicPaymentPlanData.TotalInterest.ToString("f");
					textDueSum.Text=(principalDue+_dynamicPaymentPlanData.TotalInterest).ToString("f");
					textPaymentSum.Text=totalPay.ToString("f");
					textBalanceSum.Text=balanceAmt.ToString("f");
					if(gridCharges.ListGridRows.Count>0) {
						totalsRowIndex=gridCharges.ListGridRows.Count-1;
					}
				}
			}
			//Show attached P&I checkbox only when we have one or more rows, otherwise it will do nothing.
			checkShowAttachedPnI.Visible=gridCharges.ListGridRows.Count>0;
			#endregion
			_dynamicPaymentPlanData.TotalInterest=0;
			for(int i=0;i<listPayPlanChargesExpected.Count;i++) {//combine with list expected.
				_dynamicPaymentPlanData.TotalInterest+=listPayPlanChargesExpected[i].Interest;
			}
			textTotalCost.Text=(_sumAttachedProduction+(decimal)_dynamicPaymentPlanData.TotalInterest).ToString("f");
			gridCharges.EndUpdate();
			if(gridCharges.ListGridRows.Count>0 && totalsRowIndex != -1) {
				gridCharges.ListGridRows[totalsRowIndex].ColorLborder=Color.Black;
				gridCharges.ListGridRows[totalsRowIndex].Cells[5].Bold=YN.Yes;
			}
			textAccumulatedDue.Text=PayPlans.GetAccumDue(_dynamicPaymentPlanData.PayPlan.PayPlanNum,_dynamicPaymentPlanData.ListPayPlanChargesDb).ToString("f");
			textPrincPaid.Text=PayPlans.GetPrincPaid(_dynamicPaymentPlanData.AmountPaid,_dynamicPaymentPlanData.PayPlan.PayPlanNum,_dynamicPaymentPlanData.ListPayPlanChargesDb).ToString("f");
			return true;
		}

		///<summary>Helper to get and store the UI elements so we do not need to pass in more than what is necessary. Set from the UI, not DB.</summary>
		private bool TryGetTermsFromUI(out PayPlanTerms payPlanTerms,bool isSilent=false,bool doValidateTerms=true) {
			payPlanTerms=new PayPlanTerms();
			if(!String.IsNullOrWhiteSpace(ValidateUI())) {
				return false;
			}
			if(doValidateTerms && !ValidateTerms(isSilent)) {//saveData relies on this, if removed from this method, needs to be addded to SaveData()
				return false;
			}
			payPlanTerms.APR=PIn.Double(textAPR.Text);
			payPlanTerms.DateFirstPayment=PIn.Date(textDateFirstPay.Text);
			payPlanTerms.Frequency=GetChargeFrequency();//verify this is just based on the ui, not the db.
			payPlanTerms.DynamicPayPlanTPOption=GetSelectedTreatmentPlannedOption();
			payPlanTerms.DateInterestStart=PIn.Date(textDateInterestStart.Text);//Will be DateTime.MinDate if field is blank.
			try {
				payPlanTerms.PayCount=PIn.Int(textPaymentCount.Text);
			}
			catch{
				if(!isSilent) {
					MsgBox.Show(this,"Payment Count invalid.");
				}
				return false;
			}
			payPlanTerms.PeriodPayment=PIn.Decimal(textPeriodPayment.Text);
			payPlanTerms.PrincipalAmount=PIn.Double(textTotalPrincipal.Text);
			payPlanTerms.RoundDec=CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
			payPlanTerms.DateAgreement=PIn.Date(textDate.Text);
			payPlanTerms.DownPayment=PIn.Double(textDownPayment.Text);
			payPlanTerms.PaySchedule=PayPlanEdit.GetPayScheduleFromFrequency(payPlanTerms.Frequency);
			//now that terms are set, we need to potentially calculate the periodpayment amount since we only store that and not the payCount
			if(payPlanTerms.PayCount!=0) {
				payPlanTerms.PeriodPayment=PayPlanEdit.CalculatePeriodPayment(payPlanTerms.APR,payPlanTerms.Frequency,payPlanTerms.PeriodPayment,payPlanTerms.PayCount,payPlanTerms.RoundDec
				,payPlanTerms.PrincipalAmount,payPlanTerms.DownPayment);
				payPlanTerms.PayCount=0;
			}
			payPlanTerms.Note=textNote.Text;
			return true;
		}

		///<summary>Resets the UI to what was loaded from the database.</summary>
		private void SetTermsFromDb() {
			textDateInterestStart.Text="";
			if(_dynamicPaymentPlanData.PayPlan.IsNew) {
				//If a plan is created "today" with the customer making their first payment on the spot, they will over pay interest.  
				//If there is a larger gap than 1 month before the first payment, interest will be under calculated.
				//Our temporary solution is to prefill the date of first payment box with next months date which is most accurate for calculating interest.
				textDateFirstPay.Text=DateTime.Now.AddMonths(1).ToShortDateString();
			}
			else {
				textDateFirstPay.Text=_dynamicPaymentPlanData.PayPlan.DatePayPlanStart.ToShortDateString();
				if(_dynamicPaymentPlanData.PayPlan.DateInterestStart.Year>=1880) {
					textDateInterestStart.Text=_dynamicPaymentPlanData.PayPlan.DateInterestStart.ToShortDateString();
				}
			}
			switch(_dynamicPaymentPlanData.PayPlan.DynamicPayPlanTPOption) {
				case DynamicPayPlanTPOptions.TreatAsComplete:
					radioTpTreatAsComplete.Checked=true;
					break;
				case DynamicPayPlanTPOptions.AwaitComplete:
				default:
					radioTpAwaitComplete.Checked=true;
					break;
			}
			textInterestDelay.Text="";
			textPeriodPayment.Text=_dynamicPaymentPlanData.PayPlan.PayAmt.ToString("f");//we only save the amount, not the # of payments for dynamic payment plans
			textPaymentCount.Text="";
			textDownPayment.Text=_dynamicPaymentPlanData.PayPlan.DownPayment.ToString("f");
			textDate.Text=_dynamicPaymentPlanData.PayPlan.PayPlanDate.ToShortDateString();
			textAPR.Text=_dynamicPaymentPlanData.PayPlan.APR.ToString();
			ToggleInterestDelayFieldsHelper();
			switch(_dynamicPaymentPlanData.PayPlan.ChargeFrequency) {
				case PayPlanFrequency.Weekly:
					radioWeekly.Checked=true;
					break;
				case PayPlanFrequency.EveryOtherWeek:
					radioEveryOtherWeek.Checked=true;
					break;
				case PayPlanFrequency.OrdinalWeekday:
					radioOrdinalWeekday.Checked=true;
					break;
				case PayPlanFrequency.Monthly:
					radioMonthly.Checked=true;
					break;
				case PayPlanFrequency.Quarterly:
					radioQuarterly.Checked=true;
					break;
				default://default to monthly for new plans (should be 0 and do this regardless)
					radioMonthly.Checked=true;
					break;
			}
		}

		private void ToggleInterestDelayFieldsHelper() {
			bool areVisible=true;
			if(CompareDouble.IsZero(PIn.Double(textAPR.Text))) {
				textDateInterestStart.Text="";
				textInterestDelay.Text="";
				areVisible=false;
			}
			textDateInterestStart.Visible=areVisible;
			textInterestDelay.Visible=areVisible;
			labelInterestDelay1.Visible=areVisible;
			labelInterestDelay2.Visible=areVisible;
			labelDateInterestStart.Visible=areVisible;
		}

		///<summary>Unlocks a saved payment plan for editing. Allows users to try out new terms which will not be saved unless user specifies.</summary>
		private void ButUnlock_Click(object sender,EventArgs e) {
			UnlockTerms();
		}

		private void ButSave_Click(object sender,EventArgs e) {
			LockTerms(true,FillCharges());
		}

		///<summary>Sets UI elements back to what they were before unlocking (change back to what the window loaded with originally)</summary>
		private void ButCancelTerms_Click(object sender,EventArgs e) {
			SetTermsFromDb();
			LockTerms(false,FillCharges());//re-lock. Nothing is getting saved though since nothing changed. 
		}

		///<summary></summary>
		private void LockTerms(bool isSaveData,bool isUiValid=true) {
			if(isSaveData) {
				if(SaveData(isUiValid:isUiValid)) {
					groupTerms.Enabled=false;
					groupBoxFrequency.Enabled=false;
				}
				else {
					//failed saving. Return user to form.
					return;
				}
			}
			groupTerms.Enabled=false;
			groupBoxFrequency.Enabled=false;
			if(!_dynamicPaymentPlanData.PayPlan.IsLocked || _dynamicPaymentPlanData.PayPlan.IsNew) {
				butUnlock.Visible=true;
			}
		}

		///<summary></summary>
		private void UnlockTerms() {
			groupTerms.Enabled=true;
			groupBoxFrequency.Enabled=true;
			butUnlock.Visible=false;
		}

		///<summary>different then locking the terms. This checkbox should lock the entire payment plan down. No changing terms, no adding production.</summary>
		private void LockProduction() {
			LockTerms(false);
			butAddProd.Enabled=false;
			butDeleteProduction.Enabled=false;
		}

		///<summary>allowed for user to change their mind on new payment plans. They will be stopped during validation if they have APR (required to be locked)</summary>
		private void UnlockProduction() {
			butAddProd.Enabled=true;
			butDeleteProduction.Enabled=true;
			UnlockTerms();
		}

		private void CheckProductionLock_CheckedChanged(object sender,EventArgs e) {
			if(!checkProductionLock.Checked && _dynamicPaymentPlanData.PayPlan.IsNew) {
				UnlockProduction();
			}
			else if(checkProductionLock.Checked) {
				LockProduction();
				if(!_isLoading) {
					if(CreateSchedule()) {//create schedule button will get disabled once this box is checked so we need to do this for the user 
						butAddProd.Enabled=false;
						butDeleteProduction.Enabled=false;
					}
					else {
						//terms were not correctly validated, schedule was not able to be set so uncheck the box for the user to make edits.
						checkProductionLock.Checked=false;
					}
				}
			}
			FillProduction();
		}

		///<summary>Creates pay plan links and pay plan production entries for the list of procs passed in.</summary>
		private void AddProcedures(List<Procedure> listProcs) {
			int countSkipped=0;
			List<long> listProcNums=listProcs.Select(x => x.ProcNum).ToList();
			List<PayPlanCharge> listPayPlanChargesCreditsForProcs=PayPlanCharges.GetPatientPayPlanCreditsForProcs(listProcNums);
			List<PayPlanLink> listPayPlanLinksForProcs=PayPlanLinks.GetForFKeysAndLinkType(listProcNums,PayPlanLinkType.Procedure);
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(_dynamicPaymentPlanData.Family.GetPatNums(),_dynamicPaymentPlanData.Patient.PatNum,
				new List<PaySplit>(),new Payment(),new List<AccountEntry>(),isIncomeTxfr:true);
			for(int i=0;i<listProcs.Count;i++) {
				if(listPayPlanChargesCreditsForProcs.Select(x => x.ProcNum).Contains(listProcs[i].ProcNum)
					|| listPayPlanLinksForProcs.Select(x => x.FKey).Contains(listProcs[i].ProcNum)
					|| _dynamicPaymentPlanData.ListPayPlanLinks.Exists(x => x.LinkType==PayPlanLinkType.Procedure && x.FKey==listProcs[i].ProcNum))
				{
					countSkipped++;
					continue;
				}
				AccountEntry accountEntry=constructResults.ListAccountEntries.FirstOrDefault(x => x.GetType()==typeof(Procedure) && x.PriKey==listProcs[i].ProcNum);
				if(accountEntry==null || accountEntry.AmountEnd==0) {
					continue;//Don't warn the user because there just isn't any value for the specified procedure.
				}
				PayPlanLink payPlanLinkCreditAdding=new PayPlanLink() {
					PayPlanNum=_dynamicPaymentPlanData.PayPlan.PayPlanNum,
					LinkType=PayPlanLinkType.Procedure,
					FKey=listProcs[i].ProcNum
				};
				_dynamicPaymentPlanData.ListPayPlanLinks.Add(payPlanLinkCreditAdding);
				_dynamicPaymentPlanData.ListPayPlanProductionEntries.Add(new PayPlanProductionEntry(listProcs[i],payPlanLinkCreditAdding,null,null,null,amountOriginal:accountEntry.AmountEnd));
			}
			textTotalPrincipal.Text=_sumAttachedProduction.ToString("f");
			FillProduction();
			if(countSkipped>0) {
				MsgBox.Show(this,"Procedures can only be attached to one payment plan at a time.");
			}
		}

		private void butAddProd_Click(object sender,EventArgs e) {
			List<AccountEntry> listAccountEntriesUnattached=PayPlanEdit.GetAccountEntriesUnattachedToDynamicPaymentPlan(_dynamicPaymentPlanData);
			using FormProdSelect formProdSelect=new FormProdSelect(_dynamicPaymentPlanData.Patient,listAccountEntriesUnattached);
			if(formProdSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(formProdSelect.ListAccountEntriesSelected.IsNullOrEmpty()) {
				return;
			}
			List<AccountEntry> listAccountEntriesSelected=formProdSelect.ListAccountEntriesSelected;
			_dynamicPaymentPlanData=PayPlanEdit.AddProductionToDynamicPaymentPlan(formProdSelect.ListAccountEntriesSelected,_dynamicPaymentPlanData);
			textTotalPrincipal.Text=_sumAttachedProduction.ToString("f");
			FillProduction();
		}

		private void ButDeleteProduction_Click(object sender,EventArgs e) {
			if(gridLinkedProduction.SelectedIndices.Count() <= 0) {
				MsgBox.Show(this,"Please select an item from the grid to remove.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected item will be permanently deleted from the Dynamic Payment Plan.\r\n"
				+"This cannot be undone. Continue?"))
			{
				return;
			}
			List<PayPlanProductionEntry> listPayPlanProductionEntriesSelected=gridLinkedProduction.SelectedTags<PayPlanProductionEntry>();
			_dynamicPaymentPlanData=PayPlanEdit.RemoveProductionFromDynamicPaymentPlan(listPayPlanProductionEntriesSelected,_dynamicPaymentPlanData);
			if(_dynamicPaymentPlanData.ListPayPlanProductionEntries.Exists(x=>listPayPlanProductionEntriesSelected.Any(y=>x.PriKey==y.PriKey && x.LinkType==y.LinkType))) {
				MsgBox.Show(this,"Some production was not able to be removed due to having patient payments attached. Remove those first.");
			}
			textTotalPrincipal.Text=_sumAttachedProduction.ToString("f");
			FillProduction();
			FillCharges();
		}

		private DynamicPayPlanTPOptions GetSelectedTreatmentPlannedOption() {
			if(radioTpAwaitComplete.Checked) {
				return DynamicPayPlanTPOptions.AwaitComplete;
			}
			if(radioTpTreatAsComplete.Checked) {
				return DynamicPayPlanTPOptions.TreatAsComplete;
			}
			return DynamicPayPlanTPOptions.None;//Should never be hit
		}

		private void butGoToPat_Click(object sender,System.EventArgs e) {
			GoToHelper(false);
		}

		private void butGoTo_Click(object sender,System.EventArgs e) {
			GoToHelper(true);
		}

		private void GoToHelper(bool isGuarantor) {
			if(!SaveData()) {
				return;
			}
			PatNumGoto=isGuarantor?_dynamicPaymentPlanData.PayPlan.Guarantor:_dynamicPaymentPlanData.PayPlan.PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butChangeGuar_Click(object sender,System.EventArgs e) {
			if(PayPlans.GetAmtPaid(_dynamicPaymentPlanData.PayPlan)!=0) {
				MsgBox.Show(this,"Not allowed to change the guarantor because payments are attached.");
				return;
			}
			if(_dynamicPaymentPlanData.ListPayPlanChargesDb.Count>0) {
				MsgBox.Show(this,"Not allowed to change the guarantor when charges have been created.");
				return;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_dynamicPaymentPlanData.PayPlan.Guarantor=formPatientSelect.PatNumSelected;
			textGuarantor.Text=Patients.GetLim(_dynamicPaymentPlanData.PayPlan.Guarantor).GetNameLF();
			FillCharges(isSilent:true);//Don't warn the user if the UI is in an invalid state. Odds are this is the first item they are changing.
		}
		
		private void textAmount_Validating(object sender,CancelEventArgs e) {
			if(textCompletedAmt.Text=="") {
				return;
			}
			if(PIn.Double(textCompletedAmt.Text)==PIn.Double(textTotalPrincipal.Text)) {
				return;
			}
		}

		private void TextInterestDelay_KeyPress(object sender,KeyPressEventArgs e) {
			textDateInterestStart.Text="";
		}

		private void TextDateInterestStart_KeyPress(object sender,KeyPressEventArgs e) {
			textInterestDelay.Text="";
		}

		private void TextAPR_TextChanged(object sender,EventArgs e) {
			ToggleInterestDelayFieldsHelper();
		}

		private void textPaymentCount_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e) {
			textPeriodPayment.Text="";
		}

		private void textPeriodPayment_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e) {
			textPaymentCount.Text="";
		}

		private void butCreateSched_Click(object sender,System.EventArgs e) {
			CreateSchedule();
			tabControl1.SelectedIndex=0;//flip back to charges tab
		}

		///<summary>Goes through the logic to create a new schedule. Returns true if a terms were successfully validated and correct.</summary>
		private bool CreateSchedule() {
			if(ValidateTerms(doCheckAPR:false)) {//Don't need to validate APR and full lock because we will auto-lock when APR is set (only for creating schedule).
				if(textAPR.IsValid() && !CompareDouble.IsZero(PIn.Double(textAPR.Text)) && checkProductionLock.Checked==false) {
					checkProductionLock.Checked=true;
				}
				CalculateDateInterestStartFromInterestDelay();
				FillCharges();
				SetNote();
				textCompletedAmt.Text=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(_dynamicPaymentPlanData.PayPlan,_dynamicPaymentPlanData.ListPayPlanProductionEntries).ToString("f");
				signatureBoxWrapper.FillSignature(_dynamicPaymentPlanData.PayPlan.SigIsTopaz,PayPlans.GetKeyDataForSignature(_dynamicPaymentPlanData.PayPlan),_dynamicPaymentPlanData.PayPlan.Signature);
				return true;
			}
			return false;
		}

		private bool ValidateTerms(bool isSilent=false,bool doCheckAPR=true) {
			if(_isLoading) {
				return true;
			}
			StringBuilder stringBuilderErrors=new StringBuilder();
			//Validate UI fields.
			stringBuilderErrors.Append(ValidateUI());
			//Create a temporary PayPlanTerms object for validation. Do validate must be false to prevent recursion.
			if(TryGetTermsFromUI(out PayPlanTerms payPlanTerms,doValidateTerms: false)) {
				//Validate the resultant PayPlanTerms object.
				stringBuilderErrors.AppendLine(PayPlanEdit.ValidateDynamicPaymentPlanTerms(payPlanTerms,_dynamicPaymentPlanData.PayPlan.IsNew,checkProductionLock.Checked,doCheckAPR,gridLinkedProduction.ListGridRows.Count));
			}
			//Check for any errors.
			if(!String.IsNullOrWhiteSpace(stringBuilderErrors.ToString())) {
				if(!isSilent){
					MessageBox.Show(stringBuilderErrors.ToString());
				}
				return false;
			}
			return true;
		}

		private string ValidateUI() {
			//Validate UI fields.
			StringBuilder stringBuilderErrors=new StringBuilder();
			if(!textDate.IsValid()
				|| !textDateFirstPay.IsValid()
				|| !textDownPayment.IsValid()
				|| !textAPR.IsValid()
				|| !textInterestDelay.IsValid()
				|| !textDateInterestStart.IsValid()
				|| !textPaymentCount.IsValid()
				|| !textCompletedAmt.IsValid()) 
			{
				stringBuilderErrors.AppendLine(Lan.g(this,"Please fix data entry errors first."));
			}
			if(!textPeriodPayment.IsValid() && gridLinkedProduction.ListGridRows.Count!=0) {
				stringBuilderErrors.AppendLine(Lan.g(this,"Please fix data entry errors first."));
			}
			return stringBuilderErrors.ToString();
		}

		///<summary>Calculates the interest start date and assigns it to the UI field, then returns the interest start date.</summary>
		private void CalculateDateInterestStartFromInterestDelay() {
			if(PIn.Int(textInterestDelay.Text,false)!=0) {
				textDateInterestStart.Text=PayPlanEdit.CalcNextPeriodDate(PIn.Date(textDateFirstPay.Text),PIn.Int(textInterestDelay.Text,false),
					GetChargeFrequency()).ToShortDateString();
				textInterestDelay.Text="";
			}
		}

		private void gridCharges_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			if(gridCharges.ListGridRows[e.Row].Tag==null) {//Prevent double clicking on the "Current Totals" row
				return;
			}
			DynamicPayPlanRowData rowData=(DynamicPayPlanRowData)gridCharges.ListGridRows[e.Row].Tag;
			if(rowData.IsChargeRow()) {
				//don't do anything. Dynamic payment plan charges are not editable. 
			}
			else if(rowData.IsPaymentRow()) {
				Payment payment = Payments.GetPayment(rowData.PayNum);
				if(payment==null) {
					MessageBox.Show(Lans.g(this,"No payment exists.  Please run database maintenance method")+" "+nameof(DatabaseMaintenances.PaySplitWithInvalidPayNum));
					return;
				}
				using FormPayment formPayment=new FormPayment(_dynamicPaymentPlanData.Patient,_dynamicPaymentPlanData.Family,payment,false);//FormPayment may insert and/or update the paysplits. 
				formPayment.IsNew=false;
				formPayment.ShowDialog();
				if(formPayment.DialogResult==DialogResult.Cancel) {
					return;
				}
				if(formPayment.DialogResult==DialogResult.OK) {//Get changes to the payment from DB
					_dynamicPaymentPlanData.ListPayPlanChargesDb=PayPlanCharges.GetForPayPlan(_dynamicPaymentPlanData.PayPlan.PayPlanNum).OrderBy(x => x.ChargeDate).ToList();
				}
			}
			else if(gridCharges.ListGridRows[e.Row].Tag.GetType()==typeof(DataRow)) {//Claim payment or bundle.
				DataRow rowBundledClaimProc=(DataRow)gridCharges.ListGridRows[e.Row].Tag;
				Claim claim=Claims.GetClaim(PIn.Long(rowBundledClaimProc["ClaimNum"].ToString()));
				if(claim==null) {
					MsgBox.Show(this,"The claim has been deleted.");
				}
				else {
					if(!Security.IsAuthorized(Permissions.ClaimView)) {
						return;
					}
					//FormClaimEdit inserts and/or updates the claim and/or claimprocs, which could potentially change the bundle.
					using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,_dynamicPaymentPlanData.Patient,_dynamicPaymentPlanData.Family);
					formClaimEdit.IsNew=false;
					formClaimEdit.ShowDialog();
					//Cancel from FormClaimEdit does not cancel payment edits, fill grid every time
				}
			}
			FillCharges();
		}

		private void gridLinkedProduction_CellLeave(object sender,ODGridClickEventArgs e) {
			if(checkProductionLock.Checked) {
				FillProduction();//Show the user that their changes were not saved.
				return;
			}
			if(gridLinkedProduction.ListGridRows[e.Row].Tag==null) {
				return;
			}
			PayPlanProductionEntry payPlanProductionEntry=(PayPlanProductionEntry)gridLinkedProduction.ListGridRows[e.Row].Tag;
			decimal overrideVal=PIn.Decimal(gridLinkedProduction.ListGridRows[e.Row].Cells[e.Col].Text);//if zero, attempting to remove override if set.
			SetOverride(payPlanProductionEntry,overrideVal);
		}

		private void SetOverride(PayPlanProductionEntry payPlanProductionEntry,decimal overrideVal) {
			payPlanProductionEntry.AmountOverride=overrideVal;
			payPlanProductionEntry.LinkedCredit.AmountOverride=(double)payPlanProductionEntry.AmountOverride;
			textTotalPrincipal.Text=_sumAttachedProduction.ToString("f");
			FillProduction();
			FillCharges();
		}

		private void butSignPrint_Click(object sender,EventArgs e) {
			if(!SaveData()) {
				return;
			}
			Sheet sheet=null;
			sheet=PayPlanToSheet(_dynamicPaymentPlanData.PayPlan);
			string keyData=PayPlans.GetKeyDataForSignature(_dynamicPaymentPlanData.PayPlan);
			SheetParameter.SetParameter(sheet,"keyData",keyData);
			SheetUtil.CalculateHeights(sheet);
			using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(sheet);
			formSheetFillEdit.ShowDialog();
			if(formSheetFillEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			//save signature
			if(_dynamicPaymentPlanData.PayPlan.Signature=="") {//clear signature and hide sigbox if blank sig was saved
				signatureBoxWrapper.ClearSignature();
				butSignPrint.Text="Sign && Print";
				signatureBoxWrapper.Visible=false;
				groupBoxSignature.Visible=false;
			}
			else {
				signatureBoxWrapper.Visible=true;//show after PP has been signed for the first time
				groupBoxSignature.Visible=true;
				butSignPrint.Text="View && Print";
				signatureBoxWrapper.FillSignature(_dynamicPaymentPlanData.PayPlan.SigIsTopaz,keyData,_dynamicPaymentPlanData.PayPlan.Signature); //fill signature on form
			}
		}

		private void ButPrintProduction_Click(object sender,EventArgs e) {
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Attached PayPlan Production printed"),PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			int pagesPrinted=0; 
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			bool headingPrinted=false;
			int headingPrintH=0;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Payment Plan Credits");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				text=DateTime.Today.ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				text=Patients.GetNameFLnoPref(_dynamicPaymentPlanData.Patient.LName,_dynamicPaymentPlanData.Patient.FName,"");
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridLinkedProduction.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
				text=Lan.g(this,"Total")+": "+_sumAttachedProduction.ToString("c");
				g.DrawString(text,fontSubHeading,Brushes.Black,center+gridLinkedProduction.Width/2-g.MeasureString(text,fontSubHeading).Width-10,yPos);
			}
		}

		private void butPrint_Click(object sender,System.EventArgs e) {
			if(!ValidateTerms() || !TryGetTermsFromUI(out PayPlanTerms terms)) {
				return;
			} 
			if(PrefC.GetBool(PrefName.PayPlansUseSheets)) {
				Sheet sheet=null;
				sheet=PayPlanToSheet(_dynamicPaymentPlanData.PayPlan);
				SheetPrinting.Print(sheet);
			}
			else {
				using Font font=new Font("Tahoma",9);
				using Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
				using Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
				using Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
				ReportComplex reportComplex=new ReportComplex(false,false);
				reportComplex.AddTitle("Title",Lan.g(this,"Payment Plan Terms"),fontTitle);
				reportComplex.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
				reportComplex.AddSubTitle("Date SubTitle",DateTime.Today.ToShortDateString(),fontSubTitle);
				AreaSectionType sectType=AreaSectionType.ReportHeader;
				Section section=reportComplex.Sections[AreaSectionType.ReportHeader];
				//int sectIndex=report.Sections.GetIndexOfKind(AreaSectionKind.ReportHeader);
				Size size=new Size(300,20);//big enough for any text
				ContentAlignment alignL=ContentAlignment.MiddleLeft;
				ContentAlignment alignR=ContentAlignment.MiddleRight;
				int yPos=140;
				int space=30;
				int x1=175;
				int x2=275;
				reportComplex.ReportObjects.Add(new ReportObject
					("Patient Title",sectType,new Point(x1,yPos),size,"Patient",font,alignL));
				reportComplex.ReportObjects.Add(new ReportObject
					("Patient Detail",sectType,new Point(x2,yPos),size,textPatient.Text,font,alignR));
				yPos+=space;
				reportComplex.ReportObjects.Add(new ReportObject
					("Guarantor Title",sectType,new Point(x1,yPos),size,"Guarantor",font,alignL));
				reportComplex.ReportObjects.Add(new ReportObject
					("Guarantor Detail",sectType,new Point(x2,yPos),size,textGuarantor.Text,font,alignR));
				yPos+=space;
				reportComplex.ReportObjects.Add(new ReportObject
					("Date of Agreement Title",sectType,new Point(x1,yPos),size,"Date of Agreement",font,alignL));
				reportComplex.ReportObjects.Add(new ReportObject
					("Date of Agreement Detail",sectType,new Point(x2,yPos),size,_dynamicPaymentPlanData.PayPlan.PayPlanDate.ToString("d"),font,alignR));
				yPos+=space;
				reportComplex.ReportObjects.Add(new ReportObject
					("Principal Title",sectType,new Point(x1,yPos),size,"Principal",font,alignL));
				reportComplex.ReportObjects.Add(new ReportObject
					("Principal Detail",sectType,new Point(x2,yPos),size,_sumAttachedProduction.ToString("n"),font,alignR));
				yPos+=space;
				reportComplex.ReportObjects.Add(new ReportObject
					("Annual Percentage Rate Title",sectType,new Point(x1,yPos),size,"Annual Percentage Rate",font,alignL));
				reportComplex.ReportObjects.Add(new ReportObject
					("Annual Percentage Rate Detail",sectType,new Point(x2,yPos),size,_dynamicPaymentPlanData.PayPlan.APR.ToString("f1"),font,alignR));
				yPos+=space;
				reportComplex.ReportObjects.Add(new ReportObject
					("Total Finance Charges Title",sectType,new Point(x1,yPos),size,"Total Finance Charges",font,alignL));
				reportComplex.ReportObjects.Add(new ReportObject
					("Total Finance Charges Detail",sectType,new Point(x2,yPos),size,_dynamicPaymentPlanData.TotalInterest.ToString("n"),font,alignR));
				yPos+=space;
				reportComplex.ReportObjects.Add(new ReportObject
					("Total Cost of Loan Title",sectType,new Point(x1,yPos),size,"Total Cost of Loan",font,alignL));
				reportComplex.ReportObjects.Add(new ReportObject
					("Total Cost of Loan Detail",sectType,new Point(x2,yPos),size,(_sumAttachedProduction+(decimal)_dynamicPaymentPlanData.TotalInterest).ToString("n"),font,alignR));
				yPos+=space;
				section.Height=yPos+30;
				List<PayPlanCharge> listExpectedPayPlanChargesAwaitingCompletion=new List<PayPlanCharge>();
				if(terms.DynamicPayPlanTPOption==DynamicPayPlanTPOptions.AwaitComplete) {
					List<PayPlanCharge> listPayPlanChargesExpectedDownPayment=PayPlanEdit.GetDownPaymentCharges(_dynamicPaymentPlanData.PayPlan,terms,_dynamicPaymentPlanData.ListPayPlanLinks);
					listExpectedPayPlanChargesAwaitingCompletion=PayPlanEdit.GetListExpectedChargesAwaitingCompletion(_dynamicPaymentPlanData.ListPayPlanChargesDb,terms,_dynamicPaymentPlanData.Family,_dynamicPaymentPlanData.ListPayPlanLinks,
						_dynamicPaymentPlanData.PayPlan,isNextPeriodOnly:false,listPaySplits:_dynamicPaymentPlanData.ListPaySplits,
						listExpectedChargesDownPayment:listPayPlanChargesExpectedDownPayment);
				}
				DataTable tableBundledPayments=PaySplits.GetForPayPlan(_dynamicPaymentPlanData.PayPlan.PayPlanNum);
				DataTable table=new DataTable();
				table.Columns.Add("date");
				if(listExpectedPayPlanChargesAwaitingCompletion.Count>0) {
					table.Columns.Add("prov");
				}
				table.Columns.Add("description");
				table.Columns.Add("principal");
				table.Columns.Add("interest");
				table.Columns.Add("due");
				table.Columns.Add("payment");
				table.Columns.Add("balance");
				DataRow row;
				if(!checkUngrouped.Checked) { 
					for(int i = 0;i<gridCharges.ListGridRows.Count;i++) {
						row=table.NewRow();
						row["date"]=gridCharges.ListGridRows[i].Cells[0].Text;
						if(listExpectedPayPlanChargesAwaitingCompletion.Count>0) {
							row["prov"]="";
						}
						row["description"]=gridCharges.ListGridRows[i].Cells[1].Text;
						row["principal"]=gridCharges.ListGridRows[i].Cells[2].Text;
						row["interest"]=gridCharges.ListGridRows[i].Cells[3].Text;
						row["due"]=gridCharges.ListGridRows[i].Cells[4].Text;
						row["payment"]=gridCharges.ListGridRows[i].Cells[5].Text;
						row["balance"]=gridCharges.ListGridRows[i].Cells[6].Text;
						table.Rows.Add(row);
					}
				}
				else {//This is so that when the user prints, it will only print the merged rows and not the expanded rows. This may change in the future. 
					List<GridRow> listRows=PayPlanL.CreateRowsForDynamicPayPlanCharges(_dynamicPaymentPlanData.ListPayPlanChargesExpected,tableBundledPayments,ungrouped:false);
					for(int i=0;i<listRows.Count;i++) {
						row=table.NewRow();
						row["date"]=listRows[i].Cells[0].Text;
						if(listExpectedPayPlanChargesAwaitingCompletion.Count>0) {
							row["prov"]="";
						}
						row["description"]=listRows[i].Cells[1].Text;
						row["principal"]=listRows[i].Cells[2].Text;
						row["interest"]=listRows[i].Cells[3].Text;
						row["due"]=listRows[i].Cells[4].Text;
						row["payment"]=listRows[i].Cells[5].Text;
						row["balance"]=listRows[i].Cells[6].Text;
						table.Rows.Add(row);
					}
				}
				string finalBalance="";
				if(gridCharges.ListGridRows.IsNullOrEmpty()) {
					finalBalance="0.00";
				}
				else {
					finalBalance=gridCharges.ListGridRows[gridCharges.ListGridRows.Count-1].Cells[6].Text;
				}
				for(int i = 0;i<listExpectedPayPlanChargesAwaitingCompletion.Count;i++) {
					GridRow rowForCharge=PayPlanL.CreateRowForPayPlanCharge(listExpectedPayPlanChargesAwaitingCompletion[i],0,isDynamic:true);
					row=table.NewRow();
					row["date"]="TBD";
					row["prov"]=rowForCharge.Cells[1].Text;
					row["description"]=Lans.g(this,"Planned - ")+rowForCharge.Cells[2].Text;
					row["principal"]=rowForCharge.Cells[3].Text;
					row["interest"]="";
					row["due"]="";
					row["payment"]="";
					row["balance"]=finalBalance;
					table.Rows.Add(row);
				}
				QueryObject queryObject=reportComplex.AddQuery(table,"","",SplitByKind.None,1,true);
				queryObject.AddColumn("ChargeDate",80,FieldValueType.String,font);
				queryObject.GetColumnHeader("ChargeDate").StaticText="Date";
				if(listExpectedPayPlanChargesAwaitingCompletion.Count>0) {
					queryObject.AddColumn("Provider",75,FieldValueType.String,font);
				}
				queryObject.AddColumn("Description",130,FieldValueType.String,font);
				queryObject.GetColumnDetail("Description").ContentAlignment=ContentAlignment.MiddleLeft;
				queryObject.AddColumn("Principal",70,FieldValueType.Number,font);
				queryObject.AddColumn("Interest",52,FieldValueType.Number,font);
				queryObject.AddColumn("Due",70,FieldValueType.Number,font);
				queryObject.AddColumn("Payment",70,FieldValueType.Number,font);
				queryObject.AddColumn("Balance",70,FieldValueType.String,font);
				queryObject.GetColumnHeader("Balance").ContentAlignment=ContentAlignment.MiddleRight;
				queryObject.GetColumnDetail("Balance").ContentAlignment=ContentAlignment.MiddleRight;
				reportComplex.ReportObjects.Add(new ReportObject("Note",AreaSectionType.ReportFooter,new Point(x1,20),new Size(500,200),textNote.Text,font,ContentAlignment.TopLeft));
				reportComplex.ReportObjects.Add(new ReportObject("Signature",AreaSectionType.ReportFooter,new Point(x1,220),new Size(500,20),"Signature of Guarantor: ____________________________________________",font,alignL));
				if(!reportComplex.SubmitQueries()) {
					return;
				}
				using FormReportComplex formReportComplex=new FormReportComplex(reportComplex);
				formReportComplex.ShowDialog();
			}
		}

		private void butCloseOut_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Closing out this payment plan will remove interest from all future charges "
				+"and make them due immediately.  Do you want to continue?"))
			{
				return;
			}
			if(PayPlans.GetOne(_dynamicPaymentPlanData.PayPlan.PayPlanNum)==null) {
				//The payment plan no longer exists in the database. 
				MsgBox.Show(this,"This payment plan has been deleted by another user.");
				return;
			}
			if(!TryGetTermsFromUI(out PayPlanTerms terms)) {//also validates terms
				return ;
			}
			if(!FillCharges()) {
				return;
			}
			_dynamicPaymentPlanData=PayPlanEdit.CloseOutDynamicPaymentPlan(
				terms,
				_dynamicPaymentPlanData,
				checkProductionLock.Checked,
				comboCategory.GetSelectedDefNum());
			PayPlanL.MakeSecLogEntries(_dynamicPaymentPlanData.PayPlan,_payPlanOld,signatureBoxWrapper.GetSigChanged(),
			_isSigOldValid,signatureBoxWrapper.SigIsBlank,signatureBoxWrapper.IsValid,false);
			DialogResult=DialogResult.OK;
		}

		private PayPlanFrequency GetChargeFrequency() {
			if(radioWeekly.Checked) {
				return PayPlanFrequency.Weekly;
			}
			if(radioEveryOtherWeek.Checked) {
				return PayPlanFrequency.EveryOtherWeek;
			}
			if(radioOrdinalWeekday.Checked) {
				return PayPlanFrequency.OrdinalWeekday;
			}
			if(radioMonthly.Checked) {
				return PayPlanFrequency.Monthly;
			}
			return PayPlanFrequency.Quarterly;
		}

		private void butSendToDevice_Click(object sender,EventArgs e) {
			if(!MobileAppDevices.IsClinicSignedUpForEClipboard(Clinics.ClinicNum)) {
				MsgBox.Show("Please enable eClipboard for this clinic to use this feature.");
				return;
			}
			if(!ValidateTerms()) {
					return;
			}
			SaveData();
			//The sheet that the practice uses for payment plans needs to have a signature box on it, otherwise the signature won't be
			//visible after signing. 
			if(SheetDefs.GetInternalOrCustom(SheetInternalType.PaymentPlan).SheetFieldDefs.FirstOrDefault(
				x => x.FieldType==SheetFieldType.SigBox)==null) 
			{
				MsgBox.Show(this,"Please add a signature field to your pay plan sheet.");
				return;
			}
			if(_dynamicPaymentPlanData.Patient==null){
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			if(_dynamicPaymentPlanData.PayPlan==null) {
				MsgBox.Show(this,"Please select a payment plan to send.");
				return;
			}
			else if(!string.IsNullOrEmpty(_dynamicPaymentPlanData.PayPlan.Signature)) {
				MsgBox.Show(this,"This Payment Plan has already been signed.");
				return;
			}
			if(MobileAppDevices.ShouldSendPush(_dynamicPaymentPlanData.Patient.PatNum, out MobileAppDevice device)) {
				PushSelectedPayPlanToEclipboard(device);
			}
			else {
				OpenUnlockCodeForPayPlan();
			}
			//The form needs to close, because if a patient signs a payment plan while FormPayPlan is open, and then someone in OD
			//clicks OK, then the signature gets blown away. 
			Close();
		}

		/// <summary>This will enable/disable the delete production button depending on the cell type.</summary>
		private void gridLinkedProduction_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridLinkedProduction.ListGridRows[e.Row].Tag.GetType()==typeof(PayPlanProductionEntry)) {
				butDeleteProduction.Enabled=true;
			}
			else {
				butDeleteProduction.Enabled=false;
			}
		}

		///<summary>Sends the current PayPlan to a given target mobile device.
		///Shows a MsgBox when done or if error occurs.</summary>
		private void PushSelectedPayPlanToEclipboard(MobileAppDevice mobileAppDevice){
			if(_dynamicPaymentPlanData.PayPlan==null){
				return;//document wont be null below.
			}
			using PdfDocument pdfDocument=GetPayPlanPDF(_dynamicPaymentPlanData.PayPlan);//Can't be null due to above check.
			if(PushNotificationUtils.CI_SendPaymentPlan(pdfDocument,_dynamicPaymentPlanData.PayPlan,mobileAppDevice.MobileAppDeviceNum
				,out string errorMsg,out long mobileDataByeNum)) 
			{
				//The payment plan's MobileAppDeviceNum needs to be updated so that we know it is on a device
				MsgBox.Show($"Payment Plan sent to device: {mobileAppDevice.DeviceName}");
				//The signal from CI_SendPaymentPlan will take care of refreshing Control Account
				return;
			}
			//Error occurred
			//It failed to send to device, so clear out what ever device num was there
			PayPlans.UpdateMobileAppDeviceNum(_dynamicPaymentPlanData.PayPlan,0);
			Signalods.SetInvalid(InvalidType.AccModule,KeyType.PatNum,_dynamicPaymentPlanData.PayPlan.PatNum);
			Signalods.SetInvalid(InvalidType.EClipboard);
			MsgBox.Show($"Error sending Payment Plan: {errorMsg}");
		}

		///<summary>Opens a FormMobileCode window with the current PayPlan.</summary>
		private void OpenUnlockCodeForPayPlan(){
			MobileDataByte funcInsertDataForUnlockCode(string unlockCode) {
				using PdfDocument pdfDocument=GetPayPlanPDF(_dynamicPaymentPlanData.PayPlan);
				List<string> listTagValues=new List<string>() { _dynamicPaymentPlanData.PayPlan.PayPlanNum.ToString(),_dynamicPaymentPlanData.PayPlan.PayPlanDate.Ticks.ToString(),
					_dynamicPaymentPlanData.Patient.GetNameFirstOrPrefL() };
				if(MobileDataBytes.TryInsertPDF(pdfDocument,_dynamicPaymentPlanData.Patient.PatNum,unlockCode,eActionType.PaymentPlan,out long mobileDataByteNum,out string errorMsg,
					listTagValues))
				{
					return MobileDataBytes.GetOne(mobileDataByteNum);
				}
				//Failed to insert mobile data byte and won't be retrievable in eClipboard so clear out mobile app device num
				PayPlans.UpdateMobileAppDeviceNum(_dynamicPaymentPlanData.PayPlan,0);
				Signalods.SetInvalid(InvalidType.AccModule,KeyType.PatNum,_dynamicPaymentPlanData.PayPlan.PatNum);
				Signalods.SetInvalid(InvalidType.EClipboard);
				MsgBox.Show(errorMsg);
				return null;
			}
			using FormMobileCode formMobileCode=new FormMobileCode(funcInsertDataForUnlockCode);
			formMobileCode.ShowDialog();
		}

		///<summary>Returns a PDF for the current PayPlan and sets out PayPlan to the current PayPlan.
		///If nothing is selected in gridPayPlans then returns null and out PaymentPlan is set to null.</summary>
		private PdfDocument GetPayPlanPDF(PayPlan payPlan){
			PdfDocument pdfDocument=null;
			if(payPlan!=null) {
				Action actionCloseProgress=ODProgress.Show(); //Immediately shows a progress window.
				Sheet sheet=PayPlanToSheet(payPlan);
				pdfDocument=SheetPrinting.CreatePdf(sheet,"",null,null,null,null,null,false);
				actionCloseProgress();//Closes the progress window. 
			}
			return pdfDocument;
		}

		private void SetNote() {
			textNote.Text=_dynamicPaymentPlanData.PayPlan.Note+DateTime.Today.ToShortDateString()
				+" - "+Lan.g(this,"Date of Agreement")+": "+textDate.Text
				+", "+Lan.g(this,"Total Amount")+": "+textTotalPrincipal.Text
				+", "+Lan.g(this,"APR")+": "+textAPR.Text
				+", "+Lan.g(this,"Total Cost of Loan")+": "+textTotalCost.Text;
		}

		///<summary>Creates a new sheet from a given Pay plan.</summary>
		private Sheet PayPlanToSheet(PayPlan payPlan) {
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.PaymentPlan),_dynamicPaymentPlanData.Patient.PatNum);
			sheet.Parameters.Add(new SheetParameter(true,"payplan") { ParamValue=payPlan });
			sheet.Parameters.Add(new SheetParameter(true,"Principal") { ParamValue=_sumAttachedProduction.ToString("n") });
			sheet.Parameters.Add(new SheetParameter(true,"totalFinanceCharge") { ParamValue=_dynamicPaymentPlanData.TotalInterest });
			sheet.Parameters.Add(new SheetParameter(true,"totalCostOfLoan") {ParamValue=(_sumAttachedProduction+(decimal)_dynamicPaymentPlanData.TotalInterest).ToString("n")});
			SheetFiller.FillFields(sheet);
			return sheet;
		}

		private void checkExcludePast_CheckedChanged(object sender,EventArgs e) {
			FillCharges();
		}

		private void checkUngrouped_CheckedChanged(object sender,EventArgs e) {
			FillCharges();
		}

		private void checkShowAttachedPnI_CheckedChanged(object sender,EventArgs e) {
			FillProduction();
		}

		///<summary>Returns true for successful saving and false if there was a problem. 
		///isUiValid is only true if a previous method running TryGetTermsFromUI and has returned true. 
		///isUiValid is false when another method has run TryGetTermsFromUI and returned false, 
		///meaning an error message has been presented and we don't want to present another here.</summary>
		private bool SaveData(bool isPrinting=false,bool isUiValid=true) {
			if(PayPlans.GetOne(_dynamicPaymentPlanData.PayPlan.PayPlanNum)==null) {
				//The payment plan no longer exists in the database. 
				MsgBox.Show(this,"This payment plan has been deleted by another user.");
				return false;
			}
			if(textAPR.Text=="") {
				textAPR.Text="0";
			}
			if(!isUiValid || !TryGetTermsFromUI(out PayPlanTerms terms)) {//also validates terms
				return false;
			}
			_dynamicPaymentPlanData=PayPlanEdit.SaveDynamicPaymentPlanAndTerms(
				terms,
				_dynamicPaymentPlanData,
				checkProductionLock.Checked,
				comboCategory.GetSelectedDefNum());
			PayPlanL.MakeSecLogEntries(_dynamicPaymentPlanData.PayPlan,_payPlanOld,signatureBoxWrapper.GetSigChanged(),
				_isSigOldValid,signatureBoxWrapper.SigIsBlank,signatureBoxWrapper.IsValid,isPrinting);
			//When sign & print is clicked (saves the plan) on new plan to let users know it has been saved and some thing can no longer be edited.
			//Refresh local class wide variables.
			LoadPayDataFromDB();
			FillUiForSavedPayPlan();
			return true;
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete payment plan? All debits and credits will also be deleted, and all recurring charges associated to the payment plan will stop and their settings will be cleared.")) {
				return;
			}
			try {
				PayPlans.Delete(_dynamicPaymentPlanData.PayPlan);
				//Delete log here since this button doesn't call SaveData().
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,_dynamicPaymentPlanData.Patient.PatNum,"Dynamic Payment Plan deleted.",_payPlanOld.PayPlanNum,DateTime.MinValue);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(_dynamicPaymentPlanData.PayPlan.IsClosed) {
				butOK.Text="OK";
				butDelete.Enabled=true;
				butClosePlan.Enabled=true;
				labelClosed.Visible=false;
				_dynamicPaymentPlanData.PayPlan.IsClosed=false;
				return;
			}
			if(!TryGetTermsFromUI(out PayPlanTerms terms)) {
				return;
			}
			if(!SaveData()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPayPlanDynamic_Closing(object sender,CancelEventArgs e) {
			signatureBoxWrapper?.SetTabletState(0);
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(!_dynamicPaymentPlanData.PayPlan.IsNew){
				return;
			}
			try{
				PayPlans.Delete(_dynamicPaymentPlanData.PayPlan);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				e.Cancel=true;
				return;
			}
		}
	}
}
