using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormFeatureRequestPrompt:FormODBase {
		private List<JobLink> _listRequestLinks;

		public FormFeatureRequestPrompt(List<JobLink> listRequestLinks) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listRequestLinks=listRequestLinks;
		}

		private void butAuto_Click(object sender,EventArgs e) {
			using FormVersionPrompt FormVP=new FormVersionPrompt("Please type a version number",false);
			FormVP.VersionText="XX.XX";
			if(FormVP.ShowDialog()==DialogResult.Cancel) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			FeatureRequests.CompleteRequests(_listRequestLinks.Select(x => x.FKey).ToList(),FormVP.VersionText);
			DialogResult=DialogResult.OK;
		}

		private void butManual_Click(object sender,EventArgs e) {
			foreach(JobLink link in _listRequestLinks) {
				using FormRequestEdit FormRE=new FormRequestEdit();
				FormRE.IsAdminMode=true;
				FormRE.RequestId=link.FKey;
				FormRE.ShowDialog();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}