using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using EdgeExpressProps = OpenDentBusiness.ProgramProperties.PropertyDescs.EdgeExpress;

namespace OpenDental {
	public partial class FormEdgeExpressTrans:FormODBase {
		public long ClinicNum;
		public bool DoShowSaveTokenBox=true;
		public EdgeExpressTransType TransactionType  { get; private set; }
		public decimal CashBackAmount  { get; private set; }
		public bool SaveToken { get; private set; }
		///<summary>Set on the way in based on the property value for the clinic on the payment.  Sets the initial checked state of checkSignature.
		///When the user presses OK, this value reflects the final state of checkSignature when the user hits OK.</summary>
		public bool PromptSignature { get; private set; }
		///<summary>Set on the way in based on the property value for the clinic on the payment.  Sets the initial checked state of checkPrintReceipt.
		///When the user presses OK, this value reflects the final state of checkPrintReceipt when the user hits OK.</summary>
		public bool PrintReceipt { get; private set; }
		///<summary>The transaction ID entered. Only for voids and returns.</summary>
		public string TransactionId { get; private set; }
		///<summary>'P' for pinpad, run transaction using RCM. 'C' for computer, run transaction using Card Not Present</summary>
		public EdgeExpressApiType ApiSelection { get; private set; }

		public FormEdgeExpressTrans() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEdgeExpressTrans_Load(object sender,EventArgs e) {
			CashBackAmount=0;
			textCashBackAmt.Text=CashBackAmount.ToString("F2");
			if(EdgeExpress.RCM.IsRCMRunning) {
				radioTerminal.Checked=true;
			}
			else {
				radioWeb.Checked=true;
			}
			FillList();
			checkSaveToken.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
			Program prog=Programs.GetCur(ProgramName.EdgeExpress);
			if(prog==null) {
				return;
			}
			checkSignature.Checked=PIn.Bool(ProgramProperties.GetPropVal(prog.ProgramNum,EdgeExpressProps.PromptSignature,ClinicNum));
			checkPrintReceipt.Checked=PIn.Bool(ProgramProperties.GetPropVal(prog.ProgramNum,EdgeExpressProps.PrintReceipt,ClinicNum));
			if(PIn.Bool(ProgramProperties.GetPropVal(prog.ProgramNum,EdgeExpressProps.PreventSavingNewCC,ClinicNum)) || !DoShowSaveTokenBox) {
				checkSaveToken.Checked=false;
				checkSaveToken.Enabled=false;
			}
		}

		private void FillList() {
			listBoxTransType.Items.Clear();
			List<EdgeExpressTransType> listTransTypes;
			if(radioTerminal.Checked) {
				listTransTypes=EdgeExpress.ListTerminalTransTypes;
			}
			else {
				listTransTypes=EdgeExpress.ListWebTransTypes;
			}
			foreach(EdgeExpressTransType transType in listTransTypes) {
				listBoxTransType.Items.Add(transType.GetDescription(),transType);
			}
			listBoxTransType.SelectedIndex=0;
			SetUI();
		}

		private void SetUI() {
			textCashBackAmt.Visible=listBoxTransType.GetSelected<EdgeExpressTransType>()==EdgeExpressTransType.DebitSale;
			labelCashBackAmt.Visible=textCashBackAmt.Visible;
			textTransactionId.Visible=ListTools.In(listBoxTransType.GetSelected<EdgeExpressTransType>(),EdgeExpressTransType.CreditVoid,
				EdgeExpressTransType.CreditOnlineCapture,EdgeExpressTransType.CreditReturn);
			labelTransactionID.Visible=textTransactionId.Visible;
		}

		private void listTransType_MouseClick(object sender,MouseEventArgs e) {
			if(listBoxTransType.IndexFromPoint(e.Location)==-1) {
				return;
			}
			SetUI();
		}

		private void radioTerminal_Click(object sender,EventArgs e) {
			FillList();
		}

		private void radioWebService_Click(object sender,EventArgs e) {
			FillList();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listBoxTransType.GetSelected<EdgeExpressTransType>()==EdgeExpressTransType.DebitSale) {
				if(!textCashBackAmt.IsValid()) {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
				CashBackAmount=PIn.Decimal(textCashBackAmt.Text);
			}
			if(textTransactionId.Visible && textTransactionId.Text=="") {
				MsgBox.Show(this,"Transaction ID required.");
				return;
			}
			TransactionType=listBoxTransType.GetSelected<EdgeExpressTransType>();
			SaveToken=checkSaveToken.Checked;
			PromptSignature=checkSignature.Checked;
			PrintReceipt=checkPrintReceipt.Checked;
			TransactionId=textTransactionId.Text;
			if(radioTerminal.Checked) {
				ApiSelection=EdgeExpressApiType.Terminal;
			}
			if(radioWeb.Checked) {
				ApiSelection=EdgeExpressApiType.Web;
			}
			DialogResult=DialogResult.OK;
		}

	}
}