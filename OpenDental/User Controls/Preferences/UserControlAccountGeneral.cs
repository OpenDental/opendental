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
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlAccountGeneral() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void checkShowFamilyCommByDefault_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
		}

		///<summary>Clear the DateTime value which is used as the lock. Requires SecurityAdmin permission.</summary>
		private void butClearAgingBeginDateT_Click(object sender,EventArgs e) {
			//user doesn't have permission or the pref is already cleared
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin) || PrefC.GetDateT(PrefName.AgingBeginDateTime)==DateTime.MinValue) {//blank=DateTime.MinValue
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

		#region Methods - Event Handlers Sync
		private void TextAgingServiceTimeDue_Validating(object sender,CancelEventArgs e) {
			DateTime timeAgingRun=DateTime.MinValue;//only time component is used
			if(!string.IsNullOrWhiteSpace(textAgingServiceTimeDue.Text)
				&& !DateTime.TryParse(textAgingServiceTimeDue.Text,out timeAgingRun)) 
			{
				MsgBox.Show(this,"Automated Aging Run Time must be blank or a valid time of day.");
				return;
			}
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.AgingServiceTimeDue);
			if(timeAgingRun==DateTime.MinValue) {
				prefValSync.PrefVal="";
			}
			else {
				prefValSync.PrefVal=POut.DateT(timeAgingRun,false);
			}
			SyncChanged?.Invoke(this,new EventArgs());
		}

		#endregion Methods - Event Handlers Sync

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
			checkAllowFuturePayments.Checked=PrefC.GetBool(PrefName.AllowFutureInsPayments);
			checkAllowFutureDebits.Checked=PrefC.GetBool(PrefName.AccountAllowFutureDebits);
			DateTime agingBeginDateT=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(agingBeginDateT>DateTime.MinValue) {
				textAgingBeginDateT.Text=agingBeginDateT.ToString();
			}
			//DateTime dateAgingServiceTimeDue=PrefC.GetDateT(PrefName.AgingServiceTimeDue);
			//if(dateAgingServiceTimeDue!=DateTime.MinValue) {
			//	textAgingServiceTimeDue.Text=dateAgingServiceTimeDue.ToShortTimeString();
			//}
			#endregion Misc Account
		}

		public bool SaveAccountGeneral() {
			DateTime timeAgingRun=DateTime.MinValue;//only time component is used
			if(!string.IsNullOrWhiteSpace(textAgingServiceTimeDue.Text)
				&& !DateTime.TryParse(textAgingServiceTimeDue.Text,out timeAgingRun)) 
			{
				MsgBox.Show(this,"Automated Aging Run Time must be blank or a valid time of day.");
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.BalancesDontSubtractIns,checkBalancesDontSubtractIns.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowAccountFamilyCommEntries,checkShowFamilyCommByDefault.Checked);
			Changed|=Prefs.UpdateBool(PrefName.CommLogAutoSave,checkCommLogAutoSave.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AccountShowPaymentNums,checkAccountShowPaymentNums.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InvoicePaymentsGridShowNetProd,checkStatementInvoiceGridShowWriteoffs.Checked);
			Changed|=Prefs.UpdateBool(PrefName.FutureTransDatesAllowed,checkAllowFutureTrans.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AllowFutureInsPayments,checkAllowFuturePayments.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AccountAllowFutureDebits,checkAllowFutureDebits.Checked);
			//if(autoAgingRunTime==DateTime.MinValue) {
			//	Changed|=Prefs.UpdateString(PrefName.AgingServiceTimeDue,"");
			//}
			//else {
			//	Changed|=Prefs.UpdateString(PrefName.AgingServiceTimeDue,POut.DateT(autoAgingRunTime,false));
			//}
			return true;
		}

		public void FillSynced(){
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.AgingServiceTimeDue);
			DateTime dateAgingServiceTimeDue=PIn.DateT(prefValSync.PrefVal);
			if(dateAgingServiceTimeDue!=DateTime.MinValue) {
				textAgingServiceTimeDue.Text=dateAgingServiceTimeDue.ToShortTimeString();
			}
		}
		#endregion Methods - Public
	}
}
