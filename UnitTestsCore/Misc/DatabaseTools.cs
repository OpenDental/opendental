using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OpenDentBusiness;
using CodeBase;
using DataConnectionBase;

namespace UnitTestsCore {
	///<summary>Contains the queries, scripts, and tools to clear the database of data from previous unitTest runs.</summary>
	public class DatabaseTools {

		///<summary>This is analogous to FormChooseDatabase.TryToConnect.  Empty string is allowed.</summary>
		public static bool SetDbConnection(string dbName,bool isOracle){
			return SetDbConnection(dbName,"localhost","","root","",isOracle);
		}

		//<summary>This function allows connecting to a specific server.</summary>
		public static bool SetDbConnection(string dbName,string serverAddr,string port,string userName,string password,bool isOracle) {
			DataConnection dcon;
			//Try to connect to the database directly
			try {
				if(!isOracle) {
					DataConnection.DBtype=DatabaseType.MySql;
					//Create a database connection and make sure to set the MySQL UserLow and PassLow for Middle Tier unit tests.
					new DataConnection().SetDb(serverAddr,dbName,userName,password,userName,password,DataConnection.DBtype,true);
					RemotingClient.RemotingRole=RemotingRole.ClientDirect;
					return true;
				}
				else {
					DataConnection.DBtype=DatabaseType.Oracle;
					dcon=new DataConnection(DataConnection.DBtype);
					dcon.SetDb("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST="+serverAddr+")(PORT="+port+"))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XE)));User Id="+userName+";Password="+password+";","",DataConnection.DBtype,true);
					RemotingClient.RemotingRole=RemotingRole.ClientDirect;
					return true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				if(isOracle) {
					throw new Exception("May need to create a Fresh Db for Oracle.");
				}
				return false;
			}
		}
		
		private static void ExecuteCommand(string Command){
			try {
				System.Diagnostics.ProcessStartInfo ProcessInfo;
				System.Diagnostics.Process Process;
				ProcessInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe","/C " + Command);
				ProcessInfo.CreateNoWindow = false;
				ProcessInfo.UseShellExecute = false;
				Process = System.Diagnostics.Process.Start(ProcessInfo);
				Process.Close();
			}
			catch {
				throw new Exception("Running cmd failed.");
			}
		}


		public static string ClearDb() {
			string command=@"
				DELETE FROM alertitem;
					DELETE FROM appointment;
					DELETE FROM apptreminderrule;
					DELETE FROM asapcomm;
					DELETE FROM carrier;
					DELETE FROM claim;
					DELETE FROM claimproc;
					DELETE FROM clinic;
					DELETE FROM clockevent;
					DELETE FROM confirmationrequest;
					DELETE FROM creditcard;
					DELETE FROM eclipboardsheetdef;
					DELETE FROM emailmessage;
					DELETE FROM employee;
					DELETE FROM fee;
					DELETE FROM feesched WHERE FeeSchedNum !=53; /*because this is the default fee schedule for providers*/
					DELETE FROM hl7def;
					DELETE FROM hl7msg;
					DELETE FROM insplan;
					DELETE FROM mobileappdevice;
					DELETE FROM operatory;
					DELETE FROM orthoproclink;
					DELETE FROM patient;
					DELETE FROM patientportalinvite;
					DELETE FROM patientrace;
					DELETE FROM patplan;
					DELETE FROM payment;
					DELETE FROM paysplit;
					DELETE FROM payperiod;
					DELETE FROM payplan;
					DELETE FROM payplancharge;
					DELETE FROM pharmacy;
					DELETE FROM procedurelog;
					DELETE FROM provider WHERE ProvNum>2;
					DELETE FROM recall;
					DELETE FROM schedule;
					DELETE FROM sheet;
					DELETE FROM sheetdef;
					DELETE FROM sheetfield;
					DELETE FROM sheetfielddef;
					DELETE FROM smsphone;
					DELETE FROM smstomobile;
					DELETE FROM smsfrommobile;
					DELETE FROM timeadjust;
					DELETE FROM timecardrule;
					DELETE FROM userweb;
				";
			DataCore.NonQ(command);
			Providers.RefreshCache();
			FeeScheds.RefreshCache();
			PharmacyT.CreatePharmacies();
			return "Database cleared of old data.\r\n";
		}

		public static void ClearTable(string databaseTableName) {
			string command="DELETE FROM "+POut.String(databaseTableName);
			DataCore.NonQ(command);
		}

		
	}
}
