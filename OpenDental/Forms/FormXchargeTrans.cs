using System;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.Bridges;

namespace OpenDental {
	public partial class FormXchargeTrans:FormODBase {
		public int IdxTransactionType;
		public decimal CashBackAmount;
		public bool IsSaveTokenChecked;
		///<summary>Set on the way in based on the property value for the clinic on the payment.  Sets the initial checked state of checkSignature.
		///When the user presses OK, this value reflects the final state of checkSignature when the user hits OK.</summary>
		public bool IsPromptSignatureChecked;
		///<summary>Set on the way in based on the property value for the clinic on the payment.  Sets the initial checked state of checkPrintReceipt.
		///When the user presses OK, this value reflects the final state of checkPrintReceipt when the user hits OK.</summary>
		public bool IsPrintReceiptChecked;
		public long ClinicNum;

		public FormXchargeTrans() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormXchargeTrans_Load(object sender,EventArgs e) {
			CashBackAmount=0;
			textCashBackAmt.Text=CashBackAmount.ToString("F2");
			listTransType.Items.Clear();
			listTransType.Items.Add("Purchase");
			listTransType.Items.Add("Return");
			listTransType.Items.Add("Debit Purchase");
			listTransType.Items.Add("Debit Return");
			listTransType.Items.Add("Force");
			listTransType.Items.Add("Pre-Authorization");
			listTransType.Items.Add("Adjustment");
			listTransType.Items.Add("Void");
			listTransType.SelectedIndex=0;
			checkSaveToken.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
			Program program=Programs.GetCur(ProgramName.Xcharge);
			if(program==null) {
				return;
			}
			checkSignature.Checked=IsPromptSignatureChecked;
			checkPrintReceipt.Checked=IsPrintReceiptChecked;
			if(PIn.Bool(ProgramProperties.GetPropVal(program.ProgramNum,ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC,ClinicNum))) {
				checkSaveToken.Checked=false;
				checkSaveToken.Enabled=false;
			}
		}

		private void listTransType_MouseClick(object sender,MouseEventArgs e) {
			if(listTransType.IndexFromPoint(e.Location)==-1) {
				return;
			}
			textCashBackAmt.Visible=false;
			labelCashBackAmt.Visible=false;
			if(listTransType.SelectedIndex==2) { //Debit Purchase
				textCashBackAmt.Visible=true;
				labelCashBackAmt.Visible=true;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(listTransType.SelectedIndex==2) { //Debit Purchase
				if(!textCashBackAmt.IsValid()) {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
				CashBackAmount=PIn.Decimal(textCashBackAmt.Text);
			}
			IdxTransactionType=listTransType.SelectedIndex;
			IsSaveTokenChecked=checkSaveToken.Checked;
			IsPromptSignatureChecked=checkSignature.Checked;
			IsPrintReceiptChecked=checkPrintReceipt.Checked;
			DialogResult=DialogResult.OK;
		}

	}
}