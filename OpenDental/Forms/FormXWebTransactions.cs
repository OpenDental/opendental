using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.Shared.XWeb;
using PayConnectService=OpenDentBusiness.PayConnectService;

namespace OpenDental {
	public partial class FormXWebTransactions:FormODBase {
		///<summary>The XWeb and PayConnect transactions for the selected date range and clinics.</summary>
		private DataTable _tableTrans;
		///<summary>The list of clinics available to the current user.</summary>
		private List<Clinic> _listClinics;

		public FormXWebTransactions() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormXWebTransactions_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				FillClinics();
			}
			else {
				comboClinic.Visible=false;
				labelClinic.Visible=false;
			}
			textDateFrom.Text=DateTime.Today.ToShortDateString();
			textDateTo.Text=DateTime.Today.ToShortDateString();
			FillGrid();
		}

		///<summary>Fills the clinics combo box with the clincs available to this user.</summary>
		private void FillClinics() {
			_listClinics=Clinics.GetForUserod(Security.CurUser);
			comboClinic.Items.Add(Lan.g(this,"All"));
			comboClinic.SelectedIndex=0;
			int offset=1;
			if(!Security.CurUser.ClinicIsRestricted) {
				comboClinic.Items.Add(Lan.g(this,"Unassigned"));
				offset++;
			}
			for(int i=0;i<_listClinics.Count;i++) {
				comboClinic.Items.Add(_listClinics[i].Abbr);
			}
			comboClinic.SelectedIndex=_listClinics.FindIndex(x => x.ClinicNum==Clinics.ClinicNum)+offset;
			if(comboClinic.SelectedIndex-offset<0) {
				comboClinic.SelectedIndex=0;
			}
		}

		private void FillGrid() {
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled && comboClinic.SelectedIndex!=0) {//Not 'All' selected
				if(Security.CurUser.ClinicIsRestricted) {
					listClinicNums.Add(_listClinics[comboClinic.SelectedIndex-1].ClinicNum);//Minus 1 for 'All'
				}
				else {
					if(comboClinic.SelectedIndex==1) {//'Unassigned' selected
						listClinicNums.Add(0);
					}
					else if(comboClinic.SelectedIndex>1) {
						listClinicNums.Add(_listClinics[comboClinic.SelectedIndex-2].ClinicNum);//Minus 2 for 'All' and 'Unassigned'
					}
				}
			}
			else {
				//Send an empty list of clinics to get all transactions
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			_tableTrans=XWebResponses.GetApprovedTransactions(listClinicNums,dateFrom,dateTo);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),60,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Date"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Tran Type"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Card Number"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Expiration"),70);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),100);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Transaction ID"),110);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableTrans.Rows.Count;i++) {
				bool isXWeb=IsXWebTransaction(i); //Only other option at the moment is PayConnect. This will need to be refactored if we add more payment options
				row=new GridRow();
				row.Cells.Add(_tableTrans.Rows[i]["Patient"].ToString());
				row.Cells.Add(PIn.Double(_tableTrans.Rows[i]["Amount"].ToString()).ToString("f"));
				row.Cells.Add(PIn.Date(_tableTrans.Rows[i]["DateTUpdate"].ToString()).ToShortDateString());
				if(isXWeb) {
					XWebTransactionStatus tranStatus=(XWebTransactionStatus)PIn.Int(_tableTrans.Rows[i]["TransactionStatus"].ToString());
					row.Cells.Add(GetXWebTranTypeByStatus(tranStatus));
				}
				else {
					//This is actually the PayConnectResponseWeb.TransType
					row.Cells.Add(_tableTrans.Rows[i]["TransactionStatus"].ToString());
				}
				row.Cells.Add(_tableTrans.Rows[i]["MaskedAcctNum"].ToString());
				row.Cells.Add(_tableTrans.Rows[i]["ExpDate"].ToString());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(_tableTrans.Rows[i]["Clinic"].ToString());
				}
				row.Cells.Add(_tableTrans.Rows[i]["TransactionID"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private string GetXWebTranTypeByStatus(XWebTransactionStatus status) {
			string strTranStatus;
			switch(status) {
				case XWebTransactionStatus.DtgPaymentApproved:
				case XWebTransactionStatus.HpfCompletePaymentApproved:
				case XWebTransactionStatus.HpfCompletePaymentApprovedPartial:
				case XWebTransactionStatus.EdgeExpressCompletePaymentApproved:
				case XWebTransactionStatus.EdgeExpressCompletePaymentApprovedPartial:
					strTranStatus="Sale";
					break;
				case XWebTransactionStatus.DtgPaymentReturned:
					strTranStatus="Return";
					break;
				case XWebTransactionStatus.DtgPaymentVoided:
					strTranStatus="Void";
					break;
				default://These other values should not be returned from the query.
					strTranStatus=status.ToString();
					break;
			}
			return strTranStatus;
		}

		private bool IsXWebTransaction(int selectedIndex) {
			return PIn.Int(_tableTrans.Rows[selectedIndex]["isXWeb"].ToString())==1;
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(textDateFrom.Text==""
				|| textDateTo.Text==""
				|| !textDateFrom.IsValid()
				|| !textDateTo.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			FillGrid();
		}

		private void gridMain_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right) {
				gridMain.SetAll(false);
			}
		}

		private void contextMenu_Opening(object sender,System.ComponentModel.CancelEventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				e.Cancel=true;
				return;
			}
			try {
				SetContextMenuItemVisibility();
			}
			catch(Exception ex) {
				MsgBox.Show("An error occurred: "+ex.Message);
				e.Cancel=true;
			}
		}

		private void SetContextMenuItemVisibility() {
			int idxSelected=gridMain.GetSelectedIndex();
			if(idxSelected<0) {
				return;
			}
			openPaymentToolStripMenuItem.Visible=PIn.Bool(_tableTrans.Rows[idxSelected]["doesPaymentExist"].ToString());
			voidPaymentToolStripMenuItem.Visible=false;
			processReturnToolStripMenuItem.Visible=false;
			if(IsXWebTransaction(idxSelected)) {
				switch((XWebTransactionStatus)PIn.Int(_tableTrans.Rows[idxSelected]["TransactionStatus"].ToString())) {
					case XWebTransactionStatus.DtgPaymentApproved:
					case XWebTransactionStatus.HpfCompletePaymentApproved:
					case XWebTransactionStatus.HpfCompletePaymentApprovedPartial:
					case XWebTransactionStatus.DtgPaymentReturned:
					case XWebTransactionStatus.EdgeExpressCompletePaymentApproved:
					case XWebTransactionStatus.EdgeExpressCompletePaymentApprovedPartial:
						voidPaymentToolStripMenuItem.Visible=true;
						processReturnToolStripMenuItem.Visible=true;
						break;
				}
				return;
			}
			switch(PIn.String(_tableTrans.Rows[idxSelected]["TransactionStatus"].ToString())) {
				case "SALE":
					voidPaymentToolStripMenuItem.Visible=true;
					processReturnToolStripMenuItem.Visible=true;
					break;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row<0 || !Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			long patNum=PIn.Long(_tableTrans.Rows[e.Row]["PatNum"].ToString());
			GotoModule.GotoAccount(patNum);
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1 || !Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			long patNum=PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["PatNum"].ToString());
			GotoModule.GotoAccount(patNum);
		}

		private void openPaymentToolStripMenuItem_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			Payment payment=Payments.GetPayment(PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["PaymentNum"].ToString()));
			if(payment==null) {//The payment has been deleted
				MsgBox.Show(this,"This payment no longer exists.");
				return;
			}
			Patient patient=Patients.GetPat(payment.PatNum);
			Family family=Patients.GetFamily(patient.PatNum);
			using FormPayment formPayment=new FormPayment(patient,family,payment,false);
			formPayment.ShowDialog();
			FillGrid();
		}

		private void voidPaymentToolStripMenuItem_Click(object sender,EventArgs e) {
			//A new payment is being created upon clicking this menu item. The payment date is set to "DateTime.Now" in 
			//PayConnectL.VoidOrRefundPayConnectPortalTransaction(...) and in XWebs.VoidPayment paynote
			if(!Security.IsAuthorized(Permissions.PaymentCreate,DateTime.Today)) {
				return;
			}
			if(gridMain.SelectedIndices.Length<1
				|| !MsgBox.Show(this,MsgBoxButtons.YesNo,"Void this payment?"))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(IsXWebTransaction(gridMain.SelectedIndices[0])) {
				long patNum=PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["PatNum"].ToString());
				long responseNum=PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["ResponseNum"].ToString());
				string payNote=Lan.g(this,"Void XWeb payment made from within Open Dental")+"\r\n"
					+Lan.g(this,"Amount:")+" "+PIn.Double(_tableTrans.Rows[gridMain.SelectedIndices[0]]["Amount"].ToString()).ToString("f")+"\r\n"
					+Lan.g(this,"Transaction ID:")+" "+_tableTrans.Rows[gridMain.SelectedIndices[0]]["TransactionID"].ToString()+"\r\n"
					+Lan.g(this,"Card Number:")+" "+_tableTrans.Rows[gridMain.SelectedIndices[0]]["MaskedAcctNum"].ToString()+"\r\n"
					+Lan.g(this,"Processed:")+" "+DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
				try {
					XWebs.VoidPayment(patNum,payNote,responseNum); 
				}
				catch(ODException ex) {
					Cursor=Cursors.Default;
					MessageBox.Show(ex.Message);
					return;
				}
			}
			else {
				Payment payment=Payments.GetPayment(PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["PaymentNum"].ToString()));
				PayConnectResponseWeb payConnectResponseWeb=PayConnectResponseWebs.GetOne(PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["ResponseNum"].ToString()));
				decimal amt=PIn.Decimal(_tableTrans.Rows[gridMain.SelectedIndices[0]]["Amount"].ToString());
				string refNum=_tableTrans.Rows[gridMain.SelectedIndices[0]]["TransactionID"].ToString(); //This is actually PayConnectResponseWeb.RefNumber, it's just stored in the TransactionID column
				if(!PayConnectL.VoidOrRefundPayConnectPortalTransaction(payConnectResponseWeb,payment,PayConnectService.transType.VOID,refNum,amt)) {
					Cursor=Cursors.Default;
					return;
				}
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Void successful");
			FillGrid();
		}

		private void processReturnToolStripMenuItem_Click(object sender,EventArgs e) {
			//using DateTime.Today because this process will create a new payment (refund)
			if(!Security.IsAuthorized(Permissions.PaymentCreate,DateTime.Today)) {
				return;
			}
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			Payment payment=Payments.GetPayment(PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["PaymentNum"].ToString()));
			if(IsXWebTransaction(gridMain.SelectedIndices[0])) {
				long patNum=PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["PatNum"].ToString());
				string alias=_tableTrans.Rows[gridMain.SelectedIndices[0]]["Alias"].ToString();
				List<CreditCard> listCreditCards=CreditCards.GetCardsByToken(alias,
					new List<CreditCardSource> { CreditCardSource.XWeb, CreditCardSource.XWebPortalLogin });
				if(listCreditCards.Count==0) {
					MsgBox.Show(this,"This credit card is no longer stored in the database. Return cannot be processed.");
					return;
				}
				if(listCreditCards.Count>1) {
					MsgBox.Show(this,"There is more than one card in the database with this token. Return cannot be processed due to the risk of charging the "+
						"incorrect card.");
					return;
				}
				double amt=PIn.Double(_tableTrans.Rows[gridMain.SelectedIndices[0]]["Amount"].ToString());
				using FormXWeb formXWeb=new FormXWeb(patNum,listCreditCards.FirstOrDefault(),XWebTransactionType.CreditReturnTransaction,createPayment:false,amt);
				formXWeb.LockCardInfo=true;
				if(formXWeb.ShowDialog()==DialogResult.OK) {
					Payment paymentReturn=Payments.InsertReturnXWebPayment(payment,formXWeb.XWebResponse_.GetFormattedNote(false),(-formXWeb.XWebResponse_.Amount));
					formXWeb.XWebResponse_.PaymentNum=paymentReturn.PayNum;
					XWebResponses.Update(formXWeb.XWebResponse_);
					SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,paymentReturn.PatNum,
						Patients.GetLim(paymentReturn.PatNum).GetNameLF() + ", " + paymentReturn.PayAmt.ToString("c"));
					FillGrid();
				}
				return;
			}
			PayConnectResponseWeb payConnectResponseWeb=PayConnectResponseWebs.GetOne(PIn.Long(_tableTrans.Rows[gridMain.SelectedIndices[0]]["ResponseNum"].ToString()));
			decimal amount=PIn.Decimal(_tableTrans.Rows[gridMain.SelectedIndices[0]]["Amount"].ToString());
			string refNum=_tableTrans.Rows[gridMain.SelectedIndices[0]]["TransactionID"].ToString(); //This is actually PayConnectResponseWeb.RefNumber, it's just stored in the TransactionID column
			if(!PayConnectL.VoidOrRefundPayConnectPortalTransaction(payConnectResponseWeb,payment,PayConnectService.transType.RETURN,refNum,amount)) {
				return;
			}
			MsgBox.Show("Return successful.");
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}