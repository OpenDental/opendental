using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmFaqVersionPicker:FrmODBase {
		///<summary>Holds the user's selections</summary>
		public List<int> ListSelectedVersions;

		public FrmFaqVersionPicker(List<int> listVersions,bool isNewFaq) {
			InitializeComponent();
			listBoxMain.Items.AddList(listVersions,x => x.ToString());
			//Only allowed to select one version if editing an existing FAQ because each existing one is for a single version.
			listBoxMain.SelectionMode=SelectionMode.One;
			if(isNewFaq) {//Multiple versions can be selected for a new FAQ because multiple FAQs will be made, one for each version selected.
				listBoxMain.SelectionMode=SelectionMode.MultiExtended;
			}
		}

		private void ButOk_Click(object sender,EventArgs e) {
			ListSelectedVersions=listBoxMain.GetListSelected<int>();
			IsDialogOK=true;
		}
	}
}
