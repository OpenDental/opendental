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
		public EdgeExpressTransType EdgeExpressTransTypeCur;
		public decimal AmtCashBack;
		public bool DoSaveToken;
		///<summary>Set on the way in based on the property value for the clinic on the payment.  Sets the initial checked state of checkSignature.
		///When the user presses OK, this value reflects the final state of checkSignature when the user hits OK.</summary>
		public bool DoPromptSignature;
		///<summary>Set on the way in based on the property value for the clinic on the payment.  Sets the initial checked state of checkPrintReceipt.
		///When the user presses OK, this value reflects the final state of checkPrintReceipt when the user hits OK.</summary>
		public bool DoPrintReceipt;
		///<summary>The transaction ID entered. Only for voids and returns.</summary>
		public string TransactionId;
		///<summary>'P' for pinpad, run transaction using RCM. 'C' for computer, run transaction using Card Not Present</summary>
		public EdgeExpressApiType EdgeExpressApiTypeCur;

		public FormEdgeExpressTrans() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEdgeExpressTrans_Load(object sender,EventArgs e) {
			AmtCashBack=0;
			textCashBackAmt.Text=AmtCashBack.ToString("F2");
			if(EdgeExpress.RCM.IsRCMRunning) {
				radioTerminal.Checked=true;
			}
			else {
				radioWeb.Checked=true;
			}
			FillList();
			checkSaveToken.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
			Program program=Programs.GetCur(ProgramName.EdgeExpress);
			if(program==null) {
				return;
			}
			checkSignature.Checked=PIn.Bool(ProgramProperties.GetPropVal(program.ProgramNum,EdgeExpressProps.PromptSignature,ClinicNum));
			checkPrintReceipt.Checked=PIn.Bool(ProgramProperties.GetPropVal(program.ProgramNum,EdgeExpressProps.PrintReceipt,ClinicNum));
			if(PIn.Bool(ProgramProperties.GetPropVal(program.ProgramNum,EdgeExpressProps.PreventSavingNewCC,ClinicNum)) || !DoShowSaveTokenBox) {
				checkSaveToken.Checked=false;
				checkSaveToken.Enabled=false;
			}
		}

		private void FillList() {
			listBoxTransType.Items.Clear();
			List<EdgeExpressTransType> listEdgeExpressTransTypes;
			if(radioTerminal.Checked) {
				listEdgeExpressTransTypes=EdgeExpress.ListTerminalTransTypes;
			}
			else {
				listEdgeExpressTransTypes=EdgeExpress.ListWebTransTypes;
			}
			for(int i=0;i<listEdgeExpressTransTypes.Count;i++) {
				listBoxTransType.Items.Add(listEdgeExpressTransTypes[i].GetDescription(),listEdgeExpressTransTypes[i]);
			}
			listBoxTransType.SelectedIndex=0;
			SetUI();
		}

		private void SetUI() {
			textCashBackAmt.Visible=listBoxTransType.GetSelected<EdgeExpressTransType>()==EdgeExpressTransType.DebitSale;
			labelCashBackAmt.Visible=textCashBackAmt.Visible;
			textTransactionId.Visible= listBoxTransType.GetSelected<EdgeExpressTransType>().In(EdgeExpressTransType.CreditVoid,
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

		private void butSave_Click(object sender,EventArgs e) {
			if(listBoxTransType.GetSelected<EdgeExpressTransType>()==EdgeExpressTransType.DebitSale) {
				if(!textCashBackAmt.IsValid()) {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
				AmtCashBack=PIn.Decimal(textCashBackAmt.Text);
			}
			if(textTransactionId.Visible && textTransactionId.Text=="") {
				MsgBox.Show(this,"Transaction ID required.");
				return;
			}
			EdgeExpressTransTypeCur=listBoxTransType.GetSelected<EdgeExpressTransType>();
			DoSaveToken=checkSaveToken.Checked;
			DoPromptSignature=checkSignature.Checked;
			DoPrintReceipt=checkPrintReceipt.Checked;
			TransactionId=textTransactionId.Text;
			if(radioTerminal.Checked) {
				EdgeExpressApiTypeCur=EdgeExpressApiType.Terminal;
			}
			if(radioWeb.Checked) {
				EdgeExpressApiTypeCur=EdgeExpressApiType.Web;
			}
			DialogResult=DialogResult.OK;
		}

	}
}