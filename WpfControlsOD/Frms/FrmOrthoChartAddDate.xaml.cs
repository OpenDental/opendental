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
	///<summary>Cannot return OK without a proper date.</summary>
	public partial class FrmOrthoChartAddDate:FrmODBase {
		public DateTime DateSelected;
		///<summary>Can be 0.</summary>
		public long ProvNumSelected;

		public FrmOrthoChartAddDate() {
			InitializeComponent();
			DateSelected=new DateTime();
			//Lan.F(this);
		}

		private void FrmOrthoChartAddDate_Loaded(object sender,RoutedEventArgs e) {
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

		private void butSave_Click(object sender,EventArgs e) {
			if(!DateTime.TryParse(textDate.Text,out DateSelected)) {
				MsgBox.Show(this,"Please fix date entry.");
				return;
			}
			ProvNumSelected=comboProv.GetSelectedProvNum();
			IsDialogOK=true;
		}
	}
}