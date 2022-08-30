using System.Windows.Forms;

namespace OpenDental.UI.Voice {
	public class VoiceMsgBox {
		///<summary>Displays a message box with the text and reads the text aloud.</summary>
		public static void Show(string text) {
			using FormMsgBox FormMB=new FormMsgBox(text);
			FormMB.ShowDialog();
		}

		///<summary>Displays a message box with the text and reads the text aloud. The user can respond by clicking buttons or answering by voice.</summary>
		public static bool Show(string text,MsgBoxButtons buttons) {
			using FormMsgBox FormMB=new FormMsgBox(text,buttons);
			FormMB.ShowDialog();
			if(FormMB.DialogResult==DialogResult.OK) {
				return true;
			}
			return false;
		}
	}
}
