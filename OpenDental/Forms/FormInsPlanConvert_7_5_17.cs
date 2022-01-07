using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInsPlanConvert_7_5_17:FormODBase {
		public YN InsPlanConverstion_7_5_17_AutoMergeYN;

		public FormInsPlanConvert_7_5_17() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsPlanConvert_7_5_17_Load(object sender,EventArgs e) {
			if(InsPlanConverstion_7_5_17_AutoMergeYN==YN.Yes){
				radioMergeY.Checked=true;
			}
			if(InsPlanConverstion_7_5_17_AutoMergeYN==YN.No) {
				radioMergeN.Checked=true;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!radioMergeY.Checked && !radioMergeN.Checked) {
				MessageBox.Show("One of the options must be selected before clicking OK.");
				return;
			}
			if(radioMergeY.Checked){
				InsPlanConverstion_7_5_17_AutoMergeYN=YN.Yes;
			}
			if(radioMergeN.Checked) {
				InsPlanConverstion_7_5_17_AutoMergeYN=YN.No;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}