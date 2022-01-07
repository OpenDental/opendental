using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using CodeBase;
using System.Drawing.Printing;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpPrintPreview : FormODBase {

		///<summary></summary>
		public FormRpPrintPreview() {
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}
		///<summary></summary>
		public FormRpPrintPreview(PrintDocument printDoc) : this() {
			_printPreviewControl2.Document=printDoc;
		}

		///<summary></summary>
		public FormRpPrintPreview(ODprintout printout) : this() {
			if(printout.SettingsErrorCode!=PrintoutErrorCode.Success) {
				PrinterL.ShowError(printout);
				this.DialogResult=DialogResult.Cancel;
				return;
			}
			_printPreviewControl2.Document=printout.PrintDoc;
		}

		private void FormRpPrintPreview_Load(object sender, System.EventArgs e) {
			//LayoutManager.MoveLocation(butNext,new Point(this.ClientRectangle.Width-100,this.ClientRectangle.Height-30));
			//LayoutManager.MoveLocation(butPrev,new Point(this.ClientRectangle.Width-butPrev.Width-110,this.ClientRectangle.Height-30));
			LayoutManager.MoveHeight(_printPreviewControl2,this.ClientRectangle.Height-40);
			LayoutManager.MoveWidth(_printPreviewControl2,this.ClientRectangle.Width);
			_printPreviewControl2.Zoom=(double)_printPreviewControl2.ClientSize.Height
				/1100;
		}

		private void butNext_Click(object sender,System.EventArgs e) {
			_printPreviewControl2.StartPage++;
		}

		private void butPrev_Click(object sender,EventArgs e) {
			if(_printPreviewControl2.StartPage>0) {
				_printPreviewControl2.StartPage--;
			}
		}
	}
}
