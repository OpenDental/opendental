using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPayTerminalEdit:FormODBase {
		private PayTerminal _payTerminalCur;
		private bool _isNew;

		public FormPayTerminalEdit(PayTerminal payTerminalCur=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(payTerminalCur!=null) {
				_payTerminalCur=payTerminalCur;
				_isNew=false;
			}
			else {
				_payTerminalCur=new PayTerminal();
				_isNew=true;
			}
		}
		private void FormPayTerminalEdit_Load(object sender,EventArgs e) {
			if(!_isNew) {
				comboClinic.SelectedClinicNum=_payTerminalCur.ClinicNum;
				textName.Text=_payTerminalCur.Name;
				textId.Text=_payTerminalCur.TerminalID;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_isNew) {
				DialogResult=DialogResult.Cancel;
				Close();
			}
			if(MsgBox.Show(MsgBoxButtons.YesNo,Lans.g(this,"Are you sure you would like to delete this payment terminal?"))) {
				PayTerminals.Delete(_payTerminalCur.PayTerminalNum);
				Close();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textId.Text.IsNullOrEmpty()) {
				MsgBox.Show(this,"Terminal ID is required.");
				return;
			}
			_payTerminalCur.Name=textName.Text;
			_payTerminalCur.TerminalID=textId.Text;
			_payTerminalCur.ClinicNum=comboClinic.SelectedClinicNum;
			if(_isNew) {
				PayTerminals.Insert(_payTerminalCur);
			}
			else {
				PayTerminals.Update(_payTerminalCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}