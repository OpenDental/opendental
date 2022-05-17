using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UpdateFileCopier {
	static class Program {
		/// <summary>The main entry point for the application.  Takes the following arguments: sourcedir processid destdir doKillServices</summary>
		[STAThread]
		static void Main(string[] arguments) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if(arguments.Length==9) {
				Application.Run(new FormMain(arguments[0],arguments[1],arguments[2],Convert.ToBoolean(arguments[3]),Convert.ToBoolean(arguments[4]),arguments[5],arguments[6],arguments[7],arguments[8]));
			}
			else if(arguments.Length==5) {
				Application.Run(new FormMain(arguments[0],arguments[1],arguments[2],Convert.ToBoolean(arguments[3]),Convert.ToBoolean(arguments[4])));
			}
			else if(arguments.Length==4) {
				Application.Run(new FormMain(arguments[0],arguments[1],arguments[2],Convert.ToBoolean(arguments[3]),true));
			}
			else if(arguments.Length==3) {
				Application.Run(new FormMain(arguments[0],arguments[1],arguments[2],true,true));
			}
			else if(arguments.Length==2) {
				Application.Run(new FormMain(arguments[0],arguments[1],@"C:\Program Files\Open Dental",true,true));
			}
			else {//just for rare debugging situations
				Application.Run(new FormMain(@"C:\OpenDentImages\UpdateFiles","0",@"C:\Program Files\Open Dental",true,true));
			}
		}
	}
}
