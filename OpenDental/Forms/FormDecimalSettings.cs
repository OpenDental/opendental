using System;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDecimalSettings:FormODBase {

		public FormDecimalSettings() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormDecimalSettings_Load(object sender,EventArgs e) {
			checkNoShow.Checked=ComputerPrefs.LocalComputer.NoShowDecimal;
			textDecimal.Text=CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits.ToString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			ComputerPrefs.LocalComputer.NoShowDecimal=checkNoShow.Checked;
			ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}