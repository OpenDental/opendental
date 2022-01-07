using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OpenDental;
using OpenDentBusiness;
using System.Windows.Forms;
using CodeBase;

namespace UnitTests {
	///<summary>Contains the queries, scripts, and tools to clear the database of data from previous unitTest runs.</summary>
	class DatabaseTools {
		public static string FreshFromDump(string serverAddr,string port,string userName,string password,bool isOracle) {
			Security.CurUser=Security.CurUser??new Userod();
			if(!isOracle) {
				string command="DROP DATABASE IF EXISTS "+TestBase.UnitTestDbName;
				try {
					DataCore.NonQ(command);
				}
				catch {
					throw new Exception("Database could not be dropped.  Please remove any remaining text files and try again.");
				}
				command="CREATE DATABASE "+TestBase.UnitTestDbName;
				DataCore.NonQ(command);
				UnitTestsCore.DatabaseTools.SetDbConnection(TestBase.UnitTestDbName,serverAddr,port,userName,password,false);
				command=Properties.Resources.dump;
				DataCore.NonQ(command);
				ConvertDatabases.FromVersion=new Version(Prefs.GetOne(PrefName.DataBaseVersion).ValueString);
				ConvertDatabases.InvokeConvertMethods();
				//Specific caches should be invalid after convert script update.
				Cache.Refresh(InvalidType.Prefs,InvalidType.Programs);
				AutoCodes.SetToDefault();
				ProcButtons.SetToDefault();
				ProcedureCodes.ResetApptProcsQuickAdd();
				//RefreshCache (might be missing a few)  Or, it might make more sense to do this as an entirely separate method when running.
				ProcedureCodes.RefreshCache();
				command="UPDATE userod SET Password='qhd+xdy/iMpe3xcjbBmB6A==' WHERE UserNum=1";//sets Password to 'pass' for middle tier testing.
				DataCore.NonQ(command);
				AddCdcrecCodes();
			}
			else {
				//This stopped working. Might look into it later: for now manually create the unittest db

				//Make sure the command CREATE OR REPLACE DIRECTORY dmpdir AS 'c:\oraclexe\app\tmp'; was run
				//and there is an opendental user with matching username/pass 
				//The unittest.dmp was taken from a fresh unittest db created from the code above.  No need to alter it further. 
				//string command=@"impdp opendental/opendental DIRECTORY=dmpdir DUMPFILE=unittest.dmp TABLE_EXISTS_ACTION=replace LOGFILE=impschema.log";
				//ExecuteCommand(command);
			}
		return "Fresh database loaded from sql dump.\r\n";
		}

		///<summary>Manually adds the few CDCREC codes necessary for the HL7 unit tests.</summary>
		private static void AddCdcrecCodes() {
			string command="SELECT COUNT(*) FROM cdcrec";
			if(DataCore.GetScalar(command)=="0") {
				Cdcrecs.Insert(new Cdcrec() {
					CdcrecCode="2106-3",
					HeirarchicalCode="R5",
					Description="WHITE"
				});
				Cdcrecs.Insert(new Cdcrec() {
					CdcrecCode="2135-2",
					HeirarchicalCode="E1",
					Description="HISPANIC OR LATINO"
				});
			}
		}
				
	}
}
