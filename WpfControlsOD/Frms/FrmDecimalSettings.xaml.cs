using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmDecimalSettings:FrmODBase {

		public FrmDecimalSettings() {
			InitializeComponent();
			//Lan.F(this);
			Load+=FrmDecimalSettings_Load;
		}
		
		private void FrmDecimalSettings_Load(object sender,EventArgs e) {
			checkNoShow.Checked=ComputerPrefs.LocalComputer.NoShowDecimal;
			string decimalDigits=CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits.ToString();
			label2.Text+=": "+decimalDigits;
		}

		private void butSave_Click(object sender,EventArgs e) {
			ComputerPrefs.LocalComputer.NoShowDecimal=checkNoShow.Checked.Value;
			ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			IsDialogOK=true;
		}

	}
}