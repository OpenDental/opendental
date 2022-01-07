using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;
using OpenDental.Bridges;
using EdgeExpressProps = OpenDentBusiness.ProgramProperties.PropertyDescs.EdgeExpress;

namespace OpenDental{
///<summary></summary>
	public partial class FormClaimPayEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		public ClaimPayment ClaimPaymentCur;
		///<summary>List of defs of type ClaimPaymentGroup</summary>
		private List<Def> _listCPGroups;
		///<summary>Used to tell if a InsPayCreate log is necessary instead of a InsPayEdit log when IsNew is set to false.</summary>
		public bool IsCreateLogEntry;
		private List<Def> _listInsurancePaymentTypeDefs;
		///<summary>This is the deposit that was originally associated to the claimpayment OR is set to a deposit that came back from the Deposit Edit window via the Edit button.
		///Can be null if no deposit was associated to the claimpayment passed in or if the user deletes the deposit via the Edit window.</summary>
		private Deposit _depositOld;
		///<summary>Set to the value of PrefName.ShowAutoDeposit on load.</summary>
		private bool _hasAutoDeposit;
		///<summary>Gets set to true when the user deletes the Deposit from the Edit Deposit window.</summary>
		private bool _isAutoDepositDeleted;
		private ErrorProvider _errorProv=new ErrorProvider();
		private bool _isMissingRequiredFields;
		private bool _isValidating=false;
		private List<RequiredField> _listRequiredFields;
		private ClaimPayment _claimPaymentOld;
		/// <summary>Set to true to not check Insurance Payment Edit permission when finalizing a payment.</summary>
		public bool IsFinalizePayment;

		///<summary></summary>
		public FormClaimPayEdit(ClaimPayment claimPaymentCur) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			ClaimPaymentCur=claimPaymentCur;
			_depositOld=Deposits.GetOne(claimPaymentCur.DepositNum);
			_claimPaymentOld=claimPaymentCur.Copy();
			Lan.F(this);
		}

		private void FormClaimPayEdit_Load(object sender, System.EventArgs e) {
			if(IsNew) {//security for new and finalized claim payments already checked before this form opens
				textAmount.Select();
			} 
			else {
				textCheckNum.Select();//If new, then the amount would have been selected.
				if(!IsFinalizePayment && !Security.IsAuthorized(Permissions.InsPayEdit,ClaimPaymentCur.CheckDate)){
					butOK.Enabled=false;
				}
			}
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
			}
			else {
				comboClinic.SelectedClinicNum=ClaimPaymentCur.ClinicNum;
			}
			_listInsurancePaymentTypeDefs=Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true);
			for(int i=0;i<_listInsurancePaymentTypeDefs.Count;i++) {
				comboPayType.Items.Add(_listInsurancePaymentTypeDefs[i].ItemName);
				if(_listInsurancePaymentTypeDefs[i].DefNum==ClaimPaymentCur.PayType) {
					comboPayType.SelectedIndex=i;
				}
			}
			if(comboPayType.Items.Count > 0 && comboPayType.SelectedIndex < 0) {//There are InsurancePaymentTypes and none are selected.  Should never happen.
				comboPayType.SelectedIndex=0;//Select the first one in the list.
			}
			if(ClaimPaymentCur.CheckDate.Year>1880) {
				textDate.Text=ClaimPaymentCur.CheckDate.ToShortDateString();
			}
			if(ClaimPaymentCur.DateIssued.Year>1880) {
				textDateIssued.Text=ClaimPaymentCur.DateIssued.ToShortDateString();
			}
			_listRequiredFields=RequiredFields.GetWhere(x => x.FieldType==RequiredFieldType.InsPayEdit);
			SetRequiredFields();
			_errorProv.BlinkStyle=ErrorBlinkStyle.NeverBlink;
			textCheckNum.Text=ClaimPaymentCur.CheckNum;
			textBankBranch.Text=ClaimPaymentCur.BankBranch;
			textAmount.Text=ClaimPaymentCur.CheckAmt.ToString("F");
			textCarrierName.Text=ClaimPaymentCur.CarrierName;
			textNote.Text=ClaimPaymentCur.Note;
			_listCPGroups=Defs.GetDefsForCategory(DefCat.ClaimPaymentGroups,true);
			FillComboPaymentGroup(ClaimPaymentCur.PayGroup);
			CheckUIState();
			_hasAutoDeposit=PrefC.GetBool(PrefName.ShowAutoDeposit);
			FillAutoDepositDetails();
			Plugins.HookAddCode(this,"FormClaimPayEdit.Load_end",ClaimPaymentCur);
		}

		///<summary>Fills auto deposit group box for the attached deposit. Also handles the logic of hiding itself depending on the preference 'ShowAutoDeposit'.</summary>
		private void FillAutoDepositDetails() {
			//Do not show the Auto Deposit Details when users have the preference turned off and there is no deposit currently attached.
			if(_isAutoDepositDeleted || (!_hasAutoDeposit && _depositOld==null)) {
				groupBoxDeposit.Visible=false;
				return;
			}
			//Alter the text on the group box if the deposit associated to this claim payment was an auto deposit.
			groupBoxDeposit.Text=Lan.g(this,"Auto Deposit Details");
			if(_depositOld!=null && _depositOld.DepositAccountNum==0) {
				groupBoxDeposit.Text=Lan.g(this,"Deposit Details");
			}
			//Fill deposit account num drop down
			comboDepositAccountNum.Items.Clear();
			comboDepositAccountNum.Items.AddDefs(Defs.GetDefsForCategory(DefCat.AutoDeposit,true));
			//Auto deposit pref enabled and the Claim Payment IsNew or had its Auto Deposit deleted.
			if(_hasAutoDeposit && _depositOld==null) {
				//Prefill some fields within the auto deposit details from the insurance payment fields to be nice.
				if(string.IsNullOrWhiteSpace(validDepositDate.Text)) {
					validDepositDate.Text=DateTime.Now.ToShortDateString();
				}
				if(string.IsNullOrWhiteSpace(validDoubleDepositAmt.Text)) {
					validDoubleDepositAmt.Text=textAmount.Text;
				}
			}
			if(_depositOld!=null) {//check for an existing deposit
				//Disable all controls within the Auto Deposit Details group box except the Edit button.
				//Per Mark we don't want users to edit directly in this form, but rather open FormDepositEdit.
				foreach(Control ctrl in this.groupBoxDeposit.Controls) {
					if(ctrl==butDepositEdit) {
						continue;
					}
					ODException.SwallowAnyException(() => { ctrl.Enabled=(_depositOld==null); });
				}
			}
			//Any values within _depositOld should ALWAYS override claim payment values mainly because the user could have changed depositOld via the Edit button.
			if(_depositOld!=null) {
				butDepositEdit.Visible=true;
				validDepositDate.Text=_depositOld.DateDeposit.ToShortDateString();
				validDoubleDepositAmt.Text=_depositOld.Amount.ToString();
				textBoxBatchNum.Text=_depositOld.Batch;
				comboDepositAccountNum.SetSelectedDefNum(_depositOld.DepositAccountNum);
			}
		}

		///<summary>Returns a new deposit object from the UI values on the form.
		///Any values that do not have a UI in this window will be inherited from _depositOld if it is not null.</summary>
		private Deposit GetDepositCur() {
			Deposit depositCur=new Deposit();
			if(_depositOld!=null) {//Maintain the values of the existing deposit if it exists
				depositCur=_depositOld.Copy();
			}
			//Update UI values
			depositCur.Amount=PIn.Double(validDoubleDepositAmt.Text);
			depositCur.DateDeposit=PIn.Date(validDepositDate.Text);
			depositCur.Batch=PIn.String(textBoxBatchNum.Text);
			depositCur.DepositAccountNum=comboDepositAccountNum.GetSelectedDefNum();
			return depositCur;
		}

		///<summary>Mimics FormPayment.CheckUIState().</summary>
		private void CheckUIState() {
			Program progXcharge=Programs.GetCur(ProgramName.Xcharge);
			Program progPayConnect=Programs.GetCur(ProgramName.PayConnect);
			Program progPaySimple=Programs.GetCur(ProgramName.PaySimple);
			Program progEdgeExpress=Programs.GetCur(ProgramName.EdgeExpress);
			if(progXcharge==null || progPayConnect==null || progPaySimple==null || progEdgeExpress==null) {//Should not happen.
				panelXcharge.Visible=(progXcharge!=null);
				butPayConnect.Visible=(progPayConnect!=null);
				butPaySimple.Visible=(progPaySimple!=null);
				panelEdgeExpress.Visible=(progEdgeExpress!=null);
				groupPrepaid.Visible=(panelXcharge.Visible || butPayConnect.Visible || butPaySimple.Visible || panelEdgeExpress.Visible);
				return;
			}
			panelXcharge.Visible=false;
			butPayConnect.Visible=false;
			butPaySimple.Visible=false;
			panelEdgeExpress.Visible=false;
			if(!progPayConnect.Enabled && !progXcharge.Enabled && !progPaySimple.Enabled && !progEdgeExpress.Enabled) {//if none enabled
				//show all so user can pick
				panelXcharge.Visible=true;
				butPayConnect.Visible=true;
				butPaySimple.Visible=true;
				panelEdgeExpress.Visible=true;
				groupPrepaid.Visible=true;
				return;
			}
			long clinicNum=GetClinicNumSelected();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			//Show if enabled.  User could have all enabled.
			if(progPayConnect.Enabled 
				&& !PIn.Bool(ProgramProperties.GetPropVal(progPayConnect.ProgramNum,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,clinicNum))) 
			{
				//if clinics are disabled, PayConnect is enabled if marked enabled
				if(!PrefC.HasClinicsEnabled) {
					butPayConnect.Visible=true;
				}
				else {//if clinics are enabled, PayConnect is enabled if the PaymentType is valid and the Username and Password are not blank
					string paymentType=ProgramProperties.GetPropVal(progPayConnect.ProgramNum,"PaymentType",clinicNum);
					//Decrypt password for later checks because an empty string password is not an empty string when encrypted.
					string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(progPayConnect.ProgramNum,"Password",clinicNum));
					if(!string.IsNullOrEmpty(ProgramProperties.GetPropVal(progPayConnect.ProgramNum,"Username",clinicNum))
						&& !string.IsNullOrEmpty(password)
						&& listDefs.Any(x => x.DefNum.ToString()==paymentType))
					{
						butPayConnect.Visible=true;
					}
				}
			}
			//show if enabled.  User could have both enabled.
			if(progXcharge.Enabled
				&& !PIn.Bool(ProgramProperties.GetPropVal(progXcharge.ProgramNum,ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC,clinicNum)))
			{
				//if clinics are disabled, X-Charge is enabled if marked enabled
				if(!PrefC.HasClinicsEnabled) {
					panelXcharge.Visible=true;
				}
				else {//if clinics are enabled, X-Charge is enabled if the PaymentType is valid and the Username and Password are not blank
					string paymentType=ProgramProperties.GetPropVal(progXcharge.ProgramNum,"PaymentType",clinicNum);
					if(!string.IsNullOrEmpty(ProgramProperties.GetPropVal(progXcharge.ProgramNum,"Username",clinicNum))
						&& !string.IsNullOrEmpty(ProgramProperties.GetPropVal(progXcharge.ProgramNum,"Password",clinicNum))
						&& listDefs.Any(x => x.DefNum.ToString()==paymentType))
					{
						panelXcharge.Visible=true;
					}
				}
			}
			if(progPaySimple.Enabled
				&& !PIn.Bool(ProgramProperties.GetPropVal(progPaySimple.ProgramNum,PaySimple.PropertyDescs.PaySimplePreventSavingNewCC,clinicNum)))
			{
				//if clinics are disabled, PayConnect is enabled if marked enabled
				if(!PrefC.HasClinicsEnabled) {
					butPaySimple.Visible=true;
				}
				else {//if clinics are enabled, PayConnect is enabled if the PaymentType is valid and the Username and Password are not blank
					string paymentType=ProgramProperties.GetPropValForClinicOrDefault(progPaySimple.ProgramNum,PaySimple.PropertyDescs.PaySimplePayTypeCC,clinicNum);
					if(!string.IsNullOrEmpty(ProgramProperties.GetPropValForClinicOrDefault(progPaySimple.ProgramNum,PaySimple.PropertyDescs.PaySimpleApiUserName,clinicNum))
						&& !string.IsNullOrEmpty(ProgramProperties.GetPropValForClinicOrDefault(progPaySimple.ProgramNum,PaySimple.PropertyDescs.PaySimpleApiKey,clinicNum))
						&& listDefs.Any(x => x.DefNum.ToString()==paymentType))
					{
						butPaySimple.Visible=true;
					}
				}
			}
			if(progEdgeExpress.Enabled
				&& !PIn.Bool(ProgramProperties.GetPropVal(progEdgeExpress.ProgramNum,EdgeExpressProps.PreventSavingNewCC,clinicNum)))
			{
				//if clinics are disabled, X-Charge is enabled if marked enabled
				if(!PrefC.HasClinicsEnabled) {
					panelEdgeExpress.Visible=true;
				}
				else {//if clinics are enabled, EdgeExpress is enabled if the XWeb creds are not blank
					if(!string.IsNullOrEmpty(ProgramProperties.GetPropVal(progEdgeExpress.ProgramNum,EdgeExpressProps.XWebID,clinicNum))
						&& !string.IsNullOrEmpty(ProgramProperties.GetPropVal(progEdgeExpress.ProgramNum,EdgeExpressProps.AuthKey,clinicNum))
						&& !string.IsNullOrEmpty(ProgramProperties.GetPropVal(progEdgeExpress.ProgramNum,EdgeExpressProps.TerminalID,clinicNum))) {
						panelEdgeExpress.Visible=true;
					}
				}
			}
			if(!panelXcharge.Visible && !butPayConnect.Visible && !butPaySimple.Visible && !panelEdgeExpress.Visible) {
				//This is an office with clinics and one of the payment processing bridges is enabled but this particular clinic doesn't have one set up.
				panelXcharge.Visible=progXcharge.Enabled;
				butPayConnect.Visible=progPayConnect.Enabled;
				butPaySimple.Visible=progPaySimple.Enabled;
				panelEdgeExpress.Visible=progEdgeExpress.Enabled;
			}
			groupPrepaid.Visible=(panelXcharge.Visible || butPayConnect.Visible || butPaySimple.Visible || panelEdgeExpress.Visible);
		}

		private long GetClinicNumSelected() {
			if(!PrefC.HasClinicsEnabled) {
				return 0;
			}
			return comboClinic.SelectedClinicNum;
		}

		private void butCarrierSelect_Click(object sender,EventArgs e) {
			CheckUIState();
			using FormCarriers formC=new FormCarriers();
			formC.IsSelectMode=true;
			formC.ShowDialog();
			if(formC.DialogResult==DialogResult.OK) {
				textCarrierName.Text=formC.CarrierSelected.CarrierName;
			}
		}

		private void FillComboPaymentGroup(long selectedDefNum=0) {
			comboPayGroup.Items.Clear();
			//If there are no claim payment group defs, hide the options per Nathan's request.
			if(_listCPGroups.Count==0) {
				comboPayGroup.Visible=false;
				butPickPaymentGroup.Visible=false;
				labelClaimPaymentGroup.Visible=false;
				return;
			}
			for(int i = 0;i<_listCPGroups.Count;i++) {
				Def defCur=_listCPGroups[i];
				comboPayGroup.Items.Add(defCur.ItemName);
				if(selectedDefNum==defCur.DefNum) {
					comboPayGroup.SelectedIndex=i;
				}
			}
			if(selectedDefNum==0) {
				comboPayGroup.SelectedIndex=0; //there should always be one selected.
			}
		}

		///<summary>Used to keep the Claim Payment amount and Auto Deposit amount in sync. They should always match when creating a new Claim Payment.</summary>
		private void TextAmount_Leave(object sender,EventArgs e) {
				validDoubleDepositAmt.Text=textAmount.Text;
		}

		private void butDepositEdit_Click(object sender,EventArgs e) {
			//The user may have edited fields before hitting the "Edit" button.
			using FormDepositEdit formDE=new FormDepositEdit(GetDepositCur());
			formDE.ShowDialog();
			if(formDE.DialogResult==DialogResult.OK) {//Made changes, update deposit values
				//Get the deposit associated to our claimpayment and update this form with it.
				_depositOld=Deposits.GetOne(ClaimPaymentCur.DepositNum);
				//User deleted the deposit so dissaociate it from the claim payment.
				if(_depositOld==null) {
					ClaimPaymentCur.DepositNum=0;
					_isAutoDepositDeleted=true;
				}
				FillAutoDepositDetails();
			}
		}

		private void butPickPaymentGroup_Click(object sender,EventArgs e) {
			using FormDefinitionPicker FormDP=new FormDefinitionPicker(DefCat.ClaimPaymentGroups);
			FormDP.ShowDialog();
			if(FormDP.DialogResult==DialogResult.OK && !FormDP.ListDefsSelected.IsNullOrEmpty()) {
				FillComboPaymentGroup(FormDP.ListDefsSelected[0].DefNum);
			}
		}

		///<summary>The contents of this event mimic FormPayment.panelXcharge_MouseClick().</summary>
		private void panelXcharge_MouseClick(object sender,MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) {
				return;
			}
			if(textAmount.Text=="" || PIn.Double(textAmount.Text)==0) {
				MsgBox.Show(this,"Please enter an amount first.");
				textAmount.Focus();
				return;
			}
			Payment pay=new Payment();
			pay.ClinicNum=GetClinicNumSelected();
			FormPayment form=new FormPayment(new Patient(),new Family(),pay,false);
			try {
				string tranDetail=form.MakeXChargeTransaction(PIn.Double(textAmount.Text));
				if(tranDetail!=null) {
					if(textNote.Text!="") {
						textNote.Text+="\r\n";
					}
					textNote.Text+=tranDetail;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error processing transaction.\r\n\r\nPlease contact support with the details of this error:")
					//The rest of the message is not translated on purpose because we here at HQ need to always be able to quickly read this part.
					+"\r\nLast valid milestone reached: "+form.XchargeMilestone,ex);
			}
		}

		private void butPayConnect_Click(object sender,EventArgs e) {
			if(textAmount.Text=="" || PIn.Double(textAmount.Text)==0) {
				MsgBox.Show(this,"Please enter an amount first.");
				textAmount.Focus();
				return;
			}
			Payment pay=new Payment();
			pay.ClinicNum=GetClinicNumSelected();
			FormPayment form=new FormPayment(new Patient(),new Family(),pay,false);
			string tranDetail=form.MakePayConnectTransaction(PIn.Double(textAmount.Text));
			if(tranDetail!=null) {
				if(textNote.Text!="") {
					textNote.Text+="\r\n";
				}
				textNote.Text+=tranDetail;
			}
		}

		private void butPaySimple_MouseClick(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			if(textAmount.Text=="" || PIn.Double(textAmount.Text)==0) {
				MsgBox.Show(this,"Please enter an amount first.");
				textAmount.Focus();
				return;
			}
			Payment pay=new Payment();
			pay.ClinicNum=GetClinicNumSelected();
			FormPayment form=new FormPayment(new Patient(),new Family(),pay,false);
			string tranDetail=form.MakePaySimpleTransaction(PIn.Double(textAmount.Text),textCarrierName.Text);
			if(tranDetail!=null) {
				if(textNote.Text!="") {
					textNote.Text+="\r\n";
				}
				textNote.Text+=tranDetail;
			}
		}

		private void panelEdgeExpress_MouseClick(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			if(textAmount.Text=="" || PIn.Double(textAmount.Text)==0) {
				MsgBox.Show(this,"Please enter an amount first.");
				textAmount.Focus();
				return;
			}
			Payment pay=new Payment();
			pay.ClinicNum=GetClinicNumSelected();
			FormPayment form=new FormPayment(new Patient(),new Family(),pay,false);
			try {
				string tranDetail=form.MakeEdgeExpressTransaction(PIn.Double(textAmount.Text));
				if(tranDetail!=null) {
					if(textNote.Text!="") {
						textNote.Text+="\r\n";
					}
					textNote.Text+=tranDetail;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error processing transaction.\r\n\r\nPlease contact support with the details of this error:")+"\r\n"+ex.Message,ex);
			}
		}

		//<summary>Will do the following things to the UI:
		//1.  Puts an asterisk next to each label that has a required field
		//2.  Identifies all RequiredFields that do not have their condition met.</summary>
		private void SetRequiredFields() {
			_isMissingRequiredFields=false;
			bool areConditionsMet;
			for(int i=0;i<_listRequiredFields.Count;i++) {
				areConditionsMet=ConditionsAreMet(_listRequiredFields[i]);
				if(areConditionsMet) {
					labelRequiredFields.Visible=true;
				}
				switch(_listRequiredFields[i].FieldName) {
					case RequiredFieldName.InsPayEditClinic:
						SetRequiredComboBoxClinicPicker(labelClinic,comboClinic,areConditionsMet,-1,"A clinic must be selected.");
						break;
					case RequiredFieldName.PaymentType:
						SetRequiredComboBox(label11,comboPayType,areConditionsMet,-1,"A Payment Type must be selected.");
						break;
					case RequiredFieldName.CheckDate:
						SetRequiredTextBox(labelDateIssued,textDateIssued,areConditionsMet);
						break;
					case RequiredFieldName.PaymentAmount:
						SetRequiredTextBox(label5,textAmount,areConditionsMet);
						break;
					case RequiredFieldName.CheckNumber:
						SetRequiredTextBox(label4,textCheckNum,areConditionsMet);
						break;
					case RequiredFieldName.DepositDate:
						if(!groupBoxDeposit.Visible) {//don't set required if you can't see it
							break;
						}
						SetRequiredTextBox(labelDepositDate,validDepositDate,areConditionsMet);
						break;
					case RequiredFieldName.BatchNumber:
						if(!groupBoxDeposit.Visible) {//don't set required if you can't see it
							break;
						}
						SetRequiredTextBox(labelBatchNum,textBoxBatchNum,areConditionsMet);
						break;
					case RequiredFieldName.DepositAccountNumber:
						if(!groupBoxDeposit.Visible) {//don't set required if you can't see it
							break;
						}
						SetRequiredComboBoxPlus(labelDepositAccountNum,comboDepositAccountNum,areConditionsMet,-1,"An Auto Deposit Account Number must be selected.");
						break;
				}
			}
		}

		///<summary>Returns true if all the conditions for the RequiredField are met.</summary>
		private bool ConditionsAreMet(RequiredField reqField) {
			List<RequiredFieldCondition> listConditions=reqField.ListRequiredFieldConditions;
			if(listConditions.Count==0) {//This RequiredField is always required
				return true;
			}
			bool areConditionsMet=false;
			int previousFieldName=-1;
			for(int i=0;i<listConditions.Count;i++) {
				if(areConditionsMet && (int)listConditions[i].ConditionType==previousFieldName) {
					continue;//A condition of this type has already been met
				}
				if(!areConditionsMet && previousFieldName!=-1
					&& (int)listConditions[i].ConditionType!=previousFieldName) {
					return false;//None of the conditions of the previous type were met
				}
				areConditionsMet=false;
				switch(listConditions[i].ConditionType) {
					case RequiredFieldName.InsPayEditClinic:
						if(!PrefC.HasClinicsEnabled) {
							areConditionsMet=true;
							break;
						}
						break;
					case RequiredFieldName.DepositDate:
						if(groupBoxDeposit.Visible) {//don't set required if you can't see it
							areConditionsMet=true;
							break;
						}
						break;
					case RequiredFieldName.BatchNumber:
						if(groupBoxDeposit.Visible) {//don't set required if you can't see it
							areConditionsMet=true;
							break;
						}
						SetRequiredTextBox(labelBatchNum,textBoxBatchNum,areConditionsMet);
						break;
					case RequiredFieldName.DepositAccountNumber:
						if(groupBoxDeposit.Visible) {//don't set required if you can't see it
							areConditionsMet=true;
							break;
						}
						break;
					default://The field is not on this form
						areConditionsMet=true;
						break;
				}
				previousFieldName=(int)listConditions[i].ConditionType;
			}
			return areConditionsMet;
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If it also blank, highlights the textbox
		///background.</summary>
		private void SetRequiredTextBox(Label labelCur,TextBox textBoxCur,bool areConditionsMet) {
			if(areConditionsMet) {
				labelCur.Text=labelCur.Text.Replace("*","")+"*";
				if(textBoxCur.Text=="") {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProv.SetError(textBoxCur,"Text box cannot be blank.");
					}
				}
				else {
					_errorProv.SetError(textBoxCur,"");
				}
			}
			else {
				labelCur.Text=labelCur.Text.Replace("*","");
				_errorProv.SetError(textBoxCur,"");
			}
			if(textBoxCur.Name==textDateIssued.Name) {
				label1.Text="";//remove label saying it is an optional field
			}
			else if(textBoxCur.Name==textCheckNum.Name) {
				label10.Text="";//remove label saying it is an optional field
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the combobox background.</summary>
		private void SetRequiredComboBox(Label labelCur,ComboBox comboCur,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			if(areConditionsMet) {
				labelCur.Text=labelCur.Text.Replace("*","")+"*";
				if(comboCur.SelectedIndex==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProv.SetError(comboCur,Lan.g(this,errorMsg));
					}
				}
				else {
					_errorProv.SetError(comboCur,"");
				}
			}
			else {
				labelCur.Text=labelCur.Text.Replace("*","");
				_errorProv.SetError(comboCur,"");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. </summary>
		private void SetRequiredComboBoxPlus(Label labelCur,ComboBoxOD comboCur,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			if(areConditionsMet) {
				labelCur.Text=labelCur.Text.Replace("*","")+"*";
				if(comboCur.SelectedIndex==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						//_errorProv.SetError(comboCur,Lan.g(this,errorMsg));
					}
				}
				else {
					//_errorProv.SetError(comboCur,"");
				}
			}
			else {
				labelCur.Text=labelCur.Text.Replace("*","");
				//_errorProv.SetError(comboCur,"");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. </summary>
		private void SetRequiredComboBoxClinicPicker(Label labelCur,ComboBoxClinicPicker comboCur,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			if(areConditionsMet) {
				labelCur.Text=labelCur.Text.Replace("*","")+"*";
				if(comboCur.SelectedClinicNum==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						//_errorProv.SetError(comboCur,Lan.g(this,errorMsg));
					}
				}
				else {
					//_errorProv.SetError(comboCur,"");
				}
			}
			else {
				labelCur.Text=labelCur.Text.Replace("*","");
				//_errorProv.SetError(comboCur,"");
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			SetRequiredFields();//Required field validation
			if(textDate.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			if(PIn.Date(textDate.Text).Date > DateTime.Today.Date
				&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed) 
				&& !PrefC.GetBool(PrefName.AllowFutureInsPayments)) 
			{
				MsgBox.Show(this,"Payment date cannot be in the future.");
				return;
			}
			if(textCarrierName.Text=="") {
				MsgBox.Show(this,"Please enter a carrier.");
				return;
			}
			if(!textDate.IsValid()
				|| !textAmount.IsValid()
				|| !textDateIssued.IsValid()
				|| !validDepositDate.IsValid()
				|| !validDoubleDepositAmt.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(!PrefC.GetBool(PrefName.AllowFutureInsPayments) && PIn.Date(textDate.Text).Date>MiscData.GetNowDateTime().Date) {
				MsgBox.Show(this,"Insurance Payment Date must not be a future date.");
				return;
			}
			if(textAmount.Text=="") {
				textAmount.Text="0.00";
				return;
			}
			if(_isMissingRequiredFields) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Required fields are missing or incorrect.  Click OK to save anyway or Cancel to return and "
						+"finish payment information.")) {
					_isValidating=true;
					SetRequiredFields();
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.RequiredFields,ClaimPaymentCur.ClaimPaymentNum,"Saved claim payment with required fields missing.");
			}
			#region Automatic Deposit
			Def paymentType=Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true)[comboPayType.SelectedIndex];
			//Create an Auto Deposit if the claim payment is new or does not have an attached Auto Deposit. 
			//Auto deposits will NOT be made for Payment Types marked 'N' (not selected for deposit).
			if(_hasAutoDeposit && !_isAutoDepositDeleted && _depositOld==null && paymentType.ItemValue.ToLower()!="n") {
				//Insert the deposit, this must happen first as the claimpayment FK's to deposit.
				//The deposit cannot be updated in this form, that is handled by the edit button.
				Deposit depositCur=GetDepositCur();
				//Double check that there is a valid date entered into the form before trying to insert the deposit into the database.
				if(depositCur.DateDeposit.Year < 1880) {
					MsgBox.Show(this,"Please enter a valid Auto Deposit Date.");
					return;
				}
				if(!Security.IsAuthorized(Permissions.DepositSlips,depositCur.DateDeposit)) {
					return;
				}
				ClaimPaymentCur.DepositNum=Deposits.Insert(depositCur);
				SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0
					,Lan.g(this,"Auto Deposit created via the Edit Insurance Payment window:")+" "+depositCur.DateDeposit.ToShortDateString()
					+" "+Lan.g(this,"New")+" "+depositCur.Amount.ToString("c"));
			}
			#endregion
			double amt=PIn.Double(textAmount.Text);
			if(IsNew){
				//prevents backdating of initial check
				if(!Security.IsAuthorized(Permissions.InsPayCreate,PIn.Date(textDate.Text))){
					return;
				}
				//prevents attaching claimprocs with a date that is older than allowed by security.
			}
			else{
				//Editing an old entry will already be blocked if the date was too old, and user will not be able to click OK button.
				//This catches it if user changed the date to be older.
				if(IsFinalizePayment) {//finalizing a claim payment should use the InsPayCreate permission, not InsPayEdit
					if(!Security.IsAuthorized(Permissions.InsPayCreate,PIn.Date(textDate.Text))) {
						return;
					}
				}
				else {
					if(!Security.IsAuthorized(Permissions.InsPayEdit,PIn.Date(textDate.Text))) {
						return;
					}
				}
				//Check that the attached payments match the payment amount.
				List<ClaimPaySplit> listClaimPaySplit=Claims.GetAttachedToPayment(ClaimPaymentCur.ClaimPaymentNum);
				double insPayTotal=0;
				for(int i=0; i<listClaimPaySplit.Count; i++) {
					insPayTotal+=listClaimPaySplit[i].InsPayAmt;
				}
				if(!CompareDouble.IsEqual(insPayTotal,amt)) {
					if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Amount entered does not match Total Payments attached.\r\n"
						+"If you choose to continue, this insurance payment will be flagged as a partial payment, which will affect reports.\r\n"
						+"Click OK to continue, or click Cancel to edit Amount."))
					{
						ClaimPaymentCur.IsPartial=true;
					}
					else {
						//FormClaimPayBatch will set IsPartial back to false.
						return;
					}
				}
			}
			if(!_isAutoDepositDeleted && _depositOld!=null && !CompareDouble.IsEqual(PIn.Double(validDoubleDepositAmt.Text),_depositOld.Amount) 
				&& ClaimPayments.HasAutoDeposit(ClaimPaymentCur)) 
			{
				//The autogenerated deposit needs the amounts changed before we continue. 
				Deposits.Update(GetDepositCur());
			}
			if(PrefC.HasClinicsEnabled) {
				ClaimPaymentCur.ClinicNum=comboClinic.SelectedClinicNum;
			}
			ClaimPaymentCur.PayType=paymentType.DefNum;
			if(comboPayGroup.SelectedIndex!=-1) {//If they didn't select anything, leave what was originally there
				ClaimPaymentCur.PayGroup=_listCPGroups[comboPayGroup.SelectedIndex].DefNum;
			}
			ClaimPaymentCur.CheckDate=PIn.Date(textDate.Text);
			ClaimPaymentCur.DateIssued=PIn.Date(textDateIssued.Text);
			ClaimPaymentCur.CheckAmt=PIn.Double(textAmount.Text);
			ClaimPaymentCur.CheckNum=textCheckNum.Text;
			ClaimPaymentCur.BankBranch=textBankBranch.Text;
			ClaimPaymentCur.CarrierName=textCarrierName.Text;
			ClaimPaymentCur.Note=textNote.Text;
			try{
				if(IsNew) {
					ClaimPayments.Insert(ClaimPaymentCur);//error thrown if trying to change amount and already attached to a deposit.
					SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,0,
						Lan.g(this,"Carrier Name: ")+ClaimPaymentCur.CarrierName+", "
						+Lan.g(this,"Total Amount: ")+ClaimPaymentCur.CheckAmt.ToString("c")+", "
						+Lan.g(this,"Check Date: ")+ClaimPaymentCur.CheckDate.ToShortDateString()+", "//Date the check is entered in the system (i.e. today)
						+"ClaimPaymentNum: "+ClaimPaymentCur.ClaimPaymentNum);//Column name, not translated.
				}
				else {
					try {
						ClaimPayments.Update(ClaimPaymentCur);//error thrown if trying to change amount and already attached to a deposit.
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return;
					}
					if(IsCreateLogEntry) { //need a InsPayCreate Log entry because it just was pre-inserted.
						SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,0,
							Lan.g(this,"Carrier Name: ")+ClaimPaymentCur.CarrierName+", "
							+Lan.g(this,"Total Amount: ")+ClaimPaymentCur.CheckAmt.ToString("c")+", "
							+Lan.g(this,"Check Date: ")+ClaimPaymentCur.CheckDate.ToShortDateString()+", "//Date the check is entered in the system (i.e. today)
							+"ClaimPaymentNum: "+ClaimPaymentCur.ClaimPaymentNum);//Column name, not translated.
					}
					else {
						SecurityLogs.MakeLogEntry(Permissions.InsPayEdit,0,
							Lan.g(this,"Carrier Name: ")+ClaimPaymentCur.CarrierName+", "
							+Lan.g(this,"Previous Amount: ")+_claimPaymentOld.CheckAmt.ToString("c")+", "
							+Lan.g(this,"New Amount: ")+ClaimPaymentCur.CheckAmt.ToString("c")+", "
							+Lan.g(this,"Check Date: ")+ClaimPaymentCur.CheckDate.ToShortDateString()+", "//Date the check is entered in the system
							+"ClaimPaymentNum: "+ClaimPaymentCur.ClaimPaymentNum);//Column name, not translated.
					}
				}
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			ClaimProcs.SynchDateCP(ClaimPaymentCur.ClaimPaymentNum,ClaimPaymentCur.CheckDate);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			ClaimPaymentCur=_claimPaymentOld;
			DialogResult=DialogResult.Cancel;
		}
	}

}