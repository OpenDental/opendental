using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCreditCardEdit:FormODBase {
		private Patient _patient;
		private List<PayPlan> _listPayPlans;
		private CreditCard _creditCard;
		public CreditCard CreditCardCur;
		///<summary>True if EdgeExpress is enabled.  Recurring charge section will show if using EdgeExpress.</summary>
		private bool _isEdgeExpressEnabled;
		///<summary>True if X-Charge is enabled.  Recurring charge section will show if using X-Charge.</summary>
		private bool _isXChargeEnabled;
		///<summary>True if PayConnect is enabled.  Recurring charge section will show if using PayConnect.</summary>
		private bool _isPayConnectEnabled;
		///<summary>True if PaySimple is enabled.  Recurring charge section will show if using PaySimple.</summary>
		private bool _isPaySimpleEnabled;
		/// <summary></summary>
		private bool _isDuplicate;
		public FormCreditCardEdit(Patient patient, bool isDuplicate=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
			_isEdgeExpressEnabled=Programs.IsEnabled(ProgramName.EdgeExpress);
			_isXChargeEnabled=Programs.IsEnabled(ProgramName.Xcharge);
			_isPayConnectEnabled=Programs.IsEnabled(ProgramName.PayConnect);
			_isPaySimpleEnabled=Programs.IsEnabled(ProgramName.PaySimple);
			_isDuplicate=isDuplicate;
		}

		private void FormCreditCardEdit_Load(object sender,EventArgs e) {
			_creditCard=CreditCardCur.Copy();
			if(PrefC.HasClinicsEnabled) {
				string abbrCardClinic=Lan.g(this,"None");
				if(CreditCardCur.ClinicNum > 0) {
					abbrCardClinic=Clinics.GetAbbr(CreditCardCur.ClinicNum);
				}
				textClinic.Text=abbrCardClinic;
				labelClinic.Visible=true;
				textClinic.Visible=true;
			}
			FillFrequencyCombos();
			FillData();
			FillPayTypeCombo();
			checkExcludeProcSync.Checked=CreditCardCur.ExcludeProcSync;
			if((_isEdgeExpressEnabled || _isXChargeEnabled || _isPayConnectEnabled || _isPaySimpleEnabled) 
				&& (!CreditCardCur.IsXWeb() && !CreditCardCur.IsPayConnectPortal())) 
			{//Get recurring payment plan information if using X-Charge or PayConnect and the card is not from XWeb or PayConnectPortal.
				_listPayPlans=PayPlans.GetValidPlansNoIns(_patient.PatNum);
				List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlans(_listPayPlans.Select(x => x.PayPlanNum).ToList());
				comboPaymentPlans.Items.Add("None");
				comboPaymentPlans.SelectedIndex=0;
				for(int i=0;i<_listPayPlans.Count;i++) {
					comboPaymentPlans.Items.Add(PayPlans.GetTotalCost(_listPayPlans[i].PayPlanNum,listPayPlanCharges).ToString("F")
					+"  "+Patients.GetPat(_listPayPlans[i].PatNum).GetNameFL());
					if(_listPayPlans[i].PayPlanNum==CreditCardCur.PayPlanNum) {
						comboPaymentPlans.SelectedIndex=i+1;
					}
				}
				if(PrefC.IsODHQ) {
					groupProcedures.Visible=true;
					FillProcs();
				}
				else {
					this.ClientSize=new System.Drawing.Size(this.ClientSize.Width,this.ClientSize.Height-115);
				}
				UpdateFrequencyText();
				EnableFrequencyControls();
			}
			else {//This will hide the recurring section and change the window size.
				groupRecurringCharges.Visible=false;
				groupChargeFrequency.Visible=false;
				this.ClientSize=new System.Drawing.Size(this.ClientSize.Width,this.ClientSize.Height-486);
			}
			if(_isPaySimpleEnabled && !CreditCardCur.IsNew) {
				labelAcctType.Visible=true;
				textAccountType.Visible=true;
				textExpDate.ReadOnly=true;
				textZip.ReadOnly=true;
			}
			if(_isPayConnectEnabled && !CreditCardCur.IsNew) {
				textExpDate.ReadOnly=true;
			}
			checkChrgWithNoBal.Checked=CreditCardCur.CanChargeWhenNoBal;
			//Only visible if preference is on.
			checkChrgWithNoBal.Visible=PrefC.GetBool(PrefName.RecurringChargesAllowedWhenNoPatBal);
			checkIsRecurringActive.Checked=CreditCardCur.IsRecurringActive;
			Plugins.HookAddCode(this,"FormCreditCardEdit.Load_end",_patient);
		}

		private void FillFrequencyCombos() {
			comboFrequency.Items.AddEnums<DayOfWeekFrequency>();
			comboDays.Items.AddEnums<DayOfWeek>();
			//Set Defaults
			comboFrequency.SetSelectedEnum(DayOfWeekFrequency.Every);
			comboDays.SelectedIndex=(int)DayOfWeek.Monday;
		}

		private void FillData() {
			if(CreditCardCur.IsNew) { 
				return; 
			}
			string strCardNum=CreditCardCur.CCNumberMasked;
			if(Regex.IsMatch(strCardNum,"^\\d{12}(\\d{0,7})")) {	//Credit cards can have a minimum of 12 digits, maximum of 19
				int idxLast4Digits=(strCardNum.Length-4);
				strCardNum=(new string('X',12))+strCardNum.Substring(idxLast4Digits);//replace the first 12 with 12 X's
			}
			textCardNumber.Text=strCardNum;
			textAddress.Text=CreditCardCur.Address;
			if(CreditCardCur.CCExpiration.Year>1800) {
				textExpDate.Text=CreditCardCur.CCExpiration.ToString("MMyy");
			}
			textZip.Text=CreditCardCur.Zip;
			if(_isEdgeExpressEnabled || _isXChargeEnabled || _isPayConnectEnabled || _isPaySimpleEnabled) {//Only fill information if using X-Charge, PayConnect, or PaySimple.
				if(CreditCardCur.ChargeAmt>0) {
					textChargeAmt.Text=CreditCardCur.ChargeAmt.ToString("F");
				}
				if(CreditCardCur.DateStop.Year>1880) {
					textDateStop.Text=CreditCardCur.DateStop.ToShortDateString();
				}
				textNote.Text=CreditCardCur.Note;
				if(CreditCardCur.ChargeFrequency!="") {//No charge frequency set
					if(CreditCards.GetFrequencyType(CreditCardCur.ChargeFrequency)==ChargeFrequencyType.FixedDayOfMonth) {
						//No need to change check as it is FixedDayOfMonth default
						textDayOfMonth.Text=CreditCards.GetDaysOfMonthForChargeFrequency(CreditCardCur.ChargeFrequency);
					}
					else {//FixedDayOfWeek
						radioWeekDay.Checked=true;
						comboFrequency.SelectedIndex=(int)CreditCards.GetDayOfWeekFrequency(CreditCardCur.ChargeFrequency);
						comboDays.SelectedIndex=(int)CreditCards.GetDayOfWeek(CreditCardCur.ChargeFrequency);
					}
				}
				if(CreditCardCur.DateStart.Year>1880) {
					textDateStart.Text=CreditCardCur.DateStart.ToShortDateString();
					UpdateTextNextChargeDate();
				}
			}
			if(_isPaySimpleEnabled) {
				textAccountType.Text=CreditCardCur.IsPaySimpleACH() ? Lans.g(this,"ACH") : Lans.g(this,"Credit Card");
			}
			textPreviousStartDate.Text=_creditCard.DateStart.ToShortDateString();
		}

		private void FillProcs() {
			listProcs.Items.Clear();
			if(String.IsNullOrEmpty(CreditCardCur.Procedures)) {
				return;
			}
			string[] stringArrayProcCodes=CreditCardCur.Procedures.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<stringArrayProcCodes.Length;i++) {
				listProcs.Items.Add(stringArrayProcCodes[i]+"- "+ProcedureCodes.GetLaymanTerm(ProcedureCodes.GetProcCode(stringArrayProcCodes[i]).CodeNum));
			}
		}

		private void FillPayTypeCombo() {
			comboPaymentType.Items.AddDefNone(Lan.g(this,"Use Default"));
			comboPaymentType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaymentTypes,true));
			comboPaymentType.SetSelectedDefNum(CreditCardCur.PaymentType);
		}

		private bool VerifyData() {
			if(textCardNumber.Text.Trim().Length<5) {
				MsgBox.Show(this,"Invalid Card Number.");
				return false;
			}
			try {
				if(Regex.IsMatch(textExpDate.Text,@"^\d\d[/\- ]\d\d$")) {//08/07 or 08-07 or 08 07
					CreditCardCur.CCExpiration=new DateTime(Convert.ToInt32("20"+textExpDate.Text.Substring(3,2)),Convert.ToInt32(textExpDate.Text.Substring(0,2)),1);
				}
				else if(Regex.IsMatch(textExpDate.Text,@"^\d{4}$")) {//0807
					CreditCardCur.CCExpiration=new DateTime(Convert.ToInt32("20"+textExpDate.Text.Substring(2,2)),Convert.ToInt32(textExpDate.Text.Substring(0,2)),1);
				}
				else if(!CreditCardCur.IsPaySimpleACH()) {
					MsgBox.Show(this,"Expiration format invalid.");
					return false;
				}
			}
			catch {
				MsgBox.Show(this,"Expiration format invalid.");
				return false;
			}
			if(textDateStop.Text.Trim()!="" && PIn.Date(textDateStart.Text)>PIn.Date(textDateStop.Text)) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The recurring charge start date is after the stop date.  Continue?")) {
					return false;
				}
			}
			if(_isEdgeExpressEnabled || _isXChargeEnabled || _isPayConnectEnabled || _isPaySimpleEnabled) {//Only validate recurring setup if using X-Charge, PayConnect, or PaySimple.
				if(textDateStart.Text.IsNullOrEmpty() && !textChargeAmt.Text.IsNullOrEmpty()){
					MsgBox.Show(this,"Please enter a Date Start first.");
					return false;
				}
				if(!ValidateDateStartAndStop() || !textChargeAmt.IsValid()){
					MsgBox.Show(this,"Please fix data entry errors first.");
					return false;
				}
				DateTime dateTimeStop;
				bool isDateStopValid=DateTime.TryParse(textDateStop.Text, out dateTimeStop);
				if(!isDateStopValid || dateTimeStop==null || dateTimeStop==new DateTime()) {
					textDateStop.Text="";
				}
				if((textChargeAmt.Text=="" && comboPaymentPlans.SelectedIndex>0)
					|| (textChargeAmt.Text=="" && textDateStart.Text.Trim()!="")
					|| (!textChargeAmt.Text.IsNullOrEmpty() && PIn.Double(textChargeAmt.Text)<=0))
				{
					MsgBox.Show(this,"You need a positive charge amount for recurring charges.");
					return false;
				}
				if(textChargeAmt.Text!="" && textDateStart.Text.Trim()=="") {
					MsgBox.Show(this,"You need a start date for recurring charges.");
					return false;
				}
				if(radioDayOfMonth.Checked && textDayOfMonth.Text=="" && textDateStart.Text!="") {
					MsgBox.Show(this,"You must select at least one date for recurring charges when Fixed day(s) of month is checked.");
					return false;
				}
			}
			return true;
		}

		private void textDayOfMonth_TextChanged(object sender,EventArgs e) {
			textDateStart.Text="";
			UpdateFrequencyText();
			UpdateTextNextChargeDate();
		}

		private void comboFrequency_SelectionChangeCommitted(object sender,EventArgs e) {
			textDateStart.Text="";
			UpdateFrequencyText();
			UpdateTextNextChargeDate();
		}

		private void comboDays_SelectionChangeCommitted(object sender,EventArgs e) {
			textDateStart.Text="";
			UpdateFrequencyText();
			UpdateTextNextChargeDate();
		}

		private void radioDayOfMonth_CheckedChanged(object sender,EventArgs e) {
			textDateStart.Text="";
			UpdateFrequencyText();
			EnableFrequencyControls();
			UpdateTextNextChargeDate();
		}

		//Calculates the next date the patient will be charged, and displays it in the text box
		private void UpdateTextNextChargeDate() {
			if(!ValidateDateStartAndStop()) {
				textNextChargeDate.Text="";
				return;
			}
			string frequency=GetFormattedChargeFrequency();
			DateTime.TryParse(textDateStart.Text, out DateTime dateStart);
			DateTime.TryParse(textDateStop.Text, out DateTime dateStop);
			DateTime nextChargeDate=RecurringCharges.CalculateNextChargeDate(frequency, dateStart, dateStop);
			//If date start is less than today, blank next charge date, if it is not new or equal to the prior start date.
			if(dateStart<DateTime.Today && _creditCard.DateStart!=DateTime.MinValue && (_creditCard.DateStart!=dateStart||_creditCard.ChargeFrequency.IsNullOrEmpty())){
				nextChargeDate=DateTime.MinValue;
			}
			textNextChargeDate.Value=nextChargeDate;
		}

		///<summary>This is an additional validation method for the case where we are editing a credit card with a recurring charge.</summary>
		public bool IsUpdatedFrequencyValid() {
			bool isValid=true;
			if(textDateStart.Text==_creditCard.DateStart.ToShortDateString() 
				&& GetFormattedChargeFrequency()==_creditCard.ChargeFrequency) 
			{
				return isValid;
			}
			if(textDateStart.Text.IsNullOrEmpty() && textDateStop.Text.IsNullOrEmpty() && textChargeAmt.Text.IsNullOrEmpty()) {
				return isValid;
			}
			//if(!ValidateDateStartAndStop()) {
			//	MessageBox.Show(Lans.g(this, "Start Date or Stop Date is invalid. Start Date is required, Stop Date must be valid or blank."));
			//	isValid=false;
			//	return isValid;
			//}
			string message="";
			//If dateStart is not empty, and is not valid return.
			if(!textDateStart.Text.IsNullOrEmpty() && !textDateStart.IsValid()) {
				message+=Lans.g(this, "Start Date is Invalid.")+'\n';
				isValid=false;
			}			
			//If dateStop is not empty, and is not valid return
			if(!textDateStop.Text.IsNullOrEmpty() && !textDateStop.IsValid()) {
				message+=Lans.g(this, "Stop Date is Invalid.")+'\n';
				isValid=false;
			}
			if(radioDayOfMonth.Checked && textDayOfMonth.Text.IsNullOrEmpty()) {
				message+=Lans.g(this, "Frequency Required.")+'\n';
				isValid=false;
			}
			if(radioWeekDay.Checked && (comboDays.SelectedIndex<0 || comboFrequency.SelectedIndex<0)) {
				message+=Lans.g(this, "Must choose a day.")+'\n';
				isValid=false;
			}
			//Case where charge hasnt been set up previously.
			if(_creditCard.DateStart.Year<1880 || _creditCard.ChargeFrequency=="") {
				if(!isValid) {
					MessageBox.Show(message);
				}
				return isValid;
			}
			//Date Start can be earlier than today, if this is a new charge. Must be valid so far or this could throw an exception.
			if(isValid && !textDateStart.Text.IsNullOrEmpty() && DateTime.Parse(textDateStart.Text)<DateTime.Today) {
				message+=Lans.g(this, "Start date is invalid. Must be today or later.");
				isValid=false;
			}
			if(!isValid) {
				MessageBox.Show(message);
			}
			return isValid;
		}

		private void EnableFrequencyControls() {
			comboDays.Enabled=radioWeekDay.Checked;
			comboFrequency.Enabled=radioWeekDay.Checked;
			textDayOfMonth.Enabled=radioDayOfMonth.Checked;
			butAddDay.Enabled=radioDayOfMonth.Checked;
			butClearDayOfMonth.Enabled=radioDayOfMonth.Checked;
		}

		private void UpdateFrequencyText() {
			if(radioDayOfMonth.Checked) {
				string daysOfMonth=textDayOfMonth.Text;
				if(daysOfMonth=="") {
					labelFrequencyInWords.Text="";
					return;
				}
				List<string> listDays=daysOfMonth.Split(',').ToList();
				string daysFormatted="";
				for(int i=0;i<listDays.Count;i++) {
					if(listDays.Count > 2 && i > 0) {
						daysFormatted+=",";
					}
					if(listDays.Count >= 2 && i==listDays.Count-1) {
						daysFormatted+=" "+Lan.g(this,"and");
					}
					daysFormatted+=" "+Lan.g("OrdinalIndicators",listDays[i].Trim()+MiscUtils.GetOrdinalIndicator(listDays[i].Trim()));
				}
				labelFrequencyInWords.Text=daysFormatted+" "+Lan.g(this,"day of the month");
			}
			else {//radioWeekDay
				string frequency=comboFrequency.Items.GetTextShowingAt(comboFrequency.SelectedIndex);
				string dayOfWeek=comboDays.Items.GetTextShowingAt(comboDays.SelectedIndex);
				labelFrequencyInWords.Text=frequency+" "+dayOfWeek+" "+Lan.g(this,"of the month");
			}
		}

		//Returns true if start date is a valid date, and stop date is blank or a valid date.
		private bool ValidateDateStartAndStop() {
			DateTime dateTimeStart;
			DateTime.TryParse(textDateStart.Text, out dateTimeStart);
			if((textDateStart.Text.IsNullOrEmpty()	|| textDateStart.Text!=_creditCard.DateStart.ToShortDateString()) &&_creditCard.DateStart.Year>=1880) {
				textPreviousStartDate.Visible=true;
				labelPreviousStartDate.Visible=true;
			}
			else {
				textPreviousStartDate.Visible=false;
				labelPreviousStartDate.Visible=false;
			}
			if(textDateStart.Text.IsNullOrEmpty() && textDateStop.Text.IsNullOrEmpty() && textChargeAmt.Text.IsNullOrEmpty()) {
				return true;
			}
			//Date Start is required
			if(dateTimeStart==null || dateTimeStart.Year<1880) {
				return false;
			}
			//Date Stop may be blank or default, or be a valid date.
			if(textDateStop.Text.IsNullOrEmpty()||textDateStop.Text==DateTime.MinValue.ToShortDateString()) {
				return true;
			}
			DateTime dateTimeStop;
			DateTime.TryParse(textDateStop.Text, out dateTimeStop);
			if((dateTimeStop==null || dateTimeStop.Year<1880 )&& !textDateStop.Text.IsNullOrEmpty()) {
				return false;
			}
			return true;
		}

		private void butAddDay_Click(object sender,EventArgs e) {
			List<string> listDaysOfMonth=new List<string>();
			for(int i=1;i<=31;i++) {
				listDaysOfMonth.Add(i.ToString());
			}
			using InputBox inputBox=new InputBox(new List<InputBoxParam> { new InputBoxParam {
				InputBoxType_=InputBoxType.ComboSelect,
				LabelText=Lans.g(this,"Day of month"),
				ListSelections=listDaysOfMonth,
				SizeParam=new Size(75,21),
				HorizontalAlign=HorizontalAlignment.Center,
			}});
			inputBox.Text=Lans.g(this,"Select Day");
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK) {
				return;
			}
			int selectedDay=inputBox.SelectedIndex+1;
			List<int> listCurrentDays=textDayOfMonth.Text.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries)
				.Select(x => PIn.Int(x.Trim())).ToList();
			if(listCurrentDays.Contains(selectedDay)) {
				MsgBox.Show(this,"The selected date has already been added.");
				return;
			}
			listCurrentDays.Add(selectedDay);
			listCurrentDays.Sort();
			textDayOfMonth.Text=string.Join(", ",listCurrentDays);
		}

		private void butClearDayOfMonth_Click(object sender,EventArgs e) {
			textDayOfMonth.Text="";
		}

		private string GetFormattedChargeFrequency() {
			if(textDateStart.Text=="" && CreditCardCur.ChargeFrequency=="") {
				return "";
			}
			if(radioDayOfMonth.Checked) {
				return ((int)ChargeFrequencyType.FixedDayOfMonth).ToString()
					+"|"+textDayOfMonth.Text;
			}
			else {//radioWeekDay
				return ((int)ChargeFrequencyType.FixedWeekDay).ToString()
					+"|"+comboFrequency.SelectedIndex+"|"+comboDays.SelectedIndex;//comboFrequency see enum DayOfWeekFrequency
			}
		}

		private void butClear_Click(object sender,EventArgs e) {
			//Only clear text boxes for recurring charges group.
			textChargeAmt.Text="";
			textDateStart.Text="";
			textDateStop.Text="";
			textNote.Text="";
		}

		private void butToday_Click(object sender,EventArgs e) {
			if(textDayOfMonth.Text=="" && radioDayOfMonth.Checked) {
				textDayOfMonth.Text=PrefC.IsODHQ ? _patient.BillingCycleDay.ToString() : DateTime.Today.Day.ToString();
			}
			textDateStart.Text=DateTime.Today.ToShortDateString();
			UpdateTextNextChargeDate();
		}

		private void textDateStart_Leave(object sender,EventArgs e) {
			if(radioWeekDay.Checked) {
				UpdateTextNextChargeDate();
				return;
			}
			string textDateStartCopy=textDateStart.Text;//Copy textDateStart.Text to fill textDateStart field after textDayOfMonth_TextChanged() clears it
			if(PrefC.IsODHQ) {
				textDayOfMonth.Text=_patient.BillingCycleDay.ToString();
			}
			else {
				DateTime dateStart=PIn.Date(textDateStart.Text);
				if(dateStart.Year < 1880 || textDayOfMonth.Text!="") {//if invalid date or if they already have something in the day of the month text
					UpdateTextNextChargeDate();
					return;
				}
				textDayOfMonth.Text=dateStart.Date.Day.ToString();
				UpdateTextNextChargeDate();
			}
			textDateStart.Text=textDateStartCopy;
		}

		private void textDateStop_Leave(object sender,EventArgs e) { 
			UpdateTextNextChargeDate();
		}

		private void butAddProc_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			string procCode=ProcedureCodes.GetStringProcCode(formProcCodes.CodeNumSelected);
			List<string> listProcsOnCards=CreditCardCur.Procedures.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries).ToList();
			//If the procedure is already attached to this card, return without adding the procedure again
			if(listProcsOnCards.Exists(x => x==procCode)) {
				return;
			}
			//Warn if attached to a different active card for this patient
			if(CreditCards.ProcLinkedToCard(CreditCardCur.PatNum,procCode,CreditCardCur.CreditCardNum)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This procedure is already linked with another credit card on this patient's "
					+"account. Adding the procedure to this card will result in the patient being charged twice for this procedure. Add this procedure?")) {
					return;
				}
			}
			if(CreditCardCur.Procedures!="") {
				CreditCardCur.Procedures+=",";
			}
			CreditCardCur.Procedures+=procCode;
			FillProcs();
		}

		private void butRemoveProc_Click(object sender,EventArgs e) {
			if(listProcs.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a procedure first.");
				return;
			}
			List<string> listStrings=new List<string>(CreditCardCur.Procedures.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries));
			listStrings.RemoveAt(listProcs.SelectedIndex);
			CreditCardCur.Procedures=string.Join(",",listStrings);
			FillProcs();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(CreditCardCur.IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to delete this credit card?")) {
				return;
			}
			bool hasDuplicatePaySimple=CreditCards.HasDuplicatePaySimpleToken(CreditCardCur);
			bool hasDuplicateXCharge=CreditCards.HasDuplicateXChargeToken(CreditCardCur);
			//Currently we don't send a delete request to Payconnect, but here if we need to do so in the future so this pattern isn't missed.
			bool hasDuplicatePayConnect=CreditCards.HasDuplicatePayConnectToken(CreditCardCur);
			#region X-Charge
			//Delete the archived X-Charge token 
			if(!hasDuplicateXCharge && ((_isEdgeExpressEnabled || _isXChargeEnabled) && CreditCardCur.XChargeToken!="")) {
				if(CreditCardCur.IsXWeb()) {
					OpenDentBusiness.WebTypes.Shared.XWeb.XWebs.DeleteCreditCard(_patient.PatNum,CreditCardCur.CreditCardNum);//Also deletes cc from db
				}
				else if(CreditCardCur.CCSource==CreditCardSource.EdgeExpressRCM || CreditCardCur.CCSource==CreditCardSource.EdgeExpressCNP || CreditCardCur.CCSource==CreditCardSource.EdgeExpressPaymentPortal) {
					Cursor=Cursors.WaitCursor;
					try {
						EdgeExpress.RcmResponse rcmResponse=EdgeExpress.RCM.DeleteAlias(_patient,Clinics.ClinicNum,CreditCardCur.XChargeToken,isWebPayment:false);
						Cursor=Cursors.Default;
						if(!rcmResponse.IsSuccess) {
							throw new ODException(Lans.g(this,"Error from EdgeExpress:")+" "+rcmResponse.RESULTMSG);
						}
					}
					catch(Exception ex) {
						try {
							XWebResponse xWebResponse=EdgeExpress.CNP.DeleteAlias(_patient.PatNum,CreditCardCur.XChargeToken);
							if(PIn.Enum<XWebResponseCodes>(xWebResponse.ResponseCode)==XWebResponseCodes.Approval) {
								ex.DoNothing();
							}
							else {
								throw new ODException(ex.Message,ex);
							}
						}
						catch(Exception exe) {
							Cursor=Cursors.Default;
							FriendlyException.Show(Lans.g(this,"There was a problem deleting the credit card.  Please try again."),exe);
							return;
						}
					}
				}
				else {
					DeleteXChargeAlias();
				}				
			}
			#endregion
			if(!hasDuplicatePaySimple && !DeletePaySimpleToken()) {
				return;
			}
			CreditCards.Delete(CreditCardCur.CreditCardNum);
			List<CreditCard> listCreditCards=CreditCards.RefreshAll(_patient.PatNum);
			for(int i=0;i<listCreditCards.Count;i++) {
				listCreditCards[i].ItemOrder=listCreditCards.Count-(i+1);
				CreditCards.Update(listCreditCards[i]);//Resets ItemOrder.
			}
			DialogResult=DialogResult.OK;
		}
		
		private void DeleteXChargeAlias() {
			Program program=Programs.GetCur(ProgramName.Xcharge);
			string path=Programs.GetProgramPath(program);
			if(program==null) {
				MsgBox.Show(this,"X-Charge entry is missing from the database.");//should never happen
				return;
			}
			if(!program.Enabled) {
				if(Security.IsAuthorized(EnumPermType.Setup)) {
					using FormXchargeSetup formXchargeSetup=new FormXchargeSetup();
					formXchargeSetup.ShowDialog();
				}
				return;
			}
			if(!File.Exists(path)) {
				MsgBox.Show(this,"Path is not valid.");
				if(Security.IsAuthorized(EnumPermType.Setup)) {
					using FormXchargeSetup formXchargeSetup=new FormXchargeSetup();
					formXchargeSetup.ShowDialog();
				}
				return;
			}
			ProcessStartInfo processStartInfo=new ProcessStartInfo(path);
			string resultFile=PrefC.GetRandomTempFile("txt");
			try {
				File.Delete(resultFile);//delete the old result file.
			}
			catch {
				MsgBox.Show(this,"Could not delete XResult.txt file.  It may be in use by another program, flagged as read-only, or you might not have "
					+"sufficient permissions.");
				return;
			}
			string xUsername=ProgramProperties.GetPropVal(program.ProgramNum,"Username",Clinics.ClinicNum);
			string xPassword=CodeBase.MiscUtils.Decrypt(ProgramProperties.GetPropVal(program.ProgramNum,"Password",Clinics.ClinicNum));
			processStartInfo.Arguments+="/TRANSACTIONTYPE:ARCHIVEVAULTDELETE ";
			processStartInfo.Arguments+="/XCACCOUNTID:"+CreditCardCur.XChargeToken+" ";
			processStartInfo.Arguments+="/RESULTFILE:\""+resultFile+"\" ";
			processStartInfo.Arguments+="/USERID:"+xUsername+" ";
			processStartInfo.Arguments+="/PASSWORD:"+xPassword+" ";
			processStartInfo.Arguments+="/AUTOPROCESS ";
			processStartInfo.Arguments+="/AUTOCLOSE ";
			Cursor=Cursors.WaitCursor;
			Process process=new Process();
			process.StartInfo=processStartInfo;
			process.EnableRaisingEvents=true;
			process.Start();
			process.WaitForExit();
			Thread.Sleep(200);//Wait 2/10 second to give time for file to be created.
			Cursor=Cursors.Default;
			string line="";
			using TextReader textReader = new StreamReader(resultFile);
			try {
				line=textReader.ReadLine();
			}
			catch {
				MsgBox.Show(this,"Could not read XResult.txt file.  It may be in use by another program, flagged as read-only, or you might not have "
					+"sufficient permissions.");
				return;
			}
			while(line!=null) {
				if(line=="RESULT=SUCCESS") {
					break;
				}
				if(line.StartsWith("DESCRIPTION") && !line.Contains("Alias does not exist")) {//If token doesn't exist in X-Charge, still delete from OD
					MsgBox.Show(this,"There was a problem deleting this card within X-Charge.  Please try again.");
					return;//Don't delete the card from OD
				}
				line=textReader.ReadLine(); //We've already read the file once, so subsequent ReadLines shouldn't cause issues.
			}
		}

		///<summary>Deletes the PaySimple token if there is one. Returns false if deleting the token failed.</summary>
		private bool DeletePaySimpleToken() {
			if(CreditCardCur.PaySimpleToken=="") {
				return true;
			}
			Cursor=Cursors.WaitCursor;
			try {
				if(CreditCardCur.IsPaySimpleACH()) {
					PaySimple.DeleteACHAccount(CreditCardCur);
				}
				else if(CreditCardCur.CCSource==CreditCardSource.PaySimple) {//Credit card
					PaySimple.DeleteCreditCard(CreditCardCur);
				}
			}
			catch(PaySimpleException ex) {
				MessageBox.Show(ex.Message);
				if(ex.ErrorType==PaySimpleError.CustomerDoesNotExist && MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"Delete the link to the customer id for this patient?")) 
				{
					PatientLinks.DeletePatNumTos(ex.CustomerId,PatientLinkType.PaySimple);
				}
				return false;
			}
			catch(Exception ex) {
				if(MessageBox.Show(Lans.g(this,"Error when deleting from PaySimple:")+"\r\n"+ex.Message+"\r\n\r\n"
					+Lans.g(this,"Do you still want to delete the card from ")+PrefC.GetString(PrefName.SoftwareName)+"?",
					"",MessageBoxButtons.YesNo)==DialogResult.No) 
				{
					return false;
				}
			}
			finally {
				Cursor=Cursors.Default;
			}
			return true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!IsUpdatedFrequencyValid()) {
				return;
			}
			if(!VerifyData()) {
				return;
			}
			CreditCardCur.ExcludeProcSync=checkExcludeProcSync.Checked;
			CreditCardCur.Address=textAddress.Text;
			CreditCardCur.CCNumberMasked=textCardNumber.Text;
			CreditCardCur.PatNum=_patient.PatNum;
			CreditCardCur.Zip=textZip.Text;
			CreditCardCur.PaymentType=comboPaymentType.GetSelectedDefNum();
			CreditCardCur.CanChargeWhenNoBal=checkChrgWithNoBal.Checked;
			CreditCardCur.IsRecurringActive=checkIsRecurringActive.Checked;
			if(_isEdgeExpressEnabled || _isXChargeEnabled || _isPayConnectEnabled || _isPaySimpleEnabled) {//Only update recurring if using X-Charge, PayConnect,or PaySimple.
				CreditCardCur.ChargeAmt=PIn.Double(textChargeAmt.Text);
				CreditCardCur.DateStart=PIn.Date(textDateStart.Text);
				CreditCardCur.DateStop=PIn.Date(textDateStop.Text);
				CreditCardCur.Note=textNote.Text;
				if(comboPaymentPlans.SelectedIndex>0) {
					CreditCardCur.PayPlanNum=_listPayPlans[comboPaymentPlans.SelectedIndex-1].PayPlanNum;
				}
				else {
					CreditCardCur.PayPlanNum=0;//Allows users to change from a recurring payplan charge to a normal one.
				}
				CreditCardCur.ChargeFrequency=GetFormattedChargeFrequency();
			}
			if(CreditCardCur.IsNew) {
				List<CreditCard> listCreditCards=CreditCards.RefreshAll(_patient.PatNum);
				CreditCardCur.ItemOrder=listCreditCards.Count;
				CreditCardCur.CCSource=CreditCardSource.None;
				CreditCardCur.ClinicNum=Clinics.ClinicNum;
				CreditCards.Insert(CreditCardCur);
			}
			else {
				#region EdgeExpress
				if(_isEdgeExpressEnabled && CreditCardCur.XChargeToken!="" 
					&& (_creditCard.CCNumberMasked!=CreditCardCur.CCNumberMasked || _creditCard.CCExpiration!=CreditCardCur.CCExpiration)) 
				{
					Cursor=Cursors.WaitCursor;
					try {
						EdgeExpress.RcmResponse rcmResponse=EdgeExpress.RCM.UpdateAlias(_patient,Clinics.ClinicNum,CreditCardCur.XChargeToken,
							CreditCardCur.CCExpiration,isWebPayment:false);
						Cursor=Cursors.Default;
						if(!rcmResponse.IsSuccess) {
							throw new ODException(Lans.g(this,"Error from EdgeExpress:")+" "+rcmResponse.RESULTMSG);
						}
					}
					catch(Exception ex) {
						try {
							XWebResponse xWebResponse=EdgeExpress.CNP.UpdateAlias(_patient.PatNum,CreditCardCur.XChargeToken,CreditCardCur.CCExpiration);
							if(PIn.Enum<XWebResponseCodes>(xWebResponse.ResponseCode)==XWebResponseCodes.Approval) {
								//CNP was successful, don't throw error.
								ex.DoNothing();
							}
							else {
								throw new ODException(ex.Message,ex);
							}
						}
						catch(Exception exe) {
							Cursor=Cursors.Default;
							FriendlyException.Show(Lans.g(this,"There was a problem updating the credit card.  Please try again."),exe);
						}
					}
				}
				#endregion EdgeExpress
				#region X-Charge
				//Special logic for had a token and changed number or expiration date X-Charge
				else if(_isXChargeEnabled && CreditCardCur.XChargeToken!=""
					&& (_creditCard.CCNumberMasked!=CreditCardCur.CCNumberMasked || _creditCard.CCExpiration!=CreditCardCur.CCExpiration)) 
				{ 
					Program program=Programs.GetCur(ProgramName.Xcharge);
					string path=Programs.GetProgramPath(program);
					if(program==null){
						MsgBox.Show(this,"X-Charge entry is missing from the database.");//should never happen
						return;
					}
					if(!program.Enabled){
						if(Security.IsAuthorized(EnumPermType.Setup)){
							using FormXchargeSetup formXchargeSetup=new FormXchargeSetup();
							formXchargeSetup.ShowDialog();
						}
						return;
					}
					if(!File.Exists(path)){
						MsgBox.Show(this,"Path is not valid.");
						if(Security.IsAuthorized(EnumPermType.Setup)){
							using FormXchargeSetup formXchargeSetup=new FormXchargeSetup();
							formXchargeSetup.ShowDialog();
						}
						return;
					}
					//Either update the exp date or update credit card number by deleting archive so new token can be created next time it's used.
					ProcessStartInfo processStartInfo=new ProcessStartInfo(path);
					string resultFile=PrefC.GetRandomTempFile("txt");
					try {
						File.Delete(resultFile);//delete the old result file.
					}
					catch {
						MsgBox.Show(this,"Could not delete XResult.txt file.  It may be in use by another program, flagged as read-only, or you might not have sufficient permissions.");
						return;
					}
					string xUsername=ProgramProperties.GetPropVal(program.ProgramNum,"Username",Clinics.ClinicNum);
					string xPassword=CodeBase.MiscUtils.Decrypt(ProgramProperties.GetPropVal(program.ProgramNum,"Password",Clinics.ClinicNum));
					//We can only change exp date for X-Charge via ARCHIVEAULTUPDATE.
					processStartInfo.Arguments+="/TRANSACTIONTYPE:ARCHIVEVAULTUPDATE ";
					processStartInfo.Arguments+="/XCACCOUNTID:"+CreditCardCur.XChargeToken+" ";
					if(CreditCardCur.CCExpiration!=null && CreditCardCur.CCExpiration.Year>2005) {
						processStartInfo.Arguments+="/EXP:"+CreditCardCur.CCExpiration.ToString("MMyy")+" ";
					}
					processStartInfo.Arguments+="/RESULTFILE:\""+resultFile+"\" ";
					processStartInfo.Arguments+="/USERID:"+xUsername+" ";
					processStartInfo.Arguments+="/PASSWORD:"+xPassword+" ";
					processStartInfo.Arguments+="/AUTOPROCESS ";
					processStartInfo.Arguments+="/AUTOCLOSE ";
					Cursor=Cursors.WaitCursor;
					Process process=new Process();
					process.StartInfo=processStartInfo;
					process.EnableRaisingEvents=true;
					process.Start();
					process.WaitForExit();
					Thread.Sleep(200);//Wait 2/10 second to give time for file to be created.
					Cursor=Cursors.Default;
					string resultTxt="";
					string line="";
					using TextReader textReader = new StreamReader(resultFile);
					try {
						line=textReader.ReadLine();
					}
					catch {
						MsgBox.Show(this,"There was a problem creating or editing this card with X-Charge.  Please try again.");
						return;
					}
					while(line!=null) {
						if(resultTxt!="") {
							resultTxt+="\r\n";
						}
						resultTxt+=line;
						if(line.StartsWith("RESULT=")) {
							if(line!="RESULT=SUCCESS") {
								CreditCardCur=CreditCards.GetOne(CreditCardCur.CreditCardNum);
								FillData();
								return;
							}
						}
						line=textReader.ReadLine(); //We've already read the file once, so subsequent ReadLines shouldn't cause issues.
					}
				}//End of special token logic
				#endregion
				#region PayConnect
				//Special logic for had a token and changed expiration date PayConnect
				//We have to compare the year and month of the expiration instead of just comparing expirations because the X-Charge logic stores the
				//expiration day of the month as the 1st in the db, but it makes more sense to set the expriation day of month to the last day in that month.
				//Since we only want to invalidate the PayConnect token if the expiration month or year is different, we will ignore any difference in day.
				if(_isPayConnectEnabled && CreditCardCur.PayConnectToken!=""
					&& (_creditCard.CCExpiration.Year!=CreditCardCur.CCExpiration.Year
						|| _creditCard.CCExpiration.Month!=CreditCardCur.CCExpiration.Month))
				{
					//if the expiration is changed, the token is no longer valid, so clear the token and token expiration so a new one can be
					//generated the next time a payment is processed using this card.
					CreditCardCur.PayConnectToken="";
					CreditCardCur.PayConnectTokenExp=DateTime.MinValue;
					//To match PaySimple, this should be enhanced to validate the cc number and get a new token.
				}
				#endregion
				#region PaySimple
				//Special logic for had a token and changed number or expiration date PaySimple
				//We have to compare the year and month of the expiration instead of just comparing expirations because the X-Charge logic stores the
				//expiration day of the month as the 1st in the db, but it makes more sense to set the expriation day of month to the last day in that month.
				//Since we only want to invalidate the PayConnect token if the expiration month or year is different, we will ignore any difference in day.
				if(_isPaySimpleEnabled && CreditCardCur.PaySimpleToken!=""
					&& (_creditCard.Zip!=CreditCardCur.Zip
						|| _creditCard.CCExpiration.Year!=CreditCardCur.CCExpiration.Year
						|| _creditCard.CCExpiration.Month!=CreditCardCur.CCExpiration.Month))
				{
					//TODO: Open form to have user enter the CC number.  Then make API call to update cc instead of wiping out token.
					//If the billing zip or the expiration changes, the token is invalid and they need to get a new one.
					CreditCardCur.PaySimpleToken="";
				}
				#endregion
				if(_isDuplicate) {
					CreditCards.Insert(CreditCardCur);
				} 
				else {
					CreditCards.Update(CreditCardCur);
					CreditCards.InsertAuditTrailEntry(CreditCardCur,_creditCard);
				}
			}
			DialogResult=DialogResult.OK;
		}

	}
}