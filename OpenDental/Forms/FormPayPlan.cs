using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using System.Text;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormPayPlan : FormODBase {
		private Patient PatCur;
		private PayPlan _payPlanCur;
		private PayPlan _payPlanOld;
		///<summary>Only used for new payment plan.  Pass in the starting amount.  Usually the patient account balance.</summary>
		public double TotalAmt;
		///<summary>Family for the patient of this payplan.  Used to display insurance info.</summary>
		private Family FamCur;
		///<summary>Used to display insurance info.</summary>
		private List <InsPlan> InsPlanList;
		private double AmtPaid;
		private double TotPrinc;
		private double TotInt;
		private double TotPrincIntAdj;
		private List<InsSub> SubList;
		///<summary>This form is reused as long as this parent form remains open.</summary>
		private FormPaymentPlanOptions FormPayPlanOpts;
		///<summary>Cached list of PayPlanCharges.</summary>
		private List<PayPlanCharge> _listPayPlanCharges;
		private Def[] _arrayAccountColors;//Putting this here so we do one DB call for colors instead of many.  They'll never change.
		private FormPayPlanRecalculate _formPayPlanRecalculate;
		private List<PaySplit> _listPaySplits;
		private DataTable _bundledClaimProcs;
		private string _payPlanNote;
		private int _roundDec=CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
		private bool _isSigOldValid;		
		///<summary>Go to the specified patnum.  Upon dialog close, if this number is not 0, then patients.Cur will be changed to this new patnum, and Account refreshed to the new patient.</summary>
		public long GotoPatNum;
		///<summary>If true this plan tracks expected insurance payments. If false it tracks patient payments.</summary>
		public bool IsInsPayPlan;
		///<summary></summary>
		public bool IsNew;
		///<summary>List of Negative adjustments. Will be inserted into DB as negative adjustment. </summary>
		private List<Adjustment> _listAdjustments=new List<Adjustment>();

		///<summary>Keep a total of our negative adjustments for this payment plan.</summary>
		private double _totalNegAdjAmt {
			get {
				return _listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal<0).Sum(x => x.Principal);
			}
		}

		///<summary>Total of the adjustments made to the payment plan that have not come due yet. </summary>
		private double _totalNegFutureAdjs {
			get {
				return _listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal<0 
					&& x.ChargeDate > DateTime.Today).Sum(x => x.Principal);
			}
		}

		///<summary>Total remaining balance for the payment plan. </summary>
		private double _totalRemainingBal {
			get {
				return PayPlans.GetBalance(_payPlanCur.PayPlanNum,_listPayPlanCharges,_listPaySplits);
			}
		}

		///<summary>Running total of the amount of future debits.</summary>
		private double _totalFutureDebits {
			get {
				return _listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal>=0 
					&& x.ChargeDate > DateTime.Today).Sum(x => x.Principal);
			}
		}

		///<summary>The supplied payment plan should already have been saved in the database.</summary>
		public FormPayPlan(PayPlan payPlanCur) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			PatCur=Patients.GetPat(payPlanCur.PatNum);
			_payPlanCur=payPlanCur.Copy();
			_payPlanOld=_payPlanCur.Copy();
			FamCur=Patients.GetFamily(PatCur.PatNum);
			SubList=InsSubs.RefreshForFam(FamCur);
			InsPlanList=InsPlans.RefreshForSubList(SubList);
			FormPayPlanOpts=new FormPaymentPlanOptions(_payPlanCur.PaySchedule);
			_formPayPlanRecalculate=new FormPayPlanRecalculate();
			if(_payPlanCur.PlanNum!=0) {
				IsInsPayPlan=true;//This can also be set to true on the way in before a PlanNum has been assigned.
			}
			Lan.F(this);
		}

		private void FormPayPlan_Load(object sender,System.EventArgs e) {
			comboCategory.Items.AddDefNone();
			comboCategory.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PayPlanCategories,true));
			comboCategory.SetSelectedDefNum(_payPlanCur.PlanCategory); 
			textPatient.Text=Patients.GetLim(_payPlanCur.PatNum).GetNameLF();
			textGuarantor.Text=Patients.GetLim(_payPlanCur.Guarantor).GetNameLF();
			if(_payPlanCur.NumberOfPayments!=0) {
				textPaymentCount.Text=_payPlanCur.NumberOfPayments.ToString();
			}
			else {
				textPeriodPayment.Text=_payPlanCur.PayAmt.ToString("f");
			}
			textDownPayment.Text=_payPlanCur.DownPayment.ToString("f");
			textDate.Text=_payPlanCur.PayPlanDate.ToShortDateString();
			if(IsNew) {
				textAmount.Text=TotalAmt.ToString("f");//it won't get filled in FillCharges because there are no charges yet
				//If a plan is created "today" with the customer making their first payment on the spot, they will over pay interest.  
				//If there  is a larger gap than 1 month before the first payment, interest will be under calculated.
				//For now, our temporary solution is to prefill the date of first payment box starting with next months date which is the most accurate for calculating interest.
				textDateFirstPay.Text=DateTime.Now.AddMonths(1).ToShortDateString();
				_listPayPlanCharges=new List<PayPlanCharge>();
			}
			else {
				_listPayPlanCharges=PayPlanCharges.GetForPayPlan(_payPlanCur.PayPlanNum);
				if(_payPlanCur.DateInterestStart.Year>=1880) {
					textDateInterestStart.Text=_payPlanCur.DateInterestStart.ToShortDateString();
				}
			}
			if(PrefC.HasClinicsEnabled) {
				if(IsNew) {
					comboClinic.SelectedClinicNum=PatCur.ClinicNum;
				}
				else if(_listPayPlanCharges.Count==0) {
					comboClinic.SelectedClinicNum=0;
				}
				else {
					comboClinic.SelectedClinicNum=_listPayPlanCharges[0].ClinicNum;
				}
			}
			FillComboProv();
			if(_listPayPlanCharges.Count>0) {
				comboProv.SetSelectedProvNum(_listPayPlanCharges[0].ProvNum);
			}
			else {
				comboProv.SetSelectedProvNum(PatCur.PriProv);
			}
			textAPR.Text=_payPlanCur.APR.ToString();
			ToggleInterestDelayFieldsHelper();
			AmtPaid=PayPlans.GetAmtPaid(_payPlanCur);
			textAmtPaid.Text=AmtPaid.ToString("f");
			textCompletedAmt.Text=_payPlanCur.CompletedAmt.ToString("f");
			textNote.Text=_payPlanCur.Note;
			_payPlanNote=textNote.Text;
			if(IsInsPayPlan) {
				Text=Lan.g(this,"Insurance Payment Plan");
				textInsPlan.Text=InsPlans.GetDescript(_payPlanCur.PlanNum,FamCur,InsPlanList,_payPlanCur.InsSubNum,SubList);
				labelGuarantor.Visible=false;
				textGuarantor.Visible=false;
				butGoToGuar.Visible=false;
				butChangeGuar.Visible=false;
				textCompletedAmt.ReadOnly=false;
				butAddTxCredits.Visible=false;
				textTotalTxAmt.Visible=false;
				labelTotalTx.Visible=false;
				LayoutManager.MoveLocation(labelTxAmtInfo,new Point(labelTxAmtInfo.Location.X,labelTxAmtInfo.Location.Y-20));
			}
			else {
				Text=Lan.g(this,"Patient Payment Plan");
				labelInsPlan.Visible=false;
				textInsPlan.Visible=false;
				butChangePlan.Visible=false;
				LayoutManager.MoveLocation(textDate,new Point(textDate.Location.X+22,textDate.Location.Y));//line up with text boxes below
				LayoutManager.MoveLocation(labelDateAgreement,new Point(labelDateAgreement.Location.X+22,labelDateAgreement.Location.Y));
			}
			_arrayAccountColors=Defs.GetDefsForCategory(DefCat.AccountColors,true).ToArray();
			//If the amort schedule has been created and the first payment date has passed, don't allow user to change the first payment date or downpayment
			//until the schedule is cleared.
			if(!IsNew && PIn.Date(textDateFirstPay.Text)<DateTime.Today) {
				textDateFirstPay.ReadOnly=true;
				textDateInterestStart.ReadOnly=true;
				textInterestDelay.ReadOnly=true;
				textDownPayment.ReadOnly=true;
			}
			else {
				butRecalculate.Enabled=false;//Don't allow a plan that hasn't started to be recalculated.
			}
			textTotalTxAmt.Text=PayPlans.GetTxTotalAmt(_listPayPlanCharges).ToString("f");
			if(_payPlanCur.IsClosed) {
				butOK.Text=Lan.g(this,"Reopen");
				butDelete.Enabled=false;
				butClosePlan.Enabled=false;
				labelClosed.Visible=true;
			}
			if(PrefC.GetBool(PrefName.PayPlansUseSheets)) {
				Sheet sheetPP=null;
				sheetPP=PayPlanToSheet(_payPlanCur);
				//check to see if sig box is on the sheet
				//hides butPrint and adds butSignPrint,groupbox,and sigwrapper
				for(int i = 0;i<sheetPP.SheetFields.Count;i++) {
					if(sheetPP.SheetFields[i].FieldType==SheetFieldType.SigBox) {
						butPrint.Visible=false;
						butSignPrint.Visible=true;					
					}
				}
			}
			checkExcludePast.Checked=PrefC.GetBool(PrefName.PayPlansExcludePastActivity);
			FillCharges();
			if(_payPlanCur.Signature!="" && _payPlanCur.Signature!=null) {
				//check to see if sheet is signed before showing
				signatureBoxWrapper.Visible=true;
				groupBox4.Visible=true;
				butSignPrint.Text="View && Print";
				signatureBoxWrapper.FillSignature(_payPlanCur.SigIsTopaz,GetKeyDataForSignature(),_payPlanCur.Signature); //fill signature
			}
			if(string.IsNullOrEmpty(_payPlanCur.Signature) || !signatureBoxWrapper.IsValid || signatureBoxWrapper.SigIsBlank) {
				_isSigOldValid=false;
			}
			else {
				_isSigOldValid=true;
			}
			if(IsNew) {
				butDelete.Visible=false;
				butClosePlan.Visible=false;
			}
			if(!Security.IsAuthorized(Permissions.PayPlanEdit)) {
				DisableAllExcept(butGoToGuar,butGoToPat,butCancel,butPrint,checkExcludePast,butAddTxCredits,gridCharges);
				//allow grid so users can scroll, but de-register for event so charges cannot be modified. 
				this.gridCharges.CellDoubleClick-=gridCharges_CellDoubleClick;
			}
			Plugins.HookAddCode(this,"FormPayPlan.Load_end",PatCur,IsNew,_payPlanCur);
		}

		///<summary>Gets the hashstring for generating signatures.</summary>
		private string GetKeyDataForSignature() {
			//strb is a concatenation of the following:
			//pp: DateOfAgreement+ Total Amt+ APR+ Num of Payments+ Payment Amt + Note
			StringBuilder strb = new StringBuilder();
			Sheet sheetPP=null;
			sheetPP=PayPlanToSheet(_payPlanCur);
			strb.Append(_payPlanCur.PayPlanDate.ToShortDateString());
			strb.Append(textAmount.Text);
			strb.Append(_payPlanCur.APR.ToString());
			strb.Append(_payPlanCur.NumberOfPayments.ToString());
			strb.Append(_payPlanCur.PayAmt.ToString("f"));
			strb.Append(textNote.Text);
			strb.Append(Sheets.GetSignatureKey(sheetPP));
			return PayPlans.GetHashStringForSignature(strb.ToString());
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillComboProv();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formp = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formp.SelectedProvNum=comboProv.GetSelectedProvNum();
			formp.ShowDialog();
			if(formp.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(formp.SelectedProvNum);
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillComboProv() {
			long provNum=comboProv.GetSelectedProvNum();
			comboProv.Items.Clear();
			comboProv.Items.AddProvsFull(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			comboProv.SetSelectedProvNum(provNum);
		}

		/// <summary>Called 5 times.  This also fills prov and clinic based on the first charge if not new.</summary>
		private void FillCharges() {
			gridCharges.BeginUpdate();
			gridCharges.ListGridColumns.Clear();
			GridColumn col;
			//If this column is changed from a date column then the comparer method (ComparePayPlanRows) needs to be updated.
			//If changes are made to the order of the grid, changes need to also be made for butPrint_Click
			col=new GridColumn(Lan.g("PayPlanAmortization","Date"),64,HorizontalAlignment.Center);//0
			gridCharges.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Provider"),50);//1
			gridCharges.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Description"),130);//2
			gridCharges.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Principal"),60,HorizontalAlignment.Right);//3
			gridCharges.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Interest"),52,HorizontalAlignment.Right);//4
			gridCharges.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Due"),60,HorizontalAlignment.Right);//5
			gridCharges.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Payment"),60,HorizontalAlignment.Right);//6
			gridCharges.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Adjustment"),70,HorizontalAlignment.Right);//7
			gridCharges.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Balance"),60,HorizontalAlignment.Right);//8
			gridCharges.ListGridColumns.Add(col);
			gridCharges.ListGridRows.Clear();
			List<GridRow> listPayPlanRows=new List<GridRow>();
			int numCharges=1;
			for(int i=0;i<_listPayPlanCharges.Count;i++) {//Payplan Charges
				if(_listPayPlanCharges[i].ChargeType==PayPlanChargeType.Credit) {
					continue;//hide credits from the amortization grid.
				}
				// TODO: Create a helper class within PayPlanL called PayPlanRow? that will give us structure for adding to the grid and sorting.
				listPayPlanRows.Add(PayPlanL.CreateRowForPayPlanCharge(_listPayPlanCharges[i],numCharges));
				//Don't increment the charge # for recalculated charges, since they won't have a #.
				if(!_listPayPlanCharges[i].Note.Trim().ToLower().Contains("recalculated based on")) {
					numCharges++;
				}
			}
			if(_payPlanCur.PlanNum==0) {//Normal payplan
				_listPaySplits=new List<PaySplit>();
				DataTable bundledPayments=PaySplits.GetForPayPlan(_payPlanCur.PayPlanNum);
				_listPaySplits=PaySplits.GetFromBundled(bundledPayments);
				for(int i=0;i<_listPaySplits.Count;i++) {
					listPayPlanRows.Add(PayPlanL.CreateRowForPaySplit(bundledPayments.Rows[i],_listPaySplits[i]));
				}
			}
			else {//Insurance payplan
				_bundledClaimProcs=ClaimProcs.GetBundlesForPayPlan(_payPlanCur.PayPlanNum);
				for(int i=0;i<_bundledClaimProcs.Rows.Count;i++) {
					listPayPlanRows.Add(PayPlanL.CreateRowForClaimProcs(_bundledClaimProcs.Rows[i]));
				}
			}
			listPayPlanRows.Sort(PayPlanL.ComparePayPlanRows);
			int totalsRowIndex = -1; //if -1, then don't show a bold line as the first charge showing has not come due yet.
			double balanceAmt=0;
			double negAdjAmt=0;
			TotPrinc=0;
			TotInt=0;
			double TotPay=0;
			for(int i=0;i<listPayPlanRows.Count;i++) {
				if(!checkExcludePast.Checked || DateTime.Parse(listPayPlanRows[i].Cells[0].Text)>DateTime.Today) {
					//Add the row if we aren't excluding past activity or the activity is in the future.
					gridCharges.ListGridRows.Add(listPayPlanRows[i]);
				}
				if(listPayPlanRows[i].Cells[3].Text!="") {//Principal
					TotPrinc+=PIn.Double(listPayPlanRows[i].Cells[3].Text);
					balanceAmt+=PIn.Double(listPayPlanRows[i].Cells[3].Text);
				}
				if(listPayPlanRows[i].Cells[4].Text!="") {//Interest
					TotInt+=PIn.Double(listPayPlanRows[i].Cells[4].Text);
					balanceAmt+=PIn.Double(listPayPlanRows[i].Cells[4].Text);
				}
				else if(listPayPlanRows[i].Cells[6].Text!="") {//Payment
					TotPay+=PIn.Double(listPayPlanRows[i].Cells[6].Text);
					balanceAmt-=PIn.Double(listPayPlanRows[i].Cells[6].Text);
				}
				if(listPayPlanRows[i].Cells[7].Text!="") { //adjustment
					balanceAmt+=PIn.Double(listPayPlanRows[i].Cells[7].Text);//+ because adjustments are negatvie, decrement
					negAdjAmt-=PIn.Double(listPayPlanRows[i].Cells[7].Text);//+ because adjustments are negatvie, increment
				}
				if(!checkExcludePast.Checked || DateTime.Parse(listPayPlanRows[i].Cells[0].Text)>DateTime.Today) {
					gridCharges.ListGridRows[gridCharges.ListGridRows.Count-1].Cells[8].Text=balanceAmt.ToString("f");
				}
				if(DateTime.Parse(listPayPlanRows[i].Cells[0].Text)<=DateTime.Today) {//Totals row
					textPrincipal.Text=TotPrinc.ToString("f");
					textInterest.Text=TotInt.ToString("f");
					textDue.Text=(TotPrinc+TotInt).ToString("f");
					textPayment.Text=TotPay.ToString("f");
					textAdjustment.Text=negAdjAmt.ToString("f");
					textBalance.Text=balanceAmt.ToString("f");
					if(gridCharges.ListGridRows.Count>0) {
						totalsRowIndex=gridCharges.ListGridRows.Count-1;
					}
				}
			}
			TotPrinc=0;
			TotInt=0;
			int countDebits=0;
			for(int i=0;i<_listPayPlanCharges.Count;i++) {
				if(_listPayPlanCharges[i].ChargeType==PayPlanChargeType.Credit) {
					continue;//don't include credits when calculating the total loan cost, but do include adjustments
				}
				countDebits++;
				if(_listPayPlanCharges[i].ChargeType==PayPlanChargeType.Debit && _listPayPlanCharges[i].Principal >= 0) {//Not an adjustment
					TotPrinc+=_listPayPlanCharges[i].Principal;
					TotInt+=_listPayPlanCharges[i].Interest;
				}
			}
			TotPrincIntAdj=TotPrinc+TotInt+_totalNegAdjAmt;
			if(countDebits==0) {
				//don't damage what's already present in textAmount.Text
			}
			else {
				textAmount.Text=(TotPrinc+_totalNegAdjAmt).ToString("f");
			}
			textTotalCost.Text=TotPrincIntAdj.ToString("f");
			List<PayPlanCharge> listDebits = _listPayPlanCharges.FindAll(x => x.ChargeType == PayPlanChargeType.Debit).OrderBy(x => x.ChargeDate).ToList();
			if(listDebits.Count>0) {
				textDateFirstPay.Text=listDebits[0].ChargeDate.ToShortDateString();
			}
			else {
				//don't damage what's already in textDateFirstPay.Text
			}
			gridCharges.EndUpdate();
			if(gridCharges.ListGridRows.Count>0 && totalsRowIndex != -1) {
				gridCharges.ListGridRows[totalsRowIndex].ColorLborder=Color.Black;
				gridCharges.ListGridRows[totalsRowIndex].Cells[6].Bold=YN.Yes;
			}
			textAccumulatedDue.Text=PayPlans.GetAccumDue(_payPlanCur.PayPlanNum,_listPayPlanCharges).ToString("f");
			textPrincPaid.Text=PayPlans.GetPrincPaid(AmtPaid,_payPlanCur.PayPlanNum,_listPayPlanCharges).ToString("f");
		}

		private void butGoToPat_Click(object sender,System.EventArgs e) {
			if(HasErrors()) {
				return;
			}
			SaveData();
			GotoPatNum=_payPlanCur.PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butGoTo_Click(object sender,System.EventArgs e) {
			if(HasErrors()) {
				return;
			}
			SaveData();
			GotoPatNum=_payPlanCur.Guarantor;
			DialogResult=DialogResult.OK;
		}

		private void butChangeGuar_Click(object sender,System.EventArgs e) {
			if(PayPlans.GetAmtPaid(_payPlanCur)!=0) {
				MsgBox.Show(this,"Not allowed to change the guarantor because payments are attached.");
				return;
			}
			if(gridCharges.ListGridRows.Count>0) {
				MsgBox.Show(this,"Not allowed to change the guarantor without first clearing the amortization schedule.");
				return;
			}
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.SelectionModeOnly=true;
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			_payPlanCur.Guarantor=FormPS.SelectedPatNum;
			textGuarantor.Text=Patients.GetLim(_payPlanCur.Guarantor).GetNameLF();
		}
		
		private void textAmount_Validating(object sender,CancelEventArgs e) {
			if(textCompletedAmt.Text=="") {
				return;
			}
			if(PIn.Double(textCompletedAmt.Text)==PIn.Double(textAmount.Text)) {
				return;
			}
		}

		private void butChangePlan_Click(object sender,System.EventArgs e) {
			using FormInsPlanSelect FormI=new FormInsPlanSelect(_payPlanCur.PatNum);
			FormI.ShowDialog();
			if(FormI.DialogResult==DialogResult.Cancel) {
				return;
			}
			_payPlanCur.PlanNum=FormI.SelectedPlan.PlanNum;
			_payPlanCur.InsSubNum=FormI.SelectedSub.InsSubNum;
			textInsPlan.Text=InsPlans.GetDescript(_payPlanCur.PlanNum,Patients.GetFamily(_payPlanCur.PatNum),InsPlanList,_payPlanCur.InsSubNum,SubList);
		}

		private void textPaymentCount_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e) {
			textPeriodPayment.Text="";
		}

		private void textPeriodPayment_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e) {
			textPaymentCount.Text="";
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

		private void butMoreOptions_Click(object sender,EventArgs e) {
			FormPayPlanOpts=new FormPaymentPlanOptions(_payPlanCur.PaySchedule);
			FormPayPlanOpts.ShowDialog();
		}

		private void butCreateSched_Click(object sender,System.EventArgs e) {
			//this is also where the terms get saved
			if(!textDate.IsValid()
				|| !textAmount.IsValid()
				|| !textDateFirstPay.IsValid()
				|| !textDownPayment.IsValid()
				|| !textAPR.IsValid()
				|| !textInterestDelay.IsValid()
				|| !textDateInterestStart.IsValid()
				|| !textPaymentCount.IsValid()
				|| !textPeriodPayment.IsValid()
				|| !textCompletedAmt.IsValid()) 
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(textAmount.Text=="" || PIn.Double(textAmount.Text)==0) {
				MsgBox.Show(this,"Please enter an amount first.");
				return;
			}
			if(textDateFirstPay.Text=="") {
				textDateFirstPay.Text=DateTime.Today.ToShortDateString();
			}
			if(textDownPayment.Text=="") {
				textDownPayment.Text="0";
			}
			if(textAPR.Text=="") {
				textAPR.Text="0";
			}
			CalculateDateInterestStartFromInterestDelay();
			if(textPaymentCount.Text=="" && textPeriodPayment.Text=="") {
				//message box also used when butRecalculate is clicked
				MsgBox.Show(this,"Please enter a term or payment amount first.");
				return;
			}
			if(textPaymentCount.Text=="" && PIn.Double(textPeriodPayment.Text)==0) {
				MsgBox.Show(this,"Payment cannot be 0.");
				return;
			}
			if(textPaymentCount.Text!="" && textPeriodPayment.Text!="") {
				MsgBox.Show(this,"Please choose either Number of Payments or Payment Amt.");
				return;
			}
			if(textPeriodPayment.Text=="" && PIn.Long(textPaymentCount.Text)<1) {
				MsgBox.Show(this,"Term cannot be less than 1.");
				return;
			}
			if(PIn.Double(textAmount.Text)-PIn.Double(textDownPayment.Text)<0) {
				MsgBox.Show(this,"Down payment must be less than or equal to total amount.");
				return;
			}
			//If there are any debits, this button is going to delete them and replace them all with a new amortization schedule
			if(_listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit).Count>0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Replace existing amortization schedule?")) {
					return;
				}
				_listPayPlanCharges.RemoveAll(x => x.ChargeType==PayPlanChargeType.Debit); //for version 1, debits are the only chargetype available.
			}
			CreateOrRecalculateScheduleCharges(false);
			//fill signature. most likely will invalidate the signature if new schedule was created
			signatureBoxWrapper.FillSignature(_payPlanCur.SigIsTopaz,GetKeyDataForSignature(),_payPlanCur.Signature); //fill signature		
		}

		private void CalculateDateInterestStartFromInterestDelay() {
			if(PIn.Int(textInterestDelay.Text,false)!=0) {
				textDateInterestStart.Text=PayPlanEdit.CalcNextPeriodDate(PIn.Date(textDateFirstPay.Text),PIn.Int(textInterestDelay.Text,false)
					,GetFrequencyForFormPaymentPlanOptions(FormPayPlanOpts)).ToShortDateString();
				textInterestDelay.Text="";
			}
		}

		private void butRecalculate_Click(object sender,EventArgs e) {
			if(!textDate.IsValid()
				|| !textAmount.IsValid()
				|| !textDateFirstPay.IsValid()
				|| !textDownPayment.IsValid()
				|| !textAPR.IsValid()
				|| !textInterestDelay.IsValid()
				|| !textDateInterestStart.IsValid()
				|| !textPaymentCount.IsValid()
				|| !textPeriodPayment.IsValid()
				|| !textCompletedAmt.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(_listPayPlanCharges.Count(x => x.ChargeType==PayPlanChargeType.Debit)==0) {//This is only possible if they manually delete all of their rows and try to press recalculate.
				MsgBox.Show(this,"There is no payment plan to recalculate.");
				return;
			}
			if(IsInsPayPlan) {
				MsgBox.Show(this,"Insurance payment plans can't be recalculated.");
				return;
			}
			if(textPaymentCount.Text=="" && textPeriodPayment.Text=="") {
				//message box also used when butCreateSched is clicked
				MsgBox.Show(this,"Please enter a term or payment amount first.");
				return;
			}
			if(PIn.Double(textTotalCost.Text)<=PIn.Double(textAmtPaid.Text)) {
				MsgBox.Show(this,"The payment plan has been completely paid and can't be recalculated.");
				return;
			}
			_formPayPlanRecalculate=new FormPayPlanRecalculate();
			_formPayPlanRecalculate.ShowDialog();
			if(_formPayPlanRecalculate.DialogResult==DialogResult.OK) {
				CreateOrRecalculateScheduleCharges(true);
			}
			signatureBoxWrapper.FillSignature(_payPlanCur.SigIsTopaz,GetKeyDataForSignature(),_payPlanCur.Signature); //fill signature
		}

		private void gridCharges_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) { 
			if(gridCharges.ListGridRows[e.Row].Tag==null) {//Prevent double clicking on the "Current Totals" row
				return;
			}
			if(gridCharges.ListGridRows[e.Row].Tag.GetType()==typeof(PayPlanCharge)) {
				PayPlanCharge payPlanCharge=(PayPlanCharge)gridCharges.ListGridRows[e.Row].Tag;
				double payPlanChargeOldAmt=payPlanCharge.Principal;
				using FormPayPlanChargeEdit FormP=new FormPayPlanChargeEdit(payPlanCharge,_payPlanCur);//This automatically takes care of our in-memory list because the Tag is referencing our list of objects.
				FormP.ShowDialog();
				if(FormP.DialogResult==DialogResult.Cancel) {
					return;
				}
				if(FormP.PayPlanChargeCur==null) {//The user deleted the payplancharge.
					_listPayPlanCharges.Remove(payPlanCharge);//We know the payPlanCharge object is inside _listPayPlanCharges.
					if(payPlanCharge.Principal<0) {//adjustment
						textAmount.Text=(PIn.Double(textAmount.Text)-(payPlanChargeOldAmt)).ToString("f");//charge will be negative, - to add the amount back
					}
					gridCharges.BeginUpdate();
					gridCharges.ListGridRows.RemoveAt(e.Row);
					gridCharges.EndUpdate();
					return;
				}
				//modifying the existing charge
				if(payPlanCharge.Principal<0) {//adjustment
					double amtChanged=0;
					if(payPlanCharge.Principal>payPlanChargeOldAmt) {
						//negative amounts. We decreased the adjustment amount. Total Amount needs to grow.
						amtChanged=(payPlanChargeOldAmt-payPlanCharge.Principal)*-1;//amt should be +
					}
					else {
						//We increased the adjustment. Total Amount needs to shrink. 
						amtChanged=payPlanCharge.Principal-payPlanChargeOldAmt;//amt should be -
					}
					textAmount.Text=(PIn.Double(textAmount.Text)+(amtChanged)).ToString("f");
				}
			}
			else if(gridCharges.ListGridRows[e.Row].Tag.GetType()==typeof(PaySplit)) {
				PaySplit paySplit=(PaySplit)gridCharges.ListGridRows[e.Row].Tag;
				Payment pay=Payments.GetPayment(paySplit.PayNum);
				if(pay==null) {
					MessageBox.Show(Lans.g(this,"No payment exists.  Please run database maintenance method")+" "+nameof(DatabaseMaintenances.PaySplitWithInvalidPayNum));
					return;
				}
				using FormPayment FormPayment2=new FormPayment(PatCur,FamCur,pay,false);//FormPayment may inserts and/or update the paysplits. 
				FormPayment2.IsNew=false;
				FormPayment2.ShowDialog();
				if(FormPayment2.DialogResult==DialogResult.Cancel) {
					return;
				}
			}
			else if(gridCharges.ListGridRows[e.Row].Tag.GetType()==typeof(DataRow)) {//Claim payment or bundle.
				DataRow bundledClaimProc=(DataRow)gridCharges.ListGridRows[e.Row].Tag;
				Claim claimCur=Claims.GetClaim(PIn.Long(bundledClaimProc["ClaimNum"].ToString()));
				if(claimCur==null) {
					MsgBox.Show(this,"The claim has been deleted.");
				}
				else {
					if(!Security.IsAuthorized(Permissions.ClaimView)) {
						return;
					}
					using FormClaimEdit FormCE=new FormClaimEdit(claimCur,PatCur,FamCur);//FormClaimEdit inserts and/or updates the claim and/or claimprocs, which could potentially change the bundle.
					FormCE.IsNew=false;
					FormCE.ShowDialog();
					//Cancel from FormClaimEdit does not cancel payment edits, fill grid every time
				}
			}
			FillCharges();
		}

		///<summary>Adds a debit.</summary>
		private void butAdd_Click(object sender,System.EventArgs e) {
			PayPlanCharge ppCharge=PayPlanEdit.CreateDebitCharge(_payPlanCur,FamCur,comboProv.GetSelectedProvNum(),comboClinic.SelectedClinicNum,0,0,DateTime.Today,"");
			using FormPayPlanChargeEdit FormP=new FormPayPlanChargeEdit(ppCharge,_payPlanCur);
			FormP.IsNew=true;
			FormP.ShowDialog();
			if(FormP.DialogResult==DialogResult.Cancel) {
				return;
			}
			_listPayPlanCharges.Add(ppCharge);
			FillCharges();
			//fills signature. Most likely will invalidate the signature due to changes to PP
			signatureBoxWrapper.FillSignature(_payPlanCur.SigIsTopaz,GetKeyDataForSignature(),_payPlanCur.Signature); //fill signature
		}

		private void butClear_Click(object sender,System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Clear all charges and adjustments from amortization schedule?  Credits will not be cleared.")) {
				return;
			}
			textAmount.Text=TotPrinc.ToString("f");//give the total amount back it's original value w/o adjustments.
			_listPayPlanCharges.RemoveAll(x => x.ChargeType==PayPlanChargeType.Debit); 
			textDateFirstPay.ReadOnly=false;
			textInterestDelay.ReadOnly=false;
			textDateInterestStart.ReadOnly=false;
			textDownPayment.ReadOnly=false;
			gridCharges.BeginUpdate();
			gridCharges.ListGridRows.Clear();
			gridCharges.EndUpdate();
		}

		private void butSignPrint_Click(object sender,EventArgs e) {
			if(HasErrors()) {
				return;
			}
			SaveData();
			Sheet sheetPP=null;
			sheetPP=PayPlanToSheet(_payPlanCur);
			_payPlanCur.IsNew=IsNew;
			string keyData=GetKeyDataForSignature();
			SheetParameter.SetParameter(sheetPP,"keyData",keyData);
			SheetUtil.CalculateHeights(sheetPP);
			using FormSheetFillEdit FormSF=new FormSheetFillEdit(sheetPP);
			FormSF.ShowDialog();
			if(FormSF.DialogResult==DialogResult.OK) {//save signature
				if(_payPlanCur.Signature=="") {//clear signature and hide sigbox if blank sig was saved
					signatureBoxWrapper.ClearSignature();
					butSignPrint.Text="Sign && Print";
					signatureBoxWrapper.Visible=false;
					groupBox4.Visible=false;
				}
				else {
					signatureBoxWrapper.Visible=true;//show after PP has been signed for the first time
					groupBox4.Visible=true;
					butSignPrint.Text="View && Print";
					signatureBoxWrapper.FillSignature(_payPlanCur.SigIsTopaz,keyData,_payPlanCur.Signature); //fill signature on form
				}
			}
		}


		private void butPrint_Click(object sender,System.EventArgs e) {
			if(HasErrors()) {
				return;
			}
			SaveData(true);
			if(PrefC.GetBool(PrefName.PayPlansUseSheets)) {
				Sheet sheetPP=null;
				sheetPP=PayPlanToSheet(_payPlanCur);
				SheetPrinting.Print(sheetPP);
			}
			else {
				Font font=new Font("Tahoma",9);
				Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
				Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
				Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
				ReportComplex report=new ReportComplex(false,false);
				report.AddTitle("Title",Lan.g(this,"Payment Plan Terms"),fontTitle);
				report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
				report.AddSubTitle("Date SubTitle",DateTime.Today.ToShortDateString(),fontSubTitle);
				AreaSectionType sectType=AreaSectionType.ReportHeader;
				Section section=report.Sections[AreaSectionType.ReportHeader];
				//int sectIndex=report.Sections.GetIndexOfKind(AreaSectionKind.ReportHeader);
				Size size=new Size(300,20);//big enough for any text
				ContentAlignment alignL=ContentAlignment.MiddleLeft;
				ContentAlignment alignR=ContentAlignment.MiddleRight;
				int yPos=140;
				int space=30;
				int x1=175;
				int x2=275;
				report.ReportObjects.Add(new ReportObject
					("Patient Title",sectType,new Point(x1,yPos),size,"Patient",font,alignL));
				report.ReportObjects.Add(new ReportObject
					("Patient Detail",sectType,new Point(x2,yPos),size,textPatient.Text,font,alignR));
				yPos+=space;
				report.ReportObjects.Add(new ReportObject
					("Guarantor Title",sectType,new Point(x1,yPos),size,"Guarantor",font,alignL));
				report.ReportObjects.Add(new ReportObject
					("Guarantor Detail",sectType,new Point(x2,yPos),size,textGuarantor.Text,font,alignR));
				yPos+=space;
				report.ReportObjects.Add(new ReportObject
					("Date of Agreement Title",sectType,new Point(x1,yPos),size,"Date of Agreement",font,alignL));
				report.ReportObjects.Add(new ReportObject
					("Date of Agreement Detail",sectType,new Point(x2,yPos),size,_payPlanCur.PayPlanDate.ToString("d"),font,alignR));
				yPos+=space;
				report.ReportObjects.Add(new ReportObject
					("Principal Title",sectType,new Point(x1,yPos),size,"Principal",font,alignL));
				report.ReportObjects.Add(new ReportObject
					("Principal Detail",sectType,new Point(x2,yPos),size,TotPrinc.ToString("n"),font,alignR));
				yPos+=space;
				report.ReportObjects.Add(new ReportObject
					("Annual Percentage Rate Title",sectType,new Point(x1,yPos),size,"Annual Percentage Rate",font,alignL));
				report.ReportObjects.Add(new ReportObject
					("Annual Percentage Rate Detail",sectType,new Point(x2,yPos),size,_payPlanCur.APR.ToString("f1"),font,alignR));
				yPos+=space;
				report.ReportObjects.Add(new ReportObject
					("Total Finance Charges Title",sectType,new Point(x1,yPos),size,"Total Finance Charges",font,alignL));
				report.ReportObjects.Add(new ReportObject
					("Total Finance Charges Detail",sectType,new Point(x2,yPos),size,TotInt.ToString("n"),font,alignR));
				yPos+=space;
				report.ReportObjects.Add(new ReportObject
					("Total Cost of Loan Title",sectType,new Point(x1,yPos),size,"Total Adjustments",font,alignL));
				report.ReportObjects.Add(new ReportObject
					("Total Cost of Loan Detail",sectType,new Point(x2,yPos),size,_totalNegAdjAmt.ToString("n"),font,alignR));
				yPos+=space;
				report.ReportObjects.Add(new ReportObject
					("Total Cost of Loan Title",sectType,new Point(x1,yPos),size,"Total Cost of Loan",font,alignL));
				report.ReportObjects.Add(new ReportObject
					("Total Cost of Loan Detail",sectType,new Point(x2,yPos),size,(TotPrincIntAdj).ToString("n"),font,alignR));
				yPos+=space;
				section.Height=yPos+30;
				DataTable tbl=new DataTable();
				tbl.Columns.Add("date");
				tbl.Columns.Add("prov");
				tbl.Columns.Add("description");
				tbl.Columns.Add("principal");
				tbl.Columns.Add("interest");
				tbl.Columns.Add("due");
				tbl.Columns.Add("payment");
				tbl.Columns.Add("adjustment");
				tbl.Columns.Add("balance");
				DataRow row;
				for(int i = 0;i<gridCharges.ListGridRows.Count;i++) {
					row=tbl.NewRow();
					row["date"]=gridCharges.ListGridRows[i].Cells[0].Text;
					row["prov"]=gridCharges.ListGridRows[i].Cells[1].Text;
					row["description"]=gridCharges.ListGridRows[i].Cells[2].Text;
					row["principal"]=gridCharges.ListGridRows[i].Cells[3].Text;
					row["interest"]=gridCharges.ListGridRows[i].Cells[4].Text;
					row["due"]=gridCharges.ListGridRows[i].Cells[5].Text;
					row["payment"]=gridCharges.ListGridRows[i].Cells[6].Text;
					row["adjustment"]=gridCharges.ListGridRows[i].Cells[7].Text;
					row["balance"]=gridCharges.ListGridRows[i].Cells[8].Text;
					tbl.Rows.Add(row);
				}
				QueryObject query=report.AddQuery(tbl,"","",SplitByKind.None,1,true);
				query.AddColumn("ChargeDate",80,FieldValueType.Date,font);
				query.GetColumnHeader("ChargeDate").StaticText="Date";
				query.AddColumn("Provider",75,FieldValueType.String,font);
				query.AddColumn("Description",130,FieldValueType.String,font);
				query.AddColumn("Principal",70,FieldValueType.Number,font);
				query.AddColumn("Interest",52,FieldValueType.Number,font);
				query.AddColumn("Due",70,FieldValueType.Number,font);
				query.AddColumn("Payment",70,FieldValueType.Number,font);
				query.AddColumn("Adjustment",75,FieldValueType.Number,font);
				query.AddColumn("Balance",70,FieldValueType.String,font);
				query.GetColumnHeader("Balance").ContentAlignment=ContentAlignment.MiddleRight;
				query.GetColumnDetail("Balance").ContentAlignment=ContentAlignment.MiddleRight;
				report.ReportObjects.Add(new ReportObject("Note",AreaSectionType.ReportFooter,new Point(x1,20),new Size(500,200),textNote.Text,font,ContentAlignment.TopLeft));
				report.ReportObjects.Add(new ReportObject("Signature",AreaSectionType.ReportFooter,new Point(x1,220),new Size(500,20),"Signature of Guarantor: ____________________________________________",font,alignL));
				if(!report.SubmitQueries()) {
					return;
				}
				using FormReportComplex FormR=new FormReportComplex(report);
				FormR.ShowDialog();
			}
		}

		private void butPayPlanTx_Click(object sender,EventArgs e) {
			using FormPayPlanCredits FormPPC=new FormPayPlanCredits(PatCur,_payPlanCur,comboProv.GetSelectedProvNum());
			FormPPC.ListPayPlanCreditsCur=_listPayPlanCharges.Where(x => x.ChargeType==PayPlanChargeType.Credit).Select(x => x.Copy()).ToList();
			FormPPC.ShowDialog();
			if(FormPPC.DialogResult!=DialogResult.OK) {
				return;
			}
			_listPayPlanCharges.RemoveAll(x => x.ChargeType==PayPlanChargeType.Credit);
			_listPayPlanCharges.AddRange(FormPPC.ListPayPlanCreditsCur);
			double txCompleteAmt=0;
			foreach(PayPlanCharge credit in FormPPC.ListPayPlanCreditsCur) {
				if(credit.ChargeDate.Date!=DateTime.MaxValue.Date) { //do not take into account maxvalue (tp'd) charges
					txCompleteAmt+=credit.Principal;
				}
			}
			textCompletedAmt.Text=txCompleteAmt.ToString("f");
			double txTotalAmt=PayPlans.GetTxTotalAmt(_listPayPlanCharges);
			textTotalTxAmt.Text=txTotalAmt.ToString("f");
      double amt = PIn.Double(textTotalTxAmt.Text);
      double compl = PIn.Double(textAmount.Text);
      //only attempt to change the total amt of the payment plan if an amortization schedule doesn't already exist.
      if(_listPayPlanCharges.Count(x => x.ChargeType==PayPlanChargeType.Debit)==0//amortization schedule does not exist
				&& PIn.Double(textTotalTxAmt.Text)!=PIn.Double(textAmount.Text)//Total treatment amount does not match term amount.
				&& MsgBox.Show(this,MsgBoxButtons.YesNo,"Change term Total Amount to match Total Tx Amount?")) {
				textAmount.Text=txTotalAmt.ToString("f");
			}
			FillCharges();
		}

		private void butCloseOut_Click(object sender,EventArgs e) {
			if(HasErrors()) {
				return;
			}
			if(!IsInsPayPlan) {//Patient Payment Plan
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Closing out this payment plan will remove interest from all future charges "
					+"and make them due immediately.  Do you want to continue?")) {
					return;
				}
				PayPlanCharge closeoutCharge=PayPlanEdit.CloseOutPatPayPlan(_listPayPlanCharges,_payPlanCur,DateTime.Today);
				_listPayPlanCharges.RemoveAll(x => x.ChargeDate > DateTime.Today.Date); //also removes TP Procs
				_listPayPlanCharges.Add(closeoutCharge);
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Closing out an insurance payment plan will change the Tx Completed Amt to match the amount"
					+" insurance actually paid.  Do you want to continue?")) {
					return;
				}
				double insPaidTotal=0;
				for(int i=0;i < _bundledClaimProcs.Rows.Count;i++) {
					insPaidTotal+=PIn.Double(_bundledClaimProcs.Rows[i]["InsPayAmt"].ToString());
				}
				textCompletedAmt.Text=insPaidTotal.ToString("f");
			}
			butClosePlan.Enabled=false;
			_payPlanCur.IsClosed=true;
			FillCharges();
			SaveData();
			CreditCards.RemoveRecurringCharges(_payPlanCur.PayPlanNum);
			DialogResult=DialogResult.OK;
		}

		private bool HasErrors() {
			if(!textDate.IsValid() || !textCompletedAmt.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return true;
			}
			if(gridCharges.ListGridRows.Count==0) {
				MsgBox.Show(this,"An amortization schedule must be created first.");
				return true;
			}
			if(comboProv.GetSelectedProvNum()==0) {
				MsgBox.Show(this,"A provider must be selected first.");
				return true;
			}
			if(PIn.Date(textDate.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Payment plan date cannot be set for the future.");
				return true;
			}
			return false;
		}

		private PayPlanFrequency GetFrequencyForFormPaymentPlanOptions(FormPaymentPlanOptions options) {
			if(options.radioWeekly.Checked) {
				return PayPlanFrequency.Weekly;
			}
			else if(options.radioEveryOtherWeek.Checked) {
				return PayPlanFrequency.EveryOtherWeek;
			}
			else if(options.radioOrdinalWeekday.Checked) {
				return PayPlanFrequency.OrdinalWeekday;
			}
			else if(options.radioMonthly.Checked) {
				return PayPlanFrequency.Monthly;
			}
			else {
				return PayPlanFrequency.Quarterly;
			}
		}

		///<summary>Creates the amortization schedule. Uses the textfield term values and not the ones stored in the database.</summary>
		private void CreateOrRecalculateScheduleCharges(bool isRecalculate) {
			PayPlanTerms terms=GetTermsFromUI();
			if(isRecalculate) {
				PayPlanEdit.PayPlanRecalculationData recalcData=PayPlanEdit.PayPlanRecalculationData.CreateRecalculationData(terms,_payPlanCur,FamCur
					,comboProv.GetSelectedProvNum(),comboClinic.SelectedClinicNum,_listPayPlanCharges,_listPaySplits,_formPayPlanRecalculate.IsPrepay,
					_formPayPlanRecalculate.IsRecalculateInterest);
				PayPlanEdit.RecalculateScheduleCharges(terms,recalcData);
			}
			else{
				PayPlanEdit.CreateScheduleCharges(terms,_payPlanCur,FamCur,comboProv.GetSelectedProvNum(),comboClinic.SelectedClinicNum,_listPayPlanCharges);
			}
			AreTermsValid(terms.AreTermsValid);
			FillCharges();
			SetNote();
		}

		///<summary>Creates helper object to store pay plan terms.</summary>
		private PayPlanTerms GetTermsFromUI() {
		PayPlanTerms terms=new PayPlanTerms();
			terms.APR=PIn.Double(textAPR.Text);
			terms.DateFirstPayment=PIn.Date(textDateFirstPay.Text);
			terms.DateInterestStart=PIn.Date(textDateInterestStart.Text);//Will be DateTime.MinValue if text is blank.
			terms.Frequency=GetFrequencyForFormPaymentPlanOptions(FormPayPlanOpts);
			terms.PayCount=PIn.Int(textPaymentCount.Text,false);
			terms.PrincipalAmount=PIn.Double(textAmount.Text);
			terms.RoundDec=_roundDec;
			terms.DateAgreement=PIn.Date(textDate.Text);
			terms.DownPayment=PIn.Double(textDownPayment.Text);
			terms.PaySchedule=PayPlanEdit.GetPayScheduleFromFrequency(terms.Frequency);
			//PeriodPayment either 
			terms.PeriodPayment=PayPlanEdit.CalculatePeriodPayment(terms.APR,terms.Frequency,PIn.Decimal(textPeriodPayment.Text),terms.PayCount,
				terms.RoundDec,terms.PrincipalAmount,terms.DownPayment);
			//Set some properties of object to be saved here because text fields can be changed after schedule is created.
			_payPlanCur.DownPayment=terms.DownPayment;
			_payPlanCur.APR=terms.APR;
			_payPlanCur.PaySchedule=terms.PaySchedule;
			_payPlanCur.NumberOfPayments=terms.PayCount;
			_payPlanCur.PayAmt=0;
			if(terms.PayCount==0) {
				_payPlanCur.PayAmt=(double)terms.PeriodPayment;
			}
			_payPlanCur.DateInterestStart=terms.DateInterestStart;
			return terms;
		}

		private void AreTermsValid(bool areTermsValid) {
			if(!areTermsValid) {
				//The principal is actually increasing or staying the same with each payment.
				MessageBox.Show(Lan.g(this,"This payment plan will never be paid off. The interest being charged on each payment is greater than the"+
					" payment amount. Choose a lower interest rate or a higher payment amount."));
			}
		}

		private void SetNote() {
			textNote.Text=_payPlanNote+DateTime.Today.ToShortDateString()
				+" - "+Lan.g(this,"Date of Agreement")+": "+textDate.Text
				+", "+Lan.g(this,"Total Amount")+": "+textAmount.Text
				+", "+Lan.g(this,"APR")+": "+textAPR.Text
				+", "+Lan.g(this,"Total Cost of Loan")+": "+textTotalCost.Text;
			if(_totalNegAdjAmt!=0) {
				textNote.Text+=", "+Lan.g(this,"Adjustment")+": "+_totalNegAdjAmt.ToString("f");
			}
		}

		///<summary>Creates a new sheet from a given Pay plan.</summary>
		private Sheet PayPlanToSheet(PayPlan payPlan) {
			Sheet sheetPP=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.PaymentPlan),PatCur.PatNum);
			sheetPP.Parameters.Add(new SheetParameter(true,"payplan") { ParamValue=payPlan });
			sheetPP.Parameters.Add(new SheetParameter(true,"Principal") { ParamValue=TotPrinc.ToString("n") });
			sheetPP.Parameters.Add(new SheetParameter(true,"totalFinanceCharge") { ParamValue=TotInt });
			sheetPP.Parameters.Add(new SheetParameter(true,"totalCostOfLoan") { ParamValue=TotPrincIntAdj.ToString("n") });
			SheetFiller.FillFields(sheetPP);
			return sheetPP;
		}

		private void checkExcludePast_CheckedChanged(object sender,EventArgs e) {
			FillCharges();
		}

		private void butAdj_Click(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormPayPlan.butAdj_Click_beginning",PatCur,_listAdjustments,_listPayPlanCharges,_payPlanCur)) {
				return;
			}
			if(_payPlanCur.IsClosed) {
				MsgBox.Show(this,"Cannot add adjustments to closed payment plans.");
				return;
			}
			if(PrefC.GetInt(PrefName.PayPlanAdjType)==0) {
				MsgBox.Show(this,"Adjustments cannot be created for payment plans until a default adjustment type has been selected in account preferences.");
				return;
			}
			using InputBox inputBox=new InputBox(new List<InputBoxParam>() 
			{
				new InputBoxParam(InputBoxType.ValidDouble,Lan.g(this,"Please enter an amount:")+" "),
				new InputBoxParam(InputBoxType.CheckBox,"",Lan.g(this,"Also make line item in Account Module"),new Size(250,30)),
			}
				,new Func<string,bool>((text) => {
					double amount=PIn.Double(text);
					if(amount==0) {
						MsgBox.Show(this,"Please enter a valid value");
						return false;
					}
					if(amount<0) {
						MsgBox.Show(this,"Please enter a positive value for the negative adjustment");
						return false;
					}
					return true;
				})
			);
			inputBox.setTitle(Lan.g(this,"Negative Pay Plan Adjustment"));
			inputBox.SizeInitial=new Size(350,170);
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return;
			}
			double negAdjAmt=-(PIn.Double(inputBox.textResult.Text));
			if(CompareDouble.IsGreaterThan(Math.Abs(negAdjAmt),_totalRemainingBal)) {
				MsgBox.Show(this,"Cannot add an adjustment totaling more than remaining balance due");
				return;
			}
			_listPayPlanCharges=PayPlanEdit.CreatePayPlanAdjustments(negAdjAmt,_listPayPlanCharges,_totalNegFutureAdjs);
			if(inputBox.checkBoxResult.Checked) {//Make adjustment visible in account module.
				//set the information here, insert to the db upon saving
				Adjustment adj=new Adjustment();
				adj.AdjAmt=negAdjAmt; 
				adj.DateEntry=DateTime.Now.Date;
				adj.AdjDate=DateTime.Now.Date;
				adj.PatNum=PatCur.PatNum;
				adj.ProvNum=comboProv.GetSelectedProvNum();
				adj.AdjNote=Lan.g(this,"Payment plan adjustment");
				adj.SecUserNumEntry=Security.CurUser.UserNum;
				adj.SecDateTEdit=DateTime.Now;
				adj.ClinicNum=comboClinic.SelectedClinicNum;
				if(Defs.GetDef(DefCat.AdjTypes,PrefC.GetLong(PrefName.PayPlanAdjType))!=null) {
					adj.AdjType=Defs.GetDef(DefCat.AdjTypes,PrefC.GetLong(PrefName.PayPlanAdjType)).DefNum;
				}
				_listAdjustments.Add(adj);
			}
			//add negative tx credit offset to tx credits if there is a completed amount
			if(PIn.Double(textCompletedAmt.Text)>0) {
				PayPlanCharge txOffset=new PayPlanCharge() {
					ChargeDate=DateTime.Now.Date,
					ChargeType=PayPlanChargeType.Credit,//needs to be saved as a credit to show in Tx Form
					Guarantor=PatCur.PatNum,
					Note=Lan.g(this,"Adjustment"),
					PatNum=PatCur.PatNum,
					PayPlanNum=_payPlanCur.PayPlanNum,
					Principal=negAdjAmt,
					ProcNum=0,
				};
				_listPayPlanCharges.Add(txOffset);
				double txTotalAmt=PayPlans.GetTxTotalAmt(_listPayPlanCharges);
				textTotalTxAmt.Text=txTotalAmt.ToString("f");
			}
			FillCharges();
			SetNote();
		}

		///<summary></summary>
		private void SaveData(bool isPrinting=false) {
			if(textAPR.Text=="") {
				textAPR.Text="0";
			}
			//PatNum not editable.
			//Guarantor set already
			_payPlanCur.PayPlanDate=PIn.Date(textDate.Text);
			//The following variables were handled when the amortization schedule was created.
			//PayPlanCur.APR
			//PayPlanCur.PaySchedule
			//PayPlanCur.NumberOfPayments
			//PayPlanCur.PayAmt
			//PayPlanCur.DownPayment
			//PayPlanCur.DateInterestStart
			_payPlanCur.Note=textNote.Text;
			_payPlanCur.CompletedAmt=PIn.Double(textCompletedAmt.Text);
			_payPlanCur.PlanCategory=comboCategory.GetSelectedDefNum();
			//PlanNum set already
			if(IsInsPayPlan) { //if insurance payment plan, remove all other credits and create one credit for the completed amt.
				_listPayPlanCharges.RemoveAll(x => x.ChargeType==PayPlanChargeType.Credit);//remove all credits
				PayPlanCharge addCharge=new PayPlanCharge();
				addCharge.ChargeDate=PIn.Date(textDate.Text);
				addCharge.ChargeType=PayPlanChargeType.Credit;
				addCharge.Guarantor=_payPlanCur.Guarantor; //credits always show in the account of the patient that the payplan was for.
				addCharge.Interest=0;
				addCharge.Note=Lan.g(this,"Expected Payments from")+" "+textInsPlan.Text;
				addCharge.PatNum=_payPlanCur.PatNum;
				addCharge.PayPlanNum=_payPlanCur.PayPlanNum;
				addCharge.Principal=PIn.Double(textCompletedAmt.Text);
				addCharge.ProcNum=0;
				//addCharge.ProvNum=0; //handled below
				//addCharge.ClinicNum=0; //handled below
				_listPayPlanCharges.Add(addCharge);
			}
			if(_listAdjustments.Count>0) {
				foreach(Adjustment adj in _listAdjustments) {
					Adjustments.Insert(adj);
					TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adj);
					SecurityLogs.MakeLogEntry(Permissions.AdjustmentCreate,PatCur.PatNum,Lan.g(this,"Adjustment created from payment plan for")+" "
						+PatCur.GetNameFL()+", "+adj.AdjAmt.ToString("c"));
				}				
			}
			if(PayPlans.GetOne(_payPlanCur.PayPlanNum)==null) {
				//The payment plan no longer exists in the database. 
				MsgBox.Show(this,"This payment plan has been deleted by another user.");
				return;
			}
			PayPlans.Update(_payPlanCur);//always saved to db before opening this form
			PayPlanL.MakeSecLogEntries(_payPlanCur,_payPlanOld,signatureBoxWrapper.GetSigChanged(),
				_isSigOldValid,signatureBoxWrapper.SigIsBlank,signatureBoxWrapper.IsValid,isPrinting);
			foreach(PayPlanCharge charge in _listPayPlanCharges) {
				charge.ClinicNum=comboClinic.SelectedClinicNum;
				charge.ProvNum=comboProv.GetSelectedProvNum();
			}
			PayPlanCharges.Sync(_listPayPlanCharges,_payPlanCur.PayPlanNum);
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete payment plan? All debits and credits will also be deleted, and all recurring charges associated to the payment plan will stop and their settings will be cleared.")) {
				return;
			}
			//later improvement if needed: possibly prevent deletion of some charges like older ones.
			try {
				PayPlans.Delete(_payPlanCur);
				//Delete log here since this button doesn't call SaveData().
				SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,PatCur.PatNum,
							(_payPlanOld.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan deleted.",_payPlanOld.PayPlanNum,DateTime.MinValue);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
			Plugins.HookAddCode(this,"FormPayPlan.butDelete_Click_end",PatCur,_payPlanCur);
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(_payPlanCur.IsClosed) {
				butOK.Text="OK";
				butDelete.Enabled=true;
				butClosePlan.Enabled=true;
				labelClosed.Visible=false;
				_payPlanCur.IsClosed=false;
				return;
			}
			if(HasErrors()) {
				return;
			}
			if(IsInsPayPlan && _payPlanCur.PlanNum==0) {
				MsgBox.Show(this,"An insurance plan must be selected.");
				return;
			}
			if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
				//If there are tx credits, and it's a patient pay plan with no procs attached, and not an adjustment with a negative amount
				if(!CompareDouble.IsZero(PIn.Double(textTotalTxAmt.Text)) && _payPlanCur.PlanNum==0
					&& _listPayPlanCharges.Where(x=> x.ChargeType==PayPlanChargeType.Credit).Any(x => x.ProcNum==0 && !x.IsCreditAdjustment)) 
				{
					MsgBox.Show(this,"All treatment credits (excluding adjustments) must have a procedure.");
					return;
				}
			}
			//insurance payment plans use the CompletedAmt text box, regular payment plans use totalTxAmt text box for validation.
			if(IsInsPayPlan && PIn.Double(textCompletedAmt.Text)!=PIn.Double(textAmount.Text)) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Tx Completed Amt and Total Amount do not match, continue?")) {
					return;
				}
			}
			else if(!IsInsPayPlan && PIn.Double(textTotalTxAmt.Text)!=PIn.Double(textAmount.Text) 
				&& PrefC.GetInt(PrefName.PayPlansVersion)!=(int)PayPlanVersions.NoCharges) //Credits do not matter in ppv4
			{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Total Tx Amt and Total Amount do not match, continue?")) {
					return;
				}
			}
			SaveData();
			DialogResult=DialogResult.OK;
			Plugins.HookAddCode(this,"FormPaymentPlan.butOK_Click_end",PatCur,_payPlanCur,IsNew);
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPayPlan_Closing(object sender,System.ComponentModel.CancelEventArgs e) {
			signatureBoxWrapper?.SetTabletState(0);
			_formPayPlanRecalculate.Dispose();
			FormPayPlanOpts.Dispose();
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(IsNew){
				try{
					PayPlans.Delete(_payPlanCur);
				}
				catch(Exception ex){
					MessageBox.Show(ex.Message);
					e.Cancel=true;
					return;
				}
			}
		}
	}
}
