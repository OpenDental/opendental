using System;
using System.Collections.Generic;
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
	public partial class FrmEmailMasterTemplate:FrmODBase {

		public FrmEmailMasterTemplate() {
			InitializeComponent();
			//Lan.F(this);
		}

		private void FrmEmailMasterTemplate_Loaded(object sender,RoutedEventArgs e) {
			textMaster.Text=PrefC.GetString(PrefName.EmailMasterTemplate);
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(Prefs.UpdateString(PrefName.EmailMasterTemplate,textMaster.Text)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			IsDialogOK=true;
		}
	}
}
