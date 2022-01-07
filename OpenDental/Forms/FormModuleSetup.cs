using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	public partial class FormModuleSetup:FormODBase {
		#region Fields - Private
		private List<Def> _listDefsPosAdjTypes;
		private List<Def> _listDefsNegAdjTypes;
		private bool _changed;
		private ColorDialog _colorDialog;
		private List<BrokenApptProcedure> _listBrokenApptProcedures=new List<BrokenApptProcedure>();
		///<summary>Helps store and set the Prefs for desease, medications, and alergies in the Chart Module when clicking OK on those forms.</summary>
		private long _diseaseDefNum;//DiseaseDef
		private long _medicationNum;
		private long _alergyDefNum;//AllergyDef
		private YN _usePhonenumTable=YN.Unknown;
		#endregion Fields - Private

		#region Constructors
		///<summary>Default constructor.  Opens the form with the Appts tab selected.</summary>
		public FormModuleSetup():this(0) {
		}

		///<summary>Opens the form with a specific tab selected.  Currently 0-6 are the only valid values.  Defaults to Appts tab if invalid value passed in. 0=Appts, 1=Family, 2=Account, 3=Treat' Plan, 4=Chart, 5=Images, 6=Manage</summary>
		public FormModuleSetup(int selectedTab) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(selectedTab<0 || selectedTab>6) {
				selectedTab=0;//Default to Appts tab.
			}
			tabControlMain.SelectedTab=tabControlMain.TabPages[selectedTab];
		}
		#endregion Constructors
		
		#region Events - Standard
		private void FormModuleSetup_Load(object sender, System.EventArgs e) {
			_changed=false;
			if(ODBuild.IsDebug()){
				FillAppts();
				FillFamily();
				FillAccount();
				FillTreatPlan();
				FillChart();
				FillImages();
				FillManage();
			}
			else{
				try {//try/catch used to prevent setup form from partially loading and filling controls.  Causes UEs, Example: TimeCardOvertimeFirstDayOfWeek set to -1 because UI control not filled properly.
					FillAppts();
					FillFamily();
					FillAccount();
					FillTreatPlan();
					FillChart();
					FillImages();
					FillManage();
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"An error has occurred while attempting to load preferences.  Run database maintenance and try again."),ex);
					DialogResult=DialogResult.Abort;
					return;
				}
			}
			if(PrefC.RandomKeys) {
				groupTreatPlanSort.Visible=false;
			}
			Plugins.HookAddCode(this,"FormModuleSetup.FormModuleSetup_Load_end");
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//validation is done within each save.
			//One save to db might succeed, and then a subsequent save can fail to validate.  That's ok.
			if(!SaveAppts()
				|| !SaveFamily()
				|| !SaveAccount()
				|| !SaveTreatPlan()
				|| !SaveChart()
				|| !SaveImages()
				|| !SaveManage()				
			){
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormModuleSetup_FormClosing(object sender,FormClosingEventArgs e) {
			if(_changed){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
		#endregion Events - Standard

		#region Methods - Event Handlers Appts
		private void butApptLineColor_Click(object sender,EventArgs e) {
			_colorDialog.Color=butColor.BackColor;//Pre-select current pref color
			if(_colorDialog.ShowDialog()==DialogResult.OK) {
				butApptLineColor.BackColor=_colorDialog.Color;
			}
			_colorDialog.Dispose();
		}

		private void butColor_Click(object sender,EventArgs e) {
			_colorDialog.Color=butColor.BackColor;//Pre-select current pref color
			if(_colorDialog.ShowDialog()==DialogResult.OK) {
				butColor.BackColor=_colorDialog.Color;
			}
			_colorDialog.Dispose();
		}

		private void checkAppointmentTimeIsLocked_MouseUp(object sender,MouseEventArgs e) {
			if(checkAppointmentTimeIsLocked.Checked) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to lock appointment times for all existing appointments?")){
					Appointments.SetAptTimeLocked();
				}
			}
		}

		private void checkApptsRequireProcs_CheckedChanged(object sender,EventArgs e) {
			textApptWithoutProcsDefaultLength.Enabled=(!checkApptsRequireProcs.Checked);
		}
		#endregion Methods - Event Handlers Appts

		#region Methods - Event Handlers Family
		private void checkInsDefaultAssignmentOfBenefits_Click(object sender,EventArgs e) {
			//Users with Setup permission are always allowed to change the Checked property of this check box.
			//However, there is a second step when changing the value that can only be performed by users with the InsPlanChangeAssign permission.
			if(!Security.IsAuthorized(Permissions.InsPlanChangeAssign,true)) {
				return;
			}
			string promptMsg=Lan.g(this,"Would you like to immediately change all plans to use assignment of benefits?\r\n"
					+$"Warning: This will update all existing plans to render payment to the provider on all future claims.");
			if(!checkInsDefaultAssignmentOfBenefits.Checked) {
				promptMsg=Lan.g(this,"Would you like to immediately change all plans to use assignment of benefits?\r\n"
					+$"Warning: This will update all existing plans to render payment to the patient on all future claims.");
			}
			if(MessageBox.Show(promptMsg,Lan.g(this,"Change all plans?"),MessageBoxButtons.YesNo)==DialogResult.No) {
				return;
			}
			long subsAffected=InsSubs.SetAllSubsAssignBen(checkInsDefaultAssignmentOfBenefits.Checked);
			SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeAssign,0
				,Lan.g(this,"The following count of plan(s) had their assignment of benefits updated in the Family tab in Module Preferences:")+" "+POut.Long(subsAffected)
			);
			MessageBox.Show(Lan.g(this,"Plans affected:")+" "+POut.Long(subsAffected));
		}

		private void checkInsDefaultShowUCRonClaims_Click(object sender,EventArgs e) {
			if(!checkInsDefaultShowUCRonClaims.Checked) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to immediately change all plans to show office UCR fees on claims?")) {
				return;
			}
			long plansAffected=InsPlans.SetAllPlansToShowUCR();
			MessageBox.Show(Lan.g(this,"Plans affected: ")+plansAffected.ToString());
		}

		private void comboCobRule_SelectionChangeCommitted(object sender,EventArgs e) {
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to change the COB rule for all existing insurance plans?")) {
				InsPlans.UpdateCobRuleForAll((EnumCobRule)comboCobRule.SelectedIndex);
			}
		}

		private void butSyncPhNums_Click(object sender,EventArgs e) {
			if(SyncPhoneNums()) {
				MsgBox.Show(this,"Done");
			}
		}
		#endregion Methods - Event Handlers Family

		#region Methods - Event Handlers Account
		private void butBadDebt_Click(object sender,EventArgs e) {
			string[] arrayDefNums=PrefC.GetString(PrefName.BadDebtAdjustmentTypes).Split(new char[] {','}); //comma-delimited list.
			List<long> listBadAdjDefNums = new List<long>();
			foreach(string strDefNum in arrayDefNums) {
				listBadAdjDefNums.Add(PIn.Long(strDefNum));
			}
			List<Def> listBadAdjDefs=Defs.GetDefs(DefCat.AdjTypes,listBadAdjDefNums);
			using FormDefinitionPicker FormDP = new FormDefinitionPicker(DefCat.AdjTypes,listBadAdjDefs);
			FormDP.HasShowHiddenOption=true;
			FormDP.IsMultiSelectionMode=true;
			FormDP.ShowDialog();
			if(FormDP.DialogResult==DialogResult.OK) {
				FillListboxBadDebt(FormDP.ListDefsSelected);
			}
		}

		private void butReplacements_Click(object sender,EventArgs e) {
			using FormMessageReplacements form=new FormMessageReplacements(MessageReplaceType.Patient);
			form.IsSelectionMode=true;
			form.ShowDialog();
			if(form.DialogResult!=DialogResult.OK) {
				return;
			}
			textClaimIdentifier.Focus();
			int cursorIndex=textClaimIdentifier.SelectionStart;
			textClaimIdentifier.Text=textClaimIdentifier.Text.Insert(cursorIndex,form.Replacement);
			textClaimIdentifier.SelectionStart=cursorIndex+form.Replacement.Length;
		}

		private void checkRecurringChargesAutomated_CheckedChanged(object sender,EventArgs e) {
			labelRecurringChargesAutomatedTime.Enabled=checkRecurringChargesAutomated.Checked;
			textRecurringChargesTime.Enabled=checkRecurringChargesAutomated.Checked;
		}

		private void checkRepeatingChargesAutomated_CheckedChanged(object sender,EventArgs e) {
			labelRepeatingChargesAutomatedTime.Enabled=checkRepeatingChargesAutomated.Checked;
			textRepeatingChargesAutomatedTime.Enabled=checkRepeatingChargesAutomated.Checked;
		}

		private void checkShowFamilyCommByDefault_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
		}

		private void comboPayPlansVersion_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboPayPlansVersion.SelectedIndex==(int)PayPlanVersions.AgeCreditsAndDebits-1) {//Minus 1 because the enum starts at 1.
				checkHideDueNow.Visible=true;
				checkHideDueNow.Checked=PrefC.GetBool(PrefName.PayPlanHideDueNow);
			}
			else {
				checkHideDueNow.Visible=false;
				checkHideDueNow.Checked=false;
			}
		}

		///<summary>Turning on automated repeating charges, but recurring charges are also enabled and set to run before auto repeating charges.  Prompt user that this is unadvisable.</summary>
		private void PromptRecurringRepeatingChargesTimes(object sender,EventArgs e) {
			if(checkRepeatingChargesAutomated.Checked && checkRecurringChargesAutomated.Checked
				&& PIn.DateT(textRepeatingChargesAutomatedTime.Text).TimeOfDay>=PIn.DateT(textRecurringChargesTime.Text).TimeOfDay)
			{
				MsgBox.Show(this,"Recurring charges run time is currently set before Repeating charges run time.\r\nConsider setting repeating charges to "
					+"automatically run before recurring charges.");
			}
		}
		#endregion Methods - Event Handlers Account

		#region Methods - Event Handlers Treat' Plan
		private void checkFrequency_Click(object sender,EventArgs e) {
			textInsBW.Enabled=checkFrequency.Checked;
			textInsPano.Enabled=checkFrequency.Checked;
			textInsExam.Enabled=checkFrequency.Checked;
			textInsCancerScreen.Enabled=checkFrequency.Checked;
			textInsProphy.Enabled=checkFrequency.Checked;
			textInsFlouride.Enabled=checkFrequency.Checked;
			textInsSealant.Enabled=checkFrequency.Checked;
			textInsCrown.Enabled=checkFrequency.Checked;
			textInsSRP.Enabled=checkFrequency.Checked;
			textInsDebridement.Enabled=checkFrequency.Checked;
			textInsPerioMaint.Enabled=checkFrequency.Checked;
			textInsDentures.Enabled=checkFrequency.Checked;
			textInsImplant.Enabled=checkFrequency.Checked;
		}
		
		private void radioTreatPlanSortOrder_Click(object sender,EventArgs e) {
			//Sort by order is a false 
			if(PrefC.GetBool(PrefName.TreatPlanSortByTooth)==radioTreatPlanSortOrder.Checked) {
				MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
			}
		}

		private void radioTreatPlanSortTooth_Click(object sender,EventArgs e) {
			if(PrefC.GetBool(PrefName.TreatPlanSortByTooth)!=radioTreatPlanSortTooth.Checked) {
				MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
			}
		}
		#endregion Methods - Event Handlers Treat' Plan

		#region Methods - Event Handlers Chart
		private void butAllergiesIndicateNone_Click(object sender,EventArgs e) {
			using FormAllergySetup formA=new FormAllergySetup();
			formA.IsSelectionMode=true;
			formA.ShowDialog();
			if(formA.DialogResult!=DialogResult.OK) {
				return;
			}
			_alergyDefNum=formA.SelectedAllergyDefNum;
			textAllergiesIndicateNone.Text=AllergyDefs.GetOne(formA.SelectedAllergyDefNum).Description;
		}

		private void butDiagnosisCode_Click(object sender,EventArgs e) {
			if(checkDxIcdVersion.Checked) {//ICD-10
				using FormIcd10s formI=new FormIcd10s();
				formI.IsSelectionMode=true;
				if(formI.ShowDialog()==DialogResult.OK) {
					textICD9DefaultForNewProcs.Text=formI.SelectedIcd10.Icd10Code;
				}
			}
			else {//ICD-9
				using FormIcd9s formI=new FormIcd9s();
				formI.IsSelectionMode=true;
				if(formI.ShowDialog()==DialogResult.OK) {
					textICD9DefaultForNewProcs.Text=formI.SelectedIcd9.ICD9Code;
				}
			}
		}
		
		private void butMedicationsIndicateNone_Click(object sender,EventArgs e) {
			using FormMedications formM=new FormMedications();
			formM.IsSelectionMode=true;
			formM.ShowDialog();
			if(formM.DialogResult!=DialogResult.OK) {
				return;
			}
			_medicationNum=formM.SelectedMedicationNum;
			textMedicationsIndicateNone.Text=Medications.GetDescription(formM.SelectedMedicationNum);
		}

		private void butProblemsIndicateNone_Click(object sender,EventArgs e) {
			using FormDiseaseDefs formD=new FormDiseaseDefs();
			formD.IsSelectionMode=true;
			formD.ShowDialog();
			if(formD.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			_diseaseDefNum=formD.ListDiseaseDefsSelected[0].DiseaseDefNum;
			textProblemsIndicateNone.Text=formD.ListDiseaseDefsSelected[0].DiseaseName;
		}

		private void checkClaimProcsAllowEstimatesOnCompl_CheckedChanged(object sender,EventArgs e) {
			if(checkClaimProcsAllowEstimatesOnCompl.Checked) {//user is attempting to Allow Estimates to be created for backdated complete procedures
				using InputBox inputBox=new InputBox("Please enter password");
				inputBox.ShowDialog();
				if(inputBox.DialogResult!=DialogResult.OK) {
					checkClaimProcsAllowEstimatesOnCompl.Checked=false;
					return;
				}
				if(inputBox.textResult.Text!="abracadabra") {//To prevent unaware users from clicking this box
					checkClaimProcsAllowEstimatesOnCompl.Checked=false;
					MsgBox.Show(this,"Wrong password");
					return;
				}
			}
		}

		private void checkDxIcdVersion_Click(object sender,EventArgs e) {
			SetIcdLabels();
		}

		private void checkProcLockingIsAllowed_Click(object sender,EventArgs e) {
			if(checkProcLockingIsAllowed.Checked) {//if user is checking box			
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This option is not normally used, because all notes are already locked internally, and all changes to notes are viewable in the audit mode of the Chart module.  This option is only for offices that insist on locking each procedure and only allowing notes to be appended.  Using this option, there really is no way to unlock a procedure, regardless of security permission.  So locked procedures can instead be marked as invalid in the case of mistakes.  But it's a hassle to mark procedures invalid, and they also cause clutter.  This option can be turned off later, but locked procedures will remain locked.\r\n\r\nContinue anyway?")) {
					checkProcLockingIsAllowed.Checked=false;
				}
			}
			else {//unchecking box
				MsgBox.Show(this,"Turning off this option will not affect any procedures that are already locked or invalidated.");
			}
		}
		#endregion Methods - Event Handlers Chart

		#region Methods - Event Handlers Images

		#endregion Methods - Event Handlers Images

		#region Methods - Event Handlers Manage

		#endregion Methods - Event Handlers Manage
		
		#region Methods - Appts
		private void FillAppts(){
			BrokenApptProcedure brokenApptCodeDB=(BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure);
			foreach(BrokenApptProcedure option in Enum.GetValues(typeof(BrokenApptProcedure))) {
				if(option==BrokenApptProcedure.Missed && !ProcedureCodes.HasMissedCode()) {
					continue;
				}
				if(option==BrokenApptProcedure.Cancelled && !ProcedureCodes.HasCancelledCode()) {
					continue;
				}
				if(option==BrokenApptProcedure.Both && (!ProcedureCodes.HasMissedCode() || !ProcedureCodes.HasCancelledCode())) {
					continue;
				}
				_listBrokenApptProcedures.Add(option);
				int index=comboBrokenApptProc.Items.Add(Lans.g(this,option.ToString()));
				if(option==brokenApptCodeDB) {
					comboBrokenApptProc.SelectedIndex=index;
				}
			}
			if(comboBrokenApptProc.Items.Count==1) {//None
				comboBrokenApptProc.SelectedIndex=0;
				comboBrokenApptProc.Enabled=false;
			}
			checkSolidBlockouts.Checked=PrefC.GetBool(PrefName.SolidBlockouts);
			checkBrokenApptAdjustment.Checked=PrefC.GetBool(PrefName.BrokenApptAdjustment);
			checkBrokenApptCommLog.Checked=PrefC.GetBool(PrefName.BrokenApptCommLog);
			//checkBrokenApptNote.Checked=PrefC.GetBool(PrefName.BrokenApptCommLogNotAdjustment);
			checkApptBubbleDelay.Checked = PrefC.GetBool(PrefName.ApptBubbleDelay);
			checkAppointmentBubblesDisabled.Checked=PrefC.GetBool(PrefName.AppointmentBubblesDisabled);
			_listDefsPosAdjTypes=Defs.GetPositiveAdjTypes();
			_listDefsNegAdjTypes=Defs.GetNegativeAdjTypes();
			comboBrokenApptAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboBrokenApptAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType));
			comboProcDiscountType.Items.AddDefs(_listDefsNegAdjTypes);
			comboProcDiscountType.SetSelectedDefNum(PrefC.GetLong(PrefName.TreatPlanDiscountAdjustmentType));
			textDiscountPercentage.Text=PrefC.GetDouble(PrefName.TreatPlanDiscountPercent).ToString();
			checkApptExclamation.Checked=PrefC.GetBool(PrefName.ApptExclamationShowForUnsentIns);
			comboTimeArrived.Items.AddDefNone();
			comboTimeArrived.SelectedIndex=0;
			comboTimeArrived.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboTimeArrived.SetSelectedDefNum(PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger));
			comboTimeSeated.Items.AddDefNone();
			comboTimeSeated.SelectedIndex=0;
			comboTimeSeated.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboTimeSeated.SetSelectedDefNum(PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger));
			comboTimeDismissed.Items.AddDefNone();
			comboTimeDismissed.SelectedIndex=0;
			comboTimeDismissed.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboTimeDismissed.SetSelectedDefNum(PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger));
			checkApptRefreshEveryMinute.Checked=PrefC.GetBool(PrefName.ApptModuleRefreshesEveryMinute);
			foreach(SearchBehaviorCriteria searchBehavior in Enum.GetValues(typeof(SearchBehaviorCriteria))) {
				comboSearchBehavior.Items.Add(Lan.g(this,searchBehavior.GetDescription()));
			}
			for(int i=0;i<11;i++) {
				double seconds=(double)i/10;
				if(i==0) {
					comboDelay.Items.Add(Lan.g(this,"No delay"),seconds);
				}
				else {
					comboDelay.Items.Add(seconds.ToString("f1") + " "+Lan.g(this,"seconds"),seconds);
				}
				if(PrefC.GetDouble(PrefName.FormClickDelay,doUseEnUSFormat:true)==seconds) {
					comboDelay.SelectedIndex=i;
				}
			}
			comboSearchBehavior.SelectedIndex=PrefC.GetInt(PrefName.AppointmentSearchBehavior);
			checkAppointmentTimeIsLocked.Checked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			textApptBubNoteLength.Text=PrefC.GetInt(PrefName.AppointmentBubblesNoteLength).ToString();
			checkWaitingRoomFilterByView.Checked=PrefC.GetBool(PrefName.WaitingRoomFilterByView);
			textWaitRoomWarn.Text=PrefC.GetInt(PrefName.WaitingRoomAlertTime).ToString();
			butColor.BackColor=PrefC.GetColor(PrefName.WaitingRoomAlertColor);
			butApptLineColor.BackColor=PrefC.GetColor(PrefName.AppointmentTimeLineColor);
			checkApptModuleDefaultToWeek.Checked=PrefC.GetBool(PrefName.ApptModuleDefaultToWeek);
			checkApptTimeReset.Checked=PrefC.GetBool(PrefName.AppointmentClinicTimeReset);
			if(!PrefC.HasClinicsEnabled) {
				checkApptTimeReset.Visible=false;
			}
			checkApptModuleAdjInProd.Checked=PrefC.GetBool(PrefName.ApptModuleAdjustmentsInProd);
			checkUseOpHygProv.Checked=PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly);
			checkApptModuleProductionUsesOps.Checked=PrefC.GetBool(PrefName.ApptModuleProductionUsesOps);
			checkApptsRequireProcs.Checked=PrefC.GetBool(PrefName.ApptsRequireProc);
			checkApptAllowFutureComplete.Checked=PrefC.GetBool(PrefName.ApptAllowFutureComplete);
			checkApptAllowEmptyComplete.Checked=PrefC.GetBool(PrefName.ApptAllowEmptyComplete);
			comboApptSchedEnforceSpecialty.Items.AddRange(Enum.GetValues(typeof(ApptSchedEnforceSpecialty)).OfType<ApptSchedEnforceSpecialty>()
				.Select(x => x.GetDescription()).ToArray());
			comboApptSchedEnforceSpecialty.SelectedIndex=PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty);
			if(!PrefC.HasClinicsEnabled) {
				comboApptSchedEnforceSpecialty.Visible=false;
				labelApptSchedEnforceSpecialty.Visible=false;
			}
			textApptWithoutProcsDefaultLength.Text=PrefC.GetString(PrefName.AppointmentWithoutProcsDefaultLength);
			checkReplaceBlockouts.Checked=PrefC.GetBool(PrefName.ReplaceExistingBlockout);
			checkUnscheduledListNoRecalls.Checked=PrefC.GetBool(PrefName.UnscheduledListNoRecalls);
			textApptAutoRefreshRange.Text=PrefC.GetString(PrefName.ApptAutoRefreshRange);
			checkPreventChangesToComplAppts.Checked=PrefC.GetBool(PrefName.ApptPreventChangesToCompleted);
			checkApptsAllowOverlap.CheckState=PrefC.GetYNCheckState(PrefName.ApptsAllowOverlap);
			textApptFontSize.Text=PrefC.GetString(PrefName.ApptFontSize);
			textApptProvbarWidth.Text=PrefC.GetString(PrefName.ApptProvbarWidth);
			checkBrokenApptRequiredOnMove.Checked=PrefC.GetBool(PrefName.BrokenApptRequiredOnMove);
		}

		///<summary>Returns false if validation fails.</summary>
		private bool SaveAppts(){
			int noteLength=0;
			if(!int.TryParse(textApptBubNoteLength.Text,out noteLength)) {
				MsgBox.Show(this,"Max appointment note length is invalid. Please enter a valid number to continue.");
				return false;
			}
			if(noteLength<0) {
				MsgBox.Show(this,"Max appointment note length cannot be a negative number.");
				return false;
			}
			int waitingRoomAlertTime=0;
			try {
				waitingRoomAlertTime=PIn.Int(textWaitRoomWarn.Text);
				if(waitingRoomAlertTime<0) {
					throw new ApplicationException("Waiting room time cannot be negative");//User never sees this message.
				}
			}
			catch {
				MsgBox.Show(this,"Waiting room alert time is invalid.");
				return false;
			}
			if(!textApptWithoutProcsDefaultLength.IsValid()
				| !textApptAutoRefreshRange.IsValid())
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			float apptFontSize=0;
			if(!float.TryParse(textApptFontSize.Text,out apptFontSize)){
				MsgBox.Show(this,"Appt Font Size invalid.");
				return false;
			}
			if(apptFontSize<1 || apptFontSize>40){
				MsgBox.Show(this,"Appt Font Size must be between 1 and 40.");
				return false;
			}
			if(!textApptProvbarWidth.IsValid()) {
				MsgBox.Show(this,"Please fix data errors first.");
				return false;
			}
			_changed|=Prefs.UpdateBool(PrefName.AppointmentBubblesDisabled,checkAppointmentBubblesDisabled.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ApptBubbleDelay,checkApptBubbleDelay.Checked);
			_changed|=Prefs.UpdateBool(PrefName.SolidBlockouts,checkSolidBlockouts.Checked);
			//_changed|=Prefs.UpdateBool(PrefName.BrokenApptCommLogNotAdjustment,checkBrokenApptNote.Checked) //Deprecated
			_changed|=Prefs.UpdateBool(PrefName.BrokenApptAdjustment,checkBrokenApptAdjustment.Checked);
			_changed|=Prefs.UpdateBool(PrefName.BrokenApptCommLog,checkBrokenApptCommLog.Checked);
			_changed|=Prefs.UpdateInt(PrefName.BrokenApptProcedure,(int)_listBrokenApptProcedures[comboBrokenApptProc.SelectedIndex]);
			_changed|=Prefs.UpdateBool(PrefName.ApptExclamationShowForUnsentIns,checkApptExclamation.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ApptModuleRefreshesEveryMinute,checkApptRefreshEveryMinute.Checked);
			_changed|=Prefs.UpdateInt(PrefName.AppointmentSearchBehavior,comboSearchBehavior.SelectedIndex);
			_changed|=Prefs.UpdateBool(PrefName.AppointmentTimeIsLocked,checkAppointmentTimeIsLocked.Checked);
			_changed|=Prefs.UpdateInt(PrefName.AppointmentBubblesNoteLength,noteLength);
			_changed|=Prefs.UpdateBool(PrefName.WaitingRoomFilterByView,checkWaitingRoomFilterByView.Checked);
			_changed|=Prefs.UpdateInt(PrefName.WaitingRoomAlertTime,waitingRoomAlertTime);
			_changed|=Prefs.UpdateInt(PrefName.WaitingRoomAlertColor,butColor.BackColor.ToArgb());
			_changed|=Prefs.UpdateInt(PrefName.AppointmentTimeLineColor,butApptLineColor.BackColor.ToArgb());
			_changed|=Prefs.UpdateBool(PrefName.ApptModuleDefaultToWeek,checkApptModuleDefaultToWeek.Checked);
			_changed|=Prefs.UpdateBool(PrefName.AppointmentClinicTimeReset,checkApptTimeReset.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ApptModuleAdjustmentsInProd,checkApptModuleAdjInProd.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ApptSecondaryProviderConsiderOpOnly,checkUseOpHygProv.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ApptModuleProductionUsesOps,checkApptModuleProductionUsesOps.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ApptsRequireProc,checkApptsRequireProcs.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ApptAllowFutureComplete,checkApptAllowFutureComplete.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ApptAllowEmptyComplete,checkApptAllowEmptyComplete.Checked);
			_changed|=Prefs.UpdateInt(PrefName.ApptSchedEnforceSpecialty,comboApptSchedEnforceSpecialty.SelectedIndex);
			_changed|=Prefs.UpdateDouble(PrefName.FormClickDelay,comboDelay.GetSelected<double>(),doUseEnUSFormat:true);
			_changed|=Prefs.UpdateString(PrefName.AppointmentWithoutProcsDefaultLength,textApptWithoutProcsDefaultLength.Text);
			_changed|=Prefs.UpdateBool(PrefName.ReplaceExistingBlockout,checkReplaceBlockouts.Checked);
			_changed|=Prefs.UpdateBool(PrefName.UnscheduledListNoRecalls,checkUnscheduledListNoRecalls.Checked);
			_changed|=Prefs.UpdateString(PrefName.ApptAutoRefreshRange,textApptAutoRefreshRange.Text);
			_changed|=Prefs.UpdateBool(PrefName.ApptPreventChangesToCompleted,checkPreventChangesToComplAppts.Checked);
			_changed|=Prefs.UpdateYN(PrefName.ApptsAllowOverlap,checkApptsAllowOverlap.CheckState);
			_changed|=Prefs.UpdateString(PrefName.ApptFontSize,apptFontSize.ToString());
			_changed|=Prefs.UpdateInt(PrefName.ApptProvbarWidth,PIn.Int(textApptProvbarWidth.Text));
			_changed|=Prefs.UpdateBool(PrefName.BrokenApptRequiredOnMove,checkBrokenApptRequiredOnMove.Checked);
			if(comboBrokenApptAdjType.SelectedIndex!=-1) {
				_changed|=Prefs.UpdateLong(PrefName.BrokenAppointmentAdjustmentType,comboBrokenApptAdjType.GetSelectedDefNum());
			}
			if(comboProcDiscountType.SelectedIndex!=-1) {
				_changed|=Prefs.UpdateLong(PrefName.TreatPlanDiscountAdjustmentType,comboProcDiscountType.GetSelectedDefNum());
			}
			long timeArrivedTrigger=0;
			if(comboTimeArrived.SelectedIndex>0){
				timeArrivedTrigger=comboTimeArrived.GetSelectedDefNum();
			}
			List<string> listTriggerNewNums=new List<string>();
			if(Prefs.UpdateLong(PrefName.AppointmentTimeArrivedTrigger,timeArrivedTrigger)){
				listTriggerNewNums.Add(POut.Long(timeArrivedTrigger));
				_changed=true;
			}
			long timeSeatedTrigger=0;
			if(comboTimeSeated.SelectedIndex>0){
				timeSeatedTrigger=comboTimeSeated.GetSelectedDefNum();
			}
			if(Prefs.UpdateLong(PrefName.AppointmentTimeSeatedTrigger,timeSeatedTrigger)){
				listTriggerNewNums.Add(POut.Long(timeSeatedTrigger));
				_changed=true;
			}
			long timeDismissedTrigger=0;
			if(comboTimeDismissed.SelectedIndex>0){
				timeDismissedTrigger=comboTimeDismissed.GetSelectedDefNum();
			}
			if(Prefs.UpdateLong(PrefName.AppointmentTimeDismissedTrigger,timeDismissedTrigger)){
				listTriggerNewNums.Add(POut.Long(timeDismissedTrigger));
				_changed=true;
			}
			if(listTriggerNewNums.Count>0) {
				//Adds the appointment triggers to the list of confirmation statuses excluded from sending eConfirms,eReminders, and eThankYous.
				List<string> listEConfirm=UpdateDefNumsList(PrefName.ApptConfirmExcludeEConfirm,listTriggerNewNums);
				List<string> listESend=UpdateDefNumsList(PrefName.ApptConfirmExcludeESend,listTriggerNewNums);
				List<string> listERemind=UpdateDefNumsList(PrefName.ApptConfirmExcludeERemind,listTriggerNewNums);
				List<string> listEThanks=UpdateDefNumsList(PrefName.ApptConfirmExcludeEThankYou,listTriggerNewNums);
				List<string> listExcludeEclipboard=UpdateDefNumsList(PrefName.ApptConfirmExcludeEclipboard,listTriggerNewNums);
				List<string> listArrivalSMS=UpdateDefNumsList(PrefName.ApptConfirmExcludeArrivalSend,listTriggerNewNums);
				List<string> listArrivalResponse=UpdateDefNumsList(PrefName.ApptConfirmExcludeArrivalResponse,listTriggerNewNums);
				//Update new Value strings in database.  We don't remove the old ones.
				Prefs.UpdateString(PrefName.ApptConfirmExcludeEConfirm,string.Join(",",listEConfirm));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeESend,string.Join(",",listESend));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeERemind,string.Join(",",listERemind));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeEThankYou,string.Join(",",listEThanks));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeEclipboard,string.Join(",",listExcludeEclipboard));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalSend,string.Join(",",listArrivalSMS));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalResponse,string.Join(",",listArrivalResponse));
				//Only needs to be updated with the arrival trigger
				Prefs.UpdateString(PrefName.ApptConfirmByodEnabled,string.Join(",",PrefC.GetString(PrefName.ApptConfirmByodEnabled),
					timeArrivedTrigger));
			}
			return true;
		}
		#endregion Methods - Appts

		#region Methods - Family
		private void FillFamily(){
			checkInsurancePlansShared.Checked=PrefC.GetBool(PrefName.InsurancePlansShared);
			checkPPOpercentage.Checked=PrefC.GetBool(PrefName.InsDefaultPPOpercent);
			checkCoPayFeeScheduleBlankLikeZero.Checked=PrefC.GetBool(PrefName.CoPay_FeeSchedule_BlankLikeZero);
			checkFixedBenefitBlankLikeZero.Checked=PrefC.GetBool(PrefName.FixedBenefitBlankLikeZero);
			checkInsDefaultShowUCRonClaims.Checked=PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims);
			checkInsDefaultAssignmentOfBenefits.Checked=PrefC.GetBool(PrefName.InsDefaultAssignBen);
			checkInsPPOsecWriteoffs.Checked=PrefC.GetBool(PrefName.InsPPOsecWriteoffs);
			for(int i=0;i<Enum.GetNames(typeof(EnumCobRule)).Length;i++) {
				comboCobRule.Items.Add(Lan.g("enumEnumCobRule",Enum.GetNames(typeof(EnumCobRule))[i]));
			}
			comboCobRule.SelectedIndex=PrefC.GetInt(PrefName.InsDefaultCobRule);
			List<EclaimCobInsPaidBehavior> listCobs=Enum.GetValues(typeof(EclaimCobInsPaidBehavior)).Cast<EclaimCobInsPaidBehavior>().ToList();
			listCobs.Remove(EclaimCobInsPaidBehavior.Default);//Exclude Default option, as it is only for the carrier edit window.
			//The following line is similar to ComboBoxPlus.AddEnums(), except we need to exclude Default enum value, thus we are forced to mimic.
			comboCobSendPaidByInsAt.Items.AddList(listCobs,x=>Lan.g("enum"+typeof(EclaimCobInsPaidBehavior).Name,ODPrimitiveExtensions.GetDescription(x)));
			comboCobSendPaidByInsAt.SetSelectedEnum(PrefC.GetEnum<EclaimCobInsPaidBehavior>(PrefName.ClaimCobInsPaidBehavior));
			checkTextMsgOkStatusTreatAsNo.Checked=PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo);
			checkFamPhiAccess.Checked=PrefC.GetBool(PrefName.FamPhiAccess);
			checkGoogleAddress.Checked=PrefC.GetBool(PrefName.ShowFeatureGoogleMaps);
			checkSelectProv.Checked=PrefC.GetBool(PrefName.PriProvDefaultToSelectProv);
			if(!PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				groupBoxSuperFamily.Visible=false;
			}
			else {
				foreach(SortStrategy option in Enum.GetValues(typeof(SortStrategy))) {
					comboSuperFamSort.Items.Add(option.GetDescription());
				}
				comboSuperFamSort.SelectedIndex=PrefC.GetInt(PrefName.SuperFamSortStrategy);
				checkSuperFamSync.Checked=PrefC.GetBool(PrefName.PatientAllSuperFamilySync);
				checkSuperFamAddIns.Checked=PrefC.GetBool(PrefName.SuperFamNewPatAddIns);
				checkSuperFamCloneCreate.Checked=PrefC.GetBool(PrefName.CloneCreateSuperFamily);
			}
			//users should only see the claimsnapshot tab page if they have it set to something other than ClaimCreate.
			//if a user wants to be able to change claimsnapshot settings, the following MySQL statement should be run:
			//UPDATE preference SET ValueString = 'Service'	 WHERE PrefName = 'ClaimSnapshotTriggerType'
			if(PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true) == ClaimSnapshotTrigger.ClaimCreate) {
				groupBoxClaimSnapshot.Visible=false;
			}
			foreach(ClaimSnapshotTrigger trigger in Enum.GetValues(typeof(ClaimSnapshotTrigger))) {
				comboClaimSnapshotTrigger.Items.Add(trigger.GetDescription());
			}
			comboClaimSnapshotTrigger.SelectedIndex=(int)PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true);
			textClaimSnapshotRunTime.Text=PrefC.GetDateT(PrefName.ClaimSnapshotRunTime).ToShortTimeString();
			checkClaimUseOverrideProcDescript.Checked=PrefC.GetBool(PrefName.ClaimPrintProcChartedDesc);
			checkClaimTrackingRequireError.Checked=PrefC.GetBool(PrefName.ClaimTrackingRequiresError);
			checkPatInitBillingTypeFromPriInsPlan.Checked=PrefC.GetBool(PrefName.PatInitBillingTypeFromPriInsPlan);
			checkPreferredReferrals.Checked=PrefC.GetBool(PrefName.ShowPreferedReferrals);
			checkAutoFillPatEmail.Checked=PrefC.GetBool(PrefName.AddFamilyInheritsEmail);
			if(!PrefC.HasClinicsEnabled) {
				checkAllowPatsAtHQ.Visible=false;
			}
			checkAllowPatsAtHQ.Checked=PrefC.GetBool(PrefName.ClinicAllowPatientsAtHeadquarters);
			checkInsPlanExclusionsUseUCR.Checked=PrefC.GetBool(PrefName.InsPlanUseUcrFeeForExclusions);
			checkInsPlanExclusionsMarkDoNotBill.Checked=PrefC.GetBool(PrefName.InsPlanExclusionsMarkDoNotBillIns);
			checkPatientSSNMasked.Checked=PrefC.GetBool(PrefName.PatientSSNMasked);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				checkPatientSSNMasked.Text=Lan.g(this,"Mask patient Social Insurance Numbers");
			}
			checkPatientDOBMasked.Checked=PrefC.GetBool(PrefName.PatientDOBMasked);
			checkSameForFamily.Checked=PrefC.GetBool(PrefName.SameForFamilyCheckboxesUnchecked);
			_usePhonenumTable=PrefC.GetEnum<YN>(PrefName.PatientPhoneUsePhonenumberTable);
			if(_usePhonenumTable!=YN.Unknown) {
				checkUsePhoneNumTable.CheckState=CheckState.Unchecked;
				checkUsePhoneNumTable.Checked=_usePhonenumTable==YN.Yes;
			}
		}

		///<summary>Returns false if validation fails.</summary>
		private bool SaveFamily(){
			DateTime claimSnapshotRunTime=DateTime.MinValue;
			if(!DateTime.TryParse(textClaimSnapshotRunTime.Text,out claimSnapshotRunTime)) {
				MsgBox.Show(this,"Service Snapshot Run Time must be a valid time value.");
				return false;
			}
			if(_usePhonenumTable!=YN.Yes && checkUsePhoneNumTable.CheckState==CheckState.Checked) {//use CheckState since it can be indeterminate
				string msgText="When enabling the use of the phonenumber table a one-time sync of patient phone numbers needs to take place.  This could "+
					"take a couple minutes.  Continue?";
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
					return false;
				}
				if(!SyncPhoneNums()) {
					return false;
				}
				MsgBox.Show(this,"Done");
			}
			if(checkUsePhoneNumTable.CheckState!=CheckState.Indeterminate) {
				_changed|=Prefs.UpdateYN(PrefName.PatientPhoneUsePhonenumberTable,checkUsePhoneNumTable.Checked ? YN.Yes : YN.No);
			}
			claimSnapshotRunTime=new DateTime(1881,01,01,claimSnapshotRunTime.Hour,claimSnapshotRunTime.Minute,claimSnapshotRunTime.Second);
			_changed|=Prefs.UpdateBool(PrefName.InsurancePlansShared,checkInsurancePlansShared.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsDefaultPPOpercent,checkPPOpercentage.Checked);
			_changed|=Prefs.UpdateBool(PrefName.CoPay_FeeSchedule_BlankLikeZero,checkCoPayFeeScheduleBlankLikeZero.Checked);
			_changed|=Prefs.UpdateBool(PrefName.FixedBenefitBlankLikeZero,checkFixedBenefitBlankLikeZero.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsDefaultShowUCRonClaims,checkInsDefaultShowUCRonClaims.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsDefaultAssignBen,checkInsDefaultAssignmentOfBenefits.Checked);
			_changed|=Prefs.UpdateInt(PrefName.InsDefaultCobRule,comboCobRule.SelectedIndex);
			_changed|=Prefs.UpdateInt(PrefName.ClaimCobInsPaidBehavior,(int)comboCobSendPaidByInsAt.GetSelected<EclaimCobInsPaidBehavior>());
			_changed|=Prefs.UpdateBool(PrefName.TextMsgOkStatusTreatAsNo,checkTextMsgOkStatusTreatAsNo.Checked);
			_changed|=Prefs.UpdateBool(PrefName.FamPhiAccess,checkFamPhiAccess.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsPPOsecWriteoffs,checkInsPPOsecWriteoffs.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ShowFeatureGoogleMaps,checkGoogleAddress.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PriProvDefaultToSelectProv,checkSelectProv.Checked);
			_changed|=Prefs.UpdateInt(PrefName.SuperFamSortStrategy,comboSuperFamSort.SelectedIndex);
			_changed|=Prefs.UpdateBool(PrefName.PatientAllSuperFamilySync,checkSuperFamSync.Checked);
			_changed|=Prefs.UpdateBool(PrefName.SuperFamNewPatAddIns,checkSuperFamAddIns.Checked);
			_changed|=Prefs.UpdateBool(PrefName.CloneCreateSuperFamily,checkSuperFamCloneCreate.Checked);
			_changed|=Prefs.UpdateDateT(PrefName.ClaimSnapshotRunTime,claimSnapshotRunTime);
			_changed|=Prefs.UpdateBool(PrefName.ClaimPrintProcChartedDesc,checkClaimUseOverrideProcDescript.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClaimTrackingRequiresError,checkClaimTrackingRequireError.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PatInitBillingTypeFromPriInsPlan,checkPatInitBillingTypeFromPriInsPlan.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ShowPreferedReferrals,checkPreferredReferrals.Checked);
			_changed|=Prefs.UpdateBool(PrefName.AddFamilyInheritsEmail,checkAutoFillPatEmail.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClinicAllowPatientsAtHeadquarters,checkAllowPatsAtHQ.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsPlanExclusionsMarkDoNotBillIns,checkInsPlanExclusionsMarkDoNotBill.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,checkInsPlanExclusionsUseUCR.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PatientSSNMasked,checkPatientSSNMasked.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PatientDOBMasked,checkPatientDOBMasked.Checked);
			_changed|=Prefs.UpdateBool(PrefName.SameForFamilyCheckboxesUnchecked,checkSameForFamily.Checked);
			foreach(ClaimSnapshotTrigger trigger in Enum.GetValues(typeof(ClaimSnapshotTrigger))) {
				if(trigger.GetDescription()==comboClaimSnapshotTrigger.Text) {
					if(Prefs.UpdateString(PrefName.ClaimSnapshotTriggerType,trigger.ToString())) {
						_changed=true;
					}
					break;
				}
			}
			return true;
		}

		private bool SyncPhoneNums() {
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ShowCancelButton=false;
			progressOD.ActionMain=PhoneNumbers.SyncAllPats;
			progressOD.StartingMessage=Lan.g(this,"Syncing all patient phone numbers to the phonenumber table")+"...";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				MsgBox.Show(Lan.g(this,"The patient phone number sync failed with the message")+":\r\n"+ex.Message+"\r\n"+Lan.g(this,"Please try again."));
				return false;
			}
			if(progressOD.IsCancelled){
				return false;
			}
			_usePhonenumTable=YN.Yes;//so it won't sync again if you clicked the button
			return true;
		}
		#endregion Methods - Family

		#region Methods - Account
		private void FillAccount(){
			#region Pay/Adj Group Boxes
			checkStoreCCTokens.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
			foreach(PayClinicSetting prompt in Enum.GetValues(typeof(PayClinicSetting))) {
				comboPaymentClinicSetting.Items.Add(Lan.g(this,prompt.GetDescription()));
			}
			comboPaymentClinicSetting.SelectedIndex=PrefC.GetInt(PrefName.PaymentClinicSetting);
			checkPaymentsPromptForPayType.Checked=PrefC.GetBool(PrefName.PaymentsPromptForPayType);
			comboUnallocatedSplits.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,true));
			comboUnallocatedSplits.SetSelectedDefNum(PrefC.GetLong(PrefName.PrepaymentUnearnedType));
			comboPayPlanAdj.Items.AddDefs(_listDefsNegAdjTypes);
			comboPayPlanAdj.SetSelectedDefNum(PrefC.GetLong(PrefName.PayPlanAdjType));
			comboFinanceChargeAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboFinanceChargeAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.FinanceChargeAdjustmentType));
			comboBillingChargeAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboBillingChargeAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.BillingChargeAdjustmentType));
			comboLateChargeAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboLateChargeAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.LateChargeAdjustmentType));
			comboSalesTaxAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboSalesTaxAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.SalesTaxAdjustmentType));
			comboRefundAdjustmentType.Items.AddDefs(_listDefsNegAdjTypes);
			comboRefundAdjustmentType.SetSelectedDefNum(PrefC.GetLong(PrefName.RefundAdjustmentType));
			textTaxPercent.Text=PrefC.GetDouble(PrefName.SalesTaxPercentage).ToString();
			string[] arrayDefNums=PrefC.GetString(PrefName.BadDebtAdjustmentTypes).Split(new char[] {','}); //comma-delimited list.
			List<long> listBadAdjDefNums = new List<long>();
			foreach(string strDefNum in arrayDefNums) {
				listBadAdjDefNums.Add(PIn.Long(strDefNum));
			}
			FillListboxBadDebt(Defs.GetDefs(DefCat.AdjTypes,listBadAdjDefNums));
			checkAllowFutureDebits.Checked=PrefC.GetBool(PrefName.AccountAllowFutureDebits);
			checkAllowEmailCCReceipt.Checked=PrefC.GetBool(PrefName.AllowEmailCCReceipt);
			checkIncTxfrTreatNegProdAsIncome.Checked=PrefC.GetBool(PrefName.IncomeTransfersTreatNegativeProductionAsIncome);
			checkAllowPrepayProvider.Checked=PrefC.GetBool(PrefName.AllowPrepayProvider);
			comboRecurringChargePayType.Items.AddDefNone("("+Lan.g(this,"default")+")");
			comboRecurringChargePayType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaymentTypes,true));
			comboRecurringChargePayType.SetSelectedDefNum(PrefC.GetLong(PrefName.RecurringChargesPayTypeCC)); 
			comboDppUnearnedType.Items.AddDefs(Defs.GetHiddenUnearnedDefs(true));
			comboDppUnearnedType.SetSelectedDefNum(PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType));
			//Fill the combobox with providers
			comboSalesTaxDefaultProvider.Items.AddProvNone();
			comboSalesTaxDefaultProvider.Items.AddProvsAbbr(Providers.GetDeepCopy(true));
			comboSalesTaxDefaultProvider.SetSelectedProvNum(PrefC.GetLong(PrefName.SalesTaxDefaultProvider));
			checkAutomateSalesTax.Checked=PrefC.GetBool(PrefName.SalesTaxDoAutomate);
			#endregion Pay/Adj Group Boxes
			#region Insurance Group Box
			checkProviderIncomeShows.Checked=PrefC.GetBool(PrefName.ProviderIncomeTransferShows);
			checkClaimMedTypeIsInstWhenInsPlanIsMedical.Checked=PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical);
			checkClaimFormTreatDentSaysSigOnFile.Checked=PrefC.GetBool(PrefName.ClaimFormTreatDentSaysSigOnFile);
			textInsWriteoffDescript.Text=PrefC.GetString(PrefName.InsWriteoffDescript);
			textClaimAttachPath.Text=PrefC.GetString(PrefName.ClaimAttachExportPath);
			checkEclaimsMedicalProvTreatmentAsOrdering.Checked=PrefC.GetBool(PrefName.ClaimMedProvTreatmentAsOrdering);
			checkEclaimsSeparateTreatProv.Checked=PrefC.GetBool(PrefName.EclaimsSeparateTreatProv);
			checkClaimsValidateACN.Checked=PrefC.GetBool(PrefName.ClaimsValidateACN);
			checkAllowProcAdjFromClaim.Checked=PrefC.GetBool(PrefName.AllowProcAdjFromClaim);
			comboClaimCredit.Items.AddRange(Enum.GetNames(typeof(ClaimProcCreditsGreaterThanProcFee)));
			comboClaimCredit.SelectedIndex=PrefC.GetInt(PrefName.ClaimProcAllowCreditsGreaterThanProcFee);
			checkAllowFuturePayments.Checked=PrefC.GetBool(PrefName.AllowFutureInsPayments);
			textClaimIdentifier.Text=PrefC.GetString(PrefName.ClaimIdPrefix);
			foreach(ClaimZeroDollarProcBehavior procBehavior in Enum.GetValues(typeof(ClaimZeroDollarProcBehavior))) {
				comboZeroDollarProcClaimBehavior.Items.Add(Lan.g(this,procBehavior.ToString()));
			}
			comboZeroDollarProcClaimBehavior.SelectedIndex=PrefC.GetInt(PrefName.ClaimZeroDollarProcBehavior);
			checkClaimTrackingExcludeNone.Checked=PrefC.GetBool(PrefName.ClaimTrackingStatusExcludesNone);
			checkInsPayNoWriteoffMoreThanProc.Checked=PrefC.GetBool(PrefName.InsPayNoWriteoffMoreThanProc);
			checkPromptForSecondaryClaim.Checked=PrefC.GetBool(PrefName.PromptForSecondaryClaim);
			checkInsEstRecalcReceived.Checked=PrefC.GetBool(PrefName.InsEstRecalcReceived);
			checkPriClaimAllowSetToHoldUntilPriReceived.Checked=PrefC.GetBool(PrefName.PriClaimAllowSetToHoldUntilPriReceived);
			checkShowClaimPayTracking.CheckState=PrefC.GetYNCheckState(PrefName.ClaimEditShowPayTracking);
			checkShowClaimPatResp.CheckState=PrefC.GetYNCheckState(PrefName.ClaimEditShowPatResponsibility);
			checkCanadianPpoLabEst.Checked=PrefC.GetBool(PrefName.CanadaCreatePpoLabEst);
			#endregion Insurance Group Box
			#region Misc Account
			checkBalancesDontSubtractIns.Checked=PrefC.GetBool(PrefName.BalancesDontSubtractIns);
			checkAgingMonthly.Checked=PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily);
			if(!PrefC.GetBool(PrefName.AgingIsEnterprise)) {//AgingIsEnterprise requires aging to be daily
				checkAgingMonthly.Text=Lan.g(this,"Aging calculated monthly instead of daily");
				checkAgingMonthly.Enabled=true;
			}
			checkAccountShowPaymentNums.Checked=PrefC.GetBool(PrefName.AccountShowPaymentNums);
			checkShowFamilyCommByDefault.Checked=PrefC.GetBool(PrefName.ShowAccountFamilyCommEntries);
			checkRecurChargPriProv.Checked=PrefC.GetBool(PrefName.RecurringChargesUsePriProv);
			checkPpoUseUcr.Checked=PrefC.GetBool(PrefName.InsPpoAlwaysUseUcrFee);
			checkRecurringChargesUseTransDate.Checked=PrefC.GetBool(PrefName.RecurringChargesUseTransDate);
			checkStatementInvoiceGridShowWriteoffs.Checked=PrefC.GetBool(PrefName.InvoicePaymentsGridShowNetProd);
			checkShowAllocateUnearnedPaymentPrompt.Checked=PrefC.GetBool(PrefName.ShowAllocateUnearnedPaymentPrompt);
			checkPayPlansExcludePastActivity.Checked=PrefC.GetBool(PrefName.PayPlansExcludePastActivity);
			checkPayPlansUseSheets.Checked=PrefC.GetBool(PrefName.PayPlansUseSheets);
			checkAllowFutureTrans.Checked=PrefC.GetBool(PrefName.FutureTransDatesAllowed);
			checkAgingProcLifo.CheckState=PrefC.GetYNCheckState(PrefName.AgingProcLifo);
			foreach(PayPlanVersions version in Enum.GetValues(typeof(PayPlanVersions))) {
				comboPayPlansVersion.Items.Add(Lan.g("enumPayPlanVersions",version.GetDescription()));
			}
			comboPayPlansVersion.SelectedIndex=PrefC.GetInt(PrefName.PayPlansVersion) - 1;
			if(comboPayPlansVersion.SelectedIndex==(int)PayPlanVersions.AgeCreditsAndDebits-1) {//Minus 1 because the enum starts at 1.
				checkHideDueNow.Visible=true;
				checkHideDueNow.Checked=PrefC.GetBool(PrefName.PayPlanHideDueNow);
			}
			else {
				checkHideDueNow.Visible=false;
				checkHideDueNow.Checked=false;
			}
			checkRecurringChargesShowInactive.Checked=PrefC.GetBool(PrefName.RecurringChargesShowInactive);
			checkRecurringChargesAutomated.Checked=PrefC.GetBool(PrefName.RecurringChargesAutomatedEnabled);
			textRecurringChargesTime.Text=PrefC.GetDateT(PrefName.RecurringChargesAutomatedTime).TimeOfDay.ToShortTimeString();
			checkRecurPatBal0.Checked=PrefC.GetBool(PrefName.RecurringChargesAllowedWhenNoPatBal);
			checkRecurringChargesInactivateDeclinedCards.CheckState=PrefC.GetYNCheckState(PrefName.RecurringChargesInactivateDeclinedCards);
			if(PrefC.GetBool(PrefName.EasyHideRepeatCharges)) {
				groupRepeatingCharges.Enabled=false;
			}
			checkRepeatingChargesRunAging.Checked=PrefC.GetBool(PrefName.RepeatingChargesRunAging);
			checkRepeatingChargesAutomated.Checked=PrefC.GetBool(PrefName.RepeatingChargesAutomated);
			textRepeatingChargesAutomatedTime.Text=PrefC.GetDateT(PrefName.RepeatingChargesAutomatedTime).TimeOfDay.ToShortTimeString();
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				checkCanadianPpoLabEst.Visible=false;
			}
			textDynamicPayPlan.Text=PrefC.GetDateT(PrefName.DynamicPayPlanRunTime).TimeOfDay.ToShortTimeString();
			#endregion Misc Account
		}

		private void FillListboxBadDebt(List<Def> listSelectedDefs) {
			listboxBadDebtAdjs.Items.Clear();
			foreach(Def defCur in listSelectedDefs) {
				listboxBadDebtAdjs.Items.Add(defCur.ItemName,defCur);
			}
		}

		///<summary>Returns false if validation fails.</summary>
		private bool SaveAccount(){
			double taxPercent=0;
			if(!double.TryParse(textTaxPercent.Text,out taxPercent)) {
				MsgBox.Show(this,"Sales Tax percent is invalid.  Please enter a valid number to continue.");
				return false;
			}
			if(taxPercent<0) {
				MsgBox.Show(this,"Sales Tax percent cannot be a negative number.");
				return false;
			}
			if(checkRecurringChargesAutomated.Checked 
				&& (string.IsNullOrWhiteSpace(textRecurringChargesTime.Text) || !textRecurringChargesTime.IsValid())) 
			{
				MsgBox.Show(this,"Recurring charge time must be a valid time.");
				return false;
			}
			if(checkRepeatingChargesAutomated.Checked 
				&& (string.IsNullOrWhiteSpace(textRepeatingChargesAutomatedTime.Text) || !textRepeatingChargesAutomatedTime.IsValid())) 
			{
				MsgBox.Show(this,"Repeating charge time must be a valid time.");
				return false;
			}
			if(string.IsNullOrWhiteSpace(textDynamicPayPlan.Text) || !textDynamicPayPlan.IsValid()) {
				MsgBox.Show(this,"Dynamic payment plan time must be a valid time.");
				return false;
			}
			string strListBadDebtAdjTypes=string.Join(",",listboxBadDebtAdjs.Items.GetAll<Def>().Select(x => x.DefNum));
			_changed|=Prefs.UpdateBool(PrefName.BalancesDontSubtractIns,checkBalancesDontSubtractIns.Checked);
			_changed|=Prefs.UpdateBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily,checkAgingMonthly.Checked);
			_changed|=Prefs.UpdateBool(PrefName.StoreCCtokens,checkStoreCCTokens.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ProviderIncomeTransferShows,checkProviderIncomeShows.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ShowAccountFamilyCommEntries,checkShowFamilyCommByDefault.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClaimFormTreatDentSaysSigOnFile,checkClaimFormTreatDentSaysSigOnFile.Checked);
			_changed|=Prefs.UpdateString(PrefName.ClaimAttachExportPath,textClaimAttachPath.Text);
			_changed|=Prefs.UpdateBool(PrefName.EclaimsSeparateTreatProv,checkEclaimsSeparateTreatProv.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClaimsValidateACN,checkClaimsValidateACN.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical,checkClaimMedTypeIsInstWhenInsPlanIsMedical.Checked);
			_changed|=Prefs.UpdateBool(PrefName.AccountShowPaymentNums,checkAccountShowPaymentNums.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PayPlansUseSheets,checkPayPlansUseSheets.Checked);
			_changed|=Prefs.UpdateBool(PrefName.RecurringChargesUsePriProv,checkRecurChargPriProv.Checked);
			_changed|=Prefs.UpdateString(PrefName.InsWriteoffDescript,textInsWriteoffDescript.Text);
			_changed|=Prefs.UpdateInt(PrefName.PaymentClinicSetting,comboPaymentClinicSetting.SelectedIndex);
			_changed|=Prefs.UpdateBool(PrefName.PaymentsPromptForPayType,checkPaymentsPromptForPayType.Checked);
			_changed|=Prefs.UpdateInt(PrefName.PayPlansVersion,comboPayPlansVersion.SelectedIndex+1);
			_changed|=Prefs.UpdateBool(PrefName.ClaimMedProvTreatmentAsOrdering,checkEclaimsMedicalProvTreatmentAsOrdering.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsPpoAlwaysUseUcrFee,checkPpoUseUcr.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ProcPromptForAutoNote,checkProcsPromptForAutoNote.Checked);
			_changed|=Prefs.UpdateDouble(PrefName.SalesTaxPercentage,taxPercent,false);//Do not round this double for Hawaii
			_changed|=Prefs.UpdateBool(PrefName.PayPlansExcludePastActivity,checkPayPlansExcludePastActivity.Checked);
			_changed|=Prefs.UpdateBool(PrefName.RecurringChargesUseTransDate,checkRecurringChargesUseTransDate.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InvoicePaymentsGridShowNetProd,checkStatementInvoiceGridShowWriteoffs.Checked);
			_changed|=Prefs.UpdateBool(PrefName.AccountAllowFutureDebits,checkAllowFutureDebits.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PayPlanHideDueNow,checkHideDueNow.Checked);
			_changed|=Prefs.UpdateBool(PrefName.AllowProcAdjFromClaim,checkAllowProcAdjFromClaim.Checked);
			_changed|=Prefs.UpdateInt(PrefName.ClaimProcAllowCreditsGreaterThanProcFee,comboClaimCredit.SelectedIndex);
			_changed|=Prefs.UpdateBool(PrefName.AllowEmailCCReceipt,checkAllowEmailCCReceipt.Checked);
			_changed|=Prefs.UpdateString(PrefName.ClaimIdPrefix,textClaimIdentifier.Text);
			_changed|=Prefs.UpdateString(PrefName.BadDebtAdjustmentTypes,strListBadDebtAdjTypes);
			_changed|=Prefs.UpdateBool(PrefName.AllowFutureInsPayments,checkAllowFuturePayments.Checked);
			_changed|=Prefs.UpdateBool(PrefName.IncomeTransfersTreatNegativeProductionAsIncome,checkIncTxfrTreatNegProdAsIncome.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ShowAllocateUnearnedPaymentPrompt,checkShowAllocateUnearnedPaymentPrompt.Checked);
			_changed|=Prefs.UpdateBool(PrefName.FutureTransDatesAllowed,checkAllowFutureTrans.Checked);
			_changed|=Prefs.UpdateYN(PrefName.AgingProcLifo,checkAgingProcLifo.CheckState);
			_changed|=Prefs.UpdateBool(PrefName.AllowPrepayProvider,checkAllowPrepayProvider.Checked);
			_changed|=Prefs.UpdateBool(PrefName.RecurringChargesShowInactive,checkRecurringChargesShowInactive.Checked);
			_changed|=Prefs.UpdateBool(PrefName.RecurringChargesAutomatedEnabled,checkRecurringChargesAutomated.Checked);
			_changed|=Prefs.UpdateDateT(PrefName.RecurringChargesAutomatedTime,PIn.DateT(textRecurringChargesTime.Text));
			_changed|=Prefs.UpdateBool(PrefName.RepeatingChargesAutomated,checkRepeatingChargesAutomated.Checked);
			_changed|=Prefs.UpdateDateT(PrefName.RepeatingChargesAutomatedTime,PIn.DateT(textRepeatingChargesAutomatedTime.Text));
			_changed|=Prefs.UpdateBool(PrefName.RepeatingChargesRunAging,checkRepeatingChargesRunAging.Checked);
			_changed|=Prefs.UpdateInt(PrefName.ClaimZeroDollarProcBehavior,comboZeroDollarProcClaimBehavior.SelectedIndex);
			_changed|=Prefs.UpdateBool(PrefName.ClaimTrackingStatusExcludesNone,checkClaimTrackingExcludeNone.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsPayNoWriteoffMoreThanProc,checkInsPayNoWriteoffMoreThanProc.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PromptForSecondaryClaim,checkPromptForSecondaryClaim.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsEstRecalcReceived,checkInsEstRecalcReceived.Checked);
			_changed|=Prefs.UpdateBool(PrefName.CanadaCreatePpoLabEst,checkCanadianPpoLabEst.Checked);
			_changed|=Prefs.UpdateLong(PrefName.RecurringChargesPayTypeCC,comboRecurringChargePayType.GetSelectedDefNum());
			_changed|=Prefs.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,checkRecurPatBal0.Checked);
			_changed|=Prefs.UpdateYN(PrefName.RecurringChargesInactivateDeclinedCards,checkRecurringChargesInactivateDeclinedCards.CheckState);
			_changed|=Prefs.UpdateDateT(PrefName.DynamicPayPlanRunTime,PIn.DateT(textDynamicPayPlan.Text));
			_changed|=Prefs.UpdateBool(PrefName.PriClaimAllowSetToHoldUntilPriReceived,checkPriClaimAllowSetToHoldUntilPriReceived.Checked);
			_changed|=Prefs.UpdateYN(PrefName.ClaimEditShowPayTracking,checkShowClaimPayTracking.CheckState);
			_changed|=Prefs.UpdateYN(PrefName.ClaimEditShowPatResponsibility,checkShowClaimPatResp.CheckState);
			_changed|=Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,comboSalesTaxDefaultProvider.GetSelectedProvNum());
			_changed|=Prefs.UpdateBool(PrefName.SalesTaxDoAutomate,checkAutomateSalesTax.Checked);
			_changed|=Prefs.UpdateLong(PrefName.RefundAdjustmentType,comboRefundAdjustmentType.GetSelectedDefNum());
			_changed|=Prefs.UpdateLong(PrefName.DynamicPayPlanPrepaymentUnearnedType,comboDppUnearnedType.GetSelectedDefNum());
			if(comboFinanceChargeAdjType.SelectedIndex!=-1) {
				_changed|=Prefs.UpdateLong(PrefName.FinanceChargeAdjustmentType,comboFinanceChargeAdjType.GetSelectedDefNum());
			}
			if(comboBillingChargeAdjType.SelectedIndex!=-1) {
				_changed|=Prefs.UpdateLong(PrefName.BillingChargeAdjustmentType,comboBillingChargeAdjType.GetSelectedDefNum());
			}
			if(comboLateChargeAdjType.SelectedIndex!=-1) {
				_changed|=Prefs.UpdateLong(PrefName.LateChargeAdjustmentType,comboLateChargeAdjType.GetSelectedDefNum());
			}
			if(comboSalesTaxAdjType.SelectedIndex!=-1) {
				_changed|=Prefs.UpdateLong(PrefName.SalesTaxAdjustmentType,comboSalesTaxAdjType.GetSelectedDefNum());
			}
			if(comboPayPlanAdj.SelectedIndex!=-1) {
				_changed|=Prefs.UpdateLong(PrefName.PayPlanAdjType,comboPayPlanAdj.GetSelectedDefNum());
			}
			if(comboUnallocatedSplits.SelectedIndex!=-1) {
				_changed|=Prefs.UpdateLong(PrefName.PrepaymentUnearnedType,comboUnallocatedSplits.GetSelectedDefNum());
			}
			return true;
		}
		#endregion Methods - Account

		#region Methods - Treat' Plan
		private void FillTreatPlan(){
			textTreatNote.Text=PrefC.GetString(PrefName.TreatmentPlanNote);
			checkTreatPlanShowCompleted.Checked=PrefC.GetBool(PrefName.TreatPlanShowCompleted);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkTreatPlanShowCompleted.Visible=false;
			}
			else {
				checkTreatPlanShowCompleted.Checked=PrefC.GetBool(PrefName.TreatPlanShowCompleted);
			}
			checkTreatPlanItemized.Checked=PrefC.GetBool(PrefName.TreatPlanItemized);
			checkTPSaveSigned.Checked=PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf);
			checkFrequency.Checked=PrefC.GetBool(PrefName.InsChecksFrequency);
			textInsBW.Text=PrefC.GetString(PrefName.InsBenBWCodes);
			textInsPano.Text=PrefC.GetString(PrefName.InsBenPanoCodes);
			textInsExam.Text=PrefC.GetString(PrefName.InsBenExamCodes);
			textInsCancerScreen.Text=PrefC.GetString(PrefName.InsBenCancerScreeningCodes);
			textInsProphy.Text=PrefC.GetString(PrefName.InsBenProphyCodes);
			textInsFlouride.Text=PrefC.GetString(PrefName.InsBenFlourideCodes);
			textInsSealant.Text=PrefC.GetString(PrefName.InsBenSealantCodes);
			textInsCrown.Text=PrefC.GetString(PrefName.InsBenCrownCodes);
			textInsSRP.Text=PrefC.GetString(PrefName.InsBenSRPCodes);
			textInsDebridement.Text=PrefC.GetString(PrefName.InsBenFullDebridementCodes);
			textInsPerioMaint.Text=PrefC.GetString(PrefName.InsBenPerioMaintCodes);
			textInsDentures.Text=PrefC.GetString(PrefName.InsBenDenturesCodes);
			textInsImplant.Text=PrefC.GetString(PrefName.InsBenImplantCodes);
			if(!checkFrequency.Checked) {
				textInsBW.Enabled=false;
				textInsPano.Enabled=false;
				textInsExam.Enabled=false;
				textInsCancerScreen.Enabled=false;
				textInsProphy.Enabled=false;
				textInsFlouride.Enabled=false;
				textInsSealant.Enabled=false;
				textInsCrown.Enabled=false;
				textInsSRP.Enabled=false;
				textInsDebridement.Enabled=false;
				textInsPerioMaint.Enabled=false;
				textInsDentures.Enabled=false;
				textInsImplant.Enabled=false;
			}
			#region Discount Plan Frequency Codes
			textDiscountExamCodes.Text=PrefC.GetString(PrefName.DiscountPlanExamCodes);
			textDiscountFluorideCodes.Text=PrefC.GetString(PrefName.DiscountPlanFluorideCodes);
			textDiscountLimitedCodes.Text=PrefC.GetString(PrefName.DiscountPlanLimitedCodes);
			textDiscountPACodes.Text=PrefC.GetString(PrefName.DiscountPlanPACodes);
			textDiscountPerioCodes.Text=PrefC.GetString(PrefName.DiscountPlanPerioCodes);
			textDiscountProphyCodes.Text=PrefC.GetString(PrefName.DiscountPlanProphyCodes);
			textDiscountXrayCodes.Text=PrefC.GetString(PrefName.DiscountPlanXrayCodes);
			#endregion
			radioTreatPlanSortTooth.Checked=PrefC.GetBool(PrefName.TreatPlanSortByTooth) || PrefC.RandomKeys;
			//Currently, the TreatPlanSortByTooth preference gets overridden by 
			//the RandomPrimaryKeys preferece due to "Order Entered" being based on the ProcNum
			groupTreatPlanSort.Enabled=!PrefC.RandomKeys;
			textInsHistBW.Text=PrefC.GetString(PrefName.InsHistBWCodes);
			textInsHistDebridement.Text=PrefC.GetString(PrefName.InsHistDebridementCodes);
			textInsHistExam.Text=PrefC.GetString(PrefName.InsHistExamCodes);
			textInsHistFMX.Text=PrefC.GetString(PrefName.InsHistPanoCodes);
			textInsHistPerioMaint.Text=PrefC.GetString(PrefName.InsHistPerioMaintCodes);
			textInsHistPerioLL.Text=PrefC.GetString(PrefName.InsHistPerioLLCodes);
			textInsHistPerioLR.Text=PrefC.GetString(PrefName.InsHistPerioLRCodes);
			textInsHistPerioUL.Text=PrefC.GetString(PrefName.InsHistPerioULCodes);
			textInsHistPerioUR.Text=PrefC.GetString(PrefName.InsHistPerioURCodes);
			textInsHistProphy.Text=PrefC.GetString(PrefName.InsHistProphyCodes);
			checkPromptSaveTP.Checked=PrefC.GetBool(PrefName.TreatPlanPromptSave);
		}

		///<summary>The preference passed is assumed to be comma delimited list of procedure codes.
		///Updates and returns true if all proc codes in textProcCodes are valid. Otherwise, we add these codes to errorMessage and returns false.</summary>
		private bool UpdateProcCodesPref(PrefName prefName,string textProcCodes,string labelText,ref string errorMessage) {
			List<string> listProcCodesInvalid=new List<string>();
			List<string> listProcCodes=textProcCodes
				.Split(",",StringSplitOptions.RemoveEmptyEntries)
				.ToList();
			for(int i=0;i<listProcCodes.Count;i++) {
				if(!ProcedureCodes.GetContainsKey(listProcCodes[i])) {
					listProcCodesInvalid.Add($"'{listProcCodes[i]}'");
				}
			}
			if(listProcCodesInvalid.Count > 0) {
				errorMessage+=$"\r\n  - {labelText}: {string.Join(",",listProcCodesInvalid)}";
				return false;
			}
			//All valid codes in text box.
			return Prefs.UpdateString(prefName,textProcCodes);
		}

		///<summary>Returns false if validation fails.</summary>
		private bool SaveTreatPlan(){
			float percent=0;
			if(!float.TryParse(textDiscountPercentage.Text,out percent)) {
				MsgBox.Show(this,"Procedure discount percent is invalid. Please enter a valid number to continue.");
				return false;
			}
			if(PrefC.GetString(PrefName.TreatmentPlanNote)!=textTreatNote.Text) {
				List<long> listTreatPlanNums=TreatPlans.GetNumsByNote(PrefC.GetString(PrefName.TreatmentPlanNote));//Find active/inactive TP's that match exactly.
				if(listTreatPlanNums.Count>0) {
					DialogResult dr=MessageBox.Show(Lan.g(this,"Unsaved treatment plans found with default notes")+": "+listTreatPlanNums.Count+"\r\n"
						+Lan.g(this,"Would you like to change them now?"),"",MessageBoxButtons.YesNoCancel);
					switch(dr) {
						case DialogResult.Cancel:
							return false;
						case DialogResult.Yes:
						case DialogResult.OK:
							TreatPlans.UpdateNotes(textTreatNote.Text,listTreatPlanNums);//change tp notes
							break;
						default://includes "No"
							//do nothing
							break;
					}
				}
			}
			string errorMessage="";
			//Should we somehow be returning false once all are updated and there were code errors so the window doesn't close and they can edit the errors? Or do we not care enough?
			_changed|=Prefs.UpdateString(PrefName.TreatmentPlanNote,textTreatNote.Text);
			_changed|=Prefs.UpdateBool(PrefName.TreatPlanShowCompleted,checkTreatPlanShowCompleted.Checked);
			_changed|=Prefs.UpdateDouble(PrefName.TreatPlanDiscountPercent,percent);
			_changed|=Prefs.UpdateBool(PrefName.TreatPlanItemized,checkTreatPlanItemized.Checked);
			_changed|=Prefs.UpdateBool(PrefName.TreatPlanSaveSignedToPdf,checkTPSaveSigned.Checked);
			_changed|=Prefs.UpdateBool(PrefName.InsChecksFrequency,checkFrequency.Checked);
			_changed|=Prefs.UpdateBool(PrefName.TreatPlanSortByTooth,radioTreatPlanSortTooth.Checked || PrefC.RandomKeys);
			_changed|=Prefs.UpdateBool(PrefName.TreatPlanPromptSave, checkPromptSaveTP.Checked);
			_changed|=UpdateProcCodesPref(PrefName.InsBenBWCodes,textInsBW.Text,labelInsBW.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenPanoCodes,textInsPano.Text,labelInsPano.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenExamCodes,textInsExam.Text,labelInsExam.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenCancerScreeningCodes,textInsCancerScreen.Text,labelInsCancerScreen.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenProphyCodes,textInsProphy.Text,labelInsProphy.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenFlourideCodes,textInsFlouride.Text,labelInsFlouride.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenSealantCodes,textInsSealant.Text,labelInsSealant.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenCrownCodes,textInsCrown.Text,labelInsCrown.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenSRPCodes,textInsSRP.Text,labelInsSRP.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenFullDebridementCodes,textInsDebridement.Text,labelInsDebridement.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenPerioMaintCodes,textInsPerioMaint.Text,labelInsPerioMaint.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenDenturesCodes,textInsDentures.Text,labelInsDentures.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsBenImplantCodes,textInsImplant.Text,labelInsImplant.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.DiscountPlanExamCodes,textDiscountExamCodes.Text,labelDiscountExamFreq.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.DiscountPlanProphyCodes,textDiscountProphyCodes.Text,labelDiscountProphyFreq.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.DiscountPlanFluorideCodes,textDiscountFluorideCodes.Text,labelDiscountFluorideFreq.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.DiscountPlanPerioCodes,textDiscountPerioCodes.Text,labelDiscountPerioFreq.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.DiscountPlanLimitedCodes,textDiscountLimitedCodes.Text,labelDiscountLimitedFreq.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.DiscountPlanXrayCodes,textDiscountXrayCodes.Text,labelDiscountXrayFreq.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.DiscountPlanPACodes,textDiscountPACodes.Text,labelDiscountPAFreq.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistBWCodes,textInsHistBW.Text,labelInsHistBW.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistPanoCodes,textInsHistFMX.Text,labelInsHistFMX.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistExamCodes,textInsHistExam.Text,labelInsHistExam.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistProphyCodes,textInsHistProphy.Text,labelInsHistProphy.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistDebridementCodes,textInsHistDebridement.Text,labelInsHistDebridement.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistPerioMaintCodes,textInsHistPerioMaint.Text,labelInsHistPerioMaint.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistPerioLLCodes,textInsHistPerioLL.Text,labelInsHistPerioLL.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistPerioLRCodes,textInsHistPerioLR.Text,labelInsHistPerioLR.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistPerioULCodes,textInsHistPerioUL.Text,labelInsHistPerioUL.Text,ref errorMessage);
			_changed|=UpdateProcCodesPref(PrefName.InsHistPerioURCodes,textInsHistPerioUR.Text,labelInsHistPerioUR.Text,ref errorMessage);
			if(!string.IsNullOrEmpty(errorMessage)) {//Keep them in the window if invalid codes found.
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g(this,"Invalid Treat' Plan procedure codes were detected and need to be corrected before saving.")+$"{errorMessage}");
				msgBoxCopyPaste.ShowDialog();
				return false;
			}
			return true;
		}
		#endregion Methods - Treat' Plan

		#region Methods - Chart
		private void FillChart(){
			comboToothNomenclature.Items.Add(Lan.g(this,"Universal (Common in the US, 1-32)"));
			comboToothNomenclature.Items.Add(Lan.g(this,"FDI Notation (International, 11-48)"));
			comboToothNomenclature.Items.Add(Lan.g(this,"Haderup (Danish)"));
			comboToothNomenclature.Items.Add(Lan.g(this,"Palmer (Ortho)"));
			comboToothNomenclature.SelectedIndex = PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelToothNomenclature.Visible=false;
				comboToothNomenclature.Visible=false;
			}
			checkAutoClearEntryStatus.Checked=PrefC.GetBool(PrefName.AutoResetTPEntryStatus);
			checkAllowSettingProcsComplete.Checked=PrefC.GetBool(PrefName.AllowSettingProcsComplete);
			//checkChartQuickAddHideAmalgam.Checked=PrefC.GetBool(PrefName.ChartQuickAddHideAmalgam); //Deprecated.
			//checkToothChartMoveMenuToRight.Checked=PrefC.GetBool(PrefName.ToothChartMoveMenuToRight);
			textProblemsIndicateNone.Text		=DiseaseDefs.GetName(PrefC.GetLong(PrefName.ProblemsIndicateNone)); //DB maint to fix corruption
			_diseaseDefNum=PrefC.GetLong(PrefName.ProblemsIndicateNone);
			textMedicationsIndicateNone.Text=Medications.GetDescription(PrefC.GetLong(PrefName.MedicationsIndicateNone)); //DB maint to fix corruption
			_medicationNum=PrefC.GetLong(PrefName.MedicationsIndicateNone);
			textAllergiesIndicateNone.Text	=AllergyDefs.GetDescription(PrefC.GetLong(PrefName.AllergiesIndicateNone)); //DB maint to fix corruption
			_alergyDefNum=PrefC.GetLong(PrefName.AllergiesIndicateNone);
			checkProcGroupNoteDoesAggregate.Checked=PrefC.GetBool(PrefName.ProcGroupNoteDoesAggregate);
			checkChartNonPatientWarn.Checked=PrefC.GetBool(PrefName.ChartNonPatientWarn);
			//checkChartAddProcNoRefreshGrid.Checked=PrefC.GetBool(PrefName.ChartAddProcNoRefreshGrid);//Not implemented.  May revisit some day.
			checkMedicalFeeUsedForNewProcs.Checked=PrefC.GetBool(PrefName.MedicalFeeUsedForNewProcs);
			checkProvColorChart.Checked=PrefC.GetBool(PrefName.UseProviderColorsInChart);
			checkPerioSkipMissingTeeth.Checked=PrefC.GetBool(PrefName.PerioSkipMissingTeeth);
			checkPerioTreatImplantsAsNotMissing.Checked=PrefC.GetBool(PrefName.PerioTreatImplantsAsNotMissing);
			if(PrefC.GetByte(PrefName.DxIcdVersion)==9) {
				checkDxIcdVersion.Checked=false;
			}
			else {//ICD-10
				checkDxIcdVersion.Checked=true;
			}
			SetIcdLabels();
			textICD9DefaultForNewProcs.Text=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
			checkProcLockingIsAllowed.Checked=PrefC.GetBool(PrefName.ProcLockingIsAllowed);
			textMedDefaultStopDays.Text=PrefC.GetString(PrefName.MedDefaultStopDays);
			checkScreeningsUseSheets.Checked=PrefC.GetBool(PrefName.ScreeningsUseSheets);
			checkProcsPromptForAutoNote.Checked=PrefC.GetBool(PrefName.ProcPromptForAutoNote);
			for(int i=0;i<Enum.GetNames(typeof(ProcCodeListSort)).Length;i++) {
				comboProcCodeListSort.Items.Add(Enum.GetNames(typeof(ProcCodeListSort))[i]);
			}
			comboProcCodeListSort.SelectedIndex=PrefC.GetInt(PrefName.ProcCodeListSortOrder);
			checkProcEditRequireAutoCode.Checked=PrefC.GetBool(PrefName.ProcEditRequireAutoCodes);
			checkClaimProcsAllowEstimatesOnCompl.Checked=PrefC.GetBool(PrefName.ClaimProcsAllowedToBackdate);
			checkSignatureAllowDigital.Checked=PrefC.GetBool(PrefName.SignatureAllowDigital);
			checkCommLogAutoSave.Checked=PrefC.GetBool(PrefName.CommLogAutoSave);
			comboProcFeeUpdatePrompt.Items.Add(Lan.g(this,"No prompt, don't change fee"));
			comboProcFeeUpdatePrompt.Items.Add(Lan.g(this,"No prompt, always change fee"));
			comboProcFeeUpdatePrompt.Items.Add(Lan.g(this,"Prompt, when patient portion changes"));
			comboProcFeeUpdatePrompt.Items.Add(Lan.g(this,"Prompt, always"));
			comboProcFeeUpdatePrompt.SelectedIndex=PrefC.GetInt(PrefName.ProcFeeUpdatePrompt);
			checkProcProvChangesCp.Checked=PrefC.GetBool(PrefName.ProcProvChangesClaimProcWithClaim);
			checkBoxRxClinicUseSelected.Checked=PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected);
			if(!PrefC.HasClinicsEnabled) {
				checkBoxRxClinicUseSelected.Visible=false;
			}
			checkProcNoteConcurrencyMerge.Checked=PrefC.GetBool(PrefName.ProcNoteConcurrencyMerge);
			checkIsAlertRadiologyProcsEnabled.Checked=PrefC.GetBool(PrefName.IsAlertRadiologyProcsEnabled);
			checkShowPlannedApptPrompt.Checked=PrefC.GetBool(PrefName.ShowPlannedAppointmentPrompt);
			checkNotesProviderSigOnly.Checked=PrefC.GetBool(PrefName.NotesProviderSignatureOnly);
		}

		///<summary>Returns false if validation fails.</summary>
		private bool SaveChart(){
			int daysStop=0;
			if(!int.TryParse(textMedDefaultStopDays.Text,out daysStop)) {
				MsgBox.Show(this,"Days until medication order stop date entered was is invalid. Please enter a valid number to continue.");
				return false;
			}
			if(daysStop<0) {
				MsgBox.Show(this,"Days until medication order stop date cannot be a negative number.");
				return false;
			}
			_changed|=Prefs.UpdateBool(PrefName.AllowSettingProcsComplete,checkAllowSettingProcsComplete.Checked);
			_changed|=Prefs.UpdateBool(PrefName.AutoResetTPEntryStatus,checkAutoClearEntryStatus.Checked);
			_changed|=Prefs.UpdateLong(PrefName.ProblemsIndicateNone,_diseaseDefNum);
			_changed|=Prefs.UpdateLong(PrefName.MedicationsIndicateNone,_medicationNum);
			_changed|=Prefs.UpdateLong(PrefName.AllergiesIndicateNone,_alergyDefNum);
			_changed|=Prefs.UpdateLong(PrefName.UseInternationalToothNumbers,comboToothNomenclature.SelectedIndex);
			_changed|=Prefs.UpdateBool(PrefName.ProcGroupNoteDoesAggregate,checkProcGroupNoteDoesAggregate.Checked);
			_changed|=Prefs.UpdateBool(PrefName.MedicalFeeUsedForNewProcs,checkMedicalFeeUsedForNewProcs.Checked);
			_changed|=Prefs.UpdateByte(PrefName.DxIcdVersion,(byte)(checkDxIcdVersion.Checked?10:9));
			_changed|=Prefs.UpdateString(PrefName.ICD9DefaultForNewProcs,textICD9DefaultForNewProcs.Text);
			_changed|=Prefs.UpdateBool(PrefName.ProcLockingIsAllowed,checkProcLockingIsAllowed.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ChartNonPatientWarn,checkChartNonPatientWarn.Checked);
			_changed|=Prefs.UpdateInt(PrefName.MedDefaultStopDays,daysStop);
			_changed|=Prefs.UpdateBool(PrefName.UseProviderColorsInChart,checkProvColorChart.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PerioSkipMissingTeeth,checkPerioSkipMissingTeeth.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PerioTreatImplantsAsNotMissing,checkPerioTreatImplantsAsNotMissing.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ScreeningsUseSheets,checkScreeningsUseSheets.Checked);
			_changed|=Prefs.UpdateInt(PrefName.ProcCodeListSortOrder,comboProcCodeListSort.SelectedIndex);
			_changed|=Prefs.UpdateBool(PrefName.ProcEditRequireAutoCodes,checkProcEditRequireAutoCode.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,checkClaimProcsAllowEstimatesOnCompl.Checked);
			_changed|=Prefs.UpdateBool(PrefName.SignatureAllowDigital,checkSignatureAllowDigital.Checked);
			_changed|=Prefs.UpdateBool(PrefName.CommLogAutoSave,checkCommLogAutoSave.Checked);
			_changed|=Prefs.UpdateLong(PrefName.ProcFeeUpdatePrompt,comboProcFeeUpdatePrompt.SelectedIndex);
			//_changed|=Prefs.UpdateBool(PrefName.ToothChartMoveMenuToRight,checkToothChartMoveMenuToRight.Checked);
			//_changed|=Prefs.UpdateBool(PrefName.ChartQuickAddHideAmalgam, checkChartQuickAddHideAmalgam.Checked); //Deprecated.
			//_changed|=Prefs.UpdateBool(PrefName.ChartAddProcNoRefreshGrid,checkChartAddProcNoRefreshGrid.Checked);//Not implemented.  May revisit someday.
			_changed|=Prefs.UpdateBool(PrefName.ProcProvChangesClaimProcWithClaim,checkProcProvChangesCp.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ElectronicRxClinicUseSelected,checkBoxRxClinicUseSelected.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ProcNoteConcurrencyMerge,checkProcNoteConcurrencyMerge.Checked);
			_changed|=Prefs.UpdateBool(PrefName.IsAlertRadiologyProcsEnabled,checkIsAlertRadiologyProcsEnabled.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ShowPlannedAppointmentPrompt,checkShowPlannedApptPrompt.Checked);
			_changed|=Prefs.UpdateBool(PrefName.NotesProviderSignatureOnly,checkNotesProviderSigOnly.Checked);
			return true;
		}

		private void SetIcdLabels() {
			byte icdVersion=9;
			if(checkDxIcdVersion.Checked) {
				icdVersion=10;
			}
			labelIcdCodeDefault.Text=Lan.g(this,"Default ICD")+"-"+icdVersion+" "+Lan.g(this,"code for new procedures and when set complete");
		}
		#endregion Methods - Chart

		#region Methods - Images
		private void FillImages(){
			/*switch(PrefC.GetInt(PrefName.ImagesModuleTreeIsCollapsed)) {
				case 0:
					radioImagesModuleTreeIsExpanded.Checked=true;
					break;
				case 1:
					radioImagesModuleTreeIsCollapsed.Checked=true;
					break;
				case 2:
					radioImagesModuleTreeIsPersistentPerUser.Checked=true;
					break;
			}*/
			textDefaultImageImportFolder.Text=PrefC.GetString(PrefName.DefaultImageImportFolder);
			textAutoImportFolder.Text=PrefC.GetString(PrefName.AutoImportFolder);
			checkPDFLaunchWindow.Checked=PrefC.GetBool(PrefName.PdfLaunchWindow);
		}

		///<summary>Returns false if validation fails.</summary>
		private bool SaveImages(){
			/*
			int imageModuleIsCollapsedVal=0;
			if(radioImagesModuleTreeIsExpanded.Checked) {
				imageModuleIsCollapsedVal=0;
			}
			else if(radioImagesModuleTreeIsCollapsed.Checked) {
				imageModuleIsCollapsedVal=1;
			}
			else if(radioImagesModuleTreeIsPersistentPerUser.Checked) {
				imageModuleIsCollapsedVal=2;
			}
			_changed|= Prefs.UpdateInt(PrefName.ImagesModuleTreeIsCollapsed,imageModuleIsCollapsedVal);*/
			_changed|=Prefs.UpdateString(PrefName.DefaultImageImportFolder,textDefaultImageImportFolder.Text);
			_changed|=Prefs.UpdateString(PrefName.AutoImportFolder,textAutoImportFolder.Text);
			_changed|=Prefs.UpdateBool(PrefName.PdfLaunchWindow,checkPDFLaunchWindow.Checked);
			return true;
		}
		#endregion Methods - Images

		#region Methods - Manage
		private void comboDepositSoftware_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboDepositSoftware.GetSelected<AccountingSoftware>()==AccountingSoftware.QuickBooksOnline
				&& !Programs.IsEnabled(ProgramName.QuickBooksOnline)) {
				//If users attempts to select QuickBooks Online but the program is not enabled, set accounting software back and alert them.
				comboDepositSoftware.SetSelected(PrefC.GetInt(PrefName.AccountingSoftware));
				MsgBox.Show(this,"QuickBooks Online must be enabled in Program Links before it can be selected as your Deposit Software.");
			}
		}

		private void FillManage(){
			checkRxSendNewToQueue.Checked=PrefC.GetBool(PrefName.RxSendNewToQueue);
			int claimZeroPayRollingDays=PrefC.GetInt(PrefName.ClaimPaymentNoShowZeroDate);
			if(claimZeroPayRollingDays>=0) {
				textClaimsReceivedDays.Text=(claimZeroPayRollingDays+1).ToString();//The minimum value is now 1 ("today"), to match other areas of OD.
			}
			for(int i=0;i<7;i++) {
				comboTimeCardOvertimeFirstDayOfWeek.Items.Add(Lan.g("enumDayOfWeek",Enum.GetNames(typeof(DayOfWeek))[i]));
			}
			comboTimeCardOvertimeFirstDayOfWeek.SelectedIndex=PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek);
			checkTimeCardADP.Checked=PrefC.GetBool(PrefName.TimeCardADPExportIncludesName);
			checkClaimsSendWindowValidateOnLoad.Checked=PrefC.GetBool(PrefName.ClaimsSendWindowValidatesOnLoad);
			checkScheduleProvEmpSelectAll.Checked=PrefC.GetBool(PrefName.ScheduleProvEmpSelectAll);
			checkClockEventAllowBreak.Checked=PrefC.GetBool(PrefName.ClockEventAllowBreak);
			checkEraAllowTotalPayment.Checked=PrefC.GetBool(PrefName.EraAllowTotalPayments);
			//Statements
			checkStatementShowReturnAddress.Checked=PrefC.GetBool(PrefName.StatementShowReturnAddress);
			checkStatementShowNotes.Checked=PrefC.GetBool(PrefName.StatementShowNotes);
			checkStatementShowAdjNotes.Checked=PrefC.GetBool(PrefName.StatementShowAdjNotes);
			checkStatementShowProcBreakdown.Checked=PrefC.GetBool(PrefName.StatementShowProcBreakdown);
			comboUseChartNum.Items.Add(Lan.g(this,"PatNum"));
			comboUseChartNum.Items.Add(Lan.g(this,"ChartNumber"));
			if(PrefC.GetBool(PrefName.StatementAccountsUseChartNumber)) {
				comboUseChartNum.SelectedIndex=1;
			}
			else {
				comboUseChartNum.SelectedIndex=0;
			}
			checkStatementsAlphabetically.Checked=PrefC.GetBool(PrefName.PrintStatementsAlphabetically);
			checkStatementsAlphabetically.Visible=PrefC.HasClinicsEnabled;
			if(PrefC.GetLong(PrefName.StatementsCalcDueDate)!=-1) {
				textStatementsCalcDueDate.Text=PrefC.GetLong(PrefName.StatementsCalcDueDate).ToString();
			}
			textPayPlansBillInAdvanceDays.Text=PrefC.GetLong(PrefName.PayPlansBillInAdvanceDays).ToString();
			textBillingElectBatchMax.Text=PrefC.GetInt(PrefName.BillingElectBatchMax).ToString();
			checkIntermingleDefault.Checked=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			checkBillingShowProgress.Checked=PrefC.GetBool(PrefName.BillingShowSendProgress);
			checkClaimPaymentBatchOnly.Checked=PrefC.GetBool(PrefName.ClaimPaymentBatchOnly);
			checkEraOneClaimPerPage.Checked=PrefC.GetBool(PrefName.EraPrintOneClaimPerPage);
			checkIncludeEraWOPercCoPay.Checked=PrefC.GetBool(PrefName.EraIncludeWOPercCoPay);
			checkShowAutoDeposit.Checked=PrefC.GetBool(PrefName.ShowAutoDeposit);
			FillComboEraAutomation();
			comboEraAutomation.SetSelectedEnum(PrefC.GetEnum<EraAutomationMode>(PrefName.EraAutomationBehavior));
			comboDepositSoftware.Items.AddEnums<AccountingSoftware>();
			comboDepositSoftware.SetSelectedEnum(PrefC.GetEnum<AccountingSoftware>(PrefName.AccountingSoftware));
		}

		///<summary>Fills ComboEraAutomation with EraAutomationMode enum values. Excludes EraAutomationMode.UseGlobal because that value only applies to Carrier.EraAutomationOverride.</summary>
		private void FillComboEraAutomation() {
			List<EraAutomationMode> listEraAutomationModes=typeof(EraAutomationMode).GetEnumValues()
				.AsEnumerable<EraAutomationMode>()
				.Where(x => x!=EraAutomationMode.UseGlobal)
				.ToList();
			comboEraAutomation.Items.AddListEnum(listEraAutomationModes);
		}

		///<summary>Returns false if validation fails.</summary>
		private bool SaveManage(){
			if(!textStatementsCalcDueDate.IsValid()
				| !textPayPlansBillInAdvanceDays.IsValid()
				| !textBillingElectBatchMax.IsValid())
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			if(!textClaimsReceivedDays.IsValid()) {
				MsgBox.Show(this,"Show claims received after days must be a positive integer or blank.");
				return false;
			}
			_changed|=Prefs.UpdateBool(PrefName.RxSendNewToQueue,checkRxSendNewToQueue.Checked);
			_changed|=Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,comboTimeCardOvertimeFirstDayOfWeek.SelectedIndex);
			_changed|=Prefs.UpdateBool(PrefName.TimeCardADPExportIncludesName,checkTimeCardADP.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClaimsSendWindowValidatesOnLoad,checkClaimsSendWindowValidateOnLoad.Checked);
			_changed|=Prefs.UpdateBool(PrefName.StatementShowReturnAddress,checkStatementShowReturnAddress.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ScheduleProvEmpSelectAll,checkScheduleProvEmpSelectAll.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClockEventAllowBreak,checkClockEventAllowBreak.Checked);
			_changed|=Prefs.UpdateBool(PrefName.EraAllowTotalPayments,checkEraAllowTotalPayment.Checked);
			_changed|=Prefs.UpdateBool(PrefName.StatementShowNotes,checkStatementShowNotes.Checked);
			_changed|=Prefs.UpdateBool(PrefName.StatementShowAdjNotes,checkStatementShowAdjNotes.Checked);
			_changed|=Prefs.UpdateBool(PrefName.StatementShowProcBreakdown,checkStatementShowProcBreakdown.Checked);
			_changed|=Prefs.UpdateBool(PrefName.StatementAccountsUseChartNumber,comboUseChartNum.SelectedIndex==1);
			_changed|=Prefs.UpdateLong(PrefName.PayPlansBillInAdvanceDays,PIn.Long(textPayPlansBillInAdvanceDays.Text));
			_changed|=Prefs.UpdateBool(PrefName.IntermingleFamilyDefault,checkIntermingleDefault.Checked);
			_changed|=Prefs.UpdateInt(PrefName.BillingElectBatchMax,PIn.Int(textBillingElectBatchMax.Text));
			_changed|=Prefs.UpdateBool(PrefName.BillingShowSendProgress,checkBillingShowProgress.Checked);
			_changed|=Prefs.UpdateInt(PrefName.ClaimPaymentNoShowZeroDate,(textClaimsReceivedDays.Text=="")?-1:(PIn.Int(textClaimsReceivedDays.Text)-1));
			_changed|=Prefs.UpdateBool(PrefName.ClaimPaymentBatchOnly,checkClaimPaymentBatchOnly.Checked);
			_changed|=Prefs.UpdateBool(PrefName.EraPrintOneClaimPerPage,checkEraOneClaimPerPage.Checked);
			_changed|=Prefs.UpdateBool(PrefName.EraIncludeWOPercCoPay,checkIncludeEraWOPercCoPay.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ShowAutoDeposit,checkShowAutoDeposit.Checked);
			_changed|=Prefs.UpdateBool(PrefName.PrintStatementsAlphabetically,checkStatementsAlphabetically.Checked);
			_changed|=Prefs.UpdateInt(PrefName.EraAutomationBehavior,(int)comboEraAutomation.GetSelected<EraAutomationMode>());
			_changed|=Prefs.UpdateInt(PrefName.AccountingSoftware,(int)comboDepositSoftware.GetSelected<AccountingSoftware>());
			if(textStatementsCalcDueDate.Text==""){
				if(Prefs.UpdateLong(PrefName.StatementsCalcDueDate,-1)){
					_changed=true;
				}
			}
			else{
				if(Prefs.UpdateLong(PrefName.StatementsCalcDueDate,PIn.Long(textStatementsCalcDueDate.Text))){
					_changed=true;
				}
			}
			return true;
		}
		#endregion Methods - Manage

		#region Methods - Other
		protected override string GetHelpOverride() {
			return tabControlMain.SelectedTab.Name;
		}

		private List<string> UpdateDefNumsList(PrefName pref, List<string> listAppend) {
			return PrefC.GetString(pref).Split(',')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Union(listAppend).ToList();
		}
		#endregion

		
	}
}






