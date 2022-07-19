using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlAccountGeneral:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlAccountGeneral() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butAgingProcLifoDetails_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"3 state checkbox. When indeterminate (filled), it behaves as unchecked. Recommended is checked. Adjustment and payment plan credits attached to a procedure "
			+"are summed by date. If the sum of the attached charges and credits results in a credit, the credit is applied to the balance of the procedure's aging category. Any remaining "
			+"credit is aged normally. This is a LIFO strategy.");
		}

		private void checkShowFamilyCommByDefault_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
		}

		///<summary>Clear the DateTime value which is used as the lock. Requires SecurityAdmin permission.</summary>
		private void butClearAgingBeginDateT_Click(object sender,EventArgs e) {
			//user doesn't have permission or the pref is already cleared
			if(!Security.IsAuthorized(Permissions.SecurityAdmin) || PrefC.GetDateT(PrefName.AgingBeginDateTime)==DateTime.MinValue) {//blank=DateTime.MinValue
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will override the lock on the famaging table, potentially allowing a second connection to start "
				+"the aging calculations which could cause both calculations to fail.  If this happens, this flag will need to be cleared again and aging "
				+"started by a single connection.\r\nAre you sure you want to clear this value?")) {
				return;
			}
			textAgingBeginDateT.Text="";
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");
			DataValid.SetInvalid(InvalidType.Prefs);
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillAccountGeneral() {
			#region Misc Account
			checkBalancesDontSubtractIns.Checked=PrefC.GetBool(PrefName.BalancesDontSubtractIns);
			checkAccountShowPaymentNums.Checked=PrefC.GetBool(PrefName.AccountShowPaymentNums);
			checkShowFamilyCommByDefault.Checked=PrefC.GetBool(PrefName.ShowAccountFamilyCommEntries);
			checkCommLogAutoSave.Checked=PrefC.GetBool(PrefName.CommLogAutoSave);
			checkStatementInvoiceGridShowWriteoffs.Checked=PrefC.GetBool(PrefName.InvoicePaymentsGridShowNetProd);
			checkAllowFutureTrans.Checked=PrefC.GetBool(PrefName.FutureTransDatesAllowed);
			checkAgingProcLifo.CheckState=PrefC.GetYNCheckState(PrefName.AgingProcLifo);
			DateTime agingBeginDateT=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(agingBeginDateT>DateTime.MinValue) {
				textAgingBeginDateT.Text=agingBeginDateT.ToString();
			}
			DateTime agingDateTDue=PrefC.GetDateT(PrefName.AgingServiceTimeDue);
			if(agingDateTDue!=DateTime.MinValue) {
				textAutoAgingRunTime.Text=agingDateTDue.ToShortTimeString();
			}
			#endregion Misc Account
		}

		public bool SaveAccountGeneral() {
			DateTime autoAgingRunTime=DateTime.MinValue;
			if(!string.IsNullOrWhiteSpace(textAutoAgingRunTime.Text)
				&& !DateTime.TryParse(textAutoAgingRunTime.Text,out autoAgingRunTime)) {
				MsgBox.Show(this,"Automated Aging Run Time must be blank or a valid time of day.");
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.BalancesDontSubtractIns,checkBalancesDontSubtractIns.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowAccountFamilyCommEntries,checkShowFamilyCommByDefault.Checked);
			Changed|=Prefs.UpdateBool(PrefName.CommLogAutoSave,checkCommLogAutoSave.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AccountShowPaymentNums,checkAccountShowPaymentNums.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InvoicePaymentsGridShowNetProd,checkStatementInvoiceGridShowWriteoffs.Checked);
			Changed|=Prefs.UpdateBool(PrefName.FutureTransDatesAllowed,checkAllowFutureTrans.Checked);
			Changed|=Prefs.UpdateYN(PrefName.AgingProcLifo,checkAgingProcLifo.CheckState);
			if(autoAgingRunTime==DateTime.MinValue) {
				Changed|=Prefs.UpdateString(PrefName.AgingServiceTimeDue,"");
			}
			else {
				Changed|=Prefs.UpdateString(PrefName.AgingServiceTimeDue,POut.DateT(autoAgingRunTime,false));
			}
			return true;
		}
		#endregion Methods - Public
	}
}
