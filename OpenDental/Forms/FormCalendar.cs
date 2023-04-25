using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCalendar:FormODBase {
		public DateTime DateSelected;
		public DateTime MinDate;

		public FormCalendar() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDatePicker_Load(object sender,EventArgs e) {
			monthCalendar.SetDate(DateSelected);
			monthCalendar.MinDate=MinDate;
		}

		private void monthCalendar_DateChanged(object sender,DateRangeEventArgs e) {
			DateSelected=monthCalendar.SelectionStart;
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}