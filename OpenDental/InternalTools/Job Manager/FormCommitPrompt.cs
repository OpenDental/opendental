using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormCommitPrompt:FormODBase {
		public string VersionText="";

		public FormCommitPrompt(int defaultRepo=0,string logMsg="") {
			InitializeComponent();
			InitializeLayoutManager();
			if(defaultRepo!=0) {
				switch(defaultRepo) {
					case 1:
						radioPublic.Checked=true;
						break;
					case 2:
						radioInternal.Checked=true;
						break;
					case 3:
						radioBoth.Checked=true;
						break;
					default:
						radioPublic.Checked=true;
						break;
				}
			}
			textCommit.Text=logMsg;
		}

		private void FormCommitPrompt_Load(object sender,EventArgs e) {

		}

		public int GetCommitValue() {
			if(radioPublic.Checked) {
				return 1;
			}
			if(radioInternal.Checked) {
				return 2;
			}
			if(radioBoth.Checked) {
				return 3;
			}
			return 0;
		}


		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
