using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace CentralManager {
	public partial class FormCentralConnectionEdit:Form {
		public CentralConnection CentralConnectionCur;
		public int LastItemOrder;

		public FormCentralConnectionEdit() {
			InitializeComponent();
		}

		private void FormCentralConnectionEdit_Load(object sender,EventArgs e) {
			textServerName.Text=CentralConnectionCur.ServerName;
			textDatabaseName.Text=CentralConnectionCur.DatabaseName;
			textMySqlUser.Text=CentralConnectionCur.MySqlUser;
			textMySqlPassword.Text=CentralConnections.Decrypt(CentralConnectionCur.MySqlPassword,FormCentralManager.EncryptionKey);
			textMySqlPassword.PasswordChar=textMySqlPassword.Text==""?default(char):'*';//if password entered, mask it
			textServiceURI.Text=CentralConnectionCur.ServiceURI;
			checkWebServiceIsEcw.Checked=CentralConnectionCur.WebServiceIsEcw;
			textNote.Text=CentralConnectionCur.Note;
			checkClinicBreakdown.Checked=CentralConnectionCur.IsNew ? true : CentralConnectionCur.HasClinicBreakdownReports;
		}

		private void textMySqlPassword_TextChanged(object sender,EventArgs e) {
			if(textMySqlPassword.Text=="") {
				textMySqlPassword.PasswordChar=default(char);//if text is cleared, turn off password char mask
			}
		}

		private void textMySqlPassword_Leave(object sender,EventArgs e) {
			textMySqlPassword.PasswordChar=textMySqlPassword.Text==""?default(char):'*';//mask password on leave
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(CentralConnectionCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			//no prompt
			CentralConnections.Delete(CentralConnectionCur.CentralConnectionNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			CentralConnectionCur.ServerName=textServerName.Text;
			CentralConnectionCur.DatabaseName=textDatabaseName.Text;
			CentralConnectionCur.MySqlUser=textMySqlUser.Text;
			CentralConnectionCur.MySqlPassword=CentralConnections.Encrypt(textMySqlPassword.Text,FormCentralManager.EncryptionKey);
			CentralConnectionCur.ServiceURI=textServiceURI.Text;
			CentralConnectionCur.WebServiceIsEcw=checkWebServiceIsEcw.Checked;
			CentralConnectionCur.Note=textNote.Text;
			CentralConnectionCur.HasClinicBreakdownReports=checkClinicBreakdown.Checked;
			if(CentralConnectionCur.IsNew) {
				CentralConnectionCur.ItemOrder=LastItemOrder+1;
				CentralConnections.Insert(CentralConnectionCur);
				CentralConnectionCur.IsNew=false;//so a double-click immediately in FormCentralConnections doesn't insert again
			}
			else {
				CentralConnections.Update(CentralConnectionCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}