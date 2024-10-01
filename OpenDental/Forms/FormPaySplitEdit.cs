using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	///<summary>Summary description for FormPaySplitEdit.</summary>
	public partial class FormPaySplitEdit : FormODBase {
		#region Public variables
		///<summary></summary>
		public bool IsNew;
		///<summary>The value needed to make the splits balance.</summary>
		public double Remain;
		///<summary>Used to figure out what procedures have amounts left due on them when attaching this splits to a proc. 
		///Splits from the current payment are also included in this calculation.</summary>
		public List<PaySplit> ListPaySplits;
		///<summary>The pattern in this window is different than other windows.  We generally keep PaySplitCur updated as we go, rather than when we click OK. This is because we sometimes send it out for calcs.</summary>
		public PaySplit PaySplitCur;
		#endregion
		#region _private variables
		private bool _isEditAnyway;
		private decimal _remainAmt;
		private decimal _patPort;
		private Family _family;
		private PaySplit _paySplitCopy;
		private Procedure _procedure;
		private Adjustment _adjustment;
		private double _adjPrevPaid;
		private Procedure _procedureOld;
		private Def _defUnearnedOld;
		#endregion

		public FormPaySplitEdit(Family family) {
			InitializeComponent();
			InitializeLayoutManager();
			_family=family;
			Lan.F(this);
		}

		private void FormPaySplitEdit_Load(object sender, System.EventArgs e) {
			List<PatientLink> listPatientLinks=PatientLinks.GetLinks(_family.ListPats.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			List<Patient> listPatientsNonMerged=_family.ListPats.Where(x => !PatientLinks.WasPatientMerged(x.PatNum,listPatientLinks)).ToList();
			//New object to break reference to famCur in calling method/class; avoids removing merged patients from original object.
			if(listPatientsNonMerged.Count>0) {
				_family=new Family(listPatientsNonMerged);
			}
			else {
				List<Patient>listPatientsOnlyGuarantor=new List<Patient>();
				listPatientsOnlyGuarantor.Add(_family.Guarantor);
				_family=new Family(listPatientsOnlyGuarantor);
			}
			_paySplitCopy=PaySplitCur.Copy();
			textDateEntry.Text=PaySplitCur.DateEntry.ToShortDateString();
			textDatePay.Text=PaySplitCur.DatePay.ToShortDateString();
			textAmount.Text=PaySplitCur.SplitAmt.ToString("F");
			comboUnearnedTypes.Items.AddDefNone();
			comboUnearnedTypes.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,true));
			comboUnearnedTypes.SetSelectedDefNum(PaySplitCur.UnearnedType); 
			if(comboUnearnedTypes.SelectedIndex!=-1){
				_defUnearnedOld=comboUnearnedTypes.GetSelected<Def>();
			}
			if(PrefC.HasClinicsEnabled) {
				comboClinic.ClinicNumSelected=PaySplitCur.ClinicNum;
			}
			FillComboProv();//also sets the combo to PaySplitCur.ProvNum. Handles 0
			if(PaySplitCur.PayPlanNum==0){
				checkPayPlan.Checked=false;
			}
			else{
				checkPayPlan.Checked=true;
			}
			if(Clinics.IsMedicalPracticeOrClinic(PaySplitCur.ClinicNum)) {
				textProcTooth.Visible=false;
				labelProcTooth.Visible=false;
			}
			_procedure=PaySplitCur.ProcNum==0 ? null : Procedures.GetOneProc(PaySplitCur.ProcNum,false);
			_adjustment=PaySplitCur.AdjNum==0 ? null : Adjustments.GetOne(PaySplitCur.AdjNum);
			if(_procedure!=null) {
				_procedureOld=_procedure.Copy();
				tabAdjustment.Enabled=false;//Intellisense doesn't know this is here for some reason.  Shhh it's a secret.
				tabControl.SelectedIndex=0;//Set it on Proc tab automagically (this is just a safety precaution, it should be 0 already).
			}
			else if(_adjustment!=null) {
				tabProcedure.Enabled=false;//Intellisense doesn't know this is here for some reason as well.  Super double secret.
				tabControl.SelectedIndex=1;//Set it on Adjustment tab automagically.
			}
			SetEnabledProc();
			FillPatient();
			FillProcedure();
			FillAdjustment();
		}

		///<summary>Sets the patient GroupBox, provider combobox & picker button, 
		///and clinic combobox enabled/disabled depending on whether a proc is attached.</summary>
		private void SetEnabledProc() {
			if((_procedure!=null || _adjustment!=null) && !_isEditAnyway && PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
				groupPatient.Enabled=false;
				comboProvider.Enabled=false;
				butPickProv.Enabled=false;
				if(PrefC.HasClinicsEnabled) {
					comboClinic.Enabled=false;
				}
				if(Security.IsAuthorized(EnumPermType.Setup,true)) {
					labelEditAnyway.Visible=true;
					butEditAnyway.Visible=true;
				}
				return;
			}
			groupPatient.Enabled=true;
			comboProvider.Enabled=true;
			butPickProv.Enabled=true;
			if(PrefC.HasClinicsEnabled) {
				comboClinic.Enabled=true;
			}
			comboUnearnedTypes.Enabled=true;
			labelEditAnyway.Visible=false;
			butEditAnyway.Visible=false;
			checkPatOtherFam.Enabled=true;
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			PaySplitCur.ClinicNum=comboClinic.ClinicNumSelected;
			FillComboProv();
		}

		private void comboProvider_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboProvider.SelectedIndex>-1) {
				PaySplitCur.ProvNum=comboProvider.GetSelectedProvNum();
			}
			else {
				PaySplitCur.ProvNum=0;
			}
			if(_isEditAnyway || PrefC.GetBool(PrefName.AllowPrepayProvider)) {
				return;
			}
			if(PaySplitCur.ProvNum>0) {
				comboUnearnedTypes.SelectedIndex=0;
				DetachPayPlan();
				comboUnearnedTypes.Enabled=false;
				PaySplitCur.UnearnedType=0;
				return;
			}
			comboUnearnedTypes.Enabled=true;
		}

		private void comboUnearnedTypes_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboUnearnedTypes.SelectedIndex>0) {
				PaySplitCur.UnearnedType=comboUnearnedTypes.GetSelectedDefNum();
			}
			else {
				PaySplitCur.UnearnedType=0;
			}
			if(_isEditAnyway || PrefC.GetBool(PrefName.AllowPrepayProvider)) {
				return;
			}
			if(PaySplitCur.UnearnedType>0) {//If they use an unearned type the provnum must be zero if Edit Anyway isn't pressed
				PaySplitCur.ProvNum=0;
				comboProvider.SelectedIndex=0;
				DetachPayPlan();
				comboProvider.Enabled=false;
				butPickProv.Enabled=false;
				checkPayPlan.Enabled=false;
				return;
			}
			comboProvider.Enabled=true;
			butPickProv.Enabled=true;
			checkPayPlan.Enabled=true;
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			FrmProviderPick frmProviderPick = new FrmProviderPick(comboProvider.Items.GetAll<Provider>());
			frmProviderPick.ProvNumSelected=PaySplitCur.ProvNum;
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			PaySplitCur.ProvNum=frmProviderPick.ProvNumSelected;
			comboProvider.SetSelectedProvNum(PaySplitCur.ProvNum);
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillComboProv() {
			comboProvider.Items.Clear();
			comboProvider.Items.AddProvNone();
			comboProvider.Items.AddProvsAbbr(Providers.GetProvsForClinic(PaySplitCur.ClinicNum));
			comboProvider.SetSelectedProvNum(PaySplitCur.ProvNum);
		}

		private void butRemainder_Click(object sender, System.EventArgs e) {
			textAmount.Text=Remain.ToString("F");
		}

		///<summary>PaySplit.Patient is one value that is always kept in synch with the display.  If program changes PaySplit.Patient, then it will run this method to update the display.  If user changes display, then _MouseDown is run to update the PaySplit.Patient.</summary>
		private void FillPatient(){
			listPatient.Items.Clear();
			for(int i=0;i<_family.ListPats.Length;i++){
				listPatient.Items.Add(_family.GetNameInFamLFI(i));
				if(PaySplitCur.PatNum==_family.ListPats[i].PatNum){
					listPatient.SelectedIndex=i;
				}
			}
			//this can happen if it is a new payment split or user unchecks the "Is From Other Fam" box. Need to reset and select the patient currently selected in Open Dental if possible.
			if(PaySplitCur.PatNum==0){
				listPatient.SelectedIndex=0;
				PaySplitCur.PatNum=_family.ListPats[0].PatNum;//Patient of new split will default to the first patient in the family, usually guarantor
				for(int i=0;i<_family.ListPats.Length;i++) {//This is the same order added to listPatient.
					if(FormOpenDental.PatNumCur==_family.ListPats[i].PatNum) {
						listPatient.SelectedIndex=i;
						PaySplitCur.PatNum=_family.ListPats[i].PatNum;
						break;
					}
				}
			}
			if(listPatient.SelectedIndex==-1){//patient not in family
				checkPatOtherFam.Checked=true;
				textPatient.Visible=true;
				listPatient.Visible=false;
				textPatient.Text=Patients.GetLim(PaySplitCur.PatNum).GetNameLF();
			}
			else{//show the family list that was just filled
				checkPatOtherFam.Checked=false;
				textPatient.Visible=false;
				listPatient.Visible=true;
			}
		}

		private void checkPatOtherFam_Click(object sender, System.EventArgs e) {
			//this happens after the check change has been registered
			if(checkPatOtherFam.Checked){
				FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
				frmPatientSelect.ShowDialog();
				if(frmPatientSelect.IsDialogCancel){
					checkPatOtherFam.Checked=false;
					return;
				}
				PaySplitCur.PatNum=frmPatientSelect.PatNumSelected;
			}
			else{//switch to family view
				PaySplitCur.PatNum=0;//this will reset the selected patient to current patient
			}
			if(!_isEditAnyway) {//When user clicks Edit Anyway they are specifically trying to correct a bad split, so don't clear it out.
				_procedure=null;
				DetachPayPlan();
				FillProcedure();
			}
			FillAdjustment();
			FillPatient();
		}

		private void listPatient_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(listPatient.SelectedIndex==-1){
				return;
			}
			PaySplitCur.PatNum=_family.ListPats[listPatient.SelectedIndex].PatNum;
		}

		private void FillProcedure(){
			if(_procedure==null){
				textProcDate.Text="";
				textProcProv.Text="";
				textProcTooth.Text="";
				textProcDescription.Text="";
				textProcFee.Text="";
				textProcWriteoff.Text="";
				textProcInsPaid.Text="";
				textProcInsEst.Text="";
				textProcAdj.Text="";
				textProcPrevPaid.Text="";
				textProcPaidHere.Text="";
				labelProcRemain.Text="";
				butAttachProc.Enabled=true;
				comboProvider.Enabled=true;
				comboClinic.Enabled=true;
				comboUnearnedTypes.Enabled=true;
				textPatient.Enabled=true;
				groupPatient.Enabled=true;
				butPickProv.Enabled=true;
				tabAdjustment.Enabled=true;
				if(_adjustment==null) {
					checkPatOtherFam.Enabled=true;
				}
				return;
			}
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_procedure.PatNum);
			Adjustment[] adjustmentArray=Adjustments.Refresh(_procedure.PatNum);
			List<PaySplit> listPaySplitsForProc=PaySplits.Refresh(_procedure.PatNum).Where(x => x.ProcNum==_procedure.ProcNum).ToList();
			List<PaySplit> listPaySplitsForProcPaymentWindow=ListPaySplits.Where(x => x.ProcNum==_procedure.ProcNum && x.PayNum==PaySplitCur.PayNum).ToList();
			//Add new paysplits created for the current paysplits payment.
			listPaySplitsForProc.AddRange(listPaySplitsForProcPaymentWindow.Where(x => x.SplitNum==0));
			//Remove paysplits that have been deleted in the payment window but have not been saved to db. We don't want to use these paysplits 
			//when calculating procPrevPaid.
			listPaySplitsForProc.RemoveAll(x => !listPaySplitsForProcPaymentWindow.Any(y => y.IsSame(x)) && x.PayNum==PaySplitCur.PayNum);
			textProcDate.Text=_procedure.ProcDate.ToShortDateString();
			textProcProv.Text=Providers.GetAbbr(_procedure.ProvNum);
			textProcTooth.Text=Tooth.Display(_procedure.ToothNum);
			textProcDescription.Text="";
			if(_procedure.ProcStatus==ProcStat.TP) {
				textProcDescription.Text="(TP) ";
			}
			textProcDescription.Text+=ProcedureCodes.GetProcCode(_procedure.CodeNum).Descript;
			double procWriteoff=-ClaimProcs.ProcWriteoff(listClaimProcs,_procedure.ProcNum);
			double procInsPaid=-ClaimProcs.ProcInsPay(listClaimProcs,_procedure.ProcNum);
			double procInsEst=-ClaimProcs.ProcEstNotReceived(listClaimProcs,_procedure.ProcNum);
			double procAdj=Adjustments.GetTotForProc(_procedure.ProcNum,adjustmentArray);
			//next line will still work even if IsNew
			int countSplitsAttached;
			double procPrevPaid=-PaySplits.GetTotForProc(_procedure.ProcNum,listPaySplitsForProc.ToArray(),PaySplitCur,out countSplitsAttached);
			//Intelligently sum the values associated to the procedure, claim procs, and adjustments via status instead of blindly adding them together.
			_patPort=ClaimProcs.GetPatPortion(_procedure,listClaimProcs,adjustmentArray.ToList());
			textProcFee.Text=_procedure.ProcFeeTotal.ToString("F");
			if(procWriteoff==0){
				textProcWriteoff.Text="";
			}
			else{
				textProcWriteoff.Text=procWriteoff.ToString("F");
			}
			if(procInsPaid==0){
				textProcInsPaid.Text="";
			}
			else{
				textProcInsPaid.Text=procInsPaid.ToString("F");
			}
			if(procInsEst==0){
				textProcInsEst.Text="";
			}
			else{
				textProcInsEst.Text=procInsEst.ToString("F");
			}
			if(procAdj==0){
				textProcAdj.Text="";
			}
			else{
				textProcAdj.Text=procAdj.ToString("F");
			}
			if(procPrevPaid==0 && countSplitsAttached==0){
				textProcPrevPaid.Text="";
			}
			else{
				textProcPrevPaid.Text=procPrevPaid.ToString("F");
			}
			if(PrefC.HasClinicsEnabled) {
				comboClinic.ClinicNumSelected=PaySplitCur.ClinicNum;
			}
			butAttachProc.Enabled=false;
			if(!_isEditAnyway && PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
				comboProvider.Enabled=false;
				comboClinic.Enabled=false;
				butPickProv.Enabled=false;
			}
			if(_procedure.ProcStatus!=ProcStat.C) {
				comboUnearnedTypes.Enabled=true;
			}
			else {//There is no good way to determine if a proc previously had TP unearned so we will just keep whatever loaded in and disable the box. 
				comboUnearnedTypes.SelectedIndex=0;//First item is always None, if there is a procedure it cannot be a prepayment, regardless of enforce fully.
				comboUnearnedTypes.Enabled=false;
			}
			textPatient.Enabled=false;
			groupPatient.Enabled=false;
			checkPatOtherFam.Enabled=false;
			//Find the combo option for the procedure's clinic and provider.  If they don't exist in the list (are hidden) then it will set the text of the combo box instead.
			comboProvider.SetSelectedProvNum(PaySplitCur.ProvNum);
			if(PrefC.HasClinicsEnabled) {
				comboClinic.ClinicNumSelected=PaySplitCur.ClinicNum;//not sure why this is here again
			}
			//Proc selected will always be for the pat this paysplit was made for
			listPatient.SelectedIndex=_family.ListPats.ToList().FindIndex(x => x.PatNum==PaySplitCur.PatNum);
			ComputeTotals();
			tabAdjustment.Enabled=false;
		}

		private void FillAdjustment() {
			if(_adjustment==null) {
				textAdjDate.Text="";
				textAdjProv.Text="";
				textAdjAmt.Text="";
				textAdjPrevUsed.Text="";
				_adjPrevPaid=0;
				textAdjPaidHere.Text="";
				labelAdjRemaining.Text="";
				tabProcedure.Enabled=true;//Intellisense doesn't know about this, but it does exist.
				butAttachAdjust.Enabled=true;
				if(_procedure==null) {
					checkPatOtherFam.Enabled=true;
					groupPatient.Enabled=true;
				}
				return;
			}
			textAdjDate.Text=_adjustment.AdjDate.ToShortDateString();
			textAdjProv.Text=Providers.GetAbbr(_adjustment.ProvNum);
			textAdjAmt.Text=_adjustment.AdjAmt.ToString("F");//Adjustment's original amount
			//Don't include any splits on current payment - Since they could be modified and DB doesn't know about it yet.
			_adjPrevPaid=Adjustments.GetAmtAllocated(_adjustment.AdjNum,PaySplitCur.PayNum);
			//ListSplitsCur contains current paysplit, we need to remove it somehow.  PaySplitCur could have SplitNum=0 though.
			List<PaySplit> listPaySplits=ListPaySplits.FindAll(x => x.AdjNum==_adjustment.AdjNum);
			if(listPaySplits.Count>0) {
				_adjPrevPaid+=listPaySplits.Sum(x => x.SplitAmt);
				//There needs to be something here so _adjPrevPaid isn't adjusted by current split amt if the split isn't in listSplits
				if(PaySplitCur.IsNew || (!PaySplitCur.IsNew && listPaySplits.Exists(x => x.SplitNum==PaySplitCur.SplitNum))) {
					_adjPrevPaid-=PaySplitCur.SplitAmt;//To prevent double counting the current split
				}
			}
			textAdjPrevUsed.Text=(-_adjPrevPaid).ToString("F");//How much was previously used
			if(!textAmount.IsValid() || string.IsNullOrWhiteSpace(textAmount.Text)) {
				textAdjPaidHere.Text="";
			}
			else{
				textAdjPaidHere.Text=PIn.Double(textAmount.Text).ToString("F");//How much is used here
			}
			ComputeTotals();
			butAttachAdjust.Enabled=false;
			if(!_isEditAnyway && PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
				comboProvider.Enabled=false;
				comboClinic.Enabled=false;
				butPickProv.Enabled=false;
			}
			comboUnearnedTypes.SelectedIndex=0;//First item is always None, if there is a procedure it cannot be a prepayment, regardless of enforce fully.
			comboUnearnedTypes.Enabled=false;
			textPatient.Enabled=false;
			groupPatient.Enabled=false;
			checkPatOtherFam.Enabled=false;
			//Find the combo option for the adjustment's clinic and provider.  If they don't exist in the list (are hidden) then it will set the text of the combo box instead.
			comboProvider.SetSelectedProvNum(_adjustment.ProvNum);
			if(PrefC.HasClinicsEnabled) {
				comboClinic.ClinicNumSelected=_adjustment.ClinicNum;
			}
			//Proc selected will always be for the pat this paysplit was made for
			listPatient.SelectedIndex=_family.ListPats.ToList().FindIndex(x => x.PatNum==_adjustment.PatNum);
			tabProcedure.Enabled=false;//paysplits cannot have both procedure and adjustment
		}

		///<summary>Does not alter any of the proc amounts except PaidHere and Remaining.  Also calculates Adjust Amounts</summary>
		private void ComputeTotals() {
			double procPaidHere=0;
			double adjPaidHere=0;
			if(textAmount.IsValid()){
				procPaidHere=-PIn.Double(textAmount.Text);
				adjPaidHere=+PIn.Double(textAmount.Text);	
			}
			if(procPaidHere==0){
				textProcPaidHere.Text="";
				textAdjPaidHere.Text="";	
			}
			else{
				textProcPaidHere.Text=procPaidHere.ToString("F");
				textAdjPaidHere.Text=adjPaidHere.ToString("F");
			}
			labelAdjRemaining.Text="";
			labelProcRemain.Text="";
			_remainAmt=0;
			if(_procedure!=null) {
				_remainAmt=_patPort+(decimal)procPaidHere+PIn.Decimal(textProcPrevPaid.Text);
				labelProcRemain.Text=_remainAmt.ToString("c");
			}
			else if(_adjustment!=null) {
				_remainAmt=(decimal)_adjustment.AdjAmt-(decimal)_adjPrevPaid-(decimal)adjPaidHere;
				labelAdjRemaining.Text=_remainAmt.ToString("c");//How much is remaining
			}
		}

		private void textAmount_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			//can not use textAmount_TextChanged without redesigning the validDouble control
			ComputeTotals();
		}

		private void TextAmount_TextChanged(object sender,EventArgs e) {
			//can not use textAmount_TextChanged without redesigning the validDouble control
			ComputeTotals();
		}

		/// <summary>Helper Method that detaches Payment Plan Info from PaySplitCur </summary>
		private void DetachPayPlan() {
			PaySplitCur.PayPlanNum=0;
			PaySplitCur.PayPlanChargeNum=0;
			checkPayPlan.Checked=false;
		}

		///<summary>Attaches procedure, sets the selected provider, and fills Procedure information.</summary>
		private void butAttachProc_Click(object sender, System.EventArgs e) {
			using FormProcSelect formProcSelect=new FormProcSelect(PaySplitCur.PatNum,false);
			formProcSelect.ListPaySplits = ListPaySplits;
			formProcSelect.ShowDialog();
			if(formProcSelect.DialogResult!=DialogResult.OK){
				return;
			}
			_procedure=formProcSelect.ListProceduresSelected[0];
			PaySplitCur.ProvNum=formProcSelect.ListProceduresSelected[0].ProvNum;
			PaySplitCur.ClinicNum=formProcSelect.ListProceduresSelected[0].ClinicNum;
			if(_procedure.ProcStatus==ProcStat.TP) {
				PaySplitCur.UnearnedType=PrefC.GetLong(PrefName.TpUnearnedType);//use default tp unearned for tp procedures.
				comboUnearnedTypes.SetSelectedDefNum(PaySplitCur.UnearnedType);
			}
			else {
				comboUnearnedTypes.SelectedIndex=0;
				PaySplitCur.UnearnedType=0;
			}
			DetachPayPlan();
			SetEnabledProc();
			FillProcedure();
			FillAdjustment();
		}

		private void butDetachProc_Click(object sender, System.EventArgs e) {
			if(_procedure!=null) {
				ListPaySplits.Where(x => x.ProcNum==_procedure.ProcNum && x.IsSame(PaySplitCur))
					.ForEach(x => x.ProcNum=0);
				if(_procedure.ProcStatus==ProcStat.TP) {
					checkPayPlan.Enabled=true;
				}
			}
			_procedure=null;
			DetachPayPlan();
			SetEnabledProc();
			FillProcedure();
			FillAdjustment();
		}

		private void butAttachAdjust_Click(object sender,EventArgs e) {
			List<Adjustment> listAdjustmentsPat=Adjustments.GetAdjustForPats(new List<long>() { PaySplitCur.PatNum });
			List<PaySplit> listPaySplitsAdj=PaySplits.GetForAdjustments(listAdjustmentsPat.Select(x => x.AdjNum).ToList());
			using FormAdjustSelect formAdjustSelect=new FormAdjustSelect(PIn.Double(textAmount.Text),PaySplitCur,ListPaySplits,listAdjustmentsPat,listPaySplitsAdj);
			if(formAdjustSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_adjustment=formAdjustSelect.AdjustmentSelected;
			PaySplitCur.ProvNum=_adjustment.ProvNum;
			PaySplitCur.ClinicNum=_adjustment.ClinicNum;
			PaySplitCur.UnearnedType=0;
			DetachPayPlan();
			SetEnabledProc();
			FillProcedure();
			FillAdjustment();
		}

		private void butDetachAdjust_Click(object sender,EventArgs e) {
			if(_adjustment!=null) {
				ListPaySplits.Where(x => x.AdjNum==_adjustment.AdjNum && x.IsSame(PaySplitCur))
					.ForEach(x => x.AdjNum=0);
			}
			_adjustment=null;
			DetachPayPlan();
			SetEnabledProc();//think about this.
			FillProcedure();
			FillAdjustment();
		}

		private void butEditAnyway_Click(object sender,EventArgs e) {
			_isEditAnyway=true;
			SetEnabledProc();
		}

		///<summary>Attaches this payment split to the payment plan passed in. Also attaches the split to the oldest payment plan charge due if present.</summary>
		private void AttachPayPlan(PayPlan payPlan) {
			//Always associate the payment split to the payment plan passed in.
			PaySplitCur.PayPlanNum=payPlan.PayPlanNum;
			//Don't attach a payment plan charge if none are due.
			long payPlanChargeNum=0;
			//Get all charges that are due for that payment plan.
			List<PayPlanCharge> listPayPlanChargesCurrent=PayPlanCharges.GetDueForPayPlan(payPlan,payPlan.Guarantor);
			if(listPayPlanChargesCurrent.Count > 0) {
				//Get the PayPlanChargeNum from the oldest charge.
				payPlanChargeNum=listPayPlanChargesCurrent.OrderBy(x => x.ChargeDate).First().PayPlanChargeNum;
			}
			//This will either clear out PayPlanChargeNum or will be the oldest payment plan charge that is due.
			PaySplitCur.PayPlanChargeNum=payPlanChargeNum;
		}

		private void checkPayPlan_Click(object sender, System.EventArgs e) {
			if(!checkPayPlan.Checked) {
				//The user manually unchecked 'Attached to Payment Plan'. Detach the split from the payment plan.
				DetachPayPlan();
				return;
			}
			//Do not let users associated payment splits to payment plans when 'Is from another family' is checked.
			if(checkPatOtherFam.Checked) {
				checkPayPlan.Checked=false;
				return;
			}
			if(comboUnearnedTypes.SelectedIndex!=0 && _procedure!=null && _procedure.ProcStatus==ProcStat.TP) {
				MsgBox.Show("Treatment planned unearned cannot be applied to payment plans.");
				checkPayPlan.Checked=false;
				return;
			}
			//Get all payment plans that the selected patient is associated to. Do not include insurance payment plans.
			List<PayPlan> listPayPlans=PayPlans.GetForPatNum(_family.ListPats[listPatient.SelectedIndex].PatNum).FindAll(x => x.PlanNum==0);
			if(listPayPlans.Count==0) {
				MsgBox.Show(this,"The selected patient is not the guarantor for any payment plans.");
				checkPayPlan.Checked=false;
				return;
			}
			//Automatically associate the split to the payment plan if there is only one valid payment plan.
			if(listPayPlans.Count==1) {
				AttachPayPlan(listPayPlans[0]);
				return;
			}
			//Have the user select which payment plan to associate this split to when there is more than one valid payment plan.
			using FormPayPlanSelect formPayPlanSelect=new FormPayPlanSelect(listPayPlans);
			formPayPlanSelect.ShowDialog();
			if(formPayPlanSelect.DialogResult==DialogResult.Cancel) {
				checkPayPlan.Checked=false;
				return;
			}
			PayPlan payPlanSelect=listPayPlans.First(x => x.PayPlanNum==formPayPlanSelect.PayPlanNumSelected);
			AttachPayPlan(payPlanSelect);
		}

		private string SecurityLogEntryHelper(string oldVal,string newVal,string textInLog) {
			if(oldVal!=newVal) {
				return "\r\n "+textInLog+" changed from '"+oldVal+"' to '"+newVal+"'";
			}
			return "";
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Item?")) {
				return;
			}
			if(IsNew) {
				PaySplitCur=null;
				DialogResult=DialogResult.Cancel;
				return;
			}
			//This is the main problem with leaving it up to engineers to manually set public variables before showing forms...
			//We have been getting null reference reports from this security log entry.
			//Only check if PaySplitCur is null because _paySplitCopy gets set OnLoad() which must have been invoked especially if they clicked Delete.
			if(PaySplitCur!=null) {
				SecurityLogs.MakeLogEntry(EnumPermType.PaymentEdit,PaySplitCur.PatNum,PaySplits.GetSecurityLogMsgDelete(PaySplitCur),0,_paySplitCopy.SecDateTEdit);
			}
			PaySplitCur=null;
			DialogResult=DialogResult.OK;
		}

		private bool IsValid() {
			if(!textAmount.IsValid() || !textDatePay.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(PaySplitCur.PayPlanNum > 0 && comboUnearnedTypes.SelectedIndex!=0 && _procedure!=null && _procedure.ProcStatus==ProcStat.TP) {
				MsgBox.Show("Treatment planned unearned cannot be applied to payment plans.");
				return false;
			}
			//check for TP pre-pay changes. If money has been detached from procedure it needs to be transferred to regular unearned if had been hidden.
			if(_procedureOld!=null && _procedureOld.ProcStatus==ProcStat.TP && _procedure==null && !string.IsNullOrEmpty(_defUnearnedOld?.ItemValue)) {
				//user has detached the hidden paysplit 
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Hidden split is no longer attached to a procedure. Change unearned type to default?")) {
					comboUnearnedTypes.SetSelectedDefNum(PrefC.GetLong(PrefName.PrepaymentUnearnedType));
					PaySplitCur.UnearnedType=comboUnearnedTypes.GetSelectedDefNum();
				}
				if(!PrefC.GetBool(PrefName.AllowPrepayProvider)) {
					PaySplitCur.ProvNum=0;
				}
			}
			double amount=PIn.Double(textAmount.Text);
			if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully && PaySplitCur.UnearnedType!=0 && _procedure!=null 
				&& !_isEditAnyway && _procedure.ProcStatus!=ProcStat.TP) 
			{
				MsgBox.Show(this,"Cannot have an unallocated split that also has an attached procedure.");
				return false;
			}
			if(_procedure!=null && _adjustment!=null) {//should not be possible, but as an extra precaution because this is not allowed. 
				MsgBox.Show(this,"Cannot have a paysplit with both a procedure and an adjustment.");
				return false;
			}
			if(_remainAmt<0 && _procedure!=null) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remaining amount is negative.  Continue?","Overpaid Procedure Warning")) {
					return false;
				}
			}
			if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully && _procedure==null && PaySplitCur.UnearnedType==0 
					&& _adjustment==null && PaySplitCur.PayPlanChargeNum==0) 
			{
				MsgBox.Show(this,"You must attach a procedure, adjustment, or payment plan to this payment.");
				return false;
			}
			if(comboUnearnedTypes.SelectedIndex==0 && comboProvider.SelectedIndex==0) {
				MsgBox.Show(this,"Please select an unearned type or a provider.");
				return false;
			}
			//at this point in time, TP procs are allowed to have providers even if provs are typically not allowed on prepayments.
			if((_procedure==null || _procedure.ProcStatus==ProcStat.C) && PaySplitCur.UnearnedType!=0) {
				if(PaySplitCur.ProvNum>0 && !PrefC.GetBool(PrefName.AllowPrepayProvider)) {
					PaySplitCur.UnearnedType=0;
				}
				else if(PaySplitCur.ProvNum<=0){
					if(comboUnearnedTypes.SelectedIndex==0 
						&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"Having a provider of \"None\" will mark this paysplit as a prepayment.  Continue?")) 
					{
						return false;
					}
					PaySplitCur.ProvNum=0;//This means it's unallocated.
					if(comboUnearnedTypes.SelectedIndex==0) {
						PaySplitCur.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);
					}
					else {
						PaySplitCur.UnearnedType=comboUnearnedTypes.GetSelectedDefNum();
					}
				}
			}
			return true;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!IsValid()) {
				return;
			}
			double amount=PIn.Double(textAmount.Text);
			PaySplitCur.DatePay=PIn.Date(textDatePay.Text);//gets overwritten anyway
			PaySplitCur.SplitAmt=amount;
			PaySplitCur.ProcNum=_procedure == null ? 0 : _procedure.ProcNum;
			PaySplitCur.AdjNum=_adjustment == null ? 0 : _adjustment.AdjNum;
			string secLogText;
			if(IsNew) {
				secLogText="Paysplit created";
				if(_isEditAnyway) {
					secLogText+=" using Edit Anyway";
				}
				string providerAbbr=Providers.GetAbbr(PaySplitCur.ProvNum);
				if(providerAbbr.IsNullOrEmpty()) {
					secLogText+=" without a provider";
				}
				else {
					secLogText+=" with provider "+providerAbbr;
				}
				if(Clinics.GetAbbr(PaySplitCur.ClinicNum)!="") {
					secLogText+=", clinic "+Clinics.GetAbbr(PaySplitCur.ClinicNum);
				}
				secLogText+=", amount "+PaySplitCur.SplitAmt.ToString("F");
				SecurityLogs.MakeLogEntry(EnumPermType.PaymentEdit,PaySplitCur.PatNum,secLogText);
				DialogResult=DialogResult.OK;
				return;
			}
			secLogText="Paysplit edited";
			if(_isEditAnyway) {
				secLogText+=" using Edit Anyway";
			}
			secLogText+=SecurityLogEntryHelper(Providers.GetAbbr(_paySplitCopy.ProvNum),Providers.GetAbbr(PaySplitCur.ProvNum),"provider");
			secLogText+=SecurityLogEntryHelper(Clinics.GetAbbr(_paySplitCopy.ClinicNum),Clinics.GetAbbr(PaySplitCur.ClinicNum),"clinic");
			secLogText+=SecurityLogEntryHelper(_paySplitCopy.SplitAmt.ToString("F"),PaySplitCur.SplitAmt.ToString("F"),"amount");
			secLogText+=SecurityLogEntryHelper(_paySplitCopy.PatNum.ToString(),PaySplitCur.PatNum.ToString(),"patient number");
			SecurityLogs.MakeLogEntry(EnumPermType.PaymentEdit,PaySplitCur.PatNum,secLogText,0,_paySplitCopy.SecDateTEdit);
			DialogResult=DialogResult.OK;
		}

		private void FormPaySplitEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(IsNew) {
				PaySplitCur=null;
			}
			if(PaySplitCur==null) {
				return;
			}
			PaySplitCur.ClinicNum=_paySplitCopy.ClinicNum;
			PaySplitCur.DateEntry=_paySplitCopy.DateEntry;
			PaySplitCur.DatePay=_paySplitCopy.DatePay;
			PaySplitCur.DiscountType=_paySplitCopy.DiscountType;
			PaySplitCur.IsDiscount=_paySplitCopy.IsDiscount;
			PaySplitCur.PatNum=_paySplitCopy.PatNum;
			PaySplitCur.PayNum=_paySplitCopy.PayNum;
			PaySplitCur.PayPlanNum=_paySplitCopy.PayPlanNum;
			PaySplitCur.ProcDate=_paySplitCopy.ProcDate;
			PaySplitCur.ProcNum=_paySplitCopy.ProcNum;
			PaySplitCur.ProvNum=_paySplitCopy.ProvNum;
			PaySplitCur.SecDateTEdit=_paySplitCopy.SecDateTEdit;
			PaySplitCur.SecUserNumEntry=_paySplitCopy.SecUserNumEntry;
			PaySplitCur.SplitAmt=_paySplitCopy.SplitAmt;
			PaySplitCur.SplitNum=_paySplitCopy.SplitNum;
			PaySplitCur.UnearnedType=_paySplitCopy.UnearnedType;
			ListPaySplits.Where(x => x.IsSame(_paySplitCopy)).ForEach(x => x.ProcNum=_paySplitCopy.ProcNum);
		}

	}
}
