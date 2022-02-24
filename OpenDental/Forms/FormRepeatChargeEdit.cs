using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormRepeatChargeEdit :FormODBase{
		///<summary></summary>
		public bool IsNew;
		private RepeatCharge _repeatCur;
		private RepeatCharge _repeatOld;
		private bool _isErx;

		///<summary>The eService that this procedure is associated to if it associated to one.</summary>
		private eServiceCode _eService;

		private bool _isForZipwhip {
			get {
				return ListTools.In(_eService,eServiceCode.IntegratedTexting,eServiceCode.ConfirmationRequest);
			}
		}

		///<summary></summary>
		public FormRepeatChargeEdit(RepeatCharge repeatCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_repeatCur=repeatCur;
			_repeatOld=repeatCur.Copy();
		}

		private void FormRepeatChargeEdit_Load(object sender,EventArgs e) {
			SetPatient();
			if(IsNew){
				using FormProcCodes FormP=new FormProcCodes();
				FormP.IsSelectionMode=true;
				FormP.ShowDialog();
				if(FormP.DialogResult!=DialogResult.OK){
					DialogResult=DialogResult.Cancel;
					return;
				}
				ProcedureCode procCode=ProcedureCodes.GetProcCode(FormP.SelectedCodeNum);
				if(procCode.TreatArea!=TreatmentArea.Mouth 
					&& procCode.TreatArea!=TreatmentArea.None){
					MsgBox.Show(this,"Procedure codes that require tooth numbers are not allowed.");
					DialogResult=DialogResult.Cancel;
					return;
				}
				_repeatCur.ProcCode=ProcedureCodes.GetStringProcCode(FormP.SelectedCodeNum);
				_repeatCur.IsEnabled=true;
				_repeatCur.CreatesClaim=false;
			}
			textCode.Text=_repeatCur.ProcCode;
			textDesc.Text=ProcedureCodes.GetProcCode(_repeatCur.ProcCode).Descript;
			textChargeAmt.Text=_repeatCur.ChargeAmt.ToString("F");
			if(_repeatCur.DateStart.Year>1880){
				textDateStart.Text=_repeatCur.DateStart.ToShortDateString();
			}
			if(_repeatCur.DateStop.Year>1880){
				textDateStop.Text=_repeatCur.DateStop.ToShortDateString();
			}
			textNote.Text=_repeatCur.Note;
			_isErx=false;
			if(PrefC.GetBool(PrefName.DistributorKey) && Regex.IsMatch(_repeatCur.ProcCode,"^Z[0-9]{3,}$")) {//Is eRx if HQ and a using an eRx Z code.
				_isErx=true;
				labelPatNum.Visible=true;
				textPatNum.Visible=true;
				butMoveTo.Visible=true;
				labelNpi.Visible=true;
				textNpi.Visible=true;
				labelProviderName.Visible=true;
				textProvName.Visible=true;
				labelErxAccountId.Visible=true;
				textErxAccountId.Visible=true;
				if(IsNew && _repeatCur.ProcCode=="Z100") {//DoseSpot Procedure Code
					List<string> listDoseSpotAccountIds=ClinicErxs.GetAccountIdsForPatNum(_repeatCur.PatNum)
					.Union(ProviderErxs.GetAccountIdsForPatNum(_repeatCur.PatNum))
					.Union(
						RepeatCharges.GetForErx()
						.FindAll(x => x.PatNum==_repeatCur.PatNum && x.ProcCode=="Z100")
						.Select(x => x.ErxAccountId)
						.ToList()
					)
					.Distinct()
					.ToList()
					.FindAll(x => DoseSpot.IsDoseSpotAccountId(x));
					if(listDoseSpotAccountIds.Count==0) {
						listDoseSpotAccountIds.Add(DoseSpot.GenerateAccountId(_repeatCur.PatNum));
					}
					if(listDoseSpotAccountIds.Count==1) {
						textErxAccountId.Text=listDoseSpotAccountIds[0];
					}
					else if(listDoseSpotAccountIds.Count>1) {
						using InputBox inputAccountIds=new InputBox(Lans.g(this,"Multiple Account IDs found.  Select one to assign to this repeat charge."),listDoseSpotAccountIds,0);
						inputAccountIds.ShowDialog();
						if(inputAccountIds.DialogResult==DialogResult.OK) {
							textErxAccountId.Text=listDoseSpotAccountIds[inputAccountIds.SelectedIndex];
						}
					}
				}
				else {//Existing eRx repeating charge.
					textNpi.Text=_repeatCur.Npi;
					textErxAccountId.Text=_repeatCur.ErxAccountId;
					textProvName.Text=_repeatCur.ProviderName;
					textNpi.ReadOnly=true;
					textErxAccountId.ReadOnly=true;
					textProvName.ReadOnly=true;
				}
			}
			checkCopyNoteToProc.Checked=_repeatCur.CopyNoteToProc;
			checkCreatesClaim.Checked=_repeatCur.CreatesClaim;
			checkIsEnabled.Checked=_repeatCur.IsEnabled;
			if(PrefC.GetBool(PrefName.DistributorKey)) {//OD HQ disable the IsEnabled and CreatesClaim checkboxes
				checkCreatesClaim.Enabled=false;
				checkIsEnabled.Enabled=false;
			}
			if(PrefC.IsODHQ && EServiceCodeLink.IsProcCodeAnEService(_repeatCur.ProcCode,out _eService)) {
				if(IsNew) {
					MsgBox.Show(this,"You cannot manually create any eService repeating charges.\r\n"
						+"Use the Signup Portal instead.\r\n\r\n"
						+"The Charge Amount can be manually edited after the Signup Portal has created the desired eService repeating charge.");
					DialogResult=DialogResult.Abort;
					return;
				}
				if(_isForZipwhip) {
					textZipwhipChargeAmount.Visible=true;
					labelZipwhipAmt.Visible=true;
					if(CompareDouble.IsGreaterThan(_repeatCur.ChargeAmtAlt,-1)) {
						textZipwhipChargeAmount.Text=_repeatCur.ChargeAmtAlt.ToString("F");
					}
				}
				//The only things that users should be able to do for eServices are:
				//1. Change the repeating charge amount(s).
				//2. Manipulate the Start Date.
				//3. Manipulate the Note.
				//4. Manipulate Billing Day because not all customers will have a non-eService repeating charge in order to manipulate.
				//This is because legacy users (versions prior to 17.1) need the ability to manually set their monthly charge amount, etc.
				SetFormReadOnly(this.PanelClient,butOK,butCancel
					,textChargeAmt,labelChargeAmount
					,textDateStart,labelDateStart
					,textNote,labelNote
					,textBillingDay,labelBillingCycleDay
					,textZipwhipChargeAmount,labelZipwhipAmt
					,checkUseUnearned,comboUnearnedTypes);
			}
			Patient pat=Patients.GetPat(_repeatCur.PatNum);//pat should never be null. If it is, this will fail.
			//If this is a new repeat charge and no other active repeat charges exist, set the billing cycle day to today
			if(IsNew && !RepeatCharges.ActiveRepeatChargeExists(_repeatCur.PatNum)) {
				textBillingDay.Text=DateTime.Today.Day.ToString();
			}
			else {
				textBillingDay.Text=pat.BillingCycleDay.ToString();
			}
			if(PrefC.GetBool(PrefName.BillingUseBillingCycleDay)) {
				labelBillingCycleDay.Visible=true;
				textBillingDay.Visible=true;
			}
			checkUseUnearned.Checked=_repeatCur.UsePrepay;
			List<long> listDefNumsUnearnedTypeCur=(_repeatCur.UnearnedTypes??"").Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries)
				.Select(x => PIn.Long(x,false)).ToList();
			List<Def> listUnearned=new List<Def>();
			listUnearned.AddRange(Defs.GetUnearnedDefs(isShort:true));
			comboUnearnedTypes.IncludeAll=true;
			comboUnearnedTypes.Items.AddDefs(listUnearned);
			for(int i=0;i<listDefNumsUnearnedTypeCur.Count;i++) {//There isn't an easy way to set selected multiple defs, so loop through each one that was selected on the RepeatCharge and set it selected if it matches an item from the list of all defs(listUnread)
				for(int j=0;j<comboUnearnedTypes.Items.Count;j++) {
					if(listUnearned[j].DefNum==listDefNumsUnearnedTypeCur[i]) {
						comboUnearnedTypes.SetSelected(j,true);
						continue;
					}
				}
			}
			if(string.IsNullOrEmpty(_repeatCur.UnearnedTypes)) {
				//An empty value indicates 'All'
				comboUnearnedTypes.IsAllSelected=true;
			}
		}

		///<summary>Recursively disables all controls for the control passed in by looping through any sub controls and disabling them.</summary>
		private void SetFormReadOnly(Control controlsInput,params Control[] controlsToIgnore) {
			foreach(Control ctrl in controlsInput.Controls) {
				foreach(Control ctrlSub in ctrl.Controls) {//Make sure all sub controls are read only.
					SetFormReadOnly(ctrlSub);
				}
				if(controlsToIgnore.Contains(ctrl)) {
					continue;
				}
				try {
					ctrl.Enabled=false;
				}
				catch(Exception e) {//Just in case.
					e.DoNothing();
				}
			}
		}

		private void SetPatient() {
			//Set the title bar to show the patient's name much like the main screen does.
			Text+=" - "+Patients.GetLim(_repeatCur.PatNum).GetNameLF();
			textPatNum.Text=_repeatCur.PatNum.ToString();
		}

		///<summary>Adds the procedure code of the repeating charge to a credit card on the patient's account if the user okays it.</summary>
		private void AddProcedureToCC() {
			List<CreditCard> activeCards=CreditCards.GetActiveCards(_repeatCur.PatNum);
			if(activeCards.Count==0) {
				return;
			}
			CreditCard cardToAddProc=null;
			if(activeCards.Count==1) { //Only one active card so ask the user to add the procedure to that one
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"There is one active credit card on this patient's account.\r\nDo you want to add this procedure to "+
					"that card?")) {
					cardToAddProc=activeCards[0];
				}
			}
			else if(activeCards.FindAll(x => x.Procedures!="").Count==1) { //Only one card has procedures attached so ask the user to add to that card
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"There is one active credit card on this patient's account with authorized procedures attached.\r\n"
					+"Do you want to add this procedure to that card?")) {
					cardToAddProc=activeCards.FirstOrDefault(x => x.Procedures!="");
				}
			}
			else { //At least two cards have procedures attached to them or there are multiple active cards and none have procedures attached
				MsgBox.Show(this,"If you would like to add this procedure to a credit card, go to Credit Card Manage to choose the card.");
			}
			if(cardToAddProc==null) {
				return;
			}
			//Check if the procedure is already attached to this card; CreditCard.Procedures is a comma delimited list.
			List<string> procsOnCard=cardToAddProc.Procedures.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries).ToList();
			if(procsOnCard.Exists(x => x==_repeatCur.ProcCode)) {
				return;
			}
			procsOnCard.Add(_repeatCur.ProcCode);
			cardToAddProc.Procedures=String.Join(",",procsOnCard);
			CreditCards.Update(cardToAddProc);
		}

		private void checkUseUnearned_CheckedChanged(object sender,EventArgs e) {
			comboUnearnedTypes.Enabled=checkUseUnearned.Checked;
		}

		private void butManual_Click(object sender,EventArgs e) {
			Prefs.RefreshCache();//Refresh the cache in case another machine has updated this pref
			if(PrefC.GetString(PrefName.RepeatingChargesBeginDateTime)!="") {
				MsgBox.Show(this,"Repeating charges already running on another workstation, you must wait for them to finish before continuing.");
				return;
			}
			if(_repeatCur.RepeatChargeNum==0) {
				MsgBox.Show(this,"Please click 'OK' to save the repeat charge before adding a manual charge.");
				return;
			}
			double procFee;
			try {
				procFee=Double.Parse(textChargeAmt.Text);
			}
			catch {
				MsgBox.Show(this,"Invalid charge amount.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.ProcComplCreate,DateTime.Today,ProcedureCodes.GetCodeNum(textCode.Text),procFee)) {
				return;
			}
			RepeatCharge chargeManual=_repeatCur.Copy();//Update the fields from the form in case the user made changes
			if(!UpdateRepeatCharge(chargeManual)) {
				return;
			}
			Procedures.SetDateFirstVisit(DateTime.Today,1,Patients.GetPat(_repeatCur.PatNum));
			Procedure proc=RepeatCharges.AddProcForRepeatCharge(chargeManual,DateTime.Today,DateTime.Today
				,orthoCaseProcLinkingData:new OrthoCaseProcLinkingData(_repeatCur.PatNum));
			RepeatCharges.AllocateUnearned(chargeManual,proc,DateTime.Today);
			Recalls.Synch(_repeatCur.PatNum);
			MsgBox.Show(this,"Procedure added.");
		}

		private void butCalculate_Click(object sender,EventArgs e) {
			if(CompareDouble.IsZero(PIn.Double(textNumOfCharges.Text))	|| CompareDouble.IsZero(PIn.Double(textTotalAmount.Text))) {
				textChargeAmt.Text=_repeatCur.ChargeAmt.ToString("F");
				return;
			}
			textChargeAmt.Text=(PIn.Double(textTotalAmount.Text)/PIn.Double(textNumOfCharges.Text)).ToString("F");
		}

		///<summary>This button is only visible internally and for other distributors.</summary>
		private void butMoveTo_Click(object sender,EventArgs e) {
			if(!Regex.IsMatch(textErxAccountId.Text,"^(DS;)?[0-9]+\\-[a-zA-Z0-9]{5}$")) {
				MsgBox.Show(this,"A valid ErxAccountId is required before moving this eRx repeating charge to another customer.  "
					+"The ErxAccountId is typically filled in automatically when running eRx billing.  You can manually enter by "
					+"logging into the eRx portal and clicking the Maintain Top-Level Account Kids link, "
					+"then locate the customer account in the list and copy the customer Account ID into the ErxAccountId of this repeating charge.");
				return;
			}
			using FormPatientSelect form=new FormPatientSelect();
			if(form.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_repeatCur.PatNum=form.SelectedPatNum;
			SetPatient();
			Patient pat=Patients.GetPat(_repeatCur.PatNum);
			textBillingDay.Text=pat.BillingCycleDay.ToString();
		}

		private void butDelete_Click(object sender, EventArgs e) {
			Patient oldPat=Patients.GetPat(_repeatCur.PatNum);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(_repeatCur,Permissions.RepeatChargeDelete,oldPat,isAutomated:false);
			RepeatCharges.Delete(_repeatCur);
			DialogResult=DialogResult.OK;
		}

		///<summary>Updates the repeatCharge with the values entered on the form.</summary>
		private bool UpdateRepeatCharge(RepeatCharge repeatCharge) {
			if(!textChargeAmt.IsValid()
				|| !textDateStart.IsValid()
				|| !textDateStop.IsValid()
				|| !textBillingDay.IsValid()
				|| (_isForZipwhip && !textZipwhipChargeAmount.IsValid())) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(PIn.Double(textChargeAmt.Text)<0 && checkCreatesClaim.Checked) {//user entered a value less than zero while checkCreatesClaim is checked
				MsgBox.Show(this,"Creates Claim cannot be checked while Charge Amout is less than zero.");
				return false;
			}
			if(textDateStart.Text=="") {
				MsgBox.Show(this,"Start date cannot be left blank.");
				return false;
			}
			if(PIn.Date(textDateStart.Text)!=_repeatCur.DateStart) {//if the user changed the date
				if(PIn.Date(textDateStart.Text)<DateTime.Today.AddDays(-3)) {//and if the date the user entered is more than three days in the past
					MsgBox.Show(this,"Start date cannot be more than three days in the past.  You should enter previous charges manually in the account.");
					return false;
				}
			}
			if(textDateStop.Text.Trim()!="" && PIn.Date(textDateStart.Text)>PIn.Date(textDateStop.Text)) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The start date is after the stop date.  Continue?")) {
					return false;
				}
			}
			if(_isErx && !Regex.IsMatch(textNpi.Text,"^[0-9]{10}$")) {
				MsgBox.Show(this,"Invalid NPI.  Must be 10 digits.");
				return false;
			}
			string accountId=textErxAccountId.Text;
			if(textErxAccountId.Text.Length>2 && textErxAccountId.Text.Substring(0,3).ToLower()=="ds;") {//support for DoseSpot account Ids
				accountId=textErxAccountId.Text.Substring(3);
			}
			if(_isErx && textErxAccountId.Text!="" && !Regex.IsMatch(accountId,"^[0-9]+\\-[a-zA-Z0-9]{5}$")) {
				MsgBox.Show(this,"Invalid ErxAccountId.");
				return false;
			}
			if(_isForZipwhip && CompareDouble.IsGreaterThan(_repeatCur.ChargeAmtAlt,-1) && textZipwhipChargeAmount.Text.Trim()=="") {
				MsgBox.Show(this,"Zipwhip Amount must not blank when it was previously set.");
				return false;
			}
			repeatCharge.ProcCode=textCode.Text;
			repeatCharge.ChargeAmt=PIn.Double(textChargeAmt.Text);
			repeatCharge.DateStart=PIn.Date(textDateStart.Text);
			repeatCharge.DateStop=PIn.Date(textDateStop.Text);
			repeatCharge.Npi=textNpi.Text;
			repeatCharge.ErxAccountId=textErxAccountId.Text;
			repeatCharge.Note=textNote.Text;
			repeatCharge.ProviderName=textProvName.Text;
			repeatCharge.CopyNoteToProc=checkCopyNoteToProc.Checked;
			repeatCharge.IsEnabled=checkIsEnabled.Checked;
			repeatCharge.CreatesClaim=checkCreatesClaim.Checked;
			repeatCharge.UsePrepay=checkUseUnearned.Checked;
			repeatCharge.UnearnedTypes="";//If 'All' is selected. An empty database column indicates all unearned types are to be used.
			if(!comboUnearnedTypes.IsAllSelected) {
				repeatCharge.UnearnedTypes=string.Join(",",comboUnearnedTypes.GetListSelected<Def>().Select(x => x.DefNum));
			}
			if(_isForZipwhip && textZipwhipChargeAmount.Text.Trim()!="") {
				repeatCharge.ChargeAmtAlt=PIn.Double(textZipwhipChargeAmount.Text);
			}
			return true;
		}

		private void butOK_Click(object sender, EventArgs e){
			if(!UpdateRepeatCharge(_repeatCur)) {
				return;
			}
			Patient patOldChange=Patients.GetPat(_repeatCur.PatNum);
			Patient patNewChange=patOldChange.Copy();
			if(patNewChange.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Patient has been deleted by another user.");
				return;
			}
			if(PrefC.GetBool(PrefName.BillingUseBillingCycleDay) && textBillingDay.Text!="") {
				patNewChange.BillingCycleDay=PIn.Int(textBillingDay.Text);
				Patients.Update(patNewChange,patOldChange);
			}
			if(IsNew) {
				if(!RepeatCharges.ActiveRepeatChargeExists(_repeatCur.PatNum) 
					&& (textBillingDay.Text=="" || textBillingDay.Text=="0"))
				{
					patNewChange.BillingCycleDay=PIn.Date(textDateStart.Text).Day;
					Patients.Update(patNewChange,patOldChange);
				}
				_repeatCur.RepeatChargeNum=RepeatCharges.Insert(_repeatCur);
				RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(_repeatCur,Permissions.RepeatChargeCreate,patOldChange,isAutomated:false);
				if(PrefC.IsODHQ) {
					AddProcedureToCC();
				}
			}
			else{ //not a new repeat charge
				RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(_repeatOld,Permissions.RepeatChargeUpdate,patOldChange,newCharge:_repeatCur,isAutomated:false,newPat:patNewChange);
				RepeatCharges.Update(_repeatCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}



