using System;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAlertSimple:FormODBase {

		///<summary>If message translation is desired, then translate before passing in the message.
		///Always use Show() instead of ShowDialog() and make sure to programmatically call Close(), because the user will not be able to close (no buttons are visible).</summary>
		public FormAlertSimple(string strMsgText) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			labelMsg.Text=strMsgText;//After Lan.F, because the message will probably contain numbers and/or punctuation which we do not want to include in translation.
		}

	}
}