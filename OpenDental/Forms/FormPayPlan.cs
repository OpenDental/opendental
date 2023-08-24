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
using PdfSharp.Pdf;
using OpenDentBusiness.WebTypes;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormPayPlan : FormODBase {
		private Patient _patient;
		private PayPlan _payPlan;
		private PayPlan _payPlanOld;
		///<summary>Only used for new payment plan.  Pass in the starting amount.  Usually the patient account balance.</summary>
		public double TotalAmt;
		///<summary>Family for the patient of this payplan.  Used to display insurance info.</summary>
		private Family _family;
		///<summary>Used to display insurance info.</summary>
		private List <InsPlan> _listInsPlans;
		private double _amtPaid;
		private double _totPrinc;
		private double _totInt;
		private double _totPrincIntAdj;
		private List<InsSub> _listInsSubs;
		///<summary>This form is reused as long as this parent form remains open.</summary>
		private FormPaymentPlanOptions _formPayPlanOptions;
		///<summary>Cached list of PayPlanCharges.</summary>
		private List<PayPlanCharge> _listPayPlanCharges;
		private Def[] _defArrayAccountColors;//Putting this here so we do one DB call for colors instead of many.  They'll never change.
		private FormPayPlanRecalculate _formPayPlanRecalculate;
		private List<PaySplit> _listPaySplits;
		private DataTable _tableClaimProcsBundled;
		private string _payPlanNote;
		private int _roundDec=CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
		private bool _isSigOldValid;		
		///<summary>Go to the specified patnum.  Upon dialog close, if this number is not 0, then patients.Cur will be changed to this new patnum, and Account refreshed to the new patient.</summary>
		public long PatNumGoto;
		///<summary>If true this plan tracks expected insurance payments. If false it tracks patient payments.</summary>
		public bool IsInsPayPlan;
		///<summary></summary>
		public bool IsNew;
		///<summary>List of Negative adjustments. Will be inserted into DB as negative adjustment. </summary>
		private List<Adjustment> _listAdjustments=new List<Adjustment>();

		///<summary>The supplied payment plan should already have been saved in the database.</summary>
		public FormPayPlan(PayPlan payPlan) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			_patient=Patients.GetPat(payPlan.PatNum);
			_payPlan=payPlan.Copy();
			_payPlanOld=_payPlan.Copy();
			_family=Patients.GetFamily(_patient.PatNum);
			_listInsSubs=InsSubs.RefreshForFam(_family);
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			_formPayPlanOptions=new FormPaymentPlanOptions(_payPlan.PaySchedule);
			_formPayPlanRecalculate=new FormPayPlanRecalculate();
			if(_payPlan.PlanNum!=0) {
				IsInsPayPlan=true;//This can also be set to true on the way in before a PlanNum has been assigned.
			}
			Lan.F(this);
		}

		private void FormPayPlan_Load(object sender,System.EventArgs e) {
			comboCategory.Items.AddDefNone();
			comboCategory.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PayPlanCategories,true));
			comboCategory.SetSelectedDefNum(_payPlan.PlanCategory); 
			textPatient.Text=Patients.GetLim(_payPlan.PatNum).GetNameLF();
			textGuarantor.Text=Patients.GetLim(_payPlan.Guarantor).GetNameLF();
			if(_payPlan.NumberOfPayments!=0) {
				textPaymentCount.Text=_payPlan.NumberOfPayments.ToString();
			}
			else {
				textPeriodPayment.Text=_payPlan.PayAmt.ToString("f");
			}
			warningIntegrity1.SetTypeAndVisibility(EnumWarningIntegrityType.PayPlan,PayPlans.IsPayPlanHashValid(_payPlan));
			textDownPayment.Text=_payPlan.DownPayment.ToString("f");
			textDate.Text=_payPlan.PayPlanDate.ToShortDateString();
			if(IsNew) {
				textAmount.Text=TotalAmt.ToString("f");//it won't get filled in FillCharges because there are no charges yet
				//If a plan is created "today" with the customer making their first payment on the spot, they will over pay interest.  
				//If there  is a larger gap than 1 month before the first payment, interest will be under calculated.
				//For now, our temporary solution is to prefill the date of first payment box starting with next months date which is the most accurate for calculating interest.
				textDateFirstPay.Text=DateTime.Now.AddMonths(1).ToShortDateString();
				_listPayPlanCharges=new List<PayPlanCharge>();
			}
			else {
				_listPayPlanCharges=PayPlanCharges.GetForPayPlan(_payPlan.PayPlanNum);
				if(_payPlan.DateInterestStart.Year>=1880) {
					textDateInterestStart.Text=_payPlan.DateInterestStart.ToShortDateString();
				}
			}
			if(PrefC.HasClinicsEnabled) {
				if(IsNew) {
					comboClinic.SelectedClinicNum=_patient.ClinicNum;
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
				comboProv.SetSelectedProvNum(_patient.PriProv);
			}
			textAPR.Text=_payPlan.APR.ToString();
			ToggleInterestDelayFieldsHelper();
			_amtPaid=PayPlans.GetAmtPaid(_payPlan);
			textAmtPaid.Text=_amtPaid.ToString("f");
			textCompletedAmt.Text=_payPlan.CompletedAmt.ToString("f");
			textNote.Text=_payPlan.Note;
			_payPlanNote=textNote.Text;
			if(IsInsPayPlan) {
				Text=Lan.g(this,"Insurance Payment Plan");
				textInsPlan.Text=InsPlans.GetDescript(_payPlan.PlanNum,_family,_listInsPlans,_payPlan.InsSubNum,_listInsSubs);
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
			_defArrayAccountColors=Defs.GetDefsForCategory(DefCat.AccountColors,true).ToArray();
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
			if(_payPlan.IsClosed) {
				butOK.Text=Lan.g(this,"Reopen");
				butDelete.Enabled=false;
				butClosePlan.Enabled=false;
				labelClosed.Visible=true;
			}
			if(PrefC.GetBool(PrefName.PayPlansUseSheets)) {
				Sheet sheet=null;
				sheet=PayPlanToSheet(_payPlan);
				//check to see if sig box is on the sheet
				//hides butPrint and adds butSignPrint,groupbox,and sigwrapper
				for(int i = 0;i<sheet.SheetFields.Count;i++) {
					if(sheet.SheetFields[i].FieldType==SheetFieldType.SigBox) {
						butPrint.Visible=false;
						butSignPrint.Visible=true;					
					}
				}
			}
			checkExcludePast.Checked=PrefC.GetBool(PrefName.PayPlansExcludePastActivity);
			FillCharges();
			if(_payPlan.Signature!="" && _payPlan.Signature!=null) {
				//check to see if sheet is signed before showing
				signatureBoxWrapper.Visible=true;
				groupBox4.Visible=true;
				butSignPrint.Text="View && Print";
				FillSignatureBox(); //fill signature
			}
			if(string.IsNullOrEmpty(_payPlan.Signature) || !signatureBoxWrapper.IsValid || signatureBoxWrapper.SigIsBlank) {
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
			Plugins.HookAddCode(this,"FormPayPlan.Load_end",_patient,IsNew,_payPlan);
		}

		private void FillSignatureBox() {
			signatureBoxWrapper.FillSignature(_payPlan.SigIsTopaz,PayPlans.GetKeyDataForSignature(_payPlan),_payPlan.Signature); //fill signature	
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillComboProv();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formProviderPick.ProvNumSelected=comboProv.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(formProviderPick.ProvNumSelected);
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
			gridCharges.Columns.Clear();
			GridColumn col;
			//If this column is changed from a date column then the comparer method (ComparePayPlanRows) needs to be updated.
			//If changes are made to the order of the grid, changes need to also be made for butPrint_Click
			col=new GridColumn(Lan.g("PayPlanAmortization","Date"),64,HorizontalAlignment.Center);//0
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Provider"),50);//1
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Description"),130);//2
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Principal"),60,HorizontalAlignment.Right);//3
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Interest"),52,HorizontalAlignment.Right);//4
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Due"),60,HorizontalAlignment.Right);//5
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Payment"),60,HorizontalAlignment.Right);//6
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Adjustment"),70,HorizontalAlignment.Right);//7
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g("PayPlanAmortization","Balance"),60,HorizontalAlignment.Right);//8
			gridCharges.Columns.Add(col);
			gridCharges.ListGridRows.Clear();
			List<GridRow> listGridRowsPayPlan=new List<GridRow>();
			int numCharges=1;
			for(int i=0;i<_listPayPlanCharges.Count;i++) {//Payplan Charges
				if(_listPayPlanCharges[i].ChargeType==PayPlanChargeType.Credit) {
					continue;//hide credits from the amortization grid.
				}
				// TODO: Create a helper class within PayPlanL called PayPlanRow? that will give us structure for adding to the grid and sorting.
				listGridRowsPayPlan.Add(PayPlanL.CreateRowForPayPlanCharge(_listPayPlanCharges[i],numCharges));
				//Don't increment the charge # for recalculated charges, since they won't have a #.
				if(!_listPayPlanCharges[i].Note.Trim().ToLower().Contains("recalculated based on")) {
					numCharges++;
				}
			}
			if(_payPlan.PlanNum==0) {//Normal payplan
				_listPaySplits=new List<PaySplit>();
				DataTable tablePaymentsBundled=PaySplits.GetForPayPlan(_payPlan.PayPlanNum);
				_listPaySplits=PaySplits.GetFromBundled(tablePaymentsBundled);
				for(int i=0;i<_listPaySplits.Count;i++) {
					listGridRowsPayPlan.Add(PayPlanL.CreateRowForPatientPayPlanSplit(tablePaymentsBundled.Rows[i],_listPaySplits[i]));
				}
			}
			else {//Insurance payplan
				_tableClaimProcsBundled=ClaimProcs.GetBundlesForPayPlan(_payPlan.PayPlanNum);
				for(int i=0;i<_tableClaimProcsBundled.Rows.Count;i++) {
					listGridRowsPayPlan.Add(PayPlanL.CreateRowForClaimProcs(_tableClaimProcsBundled.Rows[i]));
				}
			}
			listGridRowsPayPlan.Sort(PayPlanL.ComparePayPlanRows);
			int totalsRowIndex = -1; //if -1, then don't show a bold line as the first charge showing has not come due yet.
			double balanceAmt=0;
			double negAdjAmt=0;
			_totPrinc=0;
			_totInt=0;
			double totPay=0;
			for(int i=0;i<listGridRowsPayPlan.Count;i++) {
				if(!checkExcludePast.Checked || DateTime.Parse(listGridRowsPayPlan[i].Cells[0].Text)>DateTime.Today) {
					//Add the row if we aren't excluding past activity or the activity is in the future.
					gridCharges.ListGridRows.Add(listGridRowsPayPlan[i]);
				}
				if(listGridRowsPayPlan[i].Cells[3].Text!="") {//Principal
					_totPrinc+=PIn.Double(listGridRowsPayPlan[i].Cells[3].Text);
					balanceAmt+=PIn.Double(listGridRowsPayPlan[i].Cells[3].Text);
				}
				if(listGridRowsPayPlan[i].Cells[4].Text!="") {//Interest
					_totInt+=PIn.Double(listGridRowsPayPlan[i].Cells[4].Text);
					balanceAmt+=PIn.Double(listGridRowsPayPlan[i].Cells[4].Text);
				}
				else if(listGridRowsPayPlan[i].Cells[6].Text!="") {//Payment
					totPay+=PIn.Double(listGridRowsPayPlan[i].Cells[6].Text);
					balanceAmt-=PIn.Double(listGridRowsPayPlan[i].Cells[6].Text);
				}
				if(listGridRowsPayPlan[i].Cells[7].Text!="") { //adjustment
					balanceAmt+=PIn.Double(listGridRowsPayPlan[i].Cells[7].Text);//+ because adjustments are negatvie, decrement
					negAdjAmt-=PIn.Double(listGridRowsPayPlan[i].Cells[7].Text);//+ because adjustments are negatvie, increment
				}
				if(!checkExcludePast.Checked || DateTime.Parse(listGridRowsPayPlan[i].Cells[0].Text)>DateTime.Today) {
					gridCharges.ListGridRows[gridCharges.ListGridRows.Count-1].Cells[8].Text=balanceAmt.ToString("f");
				}
				if(DateTime.Parse(listGridRowsPayPlan[i].Cells[0].Text)<=DateTime.Today) {//Totals row
					textPrincipal.Text=_totPrinc.ToString("f");
					textInterest.Text=_totInt.ToString("f");
					textDue.Text=(_totPrinc+_totInt).ToString("f");
					textPayment.Text=totPay.ToString("f");
					textAdjustment.Text=negAdjAmt.ToString("f");
					textBalance.Text=balanceAmt.ToString("f");
					if(gridCharges.ListGridRows.Count>0) {
						totalsRowIndex=gridCharges.ListGridRows.Count-1;
					}
				}
			}
			_totPrinc=0;
			_totInt=0;
			int countDebits=0;
			for(int i=0;i<_listPayPlanCharges.Count;i++) {
				if(_listPayPlanCharges[i].ChargeType==PayPlanChargeType.Credit) {
					continue;//don't include credits when calculating the total loan cost, but do include adjustments
				}
				countDebits++;
				if(_listPayPlanCharges[i].ChargeType==PayPlanChargeType.Debit && _listPayPlanCharges[i].Principal >= 0) {//Not an adjustment
					_totPrinc+=_listPayPlanCharges[i].Principal;
					_totInt+=_listPayPlanCharges[i].Interest;
				}
			}
			_totPrincIntAdj=_totPrinc+_totInt+GetTotalNegAdjAmt();
			if(countDebits==0) {
				//don't damage what's already present in textAmount.Text
			}
			else {
				textAmount.Text=(_totPrinc+GetTotalNegAdjAmt()).ToString("f");
			}
			textTotalCost.Text=_totPrincIntAdj.ToString("f");
			List<PayPlanCharge> listPayPlanChargesDebits = _listPayPlanCharges.FindAll(x => x.ChargeType == PayPlanChargeType.Debit).OrderBy(x => x.ChargeDate).ToList();
			//If the payment plan consists of more than 1 payment, remove any downpayments from listDebits so it's ChargeDate is not used when refilling the "textDateFirstPay.Text". 
			if(listPayPlanChargesDebits.Count>1) {
				listPayPlanChargesDebits.RemoveAll(x=>x.Note.Contains("Down payment"));
			}
			if(listPayPlanChargesDebits.Count>0) {
				textDateFirstPay.Text=listPayPlanChargesDebits[0].ChargeDate.ToShortDateString();
			}
			else {
				//don't damage what's already in textDateFirstPay.Text
			}
			gridCharges.EndUpdate();
			if(gridCharges.ListGridRows.Count>0 && totalsRowIndex != -1) {
				gridCharges.ListGridRows[totalsRowIndex].ColorLborder=Color.Black;
				gridCharges.ListGridRows[totalsRowIndex].Cells[6].Bold=YN.Yes;
			}
			textAccumulatedDue.Text=PayPlans.GetAccumDue(_payPlan.PayPlanNum,_listPayPlanCharges).ToString("f");
			textPrincPaid.Text=PayPlans.GetPrincPaid(_amtPaid,_payPlan.PayPlanNum,_listPayPlanCharges).ToString("f");
		}

		private void butGoToPat_Click(object sender,System.EventArgs e) {
			//Performs the same steps as if user had clicked 'Ok'. 
			if(!Save()) {
				return;
			}
			PatNumGoto=_payPlan.PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butGoTo_Click(object sender,System.EventArgs e) {
			//Performs the same steps as if user had clicked 'Ok'. 
			if(!Save()) {
				return;
			}
			PatNumGoto=_payPlan.Guarantor;
			DialogResult=DialogResult.OK;
		}

		private void butChangeGuar_Click(object sender,System.EventArgs e) {
			if(PayPlans.GetAmtPaid(_payPlan)!=0) {
				MsgBox.Show(this,"Not allowed to change the guarantor because payments are attached.");
				return;
			}
			if(gridCharges.ListGridRows.Count>0) {
				MsgBox.Show(this,"Not allowed to change the guarantor without first clearing the amortization schedule.");
				return;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_payPlan.Guarantor=formPatientSelect.PatNumSelected;
			textGuarantor.Text=Patients.GetLim(_payPlan.Guarantor).GetNameLF();
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
			using FormInsPlanSelect formInsPlanSelect=new FormInsPlanSelect(_payPlan.PatNum);
			formInsPlanSelect.ShowDialog();
			if(formInsPlanSelect.DialogResult==DialogResult.Cancel) {
				return;
			}
			_payPlan.PlanNum=formInsPlanSelect.InsPlanSelected.PlanNum;
			_payPlan.InsSubNum=formInsPlanSelect.InsSubSelected.InsSubNum;
			textInsPlan.Text=InsPlans.GetDescript(_payPlan.PlanNum,Patients.GetFamily(_payPlan.PatNum),_listInsPlans,_payPlan.InsSubNum,_listInsSubs);
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
			_formPayPlanOptions=new FormPaymentPlanOptions(_payPlan.PaySchedule);
			_formPayPlanOptions.ShowDialog();
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
			FillSignatureBox();	
		}

		private void CalculateDateInterestStartFromInterestDelay() {
			if(PIn.Int(textInterestDelay.Text,false)!=0) {
				textDateInterestStart.Text=PayPlanEdit.CalcNextPeriodDate(PIn.Date(textDateFirstPay.Text),PIn.Int(textInterestDelay.Text,false)
					,GetFrequencyForFormPaymentPlanOptions(_formPayPlanOptions)).ToShortDateString();
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
			FillSignatureBox(); //fill signature
		}

		private void gridCharges_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) { 
			if(gridCharges.ListGridRows[e.Row].Tag==null) {//Prevent double clicking on the "Current Totals" row
				return;
			}
			if(gridCharges.ListGridRows[e.Row].Tag.GetType()==typeof(PayPlanCharge)) {
				PayPlanCharge payPlanCharge=(PayPlanCharge)gridCharges.ListGridRows[e.Row].Tag;
				double payPlanChargeOldAmt=payPlanCharge.Principal;
				using FormPayPlanChargeEdit formPayPlanChargeEdit=new FormPayPlanChargeEdit(payPlanCharge,_payPlan);//This automatically takes care of our in-memory list because the Tag is referencing our list of objects.
				formPayPlanChargeEdit.ShowDialog();
				if(formPayPlanChargeEdit.DialogResult==DialogResult.Cancel) {
					return;
				}
				if(formPayPlanChargeEdit.PayPlanChargeCur==null) {//The user deleted the payplancharge.
					_listPayPlanCharges.Remove(payPlanCharge);//We know the payPlanCharge object is inside _listPayPlanCharges.
					if(payPlanCharge.Principal<0) {//adjustment
						textAmount.Text=(PIn.Double(textAmount.Text)-(payPlanChargeOldAmt)).ToString("f");//charge will be negative, - to add the amount back
					}
					FillCharges();
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
				Payment payment=Payments.GetPayment(paySplit.PayNum);
				if(payment==null) {
					MessageBox.Show(Lans.g(this,"No payment exists.  Please run database maintenance method")+" "+nameof(DatabaseMaintenances.PaySplitWithInvalidPayNum));
					return;
				}
				using FormPayment formPayment2=new FormPayment(_patient,_family,payment,false);//FormPayment may inserts and/or update the paysplits. 
				formPayment2.IsNew=false;
				formPayment2.ShowDialog();
				if(formPayment2.DialogResult==DialogResult.Cancel) {
					return;
				}
			}
			else if(gridCharges.ListGridRows[e.Row].Tag.GetType()==typeof(DataRow)) {//Claim payment or bundle.
				DataRow rowClaimProcBundled=(DataRow)gridCharges.ListGridRows[e.Row].Tag;
				Claim claim=Claims.GetClaim(PIn.Long(rowClaimProcBundled["ClaimNum"].ToString()));
				if(claim==null) {
					MsgBox.Show(this,"The claim has been deleted.");
				}
				else {
					if(!Security.IsAuthorized(Permissions.ClaimView)) {
						return;
					}
					using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,_patient,_family);//FormClaimEdit inserts and/or updates the claim and/or claimprocs, which could potentially change the bundle.
					formClaimEdit.IsNew=false;
					formClaimEdit.ShowDialog();
					//Cancel from FormClaimEdit does not cancel payment edits, fill grid every time
				}
			}
			FillCharges();
		}

		///<summary>Adds a debit.</summary>
		private void butAdd_Click(object sender,System.EventArgs e) {
			PayPlanCharge payPlanCharge=PayPlanEdit.CreateDebitCharge(_payPlan,_family,comboProv.GetSelectedProvNum(),comboClinic.SelectedClinicNum,0,0,DateTime.Today,"");
			using FormPayPlanChargeEdit formPayPlanChargeEdit=new FormPayPlanChargeEdit(payPlanCharge,_payPlan);
			formPayPlanChargeEdit.IsNew=true;
			formPayPlanChargeEdit.ShowDialog();
			if(formPayPlanChargeEdit.DialogResult==DialogResult.Cancel) {
				return;
			}
			_listPayPlanCharges.Add(payPlanCharge);
			FillCharges();
			//fills signature. Most likely will invalidate the signature due to changes to PP
			FillSignatureBox();
		}

		private void butClear_Click(object sender,System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Clear all charges and adjustments from amortization schedule?  Credits will not be cleared.")) {
				return;
			}
			textAmount.Text=_totPrinc.ToString("f");//give the total amount back it's original value w/o adjustments.
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
			Sheet sheet=null;
			sheet=PayPlanToSheet(_payPlan);
			_payPlan.IsNew=IsNew;
			string keyData=PayPlans.GetKeyDataForSignature(_payPlan);
			SheetParameter.SetParameter(sheet,"keyData",keyData);
			SheetUtil.CalculateHeights(sheet);
			using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(sheet);
			formSheetFillEdit.ShowDialog();
			if(formSheetFillEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			//save signature
			if(_payPlan.Signature=="") {//clear signature and hide sigbox if blank sig was saved
				signatureBoxWrapper.ClearSignature();
				butSignPrint.Text="Sign && Print";
				signatureBoxWrapper.Visible=false;
				groupBox4.Visible=false;
				return;
			}
			signatureBoxWrapper.Visible=true;//show after PP has been signed for the first time
			groupBox4.Visible=true;
			butSignPrint.Text="View && Print";
			signatureBoxWrapper.FillSignature(_payPlan.SigIsTopaz,keyData,_payPlan.Signature); //fill signature on form
		}


		private void butPrint_Click(object sender,System.EventArgs e) {
			if(HasErrors()) {
				return;
			}
			SaveData(true);
			if(PrefC.GetBool(PrefName.PayPlansUseSheets)) {
				Sheet sheet=null;
				sheet=PayPlanToSheet(_payPlan);
				SheetPrinting.Print(sheet);
				return;
			}
			using Font font=new Font("Tahoma",9);
			using Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			using Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			using Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			ReportComplex reportComplex=new ReportComplex(false,false);
			reportComplex.AddTitle("Title",Lan.g(this,"Payment Plan Terms"),fontTitle);
			reportComplex.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			reportComplex.AddSubTitle("Date SubTitle",DateTime.Today.ToShortDateString(),fontSubTitle);
			AreaSectionType areaSectionType=AreaSectionType.ReportHeader;
			Section section=reportComplex.Sections[AreaSectionType.ReportHeader];
			//int sectIndex=report.Sections.GetIndexOfKind(AreaSectionKind.ReportHeader);
			Size size=new Size(300,20);//big enough for any text
			ContentAlignment contenAlignmentL=ContentAlignment.MiddleLeft;
			ContentAlignment contentAlignmentR=ContentAlignment.MiddleRight;
			int yPos=140;
			int space=30;
			int x1=175;
			int x2=275;
			reportComplex.ReportObjects.Add(new ReportObject
				("Patient Title",areaSectionType,new Point(x1,yPos),size,"Patient",font,contenAlignmentL));
			reportComplex.ReportObjects.Add(new ReportObject
				("Patient Detail",areaSectionType,new Point(x2,yPos),size,textPatient.Text,font,contentAlignmentR));
			yPos+=space;
			reportComplex.ReportObjects.Add(new ReportObject
				("Guarantor Title",areaSectionType,new Point(x1,yPos),size,"Guarantor",font,contenAlignmentL));
			reportComplex.ReportObjects.Add(new ReportObject
				("Guarantor Detail",areaSectionType,new Point(x2,yPos),size,textGuarantor.Text,font,contentAlignmentR));
			yPos+=space;
			reportComplex.ReportObjects.Add(new ReportObject
				("Date of Agreement Title",areaSectionType,new Point(x1,yPos),size,"Date of Agreement",font,contenAlignmentL));
			reportComplex.ReportObjects.Add(new ReportObject
				("Date of Agreement Detail",areaSectionType,new Point(x2,yPos),size,_payPlan.PayPlanDate.ToString("d"),font,contentAlignmentR));
			yPos+=space;
			reportComplex.ReportObjects.Add(new ReportObject
				("Principal Title",areaSectionType,new Point(x1,yPos),size,"Principal",font,contenAlignmentL));
			reportComplex.ReportObjects.Add(new ReportObject
				("Principal Detail",areaSectionType,new Point(x2,yPos),size,_totPrinc.ToString("n"),font,contentAlignmentR));
			yPos+=space;
			reportComplex.ReportObjects.Add(new ReportObject
				("Annual Percentage Rate Title",areaSectionType,new Point(x1,yPos),size,"Annual Percentage Rate",font,contenAlignmentL));
			reportComplex.ReportObjects.Add(new ReportObject
				("Annual Percentage Rate Detail",areaSectionType,new Point(x2,yPos),size,_payPlan.APR.ToString("f1"),font,contentAlignmentR));
			yPos+=space;
			reportComplex.ReportObjects.Add(new ReportObject
				("Total Finance Charges Title",areaSectionType,new Point(x1,yPos),size,"Total Finance Charges",font,contenAlignmentL));
			reportComplex.ReportObjects.Add(new ReportObject
				("Total Finance Charges Detail",areaSectionType,new Point(x2,yPos),size,_totInt.ToString("n"),font,contentAlignmentR));
			yPos+=space;
			reportComplex.ReportObjects.Add(new ReportObject
				("Total Cost of Loan Title",areaSectionType,new Point(x1,yPos),size,"Total Adjustments",font,contenAlignmentL));
			reportComplex.ReportObjects.Add(new ReportObject
				("Total Cost of Loan Detail",areaSectionType,new Point(x2,yPos),size,GetTotalNegAdjAmt().ToString("n"),font,contentAlignmentR));
			yPos+=space;
			reportComplex.ReportObjects.Add(new ReportObject
				("Total Cost of Loan Title",areaSectionType,new Point(x1,yPos),size,"Total Cost of Loan",font,contenAlignmentL));
			reportComplex.ReportObjects.Add(new ReportObject
				("Total Cost of Loan Detail",areaSectionType,new Point(x2,yPos),size,(_totPrincIntAdj).ToString("n"),font,contentAlignmentR));
			yPos+=space;
			section.Height=yPos+30;
			DataTable table=new DataTable();
			table.Columns.Add("date");
			table.Columns.Add("prov");
			table.Columns.Add("description");
			table.Columns.Add("principal");
			table.Columns.Add("interest");
			table.Columns.Add("due");
			table.Columns.Add("payment");
			table.Columns.Add("adjustment");
			table.Columns.Add("balance");
			DataRow row;
			for(int i = 0;i<gridCharges.ListGridRows.Count;i++) {
				row=table.NewRow();
				row["date"]=gridCharges.ListGridRows[i].Cells[0].Text;
				row["prov"]=gridCharges.ListGridRows[i].Cells[1].Text;
				row["description"]=gridCharges.ListGridRows[i].Cells[2].Text;
				row["principal"]=gridCharges.ListGridRows[i].Cells[3].Text;
				row["interest"]=gridCharges.ListGridRows[i].Cells[4].Text;
				row["due"]=gridCharges.ListGridRows[i].Cells[5].Text;
				row["payment"]=gridCharges.ListGridRows[i].Cells[6].Text;
				row["adjustment"]=gridCharges.ListGridRows[i].Cells[7].Text;
				row["balance"]=gridCharges.ListGridRows[i].Cells[8].Text;
				table.Rows.Add(row);
			}
			QueryObject queryObject=reportComplex.AddQuery(table,"","",SplitByKind.None,1,true);
			queryObject.AddColumn("ChargeDate",80,FieldValueType.Date,font);
			queryObject.GetColumnHeader("ChargeDate").StaticText="Date";
			queryObject.AddColumn("Provider",75,FieldValueType.String,font);
			queryObject.AddColumn("Description",130,FieldValueType.String,font);
			queryObject.AddColumn("Principal",70,FieldValueType.Number,font);
			queryObject.AddColumn("Interest",52,FieldValueType.Number,font);
			queryObject.AddColumn("Due",70,FieldValueType.Number,font);
			queryObject.AddColumn("Payment",70,FieldValueType.Number,font);
			queryObject.AddColumn("Adjustment",75,FieldValueType.Number,font);
			queryObject.AddColumn("Balance",70,FieldValueType.String,font);
			queryObject.GetColumnHeader("Balance").ContentAlignment=ContentAlignment.MiddleRight;
			queryObject.GetColumnDetail("Balance").ContentAlignment=ContentAlignment.MiddleRight;
			reportComplex.ReportObjects.Add(new ReportObject("Note",AreaSectionType.ReportFooter,new Point(x1,20),new Size(500,200),textNote.Text,font,ContentAlignment.TopLeft));
			reportComplex.ReportObjects.Add(new ReportObject("Signature",AreaSectionType.ReportFooter,new Point(x1,220),new Size(500,20),"Signature of Guarantor: ____________________________________________",font,contenAlignmentL));
			if(!reportComplex.SubmitQueries()) {
				return;
			}
			using FormReportComplex formReportComplex=new FormReportComplex(reportComplex);
			formReportComplex.ShowDialog();
		}

		private void butPayPlanTx_Click(object sender,EventArgs e) {
			using FormPayPlanCredits formPayPlanCredits=new FormPayPlanCredits(_patient,_payPlan,comboProv.GetSelectedProvNum());
			formPayPlanCredits.ListPayPlanChargesCredit=_listPayPlanCharges.Where(x => x.ChargeType==PayPlanChargeType.Credit).Select(x => x.Copy()).ToList();
			formPayPlanCredits.ShowDialog();
			if(formPayPlanCredits.DialogResult!=DialogResult.OK) {
				return;
			}
			UpdateTxPaymentPlanChargeSummary(formPayPlanCredits.ListPayPlanChargesCredit);
			//only attempt to change the total amt of the payment plan if an amortization schedule doesn't already exist.
			if(_listPayPlanCharges.Count(x => x.ChargeType==PayPlanChargeType.Debit)==0//amortization schedule does not exist
				&& PIn.Double(textTotalTxAmt.Text)!=PIn.Double(textAmount.Text)//Total treatment amount does not match term amount.
				&& MsgBox.Show(this,MsgBoxButtons.YesNo,"Change term Total Amount to match Total Tx Amount?")) {
				textAmount.Text=textTotalTxAmt.Text;
			}
			FillCharges();
		}

		private void butCloseOut_Click(object sender,EventArgs e) {
			if(HasErrors()) {
				return;
			}
			bool shouldClosePlan;
			if(IsInsPayPlan) {
				shouldClosePlan=AdjustInsPayPlanChargesForClose();
			}
			else { //Patient Payment Plan
				shouldClosePlan=AdjustPatPayPlanChargesForClose();
			}
			if (shouldClosePlan) {
				ClosePayPlan(_payPlan);
			}
		}

		private bool AdjustPatPayPlanChargesForClose() {
			List<long> listCreditedProcNums=_listPayPlanCharges.Where(x => x.ProcNum!=0).Select(x => x.ProcNum).ToList();
			List<Procedure> listCreditedProcedures=Procedures.GetManyProc(listCreditedProcNums,includeNote:false);
			string prompt=Lan.g(this,"Interest will be removed from future charges and they will be made due immediately. Would you like to continue?");
			if(listCreditedProcedures.Any(x => x.ProcStatus!=ProcStat.C)) {
				prompt=Lan.g(this,"Credits for treatment planned procedures will be removed and total principal may be reduced.")+" "+prompt;
			}
			if (!MsgBox.Show(MsgBoxButtons.YesNo,prompt)) {
				return false;
			}
			PayPlanCharge payPlanChargeCloseout=PayPlanEdit.CalculatePatPayPlanCloseoutCharge(_listPayPlanCharges,listCreditedProcedures,_payPlan,DateTime.Today);
			_listPayPlanCharges.RemoveAll(x => x.ChargeDate > DateTime.Today.Date); //also removes TP Procs
			_listPayPlanCharges.Add(payPlanChargeCloseout);
			return true;
		 }

		private bool AdjustInsPayPlanChargesForClose() {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Closing out an insurance payment plan will change the Tx Completed Amt to match the amount insurance actually paid.  Do you want to continue?")) {
				return false;
			}
			double insPaidTotal=0;
			for(int i=0;i < _tableClaimProcsBundled.Rows.Count;i++) {
				insPaidTotal+=PIn.Double(_tableClaimProcsBundled.Rows[i]["InsPayAmt"].ToString());
			}
			textCompletedAmt.Text=insPaidTotal.ToString("f");
			return true;
		}

		private void ClosePayPlan(PayPlan payPlan) {
			butClosePlan.Enabled=false;
			payPlan.IsClosed=true;
			FillCharges();
			SaveData();
			CreditCards.RemoveRecurringCharges(payPlan.PayPlanNum);
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

		private PayPlanFrequency GetFrequencyForFormPaymentPlanOptions(FormPaymentPlanOptions formPaymentPlanOptions) {
			if(formPaymentPlanOptions.radioWeekly.Checked) {
				return PayPlanFrequency.Weekly;
			}
			if(formPaymentPlanOptions.radioEveryOtherWeek.Checked) {
				return PayPlanFrequency.EveryOtherWeek;
			}
			if(formPaymentPlanOptions.radioOrdinalWeekday.Checked) {
				return PayPlanFrequency.OrdinalWeekday;
			}
			if(formPaymentPlanOptions.radioMonthly.Checked) {
				return PayPlanFrequency.Monthly;
			}
			return PayPlanFrequency.Quarterly;
		}

		///<summary>Creates the amortization schedule. Uses the textfield term values and not the ones stored in the database.</summary>
		private void CreateOrRecalculateScheduleCharges(bool isRecalculate) {
			PayPlanTerms payPlanTerms=GetTermsFromUI();
			if(isRecalculate) {
				PayPlanEdit.PayPlanRecalculationData payPlanRecalculationData=PayPlanEdit.PayPlanRecalculationData.CreateRecalculationData(payPlanTerms,_payPlan,_family
					,comboProv.GetSelectedProvNum(),comboClinic.SelectedClinicNum,_listPayPlanCharges,_listPaySplits,_formPayPlanRecalculate.IsPrepay,
					_formPayPlanRecalculate.IsRecalculateInterest);
				PayPlanEdit.RecalculateScheduleCharges(payPlanTerms,payPlanRecalculationData);
			}
			else{
				PayPlanEdit.CreateScheduleCharges(payPlanTerms,_payPlan,_family,comboProv.GetSelectedProvNum(),comboClinic.SelectedClinicNum,_listPayPlanCharges);
			}
			AreTermsValid(payPlanTerms.AreTermsValid);
			FillCharges();
			SetNote();
		}

		///<summary>Creates helper object to store pay plan terms.</summary>
		private PayPlanTerms GetTermsFromUI() {
		PayPlanTerms payPlanTerms=new PayPlanTerms();
			payPlanTerms.APR=PIn.Double(textAPR.Text);
			payPlanTerms.DateFirstPayment=PIn.Date(textDateFirstPay.Text);
			payPlanTerms.DateInterestStart=PIn.Date(textDateInterestStart.Text);//Will be DateTime.MinValue if text is blank.
			payPlanTerms.Frequency=GetFrequencyForFormPaymentPlanOptions(_formPayPlanOptions);
			payPlanTerms.PayCount=PIn.Int(textPaymentCount.Text,false);
			payPlanTerms.PrincipalAmount=PIn.Double(textAmount.Text);
			payPlanTerms.RoundDec=_roundDec;
			payPlanTerms.DateAgreement=PIn.Date(textDate.Text);
			payPlanTerms.DownPayment=PIn.Double(textDownPayment.Text);
			payPlanTerms.PaySchedule=PayPlanEdit.GetPayScheduleFromFrequency(payPlanTerms.Frequency);
			//PeriodPayment either 
			payPlanTerms.PeriodPayment=PayPlanEdit.CalculatePeriodPayment(payPlanTerms.APR,payPlanTerms.Frequency,PIn.Decimal(textPeriodPayment.Text),payPlanTerms.PayCount,
				payPlanTerms.RoundDec,payPlanTerms.PrincipalAmount,payPlanTerms.DownPayment);
			//Set some properties of object to be saved here because text fields can be changed after schedule is created.
			_payPlan.DownPayment=payPlanTerms.DownPayment;
			_payPlan.APR=payPlanTerms.APR;
			_payPlan.PaySchedule=payPlanTerms.PaySchedule;
			_payPlan.NumberOfPayments=payPlanTerms.PayCount;
			_payPlan.PayAmt=0;
			if(payPlanTerms.PayCount==0) {
				_payPlan.PayAmt=(double)payPlanTerms.PeriodPayment;
			}
			_payPlan.DateInterestStart=payPlanTerms.DateInterestStart;
			return payPlanTerms;
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
			if(GetTotalNegAdjAmt()!=0) {
				textNote.Text+=", "+Lan.g(this,"Adjustment")+": "+GetTotalNegAdjAmt().ToString("f");
			}
		}

		///<summary>Creates a new sheet from a given Pay plan.</summary>
		private Sheet PayPlanToSheet(PayPlan payPlan) {
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.PaymentPlan),_patient.PatNum);
			sheet.Parameters.Add(new SheetParameter(true,"payplan") { ParamValue=payPlan });
			sheet.Parameters.Add(new SheetParameter(true,"Principal") { ParamValue=_totPrinc.ToString("n") });
			sheet.Parameters.Add(new SheetParameter(true,"totalFinanceCharge") { ParamValue=_totInt });
			sheet.Parameters.Add(new SheetParameter(true,"totalCostOfLoan") { ParamValue=_totPrincIntAdj.ToString("n") });
			SheetFiller.FillFields(sheet);
			return sheet;
		}

		private void checkExcludePast_CheckedChanged(object sender,EventArgs e) {
			FillCharges();
		}

		private void butAdj_Click(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormPayPlan.butAdj_Click_beginning",_patient,_listAdjustments,_listPayPlanCharges,_payPlan)) {
				return;
			}
			if(_payPlan.IsClosed) {
				MsgBox.Show(this,"Cannot add adjustments to closed payment plans.");
				return;
			}
			if(PrefC.GetInt(PrefName.PayPlanAdjType)==0) {
				MsgBox.Show(this,"Adjustments cannot be created for payment plans until a default adjustment type has been selected in account preferences.");
				return;
			}
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			listInputBoxParams.Add(new InputBoxParam(InputBoxType.ValidDouble,Lan.g(this,"Please enter an amount:")+" "));
			listInputBoxParams.Add(new InputBoxParam(InputBoxType.CheckBox,"",Lan.g(this,"Also make line item in Account Module"),new Size(250,30)));
			using InputBox inputBox=new InputBox(listInputBoxParams
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
			Def def=Defs.GetDef(DefCat.AdjTypes,PrefC.GetLong(PrefName.PayPlanAdjType));
			inputBox.setTitle(Lan.g(this,"Negative Pay Plan Adjustment"));
			inputBox.SizeInitial=new Size(350,170);
			if(!GroupPermissions.HasPermissionForAdjType(Permissions.AdjustmentCreate,def)) {
				inputBox.checkBoxResult.Enabled=false;
			}
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return;
			}
			double negAdjAmt=-(PIn.Double(inputBox.textResult.Text));
			double totalRemainingBal=PayPlans.GetBalance(_payPlan.PayPlanNum,_listPayPlanCharges,_listPaySplits);
			if(CompareDouble.IsGreaterThan(Math.Abs(negAdjAmt),totalRemainingBal)) {
				MsgBox.Show(this,"Cannot add an adjustment totaling more than remaining balance due");
				return;
			}
			double totalNegFutureAdjs=_listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal<0 && x.ChargeDate > DateTime.Today).Sum(x => x.Principal);
			_listPayPlanCharges=PayPlanEdit.CreatePayPlanAdjustments(negAdjAmt,_listPayPlanCharges,totalNegFutureAdjs);
			if(inputBox.checkBoxResult.Checked) {//Make adjustment visible in account module.
				//set the information here, insert to the db upon saving
				Adjustment adjustment=new Adjustment();
				adjustment.AdjAmt=negAdjAmt; 
				adjustment.DateEntry=DateTime.Now.Date;
				adjustment.AdjDate=DateTime.Now.Date;
				adjustment.PatNum=_patient.PatNum;
				adjustment.ProvNum=comboProv.GetSelectedProvNum();
				adjustment.AdjNote=Lan.g(this,"Payment plan adjustment");
				adjustment.SecUserNumEntry=Security.CurUser.UserNum;
				adjustment.SecDateTEdit=DateTime.Now;
				adjustment.ClinicNum=comboClinic.SelectedClinicNum;
				if(Defs.GetDef(DefCat.AdjTypes,PrefC.GetLong(PrefName.PayPlanAdjType))!=null) {
					adjustment.AdjType=Defs.GetDef(DefCat.AdjTypes,PrefC.GetLong(PrefName.PayPlanAdjType)).DefNum;
				}
				_listAdjustments.Add(adjustment);
			}
			//add negative tx credit offset to tx credits if there is a completed amount
			if(PIn.Double(textCompletedAmt.Text)>0) {
				PayPlanCharge payPlanChargeTxOffset=new PayPlanCharge() {
					ChargeDate=DateTime.Now.Date,
					ChargeType=PayPlanChargeType.Credit,//needs to be saved as a credit to show in Tx Form
					Guarantor=_patient.PatNum,
					Note=Lan.g(this,"Adjustment"),
					PatNum=_patient.PatNum,
					PayPlanNum=_payPlan.PayPlanNum,
					Principal=negAdjAmt,
					ProcNum=0,
				};
				_listPayPlanCharges.Add(payPlanChargeTxOffset);
				List<PayPlanCharge> listPayPlanChargesCredit=_listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Credit);
				UpdateTxPaymentPlanChargeSummary(listPayPlanChargesCredit);
			}
			FillCharges();
			SetNote();
		}

		private void UpdateTxPaymentPlanChargeSummary(List<PayPlanCharge> listPayPlanChargesCredit) {
			_listPayPlanCharges.RemoveAll(x => x.ChargeType==PayPlanChargeType.Credit);
			_listPayPlanCharges.AddRange(listPayPlanChargesCredit);
			double txCompleteAmt=0;
			for(int i=0;i<listPayPlanChargesCredit.Count;i++) {
				if(listPayPlanChargesCredit[i].ChargeDate.Date!=DateTime.MaxValue.Date) { //do not take into account maxvalue (tp'd) charges
					txCompleteAmt+=listPayPlanChargesCredit[i].Principal;
				}
			}
			textCompletedAmt.Text=txCompleteAmt.ToString("f");
			double txTotalAmt=PayPlans.GetTxTotalAmt(_listPayPlanCharges);
			textTotalTxAmt.Text=txTotalAmt.ToString("f");
		}

		private void butSendToDevice_Click(object sender,EventArgs e) {
			if(!MobileAppDevices.IsClinicSignedUpForEClipboard(Clinics.ClinicNum)) {
				MsgBox.Show("Please enable eClipboard for this clinic to use this feature.");
				return;
			}
			//Performs the same steps as if user had clicked 'Ok'. 
			if(!Save()) {
				return;
			}
			//The sheet that the practice uses for payment plans needs to have a signature box on it, otherwise the signature won't be
			//visible after signing. 
			if(SheetDefs.GetInternalOrCustom(SheetInternalType.PaymentPlan).SheetFieldDefs.FirstOrDefault(
				x => x.FieldType==SheetFieldType.SigBox)==null) 
			{
				MsgBox.Show(this,"Please add a signature field to your pay plan sheet.");
				return;
			}
			if(_patient==null){
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			if(_payPlan==null) {
				MsgBox.Show(this,"Please select a payment plan to send.");
				return;
			}
			else if(!string.IsNullOrEmpty(_payPlan.Signature)) {
				MsgBox.Show(this,"This Payment Plan has already been signed.");
				return;
			}
			if(MobileAppDevices.ShouldSendPush(_patient.PatNum,out MobileAppDevice device)) {
				PushSelectedPayPlanToEclipboard(device);
			}
			else {
				OpenUnlockCodeForPayPlan();
			}
			DialogResult=DialogResult.OK;
			//The form needs to close, because if a patient signs a payment plan while FormPayPlan is open, and then someone in OD
			//clicks OK, then the signature gets blown away. 
			Close();
		}

		///<summary>Returns a PDF for the currently selected PaymentPlan and sets out PaymentPlan to selected PaymentPlan.
		///If nothing is selected in gridPayPlans then returns null and out PaymentPlan is set to null.</summary>
		private PdfDocument GetPayPlanPDF(PayPlan payPlan){
			PdfDocument pdfDocument=null;
			if(_payPlan==null) {
				return pdfDocument;
			}
			Action actionCloseProgress=ODProgress.Show(); //Immediately shows a progress window.
			Sheet sheet=PayPlanToSheet(payPlan);
			pdfDocument=SheetPrinting.CreatePdf(sheet,"",null,null,null,null,null,false);
			actionCloseProgress();//Closes the progress window. 
			return pdfDocument;
		}

		///<summary>Opens a FormMobileCode window with the currently selected PayPlan.</summary>
		private void OpenUnlockCodeForPayPlan(){
			MobileDataByte funcInsertDataForUnlockCode(string unlockCode) {
				using PdfDocument pdfDocument=GetPayPlanPDF(_payPlan);
				List<string> listTagValues=new List<string>();
				listTagValues.Add(_payPlan.PayPlanNum.ToString());
				listTagValues.Add(_payPlan.PayPlanDate.Ticks.ToString());
				listTagValues.Add(_patient.GetNameFirstOrPrefL());
				if(MobileDataBytes.TryInsertPDF(pdfDocument,_patient.PatNum,unlockCode,eActionType.PaymentPlan,out long mobileDataByteNum,out string errorMsg,
					listTagValues))
				{
					return MobileDataBytes.GetOne(mobileDataByteNum);
				}
				//Failed to insert mobile data byte and won't be retrievable in eClipboard so clear out mobile app device num
				PayPlans.UpdateMobileAppDeviceNum(_payPlan,0);
				Signalods.SetInvalid(InvalidType.AccModule,KeyType.PatNum,_payPlan.PatNum);
				Signalods.SetInvalid(InvalidType.EClipboard);
				MsgBox.Show(errorMsg);
				return null;
			}
			using FormMobileCode formMobileCode=new FormMobileCode(funcInsertDataForUnlockCode);
			formMobileCode.ShowDialog();
		}

		///<summary>Sends the current PayPlan to a given target mobile device.
		///Shows a MsgBox when done or if error occurs.</summary>
		private void PushSelectedPayPlanToEclipboard(MobileAppDevice mobileAppDevice){
			if(_payPlan==null){
				return;//document wont be null below.
			}
			using PdfDocument pdfDocument=GetPayPlanPDF(_payPlan);//Cant be null due to above check.
			if(PushNotificationUtils.CI_SendPaymentPlan(pdfDocument,_payPlan,mobileAppDevice.MobileAppDeviceNum
				,out string errorMsg,out long mobileDataByeNum)) 
			{
				//The payment plan's MobileAppDeviceNum needs to be updated so that we know it is on a device
				MsgBox.Show($"Payment Plan sent to device: {mobileAppDevice.DeviceName}");
				return;
				//The signal from CI_SendPaymentPlan will take care of refreshing Control Account
			}
			//Error occurred
			//It failed to send to device, so clear out what ever device num was there
			PayPlans.UpdateMobileAppDeviceNum(_payPlan,0);
			Signalods.SetInvalid(InvalidType.AccModule,KeyType.PatNum,_payPlan.PatNum);
			Signalods.SetInvalid(InvalidType.EClipboard);
			MsgBox.Show($"Error sending Payment Plan: {errorMsg}");
		}

		///<summary>Performs validation on payplan and then calls SaveData() to save payplan if the payplan was valid. Sets DialogResult to OK</summary>
		private bool Save() {
			if(_payPlan.IsClosed) {
				butOK.Text="OK";
				butDelete.Enabled=true;
				butClosePlan.Enabled=true;
				labelClosed.Visible=false;
				_payPlan.IsClosed=false;
				return false;
			}
			if(HasErrors()) {
				return false;
			}
			if(IsInsPayPlan && _payPlan.PlanNum==0) {
				MsgBox.Show(this,"An insurance plan must be selected.");
				return false;
			}
			if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
				//If there are tx credits, and it's a patient pay plan with no procs attached, and not an adjustment with a negative amount
				if(!CompareDouble.IsZero(PIn.Double(textTotalTxAmt.Text)) && _payPlan.PlanNum==0
					&& _listPayPlanCharges.Where(x=> x.ChargeType==PayPlanChargeType.Credit).Any(x => x.ProcNum==0 && !x.IsCreditAdjustment)) 
				{
					MsgBox.Show(this,"All treatment credits (excluding adjustments) must have a procedure.");
					return false;
				}
			}
			//insurance payment plans use the CompletedAmt text box, regular payment plans use totalTxAmt text box for validation.
			if(IsInsPayPlan && PIn.Double(textCompletedAmt.Text)!=PIn.Double(textAmount.Text)) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Tx Completed Amt and Total Amount do not match, continue?")) {
					return false;
				}
			}
			else if(!IsInsPayPlan && PIn.Double(textTotalTxAmt.Text)!=PIn.Double(textAmount.Text) 
				&& PrefC.GetInt(PrefName.PayPlansVersion)!=(int)PayPlanVersions.NoCharges) //Credits do not matter in ppv4
			{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Total Tx Amt and Total Amount do not match, continue?")) {
					return false;
				}
			}
			SaveData();
			Plugins.HookAddCode(this,"FormPaymentPlan.butOK_Click_end",_patient,_payPlan,IsNew);
			return true;
		}

		///<summary></summary>
		private void SaveData(bool isPrinting=false) {
			if(textAPR.Text=="") {
				textAPR.Text="0";
			}
			//PatNum not editable.
			//Guarantor set already
			_payPlan.PayPlanDate=PIn.Date(textDate.Text);
			//The following variables were handled when the amortization schedule was created.
			//PayPlanCur.APR
			//PayPlanCur.PaySchedule
			//PayPlanCur.NumberOfPayments
			//PayPlanCur.PayAmt
			//PayPlanCur.DownPayment
			//PayPlanCur.DateInterestStart
			_payPlan.Note=textNote.Text;
			_payPlan.CompletedAmt=PIn.Double(textCompletedAmt.Text);
			_payPlan.PlanCategory=comboCategory.GetSelectedDefNum();
			//PlanNum set already
			if(IsInsPayPlan) { //if insurance payment plan, remove all other credits and create one credit for the completed amt.
				_listPayPlanCharges.RemoveAll(x => x.ChargeType==PayPlanChargeType.Credit);//remove all credits
				PayPlanCharge payPlanChargeAdd=new PayPlanCharge();
				payPlanChargeAdd.ChargeDate=PIn.Date(textDate.Text);
				payPlanChargeAdd.ChargeType=PayPlanChargeType.Credit;
				payPlanChargeAdd.Guarantor=_payPlan.Guarantor; //credits always show in the account of the patient that the payplan was for.
				payPlanChargeAdd.Interest=0;
				payPlanChargeAdd.Note=Lan.g(this,"Expected Payments from")+" "+textInsPlan.Text;
				payPlanChargeAdd.PatNum=_payPlan.PatNum;
				payPlanChargeAdd.PayPlanNum=_payPlan.PayPlanNum;
				payPlanChargeAdd.Principal=PIn.Double(textCompletedAmt.Text);
				payPlanChargeAdd.ProcNum=0;
				//addCharge.ProvNum=0; //handled below
				//addCharge.ClinicNum=0; //handled below
				_listPayPlanCharges.Add(payPlanChargeAdd);
			}
			if(_listAdjustments.Count>0) {
				for(int i=0;i<_listAdjustments.Count;i++) {
					Adjustments.Insert(_listAdjustments[i]);
					TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(_listAdjustments[i]);
					SecurityLogs.MakeLogEntry(Permissions.AdjustmentCreate,_patient.PatNum,Lan.g(this,"Adjustment created from payment plan for ")+_patient.GetNameFL()+", "+_listAdjustments[i].AdjAmt.ToString("c"));
				}
			}
			if(PayPlans.GetOne(_payPlan.PayPlanNum)==null) {
				//The payment plan no longer exists in the database. 
				MsgBox.Show(this,"This payment plan has been deleted by another user.");
				return;
			}
			PayPlans.Update(_payPlan);//always saved to db before opening this form
			PayPlanL.MakeSecLogEntries(_payPlan,_payPlanOld,signatureBoxWrapper.GetSigChanged(),
				_isSigOldValid,signatureBoxWrapper.SigIsBlank,signatureBoxWrapper.IsValid,isPrinting);
			for(int i=0;i<_listPayPlanCharges.Count;i++) {
				_listPayPlanCharges[i].ClinicNum=comboClinic.SelectedClinicNum;
				_listPayPlanCharges[i].ProvNum=comboProv.GetSelectedProvNum();
			}
			PayPlanCharges.Sync(_listPayPlanCharges,_payPlan.PayPlanNum);
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete payment plan? All debits and credits will also be deleted, and all recurring charges associated to the payment plan will stop and their settings will be cleared.")) {
				return;
			}
			//later improvement if needed: possibly prevent deletion of some charges like older ones.
			try {
				PayPlans.Delete(_payPlan);
				//Delete log here since this button doesn't call SaveData().
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.PayPlanEdit,_patient.PatNum,
				(_payPlanOld.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan deleted.",_payPlanOld.PayPlanNum,DateTime.MinValue);
			DialogResult=DialogResult.OK;
			Plugins.HookAddCode(this,"FormPayPlan.butDelete_Click_end",_patient,_payPlan);
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!Save()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPayPlan_Closing(object sender,System.ComponentModel.CancelEventArgs e) {
			signatureBoxWrapper?.SetTabletState(0);
			_formPayPlanRecalculate.Dispose();
			_formPayPlanOptions.Dispose();
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(!IsNew){
				return;
			}
			try{
				PayPlans.Delete(_payPlan);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				e.Cancel=true;
				return;
			}
		}

		///<summary>Keep a total of our negative adjustments for this payment plan.</summary>
		private double GetTotalNegAdjAmt() {
			return _listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal<0).Sum(x => x.Principal);
		}


	}
}
