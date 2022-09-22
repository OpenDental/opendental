using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OpenDental.Reporting.Allocators.MyAllocator1 {
	/// <summary>
	/// Programming Utilites (Kinda like snipits)
	/// </summary>
	public class PU {
		/// <summary> Throws an exception with message given. </summary>
		public static string Ex {
			set {
				//1.ToString();
				throw new Exception(value);
				//1.ToString();
			}
		}
		/// <summary> Message Box with "Message" as Caption </summary>
		public static string MB { set { MessageBox.Show(value,"Message"); } }
		/// <summary>
		/// Returns the Name of the Calling Method. Needs Tested.
		/// If this works, I thought it is pretty cool.  Now you can tell where your
		/// error comes from.  Gives me goose bumps. - Dan Krueger
		/// </summary>
		/// <returns>Returns the Name of the Calling Method</returns>
		public static string Method {
			get {
				string rValue = "Method Name not Found";
				System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
				string s2 = st.ToString();
				string[] lines = s2.Split('\n');
				if(lines != null && lines.Length >= 1)
					rValue = lines[1];
				//if (st.FrameCount  >= 1)
				//{
				//    System.Diagnostics.StackFrame sf = st.GetFrame(1);
				//    rValue = sf.GetMethod().Name;
				//    string s1 = st.ToString();
				//}
				return rValue;
			}
		}
	}
}
