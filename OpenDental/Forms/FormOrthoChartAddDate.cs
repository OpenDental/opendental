using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Cannot return OK without a proper date.</summary>
	public partial class FormOrthoChartAddDate:FormODBase {
		public DateTime DateSelected;
		///<summary>Can be 0.</summary>
		public long ProvNumSelected;

		public FormOrthoChartAddDate() {
			InitializeComponent();
			InitializeLayoutManager();
			DateSelected=new DateTime();
			Lan.F(this);
		}

		private void FormOrthoChartAddDate_Load(object sender,EventArgs e) {
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(Clinics.ClinicNum));
			comboProv.SetSelected(0);
		}

		private void butToday_Click(object sender,EventArgs e) {
			textDate.Text=DateTime.Today.ToShortDateString();
		}

		private void butNow_Click(object sender,EventArgs e) {
			textDate.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!DateTime.TryParse(textDate.Text,out DateSelected)) {
				MsgBox.Show(this,"Please fix date entry.");
				return;
			}
			ProvNumSelected=comboProv.GetSelectedProvNum();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}