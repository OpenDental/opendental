using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormPhoneConfEdit:FormODBase {
		private PhoneConf _phoneConf;

		public FormPhoneConfEdit(PhoneConf phoneConf) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_phoneConf=phoneConf.Copy();
		}

		private void FormPhoneConfEdit_Load(object sender,EventArgs e) {
			if(_phoneConf.Extension > 0) {
				textExtension.Text=_phoneConf.Extension.ToString();
			}
			if(_phoneConf.ButtonIndex > -1) {
				textButtonIndex.Text=_phoneConf.ButtonIndex.ToString();
			}
			List<Site> listSites=Sites.GetDeepCopy();
			for(int i=0;i<listSites.Count;i++) {
				comboSite.Items.Add(listSites[i].Description,listSites[i]);
				if(_phoneConf.SiteNum==listSites[i].SiteNum) {
					comboSite.SelectedIndex=i;
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textExtension.IsValid() || !textButtonIndex.IsValid()) {
				MsgBox.Show(this,"Fix data errors first.");
				return;
			}
			if(string.IsNullOrEmpty(textExtension.Text)) {
				MsgBox.Show(this,"A valid extension is required.");
				return;
			}
			_phoneConf.Extension=PIn.Int(textExtension.Text);
			if(textButtonIndex.Text.Trim()=="") {
				_phoneConf.ButtonIndex=-1;
			}
			else {
				_phoneConf.ButtonIndex=PIn.Int(textButtonIndex.Text);
			}
			if(comboSite.SelectedIndex > -1) {
				_phoneConf.SiteNum=comboSite.GetSelected<Site>().SiteNum;
			}
			if(_phoneConf.IsNew) {
				try {
					PhoneConfs.Insert(_phoneConf);
				}
				catch(Exception ex) {
					MessageBox.Show("Error inserting conference room:\r\n"+ex.Message);
					return;
				}
			}
			else {
				PhoneConfs.Update(_phoneConf);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_phoneConf.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this conference room?")) {
				return;
			}
			PhoneConfs.Delete(_phoneConf.PhoneConfNum);
			DialogResult=DialogResult.OK;
		}
	}
}