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
	public partial class FrmImageFilter : FrmODBase {
		///<summary>Does not control showing drawings for Pearl or any other external source.</summary>
		public bool ShowOD;
		///<summary></summary>
		public bool ShowPearl;

		///<summary></summary>
		public FrmImageFilter(){
			InitializeComponent();
			Load+=FrmImageFilter_Load;
		}

		private void FrmImageFilter_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(Programs.IsEnabled(ProgramName.Pearl)){
				checkShowPearl.Checked=ShowPearl;
			}
			else{
				checkShowPearl.Visible=false;
				checkShowPearl.Checked=false;
			}
			checkShowOD.Checked=ShowOD;
		}

		private void butSave_Click(object sender, EventArgs e) {
			ShowOD=checkShowOD.Checked==true;
			ShowPearl=checkShowPearl.Checked==true;
			IsDialogOK=true;
		}
	}
}