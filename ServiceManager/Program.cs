using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServiceManager {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if(args!=null && args.Length > 0 && args[0].GetType()==typeof(string)) {
				Application.Run(new FormServiceManage(args[0],true,false));
			}
			else {
				Application.Run(new FormMain());
			}
		}
	}
}
