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
using System.Collections.Generic;
using System.Linq;

namespace OpenDental{
	///<summary>Form that displays the editable fields of a payplan charge.</summary>
	public partial class FormPayPlanChargeEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		private PayPlanCharge _payPlanChargeCurOld;
		///<summary>If the user deletes the PayPlanChargeCur, then it will be null when the form is closed.</summary>
		public PayPlanCharge PayPlanChargeCur;
		///<summary>Data object used to determine if charge edits are valid. Used in save logic.</summary>
		private DynamicPaymentPlanModuleData _dynamicPaymentPlanModuleData;
		/// <summary>Used to check if there is an APR with the PayPlanCharge</summary>
		private PayPlan _payPlan;
		///<summary>List of fields that were updated from the edit.</summary>
		public List<string> ListChangeLog;

		///<summary></summary>
		public FormPayPlanChargeEdit(PayPlanCharge payPlanCharge,PayPlan payPlan,DynamicPaymentPlanModuleData dynamicPaymentPlanModuleData=null){
			InitializeComponent();
			InitializeLayoutManager();
			PayPlanChargeCur=payPlanCharge;
			_payPlanChargeCurOld=payPlanCharge.Copy();
			_payPlan=payPlan;
			Lan.F(this);
			if(PayPlanChargeCur.Principal < 0) {//only adjustments are allowed to be negative. 
				textPrincipal.MinVal=-100000000;
				textPrincipal.MaxVal=0;
				textInterest.Visible=false;
				labelInterest.Visible=false;
			}
			if(payPlan.IsDynamic && dynamicPaymentPlanModuleData==null){
				dynamicPaymentPlanModuleData=PayPlanEdit.GetDynamicPaymentPlanModuleData(payPlan);
			}
			_dynamicPaymentPlanModuleData=dynamicPaymentPlanModuleData;
		}

		private void FormPayPlanCharge_Load(object sender, System.EventArgs e) {
			textDate.Text=PayPlanChargeCur.ChargeDate.ToShortDateString();
			textPrincipal.Text=PayPlanChargeCur.Principal.ToString("n");
			textInterest.Text=PayPlanChargeCur.Interest.ToString("n");
			textNote.Text=PayPlanChargeCur.Note;
			if(PrefC.HasClinicsEnabled) {
				comboBoxClinic.ClinicNumSelected=PayPlanChargeCur.ClinicNum;
			}
			if(_payPlan.IsDynamic) {
				comboBoxClinic.Enabled=true;
				textInterest.ReadOnly=false;
			}
			FillComboProv();
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
			//Do not let the user edit certain fields when APR is present.
			if(!CompareDouble.IsZero(_payPlan.APR)) {
				textPrincipal.ReadOnly=!_payPlan.IsDynamic; //Dynamic payment plan charges should still be able to alter their principal for now.
				if(Security.IsAuthorized(EnumPermType.PayPlanChargeDateEdit,true)) {
					string strPlanType=Lan.g(this,"Patient ");
					if(_payPlan.PlanNum > 0) {
						strPlanType=Lan.g(this,"Insurance ");
					}
					if(_payPlan.IsDynamic) {
						strPlanType="";
					}
					string logText=strPlanType+Lan.g(this,"Payment Plan Charge with amount due of")
						+" " + PayPlanChargeCur.Principal.ToString("C") + " "+ Lan.g(this,"was viewed.");
					SecurityLogs.MakeLogEntry(EnumPermType.PayPlanChargeDateEdit,_payPlan.PatNum,logText,_payPlan.PayPlanNum,DateTime.MinValue);
				}
				else {//User does not have the PayPlanChargeDateEdit permission
					//There is no need to make a security log entry, but the user should not be allowed to edit the Date field.
					textDate.ReadOnly=true;
				}
			}
			Plugins.HookAddCode(this,"FormPayPlanChargeEdit.Load_end");
		}

		private void FillComboProv() {
			comboBoxProv.Items.Clear();
			if(_payPlan.IsDynamic) {
				comboBoxProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboBoxClinic.ClinicNumSelected));
				comboBoxProv.SetSelectedProvNum(PayPlanChargeCur.ProvNum);
				comboBoxProv.Enabled=true;
				return;
			}
			List<Provider> listProviders=new List<Provider>{Providers.GetProvFromDb(PayPlanChargeCur.ProvNum)};
			comboBoxProv.Items.AddProvsAbbr(listProviders);
			comboBoxProv.SelectedIndex=0;
		}

		private void comboBoxClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillComboProv();
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textDate.IsValid()
				|| !textPrincipal.IsValid()
				|| !textInterest.IsValid())
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(textNote.Text.Contains("Down Payment")) {
				MessageBox.Show(Lan.g(this,"The phrase 'Down Payment' cannot be used in the notes."));
				return;
			}
			DateTime chargeDate=PIn.Date(textDate.Text);
			double principal=PIn.Double(textPrincipal.Text);
			double interest=PIn.Double(textInterest.Text);
			if(_payPlan.IsDynamic) {
				if(PayPlanChargeCur.IsDebitAdjustment && principal<0) {
					MsgBox.Show(this,"Payment plan adjustments cannot have negative principal.");
					return;
				}
				bool isEstimatedChargeDate=_dynamicPaymentPlanModuleData.ListPayPlanChargesExpected.Select(x=>x.ChargeDate).Contains(chargeDate);
				if(isEstimatedChargeDate && chargeDate!=_payPlanChargeCurOld.ChargeDate) {
					MsgBox.Show(this,"This charge date is the same as a future charge date. This will prevent the future charge from being issued","Warning");
				}
				//Dynamic Payment Plan charges always link to production, if there is a charge there must be an entry in this list.
				PayPlanProductionEntry payPlanProductionEntry=_dynamicPaymentPlanModuleData.ListPayPlanProductionEntries
					.First(x=>x.PriKey==PayPlanChargeCur.FKey && x.LinkType==PayPlanChargeCur.LinkType);
				double remainingProd=((double)payPlanProductionEntry.AmountRemaining);
				double amountCharged=_dynamicPaymentPlanModuleData.ListPayPlanChargesExpected
					.FindAll(x=>x.PayPlanChargeNum!=0 && x.PayPlanChargeNum!=_payPlanChargeCurOld.PayPlanChargeNum && x.FKey==PayPlanChargeCur.FKey && x.LinkType==PayPlanChargeCur.LinkType)
					.Sum(x=>x.Principal);
				remainingProd-=amountCharged;
				if(principal > remainingProd) {
					string msgString=Lans.g(this,"Principal cannot be greater than the remaining production value of")+ " " + remainingProd.ToString("c") + ".";
					MsgBox.Show(msgString);
					return;
				}
			}
			//Charge Date, Note, and Provider changed.
			ListChangeLog=new List<string>();
			if(PayPlanChargeCur.ChargeDate!=chargeDate) {
				ListChangeLog.Add("Charge Date");
			}
			if(PayPlanChargeCur.Principal!=principal) {
				ListChangeLog.Add("Principal");
			}
			if(PayPlanChargeCur.Interest!=interest) {
				ListChangeLog.Add("Interest");
			}
			if(PayPlanChargeCur.Note!=textNote.Text) {
				ListChangeLog.Add("Note");
			}
			if(PayPlanChargeCur.ProvNum!=comboBoxProv.GetSelectedProvNum()) {
				ListChangeLog.Add("Provider");
			}
			if(PayPlanChargeCur.ClinicNum!=comboBoxClinic.ClinicNumSelected) {
				ListChangeLog.Add("Clinic");
			}
			PayPlanChargeCur.ChargeDate=chargeDate;
			PayPlanChargeCur.Principal=principal;
			PayPlanChargeCur.Interest=interest;
			PayPlanChargeCur.Note=textNote.Text;
			//Patient payment plans are not allowed to change provnum or clinicNum here.
			if(_payPlan.IsDynamic) {
				PayPlanChargeCur.ProvNum=comboBoxProv.GetSelectedProvNum();
				PayPlanChargeCur.ClinicNum=comboBoxClinic.ClinicNumSelected;
			}
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//All payment plan types store their debits within the payplancharge table.
			//It is never okay to delete a payment plan charge that has a payment (paysplit) associated to it.
			List<PayPlanCharge> listPayPlanChargesNotDeleted=PayPlanCharges.DeleteDebitsWithoutPayments(new List<PayPlanCharge>{ PayPlanChargeCur },doDelete:false);
			if(listPayPlanChargesNotDeleted.Count > 0) {
				string msgString="Cannot delete";
				if(listPayPlanChargesNotDeleted.Exists(x=>x.Note.ToLower().Contains("down payment"))){
					msgString+=" down payment charges, or";
				}
				msgString+=" charges with payments attached.";
				MsgBox.Show(Lans.g(this,msgString));
				return;
			}
			if(PayPlanChargeCur.IsNew || IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			DialogResult=DialogResult.OK;
			PayPlanChargeCur=null;//Setting this null so we know to get rid of it when the form closes. 
		}

	}
}