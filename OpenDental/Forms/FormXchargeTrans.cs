using System;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.Bridges;

namespace OpenDental {
	public partial class FormXchargeTrans:FormODBase {
		public int TransactionType;
		public decimal CashBackAmount;
		public bool SaveToken;
		///<summary>Set on the way in based on the property value for the clinic on the payment.  Sets the initial checked state of checkSignature.
		///When the user presses OK, this value reflects the final state of checkSignature when the user hits OK.</summary>
		public bool PromptSignature;
		///<summary>Set on the way in based on the property value for the clinic on the payment.  Sets the initial checked state of checkPrintReceipt.
		///When the user presses OK, this value reflects the final state of checkPrintReceipt when the user hits OK.</summary>
		public bool PrintReceipt;
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
			Program prog=Programs.GetCur(ProgramName.Xcharge);
			if(prog==null) {
				return;
			}
			checkSignature.Checked=PromptSignature;
			checkPrintReceipt.Checked=PrintReceipt;
			if(PIn.Bool(ProgramProperties.GetPropVal(prog.ProgramNum,ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC,ClinicNum))) {
				checkSaveToken.Checked=false;
				checkSaveToken.Enabled=false;
			}
		}

		private void listTransType_MouseClick(object sender,MouseEventArgs e) {
			if(listTransType.IndexFromPoint(e.Location)!=-1) {
				textCashBackAmt.Visible=false;
				labelCashBackAmt.Visible=false;
				if(listTransType.SelectedIndex==2) { //Debit Purchase
					textCashBackAmt.Visible=true;
					labelCashBackAmt.Visible=true;
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listTransType.SelectedIndex==2) { //Debit Purchase
				if(!textCashBackAmt.IsValid()) {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
				CashBackAmount=PIn.Decimal(textCashBackAmt.Text);
			}
			TransactionType=listTransType.SelectedIndex;
			SaveToken=checkSaveToken.Checked;
			PromptSignature=checkSignature.Checked;
			PrintReceipt=checkPrintReceipt.Checked;
			DialogResult=DialogResult.OK;
		}

	}
}