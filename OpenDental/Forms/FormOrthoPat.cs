using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormOrthoPat:FormODBase {
		private PatPlan _patPlanCur;
		private InsPlan _insPlanCur;

		///<summary>This form does NOT take care of updating the patplan table for you. 
		///It passes around the patPlan you passed in by reference, so you can call update on the same patplan you passed in to update.</summary>
		public FormOrthoPat(PatPlan patPlanCur,InsPlan insPlanCur, string carrierName, string subID, double defaultFee) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patPlanCur=patPlanCur;
			_insPlanCur=insPlanCur;
			Patient patCur=Patients.GetLim(patPlanCur.PatNum);
			textPatient.Text=patCur.GetNameLF();
			textCarrier.Text = carrierName;
			textSubID.Text = subID;
			if(patPlanCur.OrthoAutoFeeBilledOverride==-1) {
				checkUseDefaultFee.Checked=true;
				textFee.ReadOnly=true;
				textFee.Text=defaultFee.ToString();
			}
			else {
				checkUseDefaultFee.Checked=false;
				textFee.ReadOnly=false;
				textFee.Text=patPlanCur.OrthoAutoFeeBilledOverride.ToString();
			}
			if(patPlanCur.OrthoAutoNextClaimDate.Date != DateTime.MinValue.Date) {
				textDateNextClaim.Text=patPlanCur.OrthoAutoNextClaimDate.ToShortDateString();
			}
			else { //there is no initial procedure or claim, so showing when the "next" claim will be would be misleading.
				textDateNextClaim.Text="";
			}
		}

		private void checkUseDefaultFee_CheckedChanged(object sender,EventArgs e) {
			textFee.ReadOnly=checkUseDefaultFee.Checked;
			if(checkUseDefaultFee.Checked) {
				textFee.Text=_insPlanCur.OrthoAutoFeeBilled.ToString();
			}
		}

		private void textDateNextClaim_Validated(object sender,EventArgs e) {
			try {
				if(PIn.Date(textDateNextClaim.Text).Day > 1) {
					DateTime firstOfMonth=new DateTime(PIn.Date(textDateNextClaim.Text).Year, PIn.Date(textDateNextClaim.Text).Month,1);
					textDateNextClaim.Text = firstOfMonth.ToShortDateString();
				}
			}
			catch {
				//do nothing
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textFee.IsValid()) {
				MsgBox.Show(this,"Please enter a valid fee.");
				return;
			}
			if(!textDateNextClaim.IsValid()) {
				MsgBox.Show(this,"Please enter a valid date.");
				return;
			}
			if(checkUseDefaultFee.Checked) {
				_patPlanCur.OrthoAutoFeeBilledOverride = -1;
			}
			else {
				_patPlanCur.OrthoAutoFeeBilledOverride = PIn.Double(textFee.Text);
			}
			if(textDateNextClaim.Visible) {
				_patPlanCur.OrthoAutoNextClaimDate = PIn.Date(textDateNextClaim.Text); //MinValue if blank.
			}
			DialogResult=DialogResult.OK;
		}
	}
}