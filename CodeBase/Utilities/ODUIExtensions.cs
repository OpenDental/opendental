using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CodeBase {
	public class UIHelper{
		///<summary>Sometimes ODProgress can cause other forms to open up behind other applications. Call this method to force this form to the front.</summary>
		public static void ForceBringToFront(Form form) {
			form.TopMost=true;
			Application.DoEvents();
			form.TopMost=false;
		}

		///<summary>Takes a number of elements from the end of the enumerable.</summary>
		public static IEnumerable<T> TakeLast<T>(IEnumerable<T> source,int count) {
			return source.Skip(Math.Max(0,source.Count() - count));
		}

		///<summary>Gets all controls and their children controls recursively.</summary>
		public static IEnumerable<Control> GetAllControls(Control control) {
			IEnumerable<Control> controls=control.Controls.OfType<Control>();
			return controls.SelectMany(GetAllControls).Concat(controls);
		}

		///<summary>Shows a form nonmodally then performs the given action when the form closes.</summary>
		public static void ShowThen(Form form,Action onClose) {
			form.Show();
			form.FormClosed+=(sender,e) => onClose();
		}
	}

	


}
