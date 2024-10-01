using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormResellerServiceEdit:FormODBase {
		private ResellerService _resellerService;
		public bool IsNew;


		public FormResellerServiceEdit(ResellerService resellerService) {
			_resellerService=resellerService;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormResellerServiceEdit_Load(object sender,EventArgs e) {
			if(!IsNew) {
				textCode.Text=ProcedureCodes.GetStringProcCode(_resellerService.CodeNum);
				textDesc.Text=ProcedureCodes.GetLaymanTerm(_resellerService.CodeNum);
				textFee.Text=_resellerService.Fee.ToString(Currency.GetCurrencyFormat());
				if(_resellerService.FeeRetail==-1) {
					textFeeRetail.Text="";
				}
				else {
					textFeeRetail.Text=_resellerService.FeeRetail.ToString(Currency.GetCurrencyFormat());
				}
				ShowHostedUrl(_resellerService.CodeNum);
			}
			else {
				textHostedUrl.Visible=false;
				labelHostedUrl.Visible=false;
			}
		}

		private void ShowHostedUrl(long codeNum) {
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(codeNum);
			bool doShowHostedUrl=ProcedureCodes.DoShowHostedUrl(procedureCode);
			textHostedUrl.Text=_resellerService.HostedUrl;
			textHostedUrl.Visible=doShowHostedUrl;
			labelHostedUrl.Visible=doShowHostedUrl;
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult==DialogResult.OK) {
				_resellerService.CodeNum=formProcCodes.CodeNumSelected;
				textCode.Text=ProcedureCodes.GetStringProcCode(_resellerService.CodeNum);
				textDesc.Text=ProcedureCodes.GetLaymanTerm(_resellerService.CodeNum);
				ShowHostedUrl(_resellerService.CodeNum);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!IsNew) {
				MsgBox.Show(this,"Deleting services not implemented yet.");
			}
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(_resellerService.CodeNum==0) {
				MsgBox.Show(this,"Please pick a service from the list of procedure codes.");
				return;
			}
			if(!textFee.IsValid()) {
				MsgBox.Show(this,"Please fix the service fee first.");
				return;
			}
			if(!textFeeRetail.IsValid()) {
				MsgBox.Show(this,"Please fix the retail service fee first.");
				return;
			}
			//Consider -1 as the valid default value, 
			double feeRetail=-1;
			if(!string.IsNullOrWhiteSpace(textFeeRetail.Text)) {
				feeRetail=Currency.Round(PIn.Double(textFeeRetail.Text));
			}
			_resellerService.FeeRetail=feeRetail;
			_resellerService.Fee=Currency.Round(PIn.Double(textFee.Text));
			_resellerService.HostedUrl=textHostedUrl.Text;
			if(IsNew) {
				ResellerServices.Insert(_resellerService);
			}
			else {
				ResellerServices.Update(_resellerService);
			}
			DialogResult=DialogResult.OK;
		}

	}
}