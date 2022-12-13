using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormInsPayFix:FormODBase {
		public FormInsPayFix() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);

		}

		private void butRun_Click(object sender,EventArgs e) {
			List<ClaimPaySplit> listClaimPaySplits=Claims.GetInsPayNotAttachedForFixTool();
			if(listClaimPaySplits.Count==0) {
				MsgBox.Show(this,"There are currently no insurance payments that are not attached to an insurance check.");
				DialogResult=DialogResult.OK;//Close the window because there is nothing else to do
				return;
			}
			Cursor=Cursors.WaitCursor;
			string invalidClaimDate="";
			DateTime dateNow=MiscData.GetNowDateTime().Date;
			for(int i=0;i<listClaimPaySplits.Count;i++) {
				Claim claim=Claims.GetClaim(listClaimPaySplits[i].ClaimNum);
				if(claim==null) {
					continue;
				}
				if(claim.DateReceived.Date>dateNow && !PrefC.GetBool(PrefName.AllowFutureInsPayments) && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
					invalidClaimDate+="\r\n"+Lan.g(this,"PatNum")+" "+claim.PatNum+", "+claim.DateService.ToShortDateString();
					continue;
				}
				ClaimPayment claimPayment=new ClaimPayment();
				claimPayment.CheckDate=claim.DateReceived;
				claimPayment.CheckAmt=listClaimPaySplits[i].InsPayAmt;
				claimPayment.ClinicNum=claim.ClinicNum;
				claimPayment.CarrierName=listClaimPaySplits[i].Carrier;
				claimPayment.PayType=Defs.GetFirstForCategory(DefCat.InsurancePaymentType,true).DefNum;
				ClaimPayments.Insert(claimPayment);
				List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForClaim(listClaimPaySplits[i].ClaimNum);
				for(int j=0;j<listClaimProcs.Count;j++) {
					if(listClaimProcs[j].ClaimPaymentNum!=0 || listClaimProcs[j].InsPayAmt==0) { //If claimpayment already attached to claimproc or ins didn't pay.
						continue; //Do not change
					}
					listClaimProcs[j].DateCP=claim.DateReceived;
					listClaimProcs[j].ClaimPaymentNum=claimPayment.ClaimPaymentNum;
					ClaimProcs.Update(listClaimProcs[j]);
				}
			}
			Cursor=Cursors.Default;
			if(invalidClaimDate!="") {
				invalidClaimDate="\r\n"+Lan.g(this,"Cannot make future-dated insurance payments for these claims:")+invalidClaimDate;
			}
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g(this,"Insurance checks created:")+" "+listClaimPaySplits.Count+invalidClaimDate);
			msgBoxCopyPaste.ShowDialog();
			DialogResult=DialogResult.OK;//Close the window because there is nothing else to do
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}