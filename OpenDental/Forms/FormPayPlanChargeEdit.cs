/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormPayPlanChargeEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary>If the user deletes the PayPlanChargeCur, then it will be null when the form is closed.</summary>
		public PayPlanCharge PayPlanChargeCur;
		/// <summary>Used to check if there is an APR with the PayPlanCharge</summary>
		private PayPlan _payPlanCur;

		///<summary></summary>
		public FormPayPlanChargeEdit(PayPlanCharge payPlanCharge,PayPlan payPlan){
			InitializeComponent();
			InitializeLayoutManager();
			PayPlanChargeCur=payPlanCharge;
            _payPlanCur=payPlan;
			Lan.F(this);
			if(PayPlanChargeCur.Principal < 0) {//only adjustments are allowed to be negative. 
				textPrincipal.MinVal=-100000000;
				textPrincipal.MaxVal=0;
				textInterest.Visible=false;
				labelInterest.Visible=false;
			}
		}

		private void FormPayPlanCharge_Load(object sender, System.EventArgs e) {
			textDate.Text=PayPlanChargeCur.ChargeDate.ToShortDateString();
			//comboProvNum.Items.Clear();
			//for(int i=0;i<ProviderC.List.Length;i++) {
			//	comboProvNum.Items.Add(ProviderC.List[i].Abbr);
			//	if(ProviderC.List[i].ProvNum==PayPlanChargeCur.ProvNum)
			//		comboProvNum.SelectedIndex=i;
			//}
			textPrincipal.Text=PayPlanChargeCur.Principal.ToString("n");
			textInterest.Text=PayPlanChargeCur.Interest.ToString("n");
			textNote.Text=PayPlanChargeCur.Note;
			textProv.Text=Providers.GetAbbr(PayPlanChargeCur.ProvNum);
			if(PayPlanChargeCur.SecDateTEntry==DateTime.MinValue) {
				//First time form is ever opened and the date isn't saved to the db yet, show the current datetime
				textDateEntry.Text=POut.DateT(DateTime.Now,false);
			}
			else {
				//Returning to the form, pull the stored datetime
				textDateEntry.Text=POut.DateT(PayPlanChargeCur.SecDateTEntry,false);
			}
			if(PayPlanChargeCur.SecDateTEdit==DateTime.MinValue) {
				//Until job B15806 is complete, regaurding how MySQL 5.7 handles timestamps, SectDateTEdit will always show as MinValue for the user.
				textDateEdit.Text="";
			}
			else {
				//Edits exist, show stored datetime
				textDateEdit.Text=POut.DateT(PayPlanChargeCur.SecDateTEdit,false);
			}
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
				textClinic.Visible=false;
			}
			else {
				textClinic.Text=Clinics.GetAbbr(PayPlanChargeCur.ClinicNum);
			}
			//Do not let the user edit certain fields when APR is present.
			if(!CompareDouble.IsZero(_payPlanCur.APR)) {
				textPrincipal.ReadOnly=true;
				if(Security.IsAuthorized(Permissions.PayPlanChargeDateEdit,true)) {
					string logText=Lan.g(this,(_payPlanCur.PlanNum == 0 ? "Patient" : "Insurance") + " Payment Plan Charge with amount due of")
						+" " + PayPlanChargeCur.Principal.ToString("C") + " "+ Lan.g(this,"was viewed.");
					SecurityLogs.MakeLogEntry(Permissions.PayPlanChargeDateEdit,_payPlanCur.PatNum,logText,_payPlanCur.PayPlanNum,DateTime.MinValue);
				}
				else {//User does not have the PayPlanChargeDateEdit permission
					//There is no need to make a security log entry, but the user should not be allowed to edit the Date field.
					textDate.ReadOnly=true;
				}
			}
			Plugins.HookAddCode(this,"FormPayPlanChargeEdit.Load_end");
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(PayPlanChargeCur.Principal<0//Payment plan adjustments are always negative
				&& !textPrincipal.IsValid())//This will ensure number is negative due to defined range.
			{
				MsgBox.Show(this,"Adjustments must be negative.");
				return;
			}
			if(!textDate.IsValid()
				|| !textPrincipal.IsValid()
				|| !textInterest.IsValid())
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			//if(comboProvNum.SelectedIndex==-1){
			//	MsgBox.Show(this,"Please select a provider first.");
			//	return;
			//}
			if(textPrincipal.Text==""){
				textPrincipal.Text="0";
			}
			if(textInterest.Text==""){
				textInterest.Text="0";
			}
			//todo: test dates?  The day of the month should be the same as all others
			PayPlanChargeCur.ChargeDate=PIn.Date(textDate.Text);
			PayPlanChargeCur.Principal=PIn.Double(textPrincipal.Text);
			PayPlanChargeCur.Interest=PIn.Double(textInterest.Text);
			PayPlanChargeCur.Note=textNote.Text;
			//not allowed to change provnum or clinicNum here.
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				DialogResult=DialogResult.OK;
				PayPlanChargeCur=null;//Setting this null so we know to get rid of it when the form closes. 
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		
	}

	
}
