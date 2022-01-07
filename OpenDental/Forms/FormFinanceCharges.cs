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
		private List<Def> _listBillingTypeDefs;
		///<summary>Filtered list of providers based on the current clinic--used to populate the Combo Box Providers.</summary>
		private List<Provider> _listCurClinicProviders;

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
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			if(_listBillingTypeDefs.Count==0){//highly unlikely that this would happen
				MsgBox.Show(this,"No billing types have been set up or are visible.");
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		///<summary>The following logic was moved into a Shown method, rather than a Load method, because the progress bar causes the 
		///window to popbehind FormOpenDental when in Load.</summary>
		private void FormFinanceCharges_Shown(object sender,EventArgs e) {
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				if(!RunAgingEnterprise(true)) {
					DialogResult=DialogResult.Cancel;
					return;
				}
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Finance Charges window");
				DateTime asOfDate=(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)?PrefC.GetDate(PrefName.DateLastAging):DateTime.Today);
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=Ledgers.RunAging;
				progressOD.StartingMessage=Lan.g(this,"Calculating aging for all patients as of")+" "+asOfDate.ToShortDateString()+"...";
				progressOD.MessageCancel=Lan.g(this,"You should not Cancel because this might leave some or all patient balances set to 0. If you do cancel, make sure to run Aging again. Cancel anyway?");
				try{
					progressOD.ShowDialogProgress();
				}
				catch(Exception ex){
					Ledgers.AgingExceptionHandler(ex,this);
					return;
				}
				if(progressOD.IsCancelled){
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - Finance Charges window");
			}
			textDate.Text=DateTime.Today.ToShortDateString();		
			textAPR.MaxVal=100;
			textAPR.MinVal=0;
			textAPR.Text=PrefC.GetString(PrefName.FinanceChargeAPR);
			textBillingCharge.Text=PrefC.GetString(PrefName.BillingChargeAmount);
			for(int i=0;i<_listBillingTypeDefs.Count;i++) {
				listBillType.Items.Add(_listBillingTypeDefs[i].ItemName);
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
			textExcludeNotBilledSince.Text=GetDateTimeForDisplay(GetFinanceBillingLastRun());
			comboSpecificProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(Clinics.ClinicNum));
			comboSpecificProv.SelectedIndex=0;
			radioPatPriProv.Checked=true;
		}

		///<summary>If !isPreCharges, a message box will display for any errors instructing users to try again.  If the failed aging attempt is after
		///charges have been added/deleted, we don't want to inform the user that the transaction failed so run again since the charges were successfully
		///inserted/deleted and it was only updating the aged balances that failed.  If isPreCharges, this won't run aging again if the last aging run was
		///today.  If !isPreCharges, we will run aging even if it was run today to update aged bals to include the charges added/deleted.</summary>
		private bool RunAgingEnterprise(bool isOnLoad=false) {
			DateTime dtNow=MiscData.GetNowDateTime();
			DateTime dtToday=dtNow.Date;
			DateTime dateLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(isOnLoad && dateLastAging.Date==dtToday) {
				return true;//this is prior to inserting/deleting charges and aging has already been run for this date
			}
			Prefs.RefreshCache();
			DateTime dateTAgingBeganPref=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(dateTAgingBeganPref>DateTime.MinValue) {
				if(isOnLoad) {
					MessageBox.Show(this,Lan.g(this,"In order to add finance charges, aging must be calculated, but you cannot run aging until it has finished "
						+"the current calculations which began on")+" "+dateTAgingBeganPref.ToString()+".\r\n"+Lans.g(this,"If you believe the current aging "
						+"process has finished, a user with SecurityAdmin permission can manually clear the date and time by going to Setup | Miscellaneous and "
						+"pressing the 'Clear' button."));
				}
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Finance Charges window");
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dtNow,false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=()=>{
				Ledgers.ComputeAging(0,dtToday);
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dtToday,false));
			};
			progressOD.StartingMessage=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dtToday.ToShortDateString()+"...";
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
			List<long> listSelectedBillTypes=listBillType.SelectedIndices.Select(x => _listBillingTypeDefs[x].DefNum).ToList();
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,checkExcludeInsPending.Checked,false,false,new List<long>());
			List<long> listPendingInsPatNums=new List<long>();
			if(checkExcludeInsPending.Checked) { //Only fill list if excluding pending ins
				foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
					if(kvp.Value.HasPendingIns) {
						listPendingInsPatNums.Add(kvp.Key);
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
			DateTime lastStatement=PIn.Date(textExcludeNotBilledSince.Text);
			bool hasFilterSinceLastStatement=true;
			//If the 'exclude accounts not billed since' date has been removed, select the latest Billing/Finance Statement date. 
			if(textExcludeNotBilledSince.Text=="") {
				lastStatement=GetFinanceBillingLastRun();
				//Because the user did not specify a filter date, we are NOT going to exclude accounts not billed since lastStatement.
				hasFilterSinceLastStatement=false;
			}
			bool excludeNegativeCredits=false;
			bool isSuperStatements=false;
			bool isSinglePatient=false;
			return Patients.GetAgingList(age,lastStatement,listSelectedBillTypes,checkBadAddress.Checked,excludeNegativeCredits,PIn.Double(textExcludeLessThan.Text)
				,checkExcludeInactive.Checked,checkIgnoreInPerson.Checked,new List<long>(),isSuperStatements,isSinglePatient,listPendingInsPatNums,
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),checkExcludeAccountNoTil.Checked,hasFilterSinceLastStatement,true);
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
			bool billingCharge=radioBillingCharge.Checked;
			Adjustments.ChargeUndoData chargeUndoData=new Adjustments.ChargeUndoData();
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => chargeUndoData=Adjustments.UndoFinanceOrBillingCharges(dateUndo,billingCharge);
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
					Patient patCur=listPatients[i];
					message+=$"PatNum: {patCur.PatNum}, {patCur.GetNameFL()}\r\n";
				}
				using MsgBoxCopyPaste msgBoxPatsSkipped=new MsgBoxCopyPaste(message);
				msgBoxPatsSkipped.ShowDialog();
			}
			if(chargeUndoData.CountDeletedAdjustments==0) {
				DialogResult=DialogResult.OK;
				return;
			}
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				if(!RunAgingEnterprise()) {
					MsgBox.Show(this,"There was an error calculating aging after the "+chargeType.ToLower()+" charge adjustments were deleted.\r\n"
						+"You should run aging later to update affected accounts.");
				}
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Finance Charges window");
				DateTime asOfDate=(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)?PrefC.GetDate(PrefName.DateLastAging):DateTime.Today);
				progressOD=new ProgressOD();
				progressOD.ActionMain=() => Ledgers.RunAging();
				progressOD.StartingMessage=Lan.g(this,"Calculating aging for all patients as of")+" "+asOfDate.ToShortDateString()+"...";
				progressOD.MessageCancel=Lan.g(this,"You should not Cancel because this might leave some or all patient balances set to 0. If you do cancel, make sure to run Aging again. Cancel anyway?");
				try{
					progressOD.ShowDialogProgress();
				}
				catch(Exception ex2){
					Ledgers.AgingExceptionHandler(ex2,this);
					return;
				}
				if(progressOD.IsCancelled){
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - Finance Charges window");
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
			if(PIn.Long(textAPR.Text) < 2) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The APR is much lower than normal. Do you wish to proceed?")) {
					return;
				}
			}
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily) && PrefC.GetDate(PrefName.DateLastAging).AddMonths(1)<=DateTime.Today) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"It has been more than a month since aging has been run.  It is recommended that you update the "
					+"aging date and run aging before continuing."))
				{
					return;
				}
				//we might also consider a warning if textDate.Text does not match DateLastAging.  Probably not needed for daily aging, though.
			}
			string chargeType=(radioFinanceCharge.Checked?"Finance":"Billing");//For display only
			Action actionCloseProgress=null;
			int chargesAdded=0;
			try {
				actionCloseProgress=ODProgress.Show(ODEventType.Billing,typeof(BillingEvent),Lan.g(this,"Gathering patients with aged balances")+"...");
				List<PatAging> listPatAgings=GetFinanceBillingAgingList();//Get the Aging List for Finance and Billing
				long adjType=PrefC.GetLong(PrefName.FinanceChargeAdjustmentType);
				Dictionary<long,List<Adjustment>> dictPatAdjustments=new Dictionary<long, List<Adjustment>>();
				if(!checkCompound.Checked) {
					int daysOver=(radio30.Checked ? 30
						: radio60.Checked ? 60
						: 90);
					DateTime maxAdjDate=MiscData.GetNowDateTime().Date.AddDays(-daysOver);
					dictPatAdjustments=Adjustments.GetAdjustForPatsByType(listPatAgings.Select(x => x.PatNum).ToList(),adjType,maxAdjDate);
				}
				bool isRadio60Checked=radio60.Checked;
				bool isRadio30Checked=radio30.Checked;
				bool isRadioBillingChargeChecked=radioBillingCharge.Checked;
				string billingChargeText=textBillingCharge.Text;
				string aprText=textAPR.Text;
				string atLeastText=textAtLeast.Text;
				string overText=textOver.Text;
				List<long> listPatNums=listPatAgings.Select(x => x.PatNum).ToList();
				Dictionary<long,InstallmentPlan> dictFamilyInstallmentPlans=null;
				if(!isRadioBillingChargeChecked) {//Finance charge
					dictFamilyInstallmentPlans=InstallmentPlans.GetForFams(listPatNums);
				}
				int chargesProcessed=0;
				List<Action> listActions=new List<Action>();
				foreach(PatAging patAgingCur in listPatAgings) {
					listActions.Add(new Action(() => {
						if(++chargesProcessed%5==0) {
							BillingEvent.Fire(ODEventType.Billing,Lan.g(this,"Processing "+chargeType+" charges")+": "+chargesProcessed+" out of "
								+listPatAgings.Count);
						}
						//This WILL NOT be the same as the patient's total balance. Start with BalOver90 since all options include that bucket. Add others if needed.
						double overallBalance=patAgingCur.BalOver90+(isRadio60Checked ? patAgingCur.Bal_61_90
							: isRadio30Checked ? (patAgingCur.Bal_31_60+patAgingCur.Bal_61_90)
							: 0);
						if(overallBalance<=.01d) {
							return;
						}
						long provNumToAssignCharges;
						if(radioSpecificProv.Checked) {
							long provNum=comboSpecificProv.GetSelectedProvNum();
							provNumToAssignCharges=comboSpecificProv.GetSelectedProvNum();
						}
						else {
							provNumToAssignCharges=patAgingCur.PriProv;
						}
						if(isRadioBillingChargeChecked) {
							AddBillingCharge(patAgingCur.PatNum,date,billingChargeText,provNumToAssignCharges);
						}
						else {//Finance charge
							if(dictPatAdjustments.ContainsKey(patAgingCur.PatNum)) {//Only contains key if checkCompound is not checked.
								overallBalance-=dictPatAdjustments[patAgingCur.PatNum].Sum(x => x.AdjAmt);//Dict always contains patNum as key, but list can be empty.
							}
							if(!AddFinanceCharge(patAgingCur.PatNum,date,aprText,atLeastText,overText,overallBalance,
								provNumToAssignCharges,adjType,dictFamilyInstallmentPlans))
							{
								return;
							}
						}
						chargesAdded++;
					}));
				}
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));//each group of actions gets X minutes.
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
						| Prefs.UpdateString(PrefName.BillingChargeOrFinanceIsDefault,"Billing"))
					{
						DataValid.SetInvalid(InvalidType.Prefs);
					}
				}
			}
			finally {
				actionCloseProgress?.Invoke();//terminates progress bar
			}
			MessageBox.Show(Lan.g(this,chargeType+" charges added")+": "+chargesAdded);
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				if(!RunAgingEnterprise()) {
					MsgBox.Show(this,"There was an error calculating aging after the "+chargeType.ToLower()+" charge adjustments were added.\r\n"
						+"You should run aging later to update affected accounts.");
				}
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Finance Charges window");
				DateTime asOfDate=(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)?PrefC.GetDate(PrefName.DateLastAging):DateTime.Today);
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() => Ledgers.RunAging();
				progressOD.StartingMessage=Lan.g(this,"Calculating aging for all patients as of")+" "+asOfDate.ToShortDateString()+"...";
				progressOD.MessageCancel=Lan.g(this,"You should not Cancel because this might leave some or all patient balances set to 0. If you do cancel, make sure to run Aging again. Cancel anyway?");
				try{
					progressOD.ShowDialogProgress();
				}
				catch(Exception ex){
					Ledgers.AgingExceptionHandler(ex,this);
					return;
				}
				if(progressOD.IsCancelled){
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - Finance Charges window");
			}
			DialogResult = DialogResult.OK;
		}

		/// <summary>Returns true if a finance charge is added, false if one is not added</summary>
		private bool AddFinanceCharge(long patNum,DateTime date,string APR,string atLeast,string ifOver,double OverallBalance,long PriProv,long adjType,
			Dictionary<long,InstallmentPlan> dictInstallmentPlans=null)
		{
			if(date.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Adjustments cannot be made for future dates. Finance charge was not added.");
				return false;
			}
			if(dictInstallmentPlans!=null) {
				dictInstallmentPlans.TryGetValue(patNum,out InstallmentPlan installmentPlan);
				if(installmentPlan!=null) {//Patient has an installment plan so use that APR instead.
					APR=installmentPlan.APR.ToString();
				}
			}
			Adjustment AdjustmentCur = new Adjustment();
			AdjustmentCur.PatNum = patNum;
			//AdjustmentCur.DateEntry=PIn.PDate(textDate.Text);//automatically handled
			AdjustmentCur.AdjDate = date;
			AdjustmentCur.ProcDate = date;
			AdjustmentCur.AdjType = adjType;
			AdjustmentCur.AdjNote = "";//"Finance Charge";
			AdjustmentCur.AdjAmt = Math.Round(((PIn.Double(APR) * .01d / 12d) * OverallBalance),2);
			if(CompareDouble.IsZero(AdjustmentCur.AdjAmt) || AdjustmentCur.AdjAmt<PIn.Double(ifOver)) {
				//Don't add the charge if it is less than FinanceChargeOnlyIfOver; if the charge is exactly equal to FinanceChargeOnlyIfOver,
				//the charge will be added. Ex., AdjAmt=2.00 and FinanceChargeOnlyIfOver=2.00, the charge will be added.
				//Unless AdjAmt=0.00, in which case don't add a $0.00 finance charge
				return false;
			}
			//Add an amount that is at least the amount of FinanceChargeAtLeast 
			AdjustmentCur.AdjAmt=Math.Max(AdjustmentCur.AdjAmt,PIn.Double(atLeast));
			AdjustmentCur.ProvNum = PriProv;
			Adjustments.Insert(AdjustmentCur);
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(AdjustmentCur);
			return true;
		}

		private void AddBillingCharge(long patNum,DateTime date,string BillingChargeAmount,long PriProv) {
			if(date.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Adjustments cannot be made for future dates");
				return;
			}
			Adjustment AdjustmentCur = new Adjustment();
			AdjustmentCur.PatNum = patNum;
			//AdjustmentCur.DateEntry=PIn.PDate(textDate.Text);//automatically handled
			AdjustmentCur.AdjDate = date;
			AdjustmentCur.ProcDate = date;
			AdjustmentCur.AdjType = PrefC.GetLong(PrefName.BillingChargeAdjustmentType);
			AdjustmentCur.AdjNote = "";//"Billing Charge";
			AdjustmentCur.AdjAmt = PIn.Double(BillingChargeAmount);
			AdjustmentCur.ProvNum = PriProv;
			Adjustments.Insert(AdjustmentCur);
			TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(AdjustmentCur);
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
