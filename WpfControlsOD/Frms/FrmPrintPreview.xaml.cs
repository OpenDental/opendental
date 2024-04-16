using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmPrintPreview : FrmODBase {
		//Yes, this was a quick and dirty window. Works fine, but we really need to build a custom one.
		//Buttons should have labels.
		public FixedDocument FixedDocumentCur;

		///<summary></summary>
		public FrmPrintPreview(){
			InitializeComponent();
			Load+=FrmPrintPreview_Load;
		}

		private void FrmPrintPreview_Load(object sender, EventArgs e) {
			documentViewer.Document=FixedDocumentCur;
			StartMaximized=true;
		}
	}
}