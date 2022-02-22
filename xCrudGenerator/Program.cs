using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;

namespace xCrudGenerator {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			string database=GetCommandLineValue("database=",args);
			string server=GetCommandLineValue("server=",args);
			string user=GetCommandLineValue("user=",args);
			string password=GetCommandLineValue("password=",args);
			bool doUpdateDb=ListTools.In(GetCommandLineValue("doupdatedb=",args),"true","1");
			if(string.IsNullOrEmpty(server)) {
				server="localhost";
			}
			if(string.IsNullOrEmpty(user)) {
				user="root";
			}
			if(doUpdateDb && !string.IsNullOrEmpty(database)) {
				CrudGenHelper.UpdateDatabase(database,server,user,password);
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}

		///<summary>Returns the remainer of the command line argument that is after the cmdLineKey. If not founds, returns an empty string.</summary>
		private static string GetCommandLineValue(string cmdLineKey,string[] args) {
			foreach(string clArg in args) {
				if(clArg.ToLower().StartsWith(cmdLineKey) && clArg.Length > cmdLineKey.Length) {
					return clArg.Substring(cmdLineKey.Length).Trim('"');
				}
			}
			return "";
		}
	}
}
