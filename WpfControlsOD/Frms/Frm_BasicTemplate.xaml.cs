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
	/// <summary></summary>
	public partial class FrmBasicTemplate : FrmODBase {

		///<summary></summary>
		public FrmBasicTemplate() {
			InitializeComponent();
			Load+=Frm_Load;
		}

		private void Frm_Load(object sender, EventArgs e) {
			Lang.F(this);
			//...
		}

		private void butSave_Click(object sender, EventArgs e) {
			//...
			IsDialogOK=true;
		}
	}
}