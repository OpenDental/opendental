using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormFaqVersionPicker:FormODBase {
		///<summary>Holds the user's selections</summary>
		public List<int> ListSelectedVersions;

		public FormFaqVersionPicker(List<int> versions,bool isNewFaq) {
			InitializeComponent();
			InitializeLayoutManager();
			listBoxMain.Items.AddList<int>(versions,x => x.ToString());
			//Only allowed to select one version if editing an existing FAQ because each existing one is for a single version.
			listBoxMain.SelectionMode=UI.SelectionMode.One;
			if(isNewFaq) {//Multiple versions can be selected for a new FAQ because multiple FAQs will be made, one for each version selected.
				listBoxMain.SelectionMode=UI.SelectionMode.MultiExtended;
			}
		}

		private void ButOk_Click(object sender,EventArgs e) {
			ListSelectedVersions=listBoxMain.GetListSelected<int>();
			DialogResult=DialogResult.OK;
		}

		private void ButCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
