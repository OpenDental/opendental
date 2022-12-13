using System;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDatePicker:FormODBase {

		public DateTime DateEntered;
		public Point PointStartLocation;
		public int WidthForm=0;

		public FormDatePicker() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDatePicker_Load(object sender,EventArgs e) {
			textDate.Text=DateEntered.ToShortDateString();
			Location=PointStartLocation;
			if(WidthForm!=0) {
				Width=WidthForm;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(textDate.Text)) {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Invalid date.");
				return;
			}
			DateEntered=PIn.Date(textDate.Text);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}