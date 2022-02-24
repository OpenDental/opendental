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
		public List<PaySplit> ListSplitsCur;
		///<summary>The pattern in this window is different than other windows.  We generally keep PaySplitCur updated as we go, rather than when we click OK. This is because we sometimes send it out for calcs.</summary>
		public PaySplit PaySplitCur;
		#endregion
		#region _private variables
		private bool _isEditAnyway;
		private decimal _remainAmt;
		private decimal _patPort;
		private Family _famCur;
		private PaySplit _paySplitCopy;
		private Procedure ProcCur;
		private Adjustment _adjCur;
		private double _adjPrevPaid;
		private Procedure _procedureOld;
		private Def _unearnedOld;
		#endregion

		public FormPaySplitEdit(Family famCur) {
			InitializeComponent();
			InitializeLayoutManager();
			_famCur=famCur;
			Lan.F(this);
		}

		private void FormPaySplitEdit_Load(object sender, System.EventArgs e) {
			List<PatientLink> listLinks=PatientLinks.GetLinks(_famCur.ListPats.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			List<Patient> listNonMergedPats=_famCur.ListPats.Where(x => !PatientLinks.WasPatientMerged(x.PatNum,listLinks)).ToList();
			//New object to break reference to famCur in calling method/class; avoids removing merged patients from original object.
			_famCur=new Family(listNonMergedPats);
			_paySplitCopy=PaySplitCur.Copy();
			textDateEntry.Text=PaySplitCur.DateEntry.ToShortDateString();
			textDatePay.Text=PaySplitCur.DatePay.ToShortDateString();
			textAmount.Text=PaySplitCur.SplitAmt.ToString("F");
			comboUnearnedTypes.Items.AddDefNone();
			comboUnearnedTypes.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,true));
			comboUnearnedTypes.SetSelectedDefNum(PaySplitCur.UnearnedType); 
			if(comboUnearnedTypes.SelectedIndex!=-1){
				_unearnedOld=comboUnearnedTypes.GetSelected<Def>();
			}
			if(PrefC.HasClinicsEnabled) {
				comboClinic.SelectedClinicNum=PaySplitCur.ClinicNum;
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
			ProcCur=PaySplitCur.ProcNum==0 ? null : Procedures.GetOneProc(PaySplitCur.ProcNum,false);
			_adjCur=PaySplitCur.AdjNum==0 ? null : Adjustments.GetOne(PaySplitCur.AdjNum);
			if(ProcCur!=null) {
				_procedureOld=ProcCur.Copy();
				tabAdjustment.Enabled=false;//Intellisense doesn't know this is here for some reason.  Shhh it's a secret.
				tabControl.SelectedIndex=0;//Set it on Proc tab automagically (this is just a safety precaution, it should be 0 already).
			}
			else if(_adjCur!=null) {
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
			if((ProcCur!=null || _adjCur!=null) && !_isEditAnyway && PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
				groupPatient.Enabled=false;
				comboProvider.Enabled=false;
				butPickProv.Enabled=false;
				if(PrefC.HasClinicsEnabled) {
					comboClinic.Enabled=false;
				}
				if(Security.IsAuthorized(Permissions.Setup,true)) {
					labelEditAnyway.Visible=true;
					butEditAnyway.Visible=true;
				}
			}
			else {
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
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			PaySplitCur.ClinicNum=comboClinic.SelectedClinicNum;
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
				comboUnearnedTypes.Enabled=false;
				PaySplitCur.UnearnedType=0;
			}
			else {
				comboUnearnedTypes.Enabled=true;
			}
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
				comboProvider.Enabled=false;
				butPickProv.Enabled=false;
				checkPayPlan.Checked=false;
				checkPayPlan.Enabled=false;
			}
			else {
				comboProvider.Enabled=true;
				butPickProv.Enabled=true;
				checkPayPlan.Enabled=true;
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formp = new FormProviderPick(comboProvider.Items.GetAll<Provider>());
			formp.SelectedProvNum=PaySplitCur.ProvNum;
			formp.ShowDialog();
			if(formp.DialogResult!=DialogResult.OK) {
				return;
			}
			PaySplitCur.ProvNum=formp.SelectedProvNum;
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
			for(int i=0;i<_famCur.ListPats.Length;i++){
				listPatient.Items.Add(_famCur.GetNameInFamLFI(i));
				if(PaySplitCur.PatNum==_famCur.ListPats[i].PatNum){
					listPatient.SelectedIndex=i;
				}
			}
			//this can happen if user unchecks the "Is From Other Fam" box. Need to reset.
			if(PaySplitCur.PatNum==0){
				listPatient.SelectedIndex=0;
				//the initial patient will be the first patient in the family, usually guarantor
				PaySplitCur.PatNum=_famCur.ListPats[0].PatNum;
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
				using FormPatientSelect FormPS=new FormPatientSelect();
				FormPS.SelectionModeOnly=true;
				FormPS.ShowDialog();
				if(FormPS.DialogResult!=DialogResult.OK){
					checkPatOtherFam.Checked=false;
					return;
				}
				PaySplitCur.PatNum=FormPS.SelectedPatNum;
			}
			else{//switch to family view
				PaySplitCur.PatNum=0;//this will reset the selected patient to current patient
			}
			if(!_isEditAnyway) {//When user clicks Edit Anyway they are specifically trying to correct a bad split, so don't clear it out.
				ProcCur=null;
				FillProcedure();
			}
			FillAdjustment();
			FillPatient();
		}

		private void listPatient_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(listPatient.SelectedIndex==-1){
				return;
			}
			PaySplitCur.PatNum=_famCur.ListPats[listPatient.SelectedIndex].PatNum;
		}

		private void FillProcedure(){
			if(ProcCur==null){
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
				if(_adjCur==null) {
					checkPatOtherFam.Enabled=true;
				}
				return;
			}
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(ProcCur.PatNum);
			Adjustment[] arrAdjustments=Adjustments.Refresh(ProcCur.PatNum);
			List<PaySplit> listPaySplitsForProc=PaySplits.Refresh(ProcCur.PatNum).Where(x => x.ProcNum==ProcCur.ProcNum).ToList();
			List<PaySplit> listPaySplitsForProcPaymentWindow=ListSplitsCur.Where(x => x.ProcNum==ProcCur.ProcNum && x.PayNum==PaySplitCur.PayNum).ToList();
			//Add new paysplits created for the current paysplits payment.
			listPaySplitsForProc.AddRange(listPaySplitsForProcPaymentWindow.Where(x => x.SplitNum==0));
			//Remove paysplits that have been deleted in the payment window but have not been saved to db. We don't want to use these paysplits 
			//when calculating procPrevPaid.
			listPaySplitsForProc.RemoveAll(x => !listPaySplitsForProcPaymentWindow.Any(y => y.IsSame(x)) && x.PayNum==PaySplitCur.PayNum);
			textProcDate.Text=ProcCur.ProcDate.ToShortDateString();
			textProcProv.Text=Providers.GetAbbr(ProcCur.ProvNum);
			textProcTooth.Text=Tooth.ToInternat(ProcCur.ToothNum);
			textProcDescription.Text="";
			if(ProcCur.ProcStatus==ProcStat.TP) {
				textProcDescription.Text="(TP) ";
			}
			textProcDescription.Text+=ProcedureCodes.GetProcCode(ProcCur.CodeNum).Descript;
			double procWriteoff=-ClaimProcs.ProcWriteoff(listClaimProcs,ProcCur.ProcNum);
			double procInsPaid=-ClaimProcs.ProcInsPay(listClaimProcs,ProcCur.ProcNum);
			double procInsEst=-ClaimProcs.ProcEstNotReceived(listClaimProcs,ProcCur.ProcNum);
			double procAdj=Adjustments.GetTotForProc(ProcCur.ProcNum,arrAdjustments);
			//next line will still work even if IsNew
			int countSplitsAttached;
			double procPrevPaid=-PaySplits.GetTotForProc(ProcCur.ProcNum,listPaySplitsForProc.ToArray(),PaySplitCur,out countSplitsAttached);
			//Intelligently sum the values associated to the procedure, claim procs, and adjustments via status instead of blindly adding them together.
			_patPort=ClaimProcs.GetPatPortion(ProcCur,listClaimProcs,arrAdjustments.ToList());
			textProcFee.Text=ProcCur.ProcFeeTotal.ToString("F");
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
				comboClinic.SelectedClinicNum=PaySplitCur.ClinicNum;
			}
			butAttachProc.Enabled=false;
			if(!_isEditAnyway && PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
				comboProvider.Enabled=false;
				comboClinic.Enabled=false;
				butPickProv.Enabled=false;
			}
			if(ProcCur.ProcStatus!=ProcStat.C) {
				comboUnearnedTypes.Enabled=true;
				if(ProcCur.ProcStatus==ProcStat.TP) {
					checkPayPlan.Checked=false;
				}
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
				comboClinic.SelectedClinicNum=PaySplitCur.ClinicNum;//not sure why this is here again
			}
			//Proc selected will always be for the pat this paysplit was made for
			listPatient.SelectedIndex=_famCur.ListPats.ToList().FindIndex(x => x.PatNum==PaySplitCur.PatNum);
			ComputeTotals();
			tabAdjustment.Enabled=false;
		}

		private void FillAdjustment() {
			if(_adjCur==null) {
				textAdjDate.Text="";
				textAdjProv.Text="";
				textAdjAmt.Text="";
				textAdjPrevUsed.Text="";
				_adjPrevPaid=0;
				textAdjPaidHere.Text="";
				labelAdjRemaining.Text="";
				tabProcedure.Enabled=true;//Intellisense doesn't know about this, but it does exist.
				butAttachAdjust.Enabled=true;
				if(ProcCur==null) {
					checkPatOtherFam.Enabled=true;
					groupPatient.Enabled=true;
				}
				return;
			}
			textAdjDate.Text=_adjCur.AdjDate.ToShortDateString();
			textAdjProv.Text=Providers.GetAbbr(_adjCur.ProvNum);
			textAdjAmt.Text=_adjCur.AdjAmt.ToString("F");//Adjustment's original amount
			//Don't include any splits on current payment - Since they could be modified and DB doesn't know about it yet.
			_adjPrevPaid=Adjustments.GetAmtAllocated(_adjCur.AdjNum,PaySplitCur.PayNum);
			//ListSplitsCur contains current paysplit, we need to remove it somehow.  PaySplitCur could have SplitNum=0 though.
			List<PaySplit> listSplits=ListSplitsCur.FindAll(x => x.AdjNum==_adjCur.AdjNum);
			if(listSplits.Count>0) {
				_adjPrevPaid+=listSplits.Sum(x => x.SplitAmt);
				//There needs to be something here so _adjPrevPaid isn't adjusted by current split amt if the split isn't in listSplits
				if(PaySplitCur.IsNew || (!PaySplitCur.IsNew && listSplits.Exists(x => x.SplitNum==PaySplitCur.SplitNum))) {
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
			comboProvider.SetSelectedProvNum(_adjCur.ProvNum);
			if(PrefC.HasClinicsEnabled) {
				comboClinic.SelectedClinicNum=_adjCur.ClinicNum;
			}
			//Proc selected will always be for the pat this paysplit was made for
			listPatient.SelectedIndex=_famCur.ListPats.ToList().FindIndex(x => x.PatNum==_adjCur.PatNum);
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
			if(ProcCur!=null) {
				_remainAmt=_patPort+(decimal)procPaidHere+PIn.Decimal(textProcPrevPaid.Text);
				labelProcRemain.Text=_remainAmt.ToString("c");
			}
			else if(_adjCur!=null) {
				_remainAmt=(decimal)_adjCur.AdjAmt-(decimal)_adjPrevPaid-(decimal)adjPaidHere;
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

		///<summary>Attaches procedure, sets the selected provider, and fills Procedure information.</summary>
		private void butAttachProc_Click(object sender, System.EventArgs e) {
			using FormProcSelect FormPS=new FormProcSelect(PaySplitCur.PatNum,false);
			FormPS.ListSplitsCur = ListSplitsCur;
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK){
				return;
			}
			ProcCur=FormPS.ListSelectedProcs[0];
			PaySplitCur.ProvNum=FormPS.ListSelectedProcs[0].ProvNum;
			PaySplitCur.ClinicNum=FormPS.ListSelectedProcs[0].ClinicNum;
			if(ProcCur.ProcStatus==ProcStat.TP) {
				PaySplitCur.UnearnedType=PrefC.GetLong(PrefName.TpUnearnedType);//use default tp unearned for tp procedures.
				comboUnearnedTypes.SetSelectedDefNum(PaySplitCur.UnearnedType);
			}
			else {
				comboUnearnedTypes.SelectedIndex=0;
				PaySplitCur.UnearnedType=0;
			}
			SetEnabledProc();
			FillProcedure();
			FillAdjustment();
		}

		private void butDetachProc_Click(object sender, System.EventArgs e) {
			if(ProcCur!=null) {
				ListSplitsCur.Where(x => x.ProcNum==ProcCur.ProcNum && x.IsSame(PaySplitCur))
					.ForEach(x => x.ProcNum=0);
			}
			ProcCur=null;
			SetEnabledProc();
			FillProcedure();
			FillAdjustment();
		}

		private void butAttachAdjust_Click(object sender,EventArgs e) {
			List<Adjustment> listPatAdjusts=Adjustments.GetAdjustForPats(new List<long>() { PaySplitCur.PatNum });
			List<PaySplit> listAdjPaySplits=PaySplits.GetForAdjustments(listPatAdjusts.Select(x => x.AdjNum).ToList());
			using FormAdjustSelect FormAS=new FormAdjustSelect(PIn.Double(textAmount.Text),PaySplitCur,ListSplitsCur,listPatAdjusts,listAdjPaySplits);
			if(FormAS.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_adjCur=FormAS.SelectedAdj;
			PaySplitCur.ProvNum=_adjCur.ProvNum;
			PaySplitCur.ClinicNum=_adjCur.ClinicNum;
			PaySplitCur.UnearnedType=0;
			SetEnabledProc();
			FillProcedure();
			FillAdjustment();
		}

		private void butDetachAdjust_Click(object sender,EventArgs e) {
			if(_adjCur!=null) {
				ListSplitsCur.Where(x => x.AdjNum==_adjCur.AdjNum && x.IsSame(PaySplitCur))
					.ForEach(x => x.AdjNum=0);
			}
			_adjCur=null;
			SetEnabledProc();//think about this.
			FillProcedure();
			FillAdjustment();
		}

		private void butEditAnyway_Click(object sender,EventArgs e) {
			_isEditAnyway=true;
			SetEnabledProc();
		}

		///<summary>Get the selected pay plan's current charges. If there is a charge, attach the split to that charge.</summary>
		private void AttachPlanCharge(PayPlan payPlan, long guar) {
			//get all current charges for that pay plan. If there are no current charges, don't allow the pay plan attach. 
			List<PayPlanCharge> listPayPlanChargesCurrent=PayPlanCharges.GetDueForPayPlan(payPlan,guar);
				if(listPayPlanChargesCurrent.Count==0) {
					//No current payments due for patient. Payment may be made ahead of schedule if procedure is attached.
					PaySplitCur.PayPlanChargeNum=0;
				}
				else {
					PaySplitCur.PayPlanChargeNum=listPayPlanChargesCurrent.OrderBy(x => x.ChargeDate).First().PayPlanChargeNum;//get oldest
				}
		}

		private void checkPayPlan_Click(object sender, System.EventArgs e) {
			if(checkPayPlan.Checked){
				if(checkPatOtherFam.Checked){//prevents a bug.
					checkPayPlan.Checked=false;
					return;
				}
				if(comboUnearnedTypes.SelectedIndex!=0 && ProcCur!=null && ProcCur.ProcStatus==ProcStat.TP) {
					MsgBox.Show("Treatment planned unearned cannot be applied to payment plans.");
					checkPayPlan.Checked=false;
					return;
				}
				//get all plans where the selected patient is the patnum or the guarantor of the payplan. Do not include insurance payment plans
				List<PayPlan> payPlanList=PayPlans.GetForPatNum(_famCur.ListPats[listPatient.SelectedIndex].PatNum).Where(x => x.PlanNum == 0).ToList();
				if(payPlanList.Count==0){//no valid plans
					MsgBox.Show(this,"The selected patient is not the guarantor for any payment plans.");
					checkPayPlan.Checked=false;
					return;
				}
				if(payPlanList.Count==1){ //if there is only one valid payplan
					PaySplitCur.PayPlanNum=payPlanList[0].PayPlanNum;
					AttachPlanCharge(payPlanList[0],payPlanList[0].Guarantor);
					return;
				}
				//more than one valid PayPlan
				using FormPayPlanSelect FormPPS=new FormPayPlanSelect(payPlanList);
				//FormPPS.ValidPlans=payPlanList;
				FormPPS.ShowDialog();
				if(FormPPS.DialogResult==DialogResult.Cancel){
					checkPayPlan.Checked=false;
					return;
				}
				PaySplitCur.PayPlanNum=FormPPS.SelectedPayPlanNum; 
				PayPlan selectPayPlan=payPlanList.FirstOrDefault(x => x.PayPlanNum==PaySplitCur.PayPlanNum);
				//get the selected pay plan's current charges. If there is a charge, attach the split to that charge.
				AttachPlanCharge(selectPayPlan,selectPayPlan.Guarantor);
			}
			else{//payPlan unchecked
				PaySplitCur.PayPlanNum=0;
				PaySplitCur.PayPlanChargeNum=0;
			}
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
				SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,PaySplitCur.PatNum,PaySplits.GetSecurityLogMsgDelete(PaySplitCur),0,_paySplitCopy.SecDateTEdit);
			}
			PaySplitCur=null;
			DialogResult=DialogResult.OK;
		}

		private bool IsValid() {
			if(!textAmount.IsValid() || !textDatePay.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			//check for TP pre-pay changes. If money has been detached from procedure it needs to be transferred to regular unearned if had been hidden.
			if(_procedureOld!=null && _procedureOld.ProcStatus==ProcStat.TP && ProcCur==null && !string.IsNullOrEmpty(_unearnedOld?.ItemValue)) {
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
			if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully && PaySplitCur.UnearnedType!=0 && ProcCur!=null 
				&& !_isEditAnyway && ProcCur.ProcStatus!=ProcStat.TP) 
			{
				MsgBox.Show(this,"Cannot have an unallocated split that also has an attached procedure.");
				return false;
			}
			if(ProcCur!=null && _adjCur!=null) {//should not be possible, but as an extra precaution because this is not allowed. 
				MsgBox.Show(this,"Cannot have a paysplit with both a procedure and an adjustment.");
				return false;
			}
			if(_remainAmt<0 && ProcCur!=null) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remaining amount is negative.  Continue?","Overpaid Procedure Warning")) {
					return false;
				}
			}
      if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully && ProcCur==null && PaySplitCur.UnearnedType==0 
					&& _adjCur==null && PaySplitCur.PayPlanChargeNum==0) 
			{
				MsgBox.Show(this,"You must attach a procedure, adjustment, or payment plan to this payment.");
				return false;
      }
			if(comboUnearnedTypes.SelectedIndex==0 && comboProvider.SelectedIndex==0) {
				MsgBox.Show(this,"Please select an unearned type or a provider.");
				return false;
			}
			//at this point in time, TP procs are allowed to have providers even if provs are typically not allowed on prepayments.
			if((ProcCur==null || ProcCur.ProcStatus==ProcStat.C) && PaySplitCur.UnearnedType!=0) {
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

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!IsValid()) {
				return;
			}
			double amount=PIn.Double(textAmount.Text);
			PaySplitCur.DatePay=PIn.Date(textDatePay.Text);//gets overwritten anyway
			PaySplitCur.SplitAmt=amount;
			PaySplitCur.ProcNum=ProcCur == null ? 0 : ProcCur.ProcNum;
			PaySplitCur.AdjNum=_adjCur == null ? 0 : _adjCur.AdjNum;
			if(IsNew) {
				string secLogText="Paysplit created";
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
				SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,PaySplitCur.PatNum,secLogText);
			}
			else {
				string secLogText="Paysplit edited";
				if(_isEditAnyway) {
					secLogText+=" using Edit Anyway";
				}
				secLogText+=SecurityLogEntryHelper(Providers.GetAbbr(_paySplitCopy.ProvNum),Providers.GetAbbr(PaySplitCur.ProvNum),"provider");
				secLogText+=SecurityLogEntryHelper(Clinics.GetAbbr(_paySplitCopy.ClinicNum),Clinics.GetAbbr(PaySplitCur.ClinicNum),"clinic");
				secLogText+=SecurityLogEntryHelper(_paySplitCopy.SplitAmt.ToString("F"),PaySplitCur.SplitAmt.ToString("F"),"amount");
				secLogText+=SecurityLogEntryHelper(_paySplitCopy.PatNum.ToString(),PaySplitCur.PatNum.ToString(),"patient number");
				SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,PaySplitCur.PatNum,secLogText,0,_paySplitCopy.SecDateTEdit);
			}
			DialogResult=DialogResult.OK;
		}

		private void ButCancel_Click(object sender, System.EventArgs e) {
			if(IsNew) {
				PaySplitCur=null;
			}
			DialogResult=DialogResult.Cancel;
		}

		private void FormPaySplitEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK || PaySplitCur==null) {
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
			ListSplitsCur.Where(x => x.IsSame(_paySplitCopy)).ForEach(x => x.ProcNum=_paySplitCopy.ProcNum);
		}
	}
}
