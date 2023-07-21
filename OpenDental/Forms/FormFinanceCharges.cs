using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
///<summary></summary>
	public partial class FormFinanceCharges : FormODBase {
		//private ArrayList ALPosIndices;
		private List<Def> _listDefsBillingType;
		///<summary>Filtered list of providers based on the current clinic--used to populate the Combo Box Providers.</summary>
		private List<Provider> _listProviders;
		private int _chargesAdded=0;
		private int _chargesProcessed=0;

		///<summary></summary>
		public FormFinanceCharges(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFinanceCharges_Load(object sender, System.EventArgs e) {
			if(PrefC.GetLong(PrefName.FinanceChargeAdjustmentType)==0){
				MsgBox.Show(this,"No finance charge adjustment type has been set.  Please go to Setup | Account to fix this.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(PrefC.GetLong(PrefName.BillingChargeAdjustmentType)==0){
				MsgBox.Show(this,"No billing charge adjustment type has been set.  Please go to Setup | Account to fix this.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			_listDefsBillingType=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			if(_listDefsBillingType.Count==0){//highly unlikely that this would happen
				MsgBox.Show(this,"No billing types have been set up or are visible.");
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		///<summary>The following logic was moved into a Shown method, rather than a Load method, because the progress bar causes the 
		///window to popbehind FormOpenDental when in Load.</summary>
		private void FormFinanceCharges_Shown(object sender,EventArgs e) {
			if(!RunAgingEnterprise(true)) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			textDate.Text=DateTime.Today.ToShortDateString();		
			textAPR.MaxVal=100;
			textAPR.MinVal=0;
			textAPR.Text=PrefC.GetString(PrefName.FinanceChargeAPR);
			textBillingCharge.Text=PrefC.GetString(PrefName.BillingChargeAmount);
			for(int i=0;i<_listDefsBillingType.Count;i++) {
				listBillType.Items.Add(_listDefsBillingType[i].ItemName);
				listBillType.SetSelected(i);
			}
			string defaultChargeMethod = PrefC.GetString(PrefName.BillingChargeOrFinanceIsDefault);
			if (defaultChargeMethod == "Finance") {
				radioFinanceCharge.Checked = true;
				textDateLastRun.Text=GetDateTimeForDisplay(PrefC.GetDate(PrefName.FinanceChargeLastRun));
				textDateUndo.Text=GetDateTimeForDisplay(PrefC.GetDate(PrefName.FinanceChargeLastRun));
				textBillingCharge.ReadOnly=true;
				textBillingCharge.BackColor=System.Drawing.SystemColors.Control;
			}
			else if (defaultChargeMethod == "Billing") {
				radioBillingCharge.Checked = true;
				textDateLastRun.Text=GetDateTimeForDisplay(PrefC.GetDate(PrefName.BillingChargeLastRun));
				textDateUndo.Text=GetDateTimeForDisplay(PrefC.GetDate(PrefName.BillingChargeLastRun));
			}
			textAtLeast.Text=PrefC.GetString(PrefName.FinanceChargeAtLeast);
			textOver.Text=PrefC.GetString(PrefName.FinanceChargeOnlyIfOver);
			checkExcludeInsPending.Checked=PrefC.GetBool(PrefName.BillingExcludeInsPending);
			if(checkExcludeInsPending.Checked) {
				textDaysInsPendingExclude.Value=PrefC.GetInt(PrefName.BillingDaysExcludeInsPending);
			}
			else {
				textDaysInsPendingExclude.Value=0;
			}
			textExcludeNotBilledSince.Text=GetDateTimeForDisplay(GetFinanceBillingLastRun());
			comboSpecificProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(Clinics.ClinicNum));
			comboSpecificProv.SelectedIndex=0;
			radioPatPriProv.Checked=true;
		}

		///<summary>Runs enterprise aging. Returns false if there are any errors while running aging. Changes to this method will also need to be made to OpenDentalService.ProcessStatements.RunAgingEnterprise().</summary>
		private bool RunAgingEnterprise(bool isOnLoad=false) {
			DateTime dateTNow=MiscData.GetNowDateTime();
			DateTime dateToday=dateTNow.Date;
			DateTime dateLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(isOnLoad && dateLastAging.Date==dateToday) {
				return true;//this is prior to inserting/deleting charges and aging has already been run for this date
			}
			Prefs.RefreshCache();
			DateTime dateTAgingBeganPref=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(dateTAgingBeganPref>DateTime.MinValue) {
				if(isOnLoad) {
					MessageBox.Show(this,Lan.g(this,"In order to add finance charges, aging must be calculated, but you cannot run aging until it has finished "
						+"the current calculations which began on")+" "+dateTAgingBeganPref.ToString()+".\r\n"+Lans.g(this,"If you believe the current aging "
						+"process has finished, a user with SecurityAdmin permission can manually clear the date and time by going to Setup | Preferences | Account - General and "
						+"pressing the 'Clear' button."));
				}
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Finance Charges window");
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dateTNow,false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=()=>{
				Ledgers.ComputeAging(0,dateToday);
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dateToday,false));
			};
			progressOD.StartingMessage=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dateToday.ToShortDateString()+"...";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				Ledgers.AgingExceptionHandler(ex,this);
			}
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");//clear lock on pref whether aging was successful or not
			Signalods.SetInvalid(InvalidType.Prefs);
			if(!progressOD.IsSuccess){
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - Finance Charges window");
			return true;
		}

		///<summary>Returns the short date time string version of the DateTime passed in, empty string if the year falls before 1880.</summary>
		private string GetDateTimeForDisplay(DateTime dateTime) {
			string strDateTime="";
			if(dateTime.Year>1880) {
				strDateTime=dateTime.ToShortDateString();
			}
			return strDateTime;
		}

		/// <summary>Gets an Aging List from the Finance/Billing Charges with the filter settings from the UI.</summary>
		private List<PatAging> GetFinanceBillingAgingList() {
			List<long> listSelectedBillTypes=listBillType.SelectedIndices.Select(x => _listDefsBillingType[x].DefNum).ToList();
			SerializableDictionary<long,PatAgingData> serializableDictionaryPatAgingData=AgingData.GetAgingData(isSinglePatient:false,includeChanged:true,
				checkExcludeInsPending.Checked,excludeIfUnsentProcs:false,isSuperBills:false,new List<long>(),daysExcludeInsPending:textDaysInsPendingExclude.Value);
			List<long> listPendingInsPatNums=new List<long>();
			List<long> listKeys=serializableDictionaryPatAgingData.Keys.ToList();//keys are almost certainly not the same as the value of i below
			if(checkExcludeInsPending.Checked) { //Only fill list if excluding pending ins
				for(int i=0;i<serializableDictionaryPatAgingData.Count;i++) { 
					if(serializableDictionaryPatAgingData[listKeys[i]].HasPendingIns) {
						listPendingInsPatNums.Add(listKeys[i]);
					}
				}
			}
			string age="";
			if(radio30.Checked) {
				age="30";
			}
			else if(radio60.Checked) {
				age="60";
			}
			else if(radio90.Checked) {
				age="90";
			}
			DateTime dateLastStatement=PIn.Date(textExcludeNotBilledSince.Text);
			bool hasFilterSinceLastStatement=true;
			//If the 'exclude accounts not billed since' date has been removed, select the latest Billing/Finance Statement date. 
			if(textExcludeNotBilledSince.Text=="") {
				dateLastStatement=GetFinanceBillingLastRun();
				//Because the user did not specify a filter date, we are NOT going to exclude accounts not billed since lastStatement.
				hasFilterSinceLastStatement=false;
			}
			bool excludeNegativeCredits=false;
			bool isSuperStatements=false;
			bool isSinglePatient=false;
			List<PatAging> listPatAgings=Patients.GetAgingList(age,dateLastStatement,listSelectedBillTypes,checkBadAddress.Checked,excludeNegativeCredits,PIn.Double(textExcludeLessThan.Text)
				,checkExcludeInactive.Checked,checkIgnoreInPerson.Checked,new List<long>(),isSuperStatements,isSinglePatient,listPendingInsPatNums,
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),checkExcludeAccountNoTil.Checked,hasFilterSinceLastStatement,isFinanceBilling:true);
			return listPatAgings;
		}

		///<summary>Returns the date the Finance or Billing was last run depending on the currently selected preference.</summary>
		private DateTime GetFinanceBillingLastRun() {
			if(PrefC.GetString(PrefName.BillingChargeOrFinanceIsDefault)=="Finance") {
				return PrefC.GetDate(PrefName.FinanceChargeLastRun);
			}
			else {
				return PrefC.GetDate(PrefName.BillingChargeLastRun);
			}
		}

		///<summary>The Treating Provider radio button is clicked: disable the provider combo box and picker.
		///This setting will run the Finance/Billing Charge on the primary provider (PriProv)</summary>
		private void RadioPatPriProv_CheckedChanged(object sender,EventArgs e) {
			if(radioPatPriProv.Checked) {
				comboSpecificProv.Enabled=false;
			}
			else {
				comboSpecificProv.Enabled=true;
			}
		}

		private void radioFinanceCharge_CheckedChanged(object sender, EventArgs e) {
			textAPR.ReadOnly = false;
			textAPR.BackColor = System.Drawing.SystemColors.Window;
			textAtLeast.ReadOnly=false;
			textAtLeast.BackColor = System.Drawing.SystemColors.Window;
			labelAtLeast.Enabled=true;
			textOver.ReadOnly=false;
			textOver.BackColor = System.Drawing.SystemColors.Window;
			labelOver.Enabled=true;
			textBillingCharge.ReadOnly = true;
			textBillingCharge.BackColor = System.Drawing.SystemColors.Control;
			textDateLastRun.Text=GetDateTimeForDisplay(PrefC.GetDate(PrefName.FinanceChargeLastRun));
			textDateUndo.Text=GetDateTimeForDisplay(PrefC.GetDate(PrefName.FinanceChargeLastRun));
		}

		private void radioBillingCharge_CheckedChanged(object sender, EventArgs e) {
			textAPR.ReadOnly = true;
			textAPR.BackColor = System.Drawing.SystemColors.Control;
			textAtLeast.ReadOnly=true;
			textAtLeast.BackColor = System.Drawing.SystemColors.Control;
			labelAtLeast.Enabled=false;
			textOver.ReadOnly=true;
			textOver.BackColor = System.Drawing.SystemColors.Control;
			labelOver.Enabled=false;
			textBillingCharge.ReadOnly = false;
			textBillingCharge.BackColor = System.Drawing.SystemColors.Window;
			textDateLastRun.Text=GetDateTimeForDisplay(PrefC.GetDate(PrefName.BillingChargeLastRun));
			textDateUndo.Text=GetDateTimeForDisplay(PrefC.GetDate(PrefName.BillingChargeLastRun));
		}

		private void butUndo_Click(object sender,EventArgs e) {
			DateTime dateUndo=PIn.Date(textDateUndo.Text);
			if(dateUndo.Year<1880) {
				MsgBox.Show(this,"There are no previous billing/finance charges to undo.");
				return;
			}
			string chargeType=(radioFinanceCharge.Checked?"Finance":"Billing");
			if(MessageBox.Show(Lan.g(this,"Undo all "+chargeType.ToLower()+" charges for")+" "+textDateUndo.Text+"?","",MessageBoxButtons.OKCancel)
				!=DialogResult.OK)
			{
				return;
			}
			bool isBillingCharge=radioBillingCharge.Checked;
			Adjustments.ChargeUndoData chargeUndoData=new Adjustments.ChargeUndoData();
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => chargeUndoData=Adjustments.UndoFinanceOrBillingCharges(dateUndo,isBillingCharge);
			progressOD.StartingMessage=Lan.g(this,"Deleting "+chargeType.ToLower()+" charge adjustments")+"...";
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			MessageBox.Show(Lan.g(this,chargeType+" charge adjustments deleted")+": "+chargeUndoData.CountDeletedAdjustments);
			if(!chargeUndoData.ListSkippedPatNums.IsNullOrEmpty()
				&& MessageBox.Show($"Some "+chargeType.ToLower()+" charges could not be deleted because they have pay splits or a dynamic payment plans attached. "
				+"Would you like to see a list of the patients for whom we could not delete "
				+chargeType.ToLower()+" charges?","",MessageBoxButtons.YesNo)==DialogResult.Yes)
			{
				string message="";
				List<Patient> listPatients=Patients.GetLimForPats(chargeUndoData.ListSkippedPatNums);
				for(int i=0;i<listPatients.Count;i++) {
					Patient patient=listPatients[i];
					message+=$"PatNum: {patient.PatNum}, {patient.GetNameFL()}\r\n";
				}
				using MsgBoxCopyPaste msgBoxCopyPastePatsSkipped=new MsgBoxCopyPaste(message);
				msgBoxCopyPastePatsSkipped.ShowDialog();
			}
			if(chargeUndoData.CountDeletedAdjustments==0) {
				DialogResult=DialogResult.OK;
				return;
			}
			if(!RunAgingEnterprise()) {
				MsgBox.Show(this,"There was an error calculating aging after the "+chargeType.ToLower()+" charge adjustments were deleted.\r\n"
					+"You should run aging later to update affected accounts.");
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,chargeType+" Charges undo. Date "+textDateUndo.Text);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!textDate.IsValid()
				|| !textAPR.IsValid()
				|| !textAtLeast.IsValid()
				|| !textOver.IsValid()
				|| !textExcludeLessThan.IsValid()
				|| !textExcludeNotBilledSince.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DateTime date=PIn.Date(textDate.Text);
			if(date.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Adjustments cannot be made for future dates");
				return;
			}
			if(PrefC.GetDate(PrefName.FinanceChargeLastRun).AddDays(25)>date && radioFinanceCharge.Checked) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning.  Finance charges should not be run more than once per month.  Continue?")) {
					return;
				}
			} 
			else if(PrefC.GetDate(PrefName.BillingChargeLastRun).AddDays(25)>date && radioBillingCharge.Checked) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning.  Billing charges should not be run more than once per month.  Continue?")) {
					return;
				}
			}
			if(listBillType.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select at least one billing type first.");
				return;
			}
			if(PIn.Double(textAPR.Text) < 2) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The APR is much lower than normal. Do you wish to proceed?")) {
					return;
				}
			}
			string chargeType=(radioFinanceCharge.Checked?"Finance":"Billing");//For display only
			List<PatAging> listPatAgings=GetFinanceBillingAgingList();//Get the Aging List for Finance and Billing
			long adjType=PrefC.GetLong(PrefName.FinanceChargeAdjustmentType);
			List<Adjustment> listAdjustments = new List<Adjustment>();
			if(!checkCompound.Checked) {
				int daysOver=90;
				if(radio30.Checked) {
					daysOver=30;
				}
				else if(radio60.Checked) {
					daysOver=60;
				}
				DateTime dateTMaxAdj=MiscData.GetNowDateTime().Date.AddDays(-daysOver);
				listAdjustments=Adjustments.GetAdjustForPatsByType(listPatAgings.Select(x => x.PatNum).ToList(),adjType,dateTMaxAdj);
			}
			bool isRadioBillingChargeChecked=radioBillingCharge.Checked;
			List<long> listPatNums=listPatAgings.Select(x => x.PatNum).ToList();
			List<Action> listActions=new List<Action>();
			List<InstallmentPlan> listInstallmentPlans=new List<InstallmentPlan>();
			if(!isRadioBillingChargeChecked) {//Finance charge
				listInstallmentPlans=InstallmentPlans.GetListForFams(listPatNums);
			}
			for(int i = 0;i<listPatAgings.Count;i++) {
				PatAging patAging = listPatAgings[i];
				listActions.Add(new Action(() => AddBillingOrFinanceCharge(patAging,listPatAgings.Count,listAdjustments,listInstallmentPlans)));
			}
			ProgressOD progressOD = new ProgressOD();
			progressOD.ActionMain=() => {
				//System.Threading.Thread.Sleep(5000);
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));//each group of actions gets X minutes.
			};
			progressOD.StartingMessage=Lan.g(this,"Gathering patients with aged balances")+"...";
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled) {
				MessageBox.Show(Lan.g(this,$"{_chargesAdded} {chargeType} charges added out of {listPatAgings.Count}"));
				return;
			}
			if(radioFinanceCharge.Checked) {
				if(Prefs.UpdateString(PrefName.FinanceChargeAPR,textAPR.Text) 
					| Prefs.UpdateString(PrefName.FinanceChargeLastRun,POut.Date(date,false))
					| Prefs.UpdateString(PrefName.FinanceChargeAtLeast,textAtLeast.Text)
					| Prefs.UpdateString(PrefName.FinanceChargeOnlyIfOver,textOver.Text)
					| Prefs.UpdateString(PrefName.BillingChargeOrFinanceIsDefault,"Finance"))
				{
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else if(radioBillingCharge.Checked) {
				if(Prefs.UpdateString(PrefName.BillingChargeAmount,textBillingCharge.Text)
					| Prefs.UpdateString(PrefName.BillingChargeLastRun,POut.Date(date,false))
					| Prefs.UpdateString(PrefName.BillingChargeOrFinanceIsDefault,"Billing")
					| Prefs.UpdateBool(PrefName.BillingExcludeInsPending,checkExcludeInsPending.Checked)
					| Prefs.UpdateInt(PrefName.BillingDaysExcludeInsPending,textDaysInsPendingExclude.Value))
				{
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			MessageBox.Show(Lan.g(this,chargeType+" charges added")+": "+_chargesAdded);
			if(!RunAgingEnterprise()) {
				MsgBox.Show(this,"There was an error calculating aging after the "+chargeType.ToLower()+" charge adjustments were added.\r\n"
					+"You should run aging later to update affected accounts.");
			}
			DialogResult = DialogResult.OK;
		}

		
		///<summary>Adds a billing charge or finance charge based on the patAging and adjustments passed in.</summary>
		private void AddBillingOrFinanceCharge(PatAging patAging,int listPatAgingsCount,List<Adjustment> listAdjustments,List<InstallmentPlan> listInstallmentPlans) {
			bool isRadio60Checked=radio60.Checked;
			bool isRadio30Checked=radio30.Checked;
			bool isRadioBillingChargeChecked=radioBillingCharge.Checked;
			string billingChargeText=textBillingCharge.Text;
			string aprText=textAPR.Text;
			string atLeastText=textAtLeast.Text;
			string overText=textOver.Text;
			string chargeType=(radioFinanceCharge.Checked?"Finance":"Billing");
			long adjType=PrefC.GetLong(PrefName.FinanceChargeAdjustmentType);
			DateTime date=PIn.Date(textDate.Text);
			_chargesProcessed++;
			if(_chargesProcessed%5==0) {
				BillingEvent.Fire(ODEventType.Billing,Lan.g(this,"Processing "+chargeType+" charges")+": "+_chargesProcessed+" out of "
					+listPatAgingsCount);
			}
			//This WILL NOT be the same as the patient's total balance. Start with BalOver90 since all options include that bucket. Add others if needed.
			double overallBalance=patAging.BalOver90;
			if(isRadio60Checked) {
				overallBalance+=patAging.Bal_61_90;
			}
			else if(isRadio30Checked) {
				overallBalance+=patAging.Bal_31_60+patAging.Bal_61_90;
			}
			if(overallBalance<=.01d) {
				return; 
			}
			long provNumToAssignCharges;
			if(radioSpecificProv.Checked) {
				long provNum=comboSpecificProv.GetSelectedProvNum();
				provNumToAssignCharges=comboSpecificProv.GetSelectedProvNum();
			}
			else {
				provNumToAssignCharges=patAging.PriProv;
			}
			if(isRadioBillingChargeChecked) {
				AddBillingCharge(patAging.PatNum,date,billingChargeText,provNumToAssignCharges);
			}
			else {//Finance charge
				if(listAdjustments.Any(x=>x.PatNum==patAging.PatNum)){
					overallBalance-=listAdjustments.FindAll(x => x.PatNum==patAging.PatNum).Sum(x => x.AdjAmt);
				}
				if(!AddFinanceCharge(patAging.PatNum,date,aprText,atLeastText,overText,overallBalance,
					provNumToAssignCharges,adjType,listInstallmentPlans))
				{
					return; 
				}
			}
			_chargesAdded++;
		}

		/// <summary>Returns true if a finance charge is added, false if one is not added</summary>
		private bool AddFinanceCharge(long patNum,DateTime date,string APR,string atLeast,string ifOver,double overallBalance,long priProv,long adjType,
			List<InstallmentPlan> listInstallmentPlans=null)
		{
			if(listInstallmentPlans!=null) {
				InstallmentPlan installmentPlan=listInstallmentPlans.Find(x => x.PatNum==patNum);
				if(installmentPlan!=null) {//Patient has an installment plan so use that APR instead.
					APR=installmentPlan.APR.ToString();
				}
			}
			Adjustment adjustment = new Adjustment();
			adjustment.PatNum = patNum;
			//AdjustmentCur.DateEntry=PIn.PDate(textDate.Text);//automatically handled
			adjustment.AdjDate = date;
			adjustment.ProcDate = date;
			adjustment.AdjType = adjType;
			adjustment.AdjNote = "";//"Finance Charge";
			adjustment.AdjAmt = Math.Round(((PIn.Double(APR) * .01d / 12d) * overallBalance),2);
			if(CompareDouble.IsZero(adjustment.AdjAmt) || adjustment.AdjAmt<PIn.Double(ifOver)) {
				//Don't add the charge if it is less than FinanceChargeOnlyIfOver; if the charge is exactly equal to FinanceChargeOnlyIfOver,
				//the charge will be added. Ex., AdjAmt=2.00 and FinanceChargeOnlyIfOver=2.00, the charge will be added.
				//Unless AdjAmt=0.00, in which case don't add a $0.00 finance charge
				return false;
			}
			//Add an amount that is at least the amount of FinanceChargeAtLeast 
			adjustment.AdjAmt=Math.Max(adjustment.AdjAmt,PIn.Double(atLeast));
			adjustment.ProvNum = priProv;
			Adjustments.Insert(adjustment);
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adjustment);
			return true;
		}

		private void AddBillingCharge(long patNum,DateTime date,string billingChargeAmount,long priProv) {
			Adjustment AdjustmentCur = new Adjustment();
			AdjustmentCur.PatNum = patNum;
			//AdjustmentCur.DateEntry=PIn.PDate(textDate.Text);//automatically handled
			AdjustmentCur.AdjDate = date;
			AdjustmentCur.ProcDate = date;
			AdjustmentCur.AdjType = PrefC.GetLong(PrefName.BillingChargeAdjustmentType);
			AdjustmentCur.AdjNote = "";//"Billing Charge";
			AdjustmentCur.AdjAmt = PIn.Double(billingChargeAmount);
			AdjustmentCur.ProvNum = priProv;
			Adjustments.Insert(AdjustmentCur);
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(AdjustmentCur);
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
