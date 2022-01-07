using System;
using System.Windows.Forms;

namespace OpenDental {
	///<summary></summary>
	public class MsgBox {

		///<summary>Automates the language translation of the entire string. Do NOT use if the text could be variable in any way.</summary>
		public static void Show(object sender,string text) {
			Show(sender,text,"");
		}

		///<summary>Use this when you don't want automatic language translation.  Any substrings that need translation should be wrapped with Lan.g().</summary>
		public static void Show(string text) {
			MessageBox.Show(text);
		}

		///<summary>Automates the language translation of the entire string. Do NOT use if the text could be variable in any way.</summary>
		public static void Show(object sender,string text,string titleBarText) {
			MessageBox.Show(Lan.g(sender.GetType().Name,text),Lan.g(sender.GetType().Name,titleBarText));
		}

		///<summary>Automates the language translation of the entire string. Do NOT use if the text could be variable in any way. Returns true if result is OK or Yes.</summary>
		public static bool Show(object sender,MsgBoxButtons buttons,string question) {
			return Show(sender,buttons,question,"");
		}

		///<summary>No language translation.  Returns true if result is OK or Yes.</summary>
		public static bool Show(MsgBoxButtons buttons,string question) {
			return Show(buttons,question,"");
		}

		///<summary>Automates the language translation of the entire string. Do NOT use if the text could be variable in any way.</summary>
		public static bool Show(object sender,MsgBoxButtons buttons,string question,string titleBarText) {
			switch(buttons) {
				case MsgBoxButtons.OKCancel:
					return MessageBox.Show(Lan.g(sender.GetType().Name,question),Lan.g(sender.GetType().Name,titleBarText),MessageBoxButtons.OKCancel)==DialogResult.OK;
				case MsgBoxButtons.YesNo:
					return MessageBox.Show(Lan.g(sender.GetType().Name,question),Lan.g(sender.GetType().Name,titleBarText),MessageBoxButtons.YesNo)==DialogResult.Yes;
				default:
					return false;
			}
		}

		///<summary>No language translation.</summary>
		public static bool Show(MsgBoxButtons buttons,string question,string titleBarText) {
			switch(buttons) {
				case MsgBoxButtons.OKCancel:
					return MessageBox.Show(question,titleBarText,MessageBoxButtons.OKCancel)==DialogResult.OK;
				case MsgBoxButtons.YesNo:
					return MessageBox.Show(question,titleBarText,MessageBoxButtons.YesNo)==DialogResult.Yes;
				default:
					return false;
			}
		}
	}

	///<summary></summary>
	public enum MsgBoxButtons {
		///<summary></summary>
		OKCancel,
		///<summary></summary>
		YesNo
	}

}
