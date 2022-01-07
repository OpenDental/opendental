using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormResellerServiceEdit:FormODBase {
		private ResellerService ResellerServiceCur;
		public bool IsNew;


		public FormResellerServiceEdit(ResellerService resellerService) {
			ResellerServiceCur=resellerService;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormResellerServiceEdit_Load(object sender,EventArgs e) {
			if(!IsNew) {
				textCode.Text=ProcedureCodes.GetStringProcCode(ResellerServiceCur.CodeNum);
				textDesc.Text=ProcedureCodes.GetLaymanTerm(ResellerServiceCur.CodeNum);
				textFee.Text=ResellerServiceCur.Fee.ToString("F");
			}
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormProcCodes FormPC=new FormProcCodes();
			FormPC.IsSelectionMode=true;
			FormPC.ShowDialog();
			if(FormPC.DialogResult==DialogResult.OK) {
				ResellerServiceCur.CodeNum=FormPC.SelectedCodeNum;
				textCode.Text=ProcedureCodes.GetStringProcCode(ResellerServiceCur.CodeNum);
				textDesc.Text=ProcedureCodes.GetLaymanTerm(ResellerServiceCur.CodeNum);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!IsNew) {
				MsgBox.Show(this,"Deleting services not implemented yet.");
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(ResellerServiceCur.CodeNum==0) {
				MsgBox.Show(this,"Please pick a service from the list of procedure codes.");
				return;
			}
			if(!textFee.IsValid()) {
				MsgBox.Show(this,"Please fix the service fee first.");
				return;
			}
			ResellerServiceCur.Fee=PIn.Double(textFee.Text);
			if(IsNew) {
				ResellerServices.Insert(ResellerServiceCur);
			}
			else {
				ResellerServices.Update(ResellerServiceCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}