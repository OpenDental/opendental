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
	public partial class FrmAutoNotePromptPreview:FrmODBase {
		public string ResultText;

		public FrmAutoNotePromptPreview() {
			InitializeComponent();
			//Lan.F(this);
			Load+=FrmAutoNotePromptPreview_Load;
		}

		private void FrmAutoNotePromptPreview_Load(object sender,EventArgs e) {
			textMain.Text=ResultText;
		}

		private void butSave_Click(object sender,EventArgs e) {
			ResultText=textMain.Text;
			IsDialogOK=true;
		}

	}
}