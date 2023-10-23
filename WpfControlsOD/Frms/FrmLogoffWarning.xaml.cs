using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
///<summary></summary>
	public partial class FrmLogoffWarning:FrmODBase {
		DispatcherTimer _dispatcherTimer=new DispatcherTimer();
		///<summary></summary>
		public FrmLogoffWarning() {
			InitializeComponent();
			//Lan.F(this);
			_dispatcherTimer.Interval=TimeSpan.FromSeconds(10);
			_dispatcherTimer.IsEnabled=true;
			_dispatcherTimer.Tick+=timer1_Tick;
			Load+=FrmLogoffWarning_Load;
			FormClosed+=FrmLogoffWarning_Closed;
		}

		private void FrmLogoffWarning_Load(object sender,EventArgs e) {
			_dispatcherTimer.Start();
		}

		private void timer1_Tick(object sender,EventArgs e) {
			IsDialogOK=true;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			IsDialogOK=false;
		}

		private void FrmLogoffWarning_Closed(object sender,EventArgs e) {
			_dispatcherTimer.Stop();
		}


	}
}
