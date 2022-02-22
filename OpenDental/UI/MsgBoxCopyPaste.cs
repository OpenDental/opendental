using System;
using System.Drawing.Printing;

namespace OpenDental {
	///<summary>Used in order to allow for our centralized printing logic to extend to MsgBoxCopyPaste in Codebase. Use only in OD proper.</summary>
	public partial class MsgBoxCopyPaste:CodeBase.MsgBoxCopyPaste {

		public MsgBoxCopyPaste(string displayText):base(displayText) {
			InitializeComponent();
		}

		protected override void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			PrinterL.TryPrint(pd_PrintPage,margins:new Margins(50,50,50,50),duplex:Duplex.Horizontal);
		}
	}
}
