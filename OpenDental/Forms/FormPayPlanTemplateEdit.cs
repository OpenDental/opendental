using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormPayPlanTemplateEdit:FormODBase {
		private PayPlanTemplate _payPlanTemplate;

		public FormPayPlanTemplateEdit(PayPlanTemplate payPlanTemplate) {
			InitializeComponent();
			InitializeLayoutManager();
			if(payPlanTemplate==null) {
				_payPlanTemplate=new PayPlanTemplate() { IsNew=true };
				_payPlanTemplate.ChargeFrequency=PayPlanFrequency.Monthly;//Setting the charge frequency to monthly by default.
			}
			else {
				_payPlanTemplate=payPlanTemplate;
			}
			Lan.F(this);
		}

		private void FormPayPlanTemplateEdit_Load(object sender,EventArgs e) {
			checkHidden.Checked=_payPlanTemplate.IsHidden;
			if(_payPlanTemplate.PayPlanTemplateName!=null) {
				textTemplateName.Text=_payPlanTemplate.PayPlanTemplateName;
			}
			long curUserNum=Security.CurUser.UserNum;
			List<UserClinic> listUserClinics=UserClinics.GetForUser(curUserNum);
			if(_payPlanTemplate.ClinicNum==0 && listUserClinics.Count>0) {
				comboBoxClinic.ClinicNumSelected=listUserClinics[0].ClinicNum;
			}
			else {
				comboBoxClinic.ClinicNumSelected=_payPlanTemplate.ClinicNum;
			}
			textAPR.Text=_payPlanTemplate.APR.ToString();
			ToggleInterestDelayFieldsHelper();
			textInterestDelay.Text=_payPlanTemplate.InterestDelay.ToString();
			textPeriodPayment.Text=_payPlanTemplate.PayAmt.ToString("f");
			if(_payPlanTemplate.NumberOfPayments>0) {
				textPaymentCount.Text=_payPlanTemplate.NumberOfPayments.ToString();
				textPeriodPayment.Text="";
			}
			textDownPayment.Text=_payPlanTemplate.DownPayment.ToString("f");
			switch(_payPlanTemplate.ChargeFrequency) {
				case PayPlanFrequency.Weekly:
					radioWeekly.Checked=true;
					break;
				case PayPlanFrequency.EveryOtherWeek:
					radioEveryOtherWeek.Checked=true;
					break;
				case PayPlanFrequency.OrdinalWeekday:
					radioOrdinalWeekday.Checked=true;
					break;
				case PayPlanFrequency.Monthly:
					radioMonthly.Checked=true;
					break;
				case PayPlanFrequency.Quarterly:
					radioQuarterly.Checked=true;
					break;
				default://default to monthly for new plans (should be 0 and do this regardless)
					radioMonthly.Checked=true;
					break;
			}
			switch(_payPlanTemplate.DynamicPayPlanTPOption) {
				case DynamicPayPlanTPOptions.TreatAsComplete:
					radioTpTreatAsComplete.Checked=true;
					break;
				case DynamicPayPlanTPOptions.AwaitComplete:
				default:
					radioTpAwaitComplete.Checked=true;
					break;
			}
		}

		private void ToggleInterestDelayFieldsHelper() {
			bool areVisible=true;
			if(CompareDouble.IsZero(PIn.Double(textAPR.Text))) {
				textInterestDelay.Text="";
				areVisible=false;
			}
			textInterestDelay.Visible=areVisible;
			labelInterestDelay1.Visible=areVisible;
			labelInterestDelay2.Visible=areVisible;
		}

		private void textAPR_TextChanged(object sender,EventArgs e) {
			ToggleInterestDelayFieldsHelper();
		}

		private void textPaymentCount_TextChanged(object sender,EventArgs e) {
			if(PIn.Double(textPaymentCount.Text)>0) {
				textPeriodPayment.Text="";
			} 
		}

		private string ValidateUI() {
			//Validate UI fields.
			StringBuilder stringBuilder=new StringBuilder();
			if(!textDownPayment.IsValid()
				|| !textAPR.IsValid()
				|| !textInterestDelay.IsValid()
				|| !textPeriodPayment.IsValid()
				|| !textPaymentCount.IsValid())
			{
				stringBuilder.AppendLine(Lan.g(this,"Please fix data entry errors first."));
			}
			return stringBuilder.ToString();
		}
	
		///<summary>Validates the template</summary>
		private bool ValidateTemplate() {
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.Append(ValidateUI());
			if(textTemplateName.Text=="") {
				stringBuilder.AppendLine(Lan.g(this,"Please enter a name first."));
			}
			if(String.IsNullOrWhiteSpace(textPaymentCount.Text) && String.IsNullOrWhiteSpace(textPeriodPayment.Text)) {
				stringBuilder.AppendLine(Lan.g(this,"Please add values to either Payment Amount or Number of Payments."));
			}
			if(!String.IsNullOrWhiteSpace(textPaymentCount.Text) && !String.IsNullOrWhiteSpace(textPeriodPayment.Text)) {
				stringBuilder.AppendLine(Lan.g(this,"You can not have values for both Payment Amount and Number of Payments."));
			}
			if(!String.IsNullOrWhiteSpace(stringBuilder.ToString())) {
				MessageBox.Show(stringBuilder.ToString());
				return false;
			}
			return true;
		}

		private PayPlanFrequency GetChargeFrequency() {
			if(radioWeekly.Checked) {
				return PayPlanFrequency.Weekly;
			}
			if(radioEveryOtherWeek.Checked) {
				return PayPlanFrequency.EveryOtherWeek;
			}
			if(radioOrdinalWeekday.Checked) {
				return PayPlanFrequency.OrdinalWeekday;
			}
			if(radioMonthly.Checked) {
				return PayPlanFrequency.Monthly;
			}
			return PayPlanFrequency.Quarterly;
		}

		private DynamicPayPlanTPOptions GetSelectedTreatmentPlannedOption() {
			if(radioTpAwaitComplete.Checked) {
				return DynamicPayPlanTPOptions.AwaitComplete;
			}
			if(radioTpTreatAsComplete.Checked) {
				return DynamicPayPlanTPOptions.TreatAsComplete;
			}
			return DynamicPayPlanTPOptions.None;//Should never be hit
		}

		private void butSave_Click(object sender,EventArgs e) {
			//Validate before saving to DB.
			if(!ValidateTemplate()) {
				return;
			}
			_payPlanTemplate.PayPlanTemplateName=textTemplateName.Text;
			if(PrefC.HasClinicsEnabled) {
				_payPlanTemplate.ClinicNum=comboBoxClinic.GetSelectedClinic().ClinicNum;
			}
			_payPlanTemplate.APR=PIn.Double(textAPR.Text);
			_payPlanTemplate.InterestDelay=PIn.Int(textInterestDelay.Text);
			_payPlanTemplate.PayAmt=PIn.Double(textPeriodPayment.Text);
			_payPlanTemplate.NumberOfPayments=PIn.Int(textPaymentCount.Text);
			_payPlanTemplate.ChargeFrequency=GetChargeFrequency();
			_payPlanTemplate.DownPayment=PIn.Double(textDownPayment.Text);
			_payPlanTemplate.DynamicPayPlanTPOption=GetSelectedTreatmentPlannedOption();
			_payPlanTemplate.IsHidden=checkHidden.Checked;
			if(_payPlanTemplate.IsNew) {
				PayPlanTemplates.Insert(_payPlanTemplate);
				DialogResult=DialogResult.OK;
				return;
			}
			PayPlanTemplates.Update(_payPlanTemplate);
			DialogResult=DialogResult.OK;
		}

		private void FormPayPlanTemplateEdit_CloseXClicked(object sender,System.ComponentModel.CancelEventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}