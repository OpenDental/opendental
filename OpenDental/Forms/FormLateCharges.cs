using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FormLateCharges:FormODBase {
		///<summary></summary>
		public FormLateCharges() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary></summary>
		private void FormLateCharges_Load(object sender,EventArgs e) {
			if(PrefC.GetLong(PrefName.LateChargeAdjustmentType)==0) {
				MsgBox.Show(this,"Late charge adjustment type has not been set. Please go to Setup | Account to fix this.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(Defs.GetDefsForCategory(DefCat.BillingTypes,true).Count==0) {//highly unlikely that this would happen
				MsgBox.Show(this,"No billing types have been set up or are visible.");
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		///<summary>The following logic was moved into a Shown method, rather than a Load method, because the progress bar causes the window to pop behind FormOpenDental when in Load.</summary>
		private void FormLateCharges_Shown(object sender,EventArgs e) {
			if(!RunAgingEnterprise(true)) {
				MsgBox.Show(this,"There was a problem calculating aging.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			listBillType.Items.Clear();
			listBillType.Items.AddList(Defs.GetDefsForCategory(DefCat.BillingTypes,true),x => x.ItemName);
			List<long> listDefaultBillTypes=PrefC.GetString(PrefName.LateChargeDefaultBillingTypes)
				.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)
				.Select(x => PIn.Long(x,false))
				.ToList();
			for(int i=0;i<listBillType.Items.Count;i++) {
				if(listDefaultBillTypes.Contains(((Def)listBillType.Items.GetObjectAt(i)).DefNum)) {
					listBillType.SetSelected(i,true);
				}
			}
			//Using Prefs.UpdateString instead of Prefs.UpdateDouble or Prefs.UpdateInt because we need to destenguish between a blank string and 0.
			DateTime dateLastRun=PrefC.GetDate(PrefName.LateChargeLastRunDate);
			textDateNewCharges.Text=DateTime.Today.ToShortDateString();
			checkExcludeAccountNoTil.Checked=PrefC.GetBool(PrefName.LateChargeExcludeAccountNoTil);
			checkExcludeExistingLateCharges.Checked=PrefC.GetBool(PrefName.LateChargeExcludeExistingLateCharges);
			textExcludeLessThan.Text=PrefC.GetString(PrefName.LateChargeExcludeBalancesLessThan);
			textChargePercent.Text=PrefC.GetString(PrefName.LateChargePercent);
			textMinCharge.Text=PrefC.GetString(PrefName.LateChargeMin);
			textMaxCharge.Text=PrefC.GetString(PrefName.LateChargeMax);
			textDateRangeEnd.Text=PrefC.GetString(PrefName.LateChargeDateRangeEnd);
			textDateRangeStart.Text=PrefC.GetString(PrefName.LateChargeDateRangeStart);
			if(dateLastRun.Year<1880) {//Has never been run.
				textDateUndo.Text="";
				textDateLastRun.Text="";
			}
			else {
				textDateLastRun.Text=dateLastRun.ToShortDateString();
				textDateUndo.Text=dateLastRun.ToShortDateString();
			}
			comboSpecificProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(Clinics.ClinicNum));
			comboSpecificProv.SelectedIndex=0;
			comboSpecificProv.Enabled=false;
			radioPatPriProv.Checked=true;
		}

		///<summary>Runs enterprise aging. Returns false if there are any errors while running aging. Changes to this method will also need to be made to 
		///OpenDentalService.ProcessStatements.RunAgingEnterprise().</summary>
		private bool RunAgingEnterprise(bool isOnLoad) {
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			DateTime dateTimeToday=dateTimeNow.Date;
			DateTime dateLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(isOnLoad && dateLastAging.Date==dateTimeToday) {
				return true;//this is prior to inserting/deleting charges and aging has already been run for this date
			}
			Prefs.RefreshCache();
			DateTime dateTAgingBeganPref=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(dateTAgingBeganPref>DateTime.MinValue) {
				if(isOnLoad) {
					MessageBox.Show(this,Lan.g(this,"In order to add late charges, aging must be calculated, but you cannot run aging until it has finished "
						+"the current calculations which began on")+" "+dateTAgingBeganPref.ToString()+".\r\n"+Lans.g(this,"If you believe the current aging "
						+"process has finished, a user with SecurityAdmin permission can manually clear the date and time by going to Setup | Preferences | Account - General and "
						+"pressing the 'Clear' button."));
				}
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Late Charges window");
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dateTimeNow,false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				Ledgers.ComputeAging(0,dateTimeToday);
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dateTimeToday,false));
			};
			progressOD.StartingMessage=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dateTimeToday.ToShortDateString()+"...";
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				Ledgers.AgingExceptionHandler(ex,this);
			}
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");//clear lock on pref whether aging was successful or not
			Signalods.SetInvalid(InvalidType.Prefs);
			if(!progressOD.IsSuccess) {
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - Late Charges window");
			return true;
		}

		///<summary>Checks validity of the controls in the Late charge settings group box.</summary>
		private bool AreLateChargeSettingsValid() {
			if(textExcludeLessThan.IsValid()
				&& textChargePercent.IsValid()
				&& textMinCharge.IsValid()
				&& textMaxCharge.IsValid()
				&& textDateRangeEnd.IsValid()
				&& textDateRangeStart.IsValid())
			{
				return true;
			}
			return false;
		}

		///<summary>All controls in the Late charge settings groupbox and the Date of new charges must be filled before running.</summary>
		private bool AreRequiredFieldsBlank() {
			if(textExcludeLessThan.Text==""
				|| textMinCharge.Text==""
				|| textMaxCharge.Text==""
				|| textMinCharge.Text==""
				|| textDateRangeEnd.Text==""
				|| textDateRangeStart.Text==""
				|| textDateNewCharges.Text==""
				|| listBillType.SelectedIndices.Count==0)
			{
				return true;
			}
			return false;
		}

		///<summary>Checks the validity of all fields required for running late charges.</summary>
		private bool HasErrors() {
			if(!textDateNewCharges.IsValid()
				|| !AreLateChargeSettingsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return true;
			}
			RoundDoubleFields();
			if(AreRequiredFieldsBlank()) {
				MsgBox.Show(this,"All fields other than the percentage must be filled out and at least one Billing Type must be selected.");
				return true;
			}
			if(PIn.Date(textDateNewCharges.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Adjustments cannot be made for future dates. Late charges were not made.");
				return true;
			}
			if(CompareDouble.IsLessThanOrEqualToZero(PIn.Double(textMaxCharge.Text))) {
				MsgBox.Show(this,"The maximum charge is less than or equal to zero. Late Charges were not made.");
				return true;
			}
			if(CompareDouble.IsLessThan(PIn.Double(textMaxCharge.Text),PIn.Double(textMinCharge.Text))) {
				MsgBox.Show(this,"The maximum charge must be greater than or equal to the minimum charge.");
				return true;
			}
			return false;
		}

		///<summary>Only call after AreLateChargeSettingsValid() returns false. Rounds all double fields to two decimal places.</summary>
		private void RoundDoubleFields() {
			textExcludeLessThan.Text=PIn.Double(textExcludeLessThan.Text).ToString("f");
			textMinCharge.Text=PIn.Double(textMinCharge.Text).ToString("f");
			textMaxCharge.Text=PIn.Double(textMaxCharge.Text).ToString("f");
		}

		///<summary>Saves preferences for all of the fields in the Late charge settings groupbox.</summary>
		private void butSaveDefaults_Click(object sender,EventArgs e) {
			if(!AreLateChargeSettingsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			RoundDoubleFields();
			if(!AreLateChargeSettingsValid()) {//Check again because a blank cell will not be considered invalid, even when the minimum value is not zero.
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			string selectedBillingTypes="";
			if(listBillType.SelectedIndices.Count>0) {
				selectedBillingTypes=string.Join(",",listBillType.GetListSelected<Def>().Select(x => x.DefNum));
			}
			//Using Prefs.UpdateString instead of Prefs.UpdateDouble or Prefs.UpdateInt becuase we need to destenguish between a blank string and 0.
			if(Prefs.UpdateString(PrefName.LateChargeDefaultBillingTypes,selectedBillingTypes)//comma delimited list of DefNums.
				| Prefs.UpdateBool(PrefName.LateChargeExcludeAccountNoTil,checkExcludeAccountNoTil.Checked)
				| Prefs.UpdateBool(PrefName.LateChargeExcludeExistingLateCharges,checkExcludeExistingLateCharges.Checked)
				| Prefs.UpdateString(PrefName.LateChargeExcludeBalancesLessThan,textExcludeLessThan.Text)
				| Prefs.UpdateString(PrefName.LateChargePercent,textChargePercent.Text)
				| Prefs.UpdateString(PrefName.LateChargeMin,textMinCharge.Text)
				| Prefs.UpdateString(PrefName.LateChargeMax,textMaxCharge.Text)
				| Prefs.UpdateString(PrefName.LateChargeDateRangeEnd,textDateRangeEnd.Text)
				| Prefs.UpdateString(PrefName.LateChargeDateRangeStart,textDateRangeStart.Text))
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		///<summary>Returns the Specific Provider if one is set, otherwise returns priProvNum.</summary>
		private long GetProvNumForLateCharge(long priProvNum) {
			if(radioSpecificProv.Checked) {
				return comboSpecificProv.GetSelectedProvNum();
			}
			return priProvNum;
		}

		///<summary>Determines if we should use the percentage of the statement balance, the min charge, or the max charge.</summary>
		private decimal CalculateLateChargeAmount(decimal statementBalanceRemaining) {
			decimal minCharge=PIn.Decimal(textMinCharge.Text);
			decimal maxCharge=PIn.Decimal(textMaxCharge.Text);
			int percent=PIn.Int(textChargePercent.Text);
			if(percent==0) {
				return minCharge;
			}
			decimal chargeAmount=(((decimal)percent)/100)*statementBalanceRemaining;
			if(!CompareDecimal.IsGreaterThan(chargeAmount,minCharge)) {
				return minCharge;
			}
			if(CompareDecimal.IsGreaterThan(chargeAmount,maxCharge)) {
				return maxCharge;
			}
			return Math.Round(chargeAmount,2);
		}

		///<summary>Iterates over statement data, making a late charge for each statement that has a remaining balance. Avoids double charging for production items that are on multiple statements.</summary>
		private List<long> CreateLateCharges(List<StatementData> listStatementDatas,DateTime dateForCharges,DateTime dateRangeEnd)	{
			//Holds the balances of all account entries that represent procs, adjustments, or payplancharges for each family.
			List<long> listGuarantorNums=listStatementDatas.SelectMany(x => x.ListGuarantorPatNums).Distinct().ToList();
			List<FamilyProdBalances> listFamilyProdBalances=new List<FamilyProdBalances>();
			for(int i=0;i<listGuarantorNums.Count;i++){
				listFamilyProdBalances.Add(new FamilyProdBalances(listGuarantorNums[i]));
			}
			//Create
			List<long> listLateChargeAdjNums=new List<long>();
			//Looping through each statement data (family) set to create late charges.
			for(int i=0;i<listStatementDatas.Count;i++) {
				decimal statementBalanceRemaining=CalcStatementBalanceRemaining(listStatementDatas[i],listFamilyProdBalances,
					out List<long> listProcNumsOnLateCharge,out List<long> listAdjNumsOnLateCharge,out List<long> listPayPlanChargeNumsOnLateCharge);
				if(CompareDecimal.IsLessThanOrEqualToZero(statementBalanceRemaining)) {
					continue;//None of the production items on this statement still have a balance, so do nothing.
				}
				decimal lateChargeAmount=CalculateLateChargeAmount(statementBalanceRemaining);
				if(CompareDecimal.IsLessThanOrEqualToZero(lateChargeAmount)) {
					continue;//Only create an adjustment if the late charge amount is greater than zero.
				}
				long provNum=GetProvNumForLateCharge(listStatementDatas[i].GuarantorPriProvNum);
				long adjNum=Adjustments.CreateLateChargeAdjustment((double)lateChargeAmount,dateForCharges
					,provNum,listStatementDatas[i].GuarantorPatNum,listStatementDatas[i].DateSent,
					dateRangeEnd,listProcNumsOnLateCharge,listAdjNumsOnLateCharge,listPayPlanChargeNumsOnLateCharge);
				listLateChargeAdjNums.Add(adjNum);
			}
			return listLateChargeAdjNums;
		}

		///<summary>Helper method that calculates the remaining balance for a statement.</summary>
		private decimal CalcStatementBalanceRemaining(StatementData statementData,List<FamilyProdBalances> listFamilyProdBalances
			,out List<long> listProcNumsOnLateCharge,out List<long> listAdjNumsOnLateCharge,out List<long> listPayPlanChargeNumsOnLateCharge)
		{
			listProcNumsOnLateCharge=new List<long>();
			listAdjNumsOnLateCharge=new List<long>();
			listPayPlanChargeNumsOnLateCharge=new List<long>();
			decimal statementBalanceRemaining=0;
			for(int i=0;i<statementData.ListStatementProds.Count;i++) {
				StatementProd statementProd=statementData.ListStatementProds[i];
				decimal prodItemBalance=FindAndRemoveStatementProdBalance(statementProd,statementData.ListGuarantorPatNums,listFamilyProdBalances);
				if(CompareDecimal.IsLessThanOrEqualToZero(prodItemBalance)) {
					continue;//Production item doesn't have a positive balance, we removed it, or it wasn't found, so no late charge can be assessed.
				}
				statementBalanceRemaining+=prodItemBalance;
				//Add the Fkey of the production item to the appropriate list.
				//These will be used to update the StatementProds with the AdjNum of the late charge.
				switch(statementProd.ProdType) {
					case ProductionType.Procedure:
						listProcNumsOnLateCharge.Add(statementProd.FKey);
						break;
					case ProductionType.Adjustment:
						listAdjNumsOnLateCharge.Add(statementProd.FKey);
						break;
					case ProductionType.PayPlanCharge:
						listPayPlanChargeNumsOnLateCharge.Add(statementProd.FKey);
						break;
				}
			}
			return statementBalanceRemaining;
		}

		///<summary>Helper method finds the balance of a specific production item and removes it from the FamilyProdBalances so that we don't make a late charge for it twice.</summary>
		private decimal FindAndRemoveStatementProdBalance(StatementProd statementProd,List<long> listGuarantorPatNums
			, List<FamilyProdBalances> listFamilyProdBalances)
		{
			for(int i=0;i<listGuarantorPatNums.Count;i++) {
				FamilyProdBalances familyProdBalances=listFamilyProdBalances.Find(x=>x.GuarantorPatNum==listGuarantorPatNums[i]);
				if(familyProdBalances is null){
					continue;
				}
				decimal amount=familyProdBalances.GetAmountForStatementProdIfExists(statementProd);
				return amount;
			}
			return 0;
		}

		///<summary></summary>
		private void radioPatPriProv_Click(object sender,EventArgs e) {
			comboSpecificProv.Enabled=!radioPatPriProv.Checked;
		}

		///<summary></summary>
		private void radioSpecificProv_Click(object sender,EventArgs e) {
			comboSpecificProv.Enabled=radioSpecificProv.Checked;
		}

		///<summary>Deletes the adjustments for late charges on the last date they were run. It is possible to miss some of the adjustments if the user has manually changed the date of late charge adjustments. Runs aging after undoing charges.</summary>
		private void butUndo_Click(object sender,EventArgs e) {
			if(textDateUndo.Text=="") {
				MsgBox.Show(this,"Late Charges have never been run. There are no late charge adjustments to undo.");
				return;
			}
			string stringMessageConfirm=Lan.g(this,"Delete all Late Charge adjustments from")+" "+PIn.Date(textDateUndo.Text).ToShortDateString()+"?";
			if(MessageBox.Show(this,stringMessageConfirm,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			Adjustments.ChargeUndoData chargeUndoDataLate=new Adjustments.ChargeUndoData();
			DateTime dateUndo=PIn.Date(textDateUndo.Text);
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				chargeUndoDataLate=Adjustments.UndoLateCharges(dateUndo);
			};
			progressOD.StartingMessage=Lan.g(this,"Deleting Late Charge Adjustments...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled) {
				MsgBox.Show(this,"Some Late Charges may have been deleted before you cancelled. " +
					"Search for Audit Trail entries under the AdjustmentEdit permission to see any Late Charges that were deleted.");
				return;
				//Cancelling aborts the thread. This may result in a log being created for an adjustment that is not deleted, or the StatementProds to
				//not be updated for a deleted adjustment.
			}
			MessageBox.Show(Lan.g(this,$"Late charge adjustments deleted:")+$" {chargeUndoDataLate.CountDeletedAdjustments.ToString()}");
			if(!chargeUndoDataLate.ListSkippedPatNums.IsNullOrEmpty()
				&& MsgBox.Show(this,MsgBoxButtons.YesNo,"Some late charges could not be deleted because they have pay splits or a dynamic payment plans attached. "
				+"Would you like to see a list of the patients for whom we could not delete late charges?"))
			{
				string message="";
				List<Patient> listPatients=Patients.GetLimForPats(chargeUndoDataLate.ListSkippedPatNums);
				for(int i=0;i<listPatients.Count;i++) {
					Patient patient=listPatients[i];
					message+=$"PatNum: {patient.PatNum}, {patient.GetNameFL()}\r\n";
				}
				using MsgBoxCopyPaste msgBoxCopyPasteSkipped=new MsgBoxCopyPaste(message);
				msgBoxCopyPasteSkipped.ShowDialog();
			}
			if(chargeUndoDataLate.CountDeletedAdjustments==0) {
				DialogResult=DialogResult.OK;
				return;//No late charges were deleted, so no need to run aging.
			}
			if(!RunAgingEnterprise(false)) {
				MsgBox.Show(this,"There was an problem calculating aging. You should run aging later to update affected accounts.");
			}
			DialogResult=DialogResult.OK;
		}

		///<summary>Gets statement data and creates late charge adjustments for statements that have a remaining balance. Charges are applied to the family guarantor. Runs aging after creating charges.</summary>
		private void butRun_Click(object sender,EventArgs e) {
			if(HasErrors()) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Run Late Charges?")) {
				return;
			}
			List<StatementData> listStatementDatas=new List<StatementData>();
			List<long> listSelectedBillingTypes=listBillType.GetListSelected<Def>().Select(x => x.DefNum).ToList();
			DateTime dateRangeEnd=DateTime.Now.AddDays(-PIn.Int(textDateRangeEnd.Text)).Date;
			DateTime dateRangeStart=DateTime.Now.AddDays(-PIn.Int(textDateRangeStart.Text)).Date;
			DateTime dateTemp;
			DateTime dateNewCharges=PIn.Date(textDateNewCharges.Text);
			//Users can enter the start or end of the date range in either field, so we figure out which is which here and assign them accordingly.
			if(dateRangeEnd < dateRangeStart) {
				dateTemp=dateRangeStart;
				dateRangeStart=dateRangeEnd;
				dateRangeEnd=dateTemp;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				listStatementDatas=StatementData.GetListStatementDataForLateCharges(checkExcludeAccountNoTil.Checked,checkExcludeExistingLateCharges.Checked,
					PIn.Decimal(textExcludeLessThan.Text),dateRangeStart,dateRangeEnd,listSelectedBillingTypes);
			};
			progressOD.StartingMessage=Lans.g(this,"Getting statements...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled) {
				return;//No Edits to DB to worry about.
			}
			List<long> listLateChargeAdjNums=new List<long>();
			progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				listLateChargeAdjNums=CreateLateCharges(listStatementDatas,dateNewCharges,dateRangeEnd);
			};
			progressOD.StartingMessage=Lans.g(this,"Creating Late Charges...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled) {
				MsgBox.Show(this,"Some Late Charges may have been made before you cancelled. " +
					"Search for Audit Trail entries under the AdjustmentCreate permission to find any Late Charges tha were created.");
				return;
				//Cancelling on the progress bar aborts the thread. This may result in a log being created for an adjustment that didn't get inserted,
				//or the statementprods to not be updated with the adjustments AdjNum.
			}
			MessageBox.Show(this,Lan.g(this,"Late Charges ran successfully. Adjustments created:")+" "+listLateChargeAdjNums.Count.ToString());
			if(Prefs.UpdateDateT(PrefName.LateChargeLastRunDate,dateNewCharges)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(listLateChargeAdjNums.IsNullOrEmpty()) {//No adjustments were made so we don't want to run aging.
				DialogResult=DialogResult.OK;
				return;
			}
			if(RunAgingEnterprise(false)){
				DialogResult=DialogResult.OK;
				return;
			}
			MsgBox.Show(this,"There was an problem calculating aging. You should run aging later to update affected accounts.");
			DialogResult=DialogResult.OK;
		}

		///<summary></summary>
		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}