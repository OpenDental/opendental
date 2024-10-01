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
		public FrmImageFilter(){
			InitializeComponent();
			Load+=FrmImageFilter_Load;
			PreviewKeyDown+=FrmImageFilter_PreviewKeyDown;
		}

		private void FrmImageFilter_Load(object sender, EventArgs e) {
			Lang.F(this);
			checkShowOD.Checked=ShowOD;
		}

		private void FrmImageFilter_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			ShowOD=checkShowOD.Checked==true;
			IsDialogOK=true;
		}
	}
}