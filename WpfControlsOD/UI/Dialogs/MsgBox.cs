using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenDentBusiness;

namespace OpenDental {
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
			MessageBox.Show(Lans.g(sender.GetType().Name,text),Lans.g(sender.GetType().Name,titleBarText));
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
					return MessageBox.Show(Lans.g(sender.GetType().Name,question),Lans.g(sender.GetType().Name,titleBarText),MessageBoxButton.OKCancel)==MessageBoxResult.OK;
				case MsgBoxButtons.YesNo:
					return MessageBox.Show(Lans.g(sender.GetType().Name,question),Lans.g(sender.GetType().Name,titleBarText),MessageBoxButton.YesNo)==MessageBoxResult.Yes;
				default:
					return false;
			}
		}

		///<summary>No language translation.</summary>
		public static bool Show(MsgBoxButtons buttons,string question,string titleBarText) {
			switch(buttons) {
				case MsgBoxButtons.OKCancel:
					return MessageBox.Show(question,titleBarText,MessageBoxButton.OKCancel)==MessageBoxResult.OK;
				case MsgBoxButtons.YesNo:
					return MessageBox.Show(question,titleBarText,MessageBoxButton.YesNo)==MessageBoxResult.Yes;
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
