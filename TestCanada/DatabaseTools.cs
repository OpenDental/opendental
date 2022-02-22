using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OpenDental;
using OpenDentBusiness;
using System.Windows.Forms;

namespace TestCanada {
	///<summary>Contains the queries, scripts, and tools to clear the database of data from previous unitTest runs.</summary>
	class DatabaseTools {
		//public static string DbName;

		//public static bool DbExists(){
		//	string command="";
		//}

		///<summary>This is analogous to FormChooseDatabase.TryToConnect.  Empty string is allowed.</summary>
		public static bool SetDbConnection(string dbName,string serverName) {
			try {
				OpenDentBusiness.DataConnection dcon;
				DataConnection.DBtype=DatabaseType.MySql;
				dcon=new OpenDentBusiness.DataConnection(DataConnection.DBtype);
				dcon.SetDb("Server="+serverName+";Database="+dbName+";User ID=root;Password=;CharSet=utf8;Treat Tiny As Boolean=false","",DataConnection.DBtype,true);
				RemotingClient.RemotingRole=RemotingRole.ClientDirect;
				return true;
			}
			catch {
				return false;
			}
		}

		///<summary>This is analogous to FormChooseDatabase.TryToConnect.  Empty string is allowed.  This version implies that localhost is the server.</summary>
		public static bool SetDbConnection(string dbName) {
			return SetDbConnection(dbName,"localhost");
		}

		public static string FreshFromDump(){
			SetDbConnection("mysql","localhost");//Must connect to the mysql database so we can drop and recreate the test database.
			string command="DROP DATABASE IF EXISTS canadatest";
			try{
				DataCore.NonQ(command);
			}
			catch{
				throw new Exception("Database could not be dropped.  Please remove any remaining text files and try again.");
			}
			command="CREATE DATABASE canadatest";
			DataCore.NonQ(command);
			SetDbConnection("canadatest");
			command=Properties.Resources.dumpcanada;
			DataCore.NonQ(command);
			Cache.ClearAllCache();
			string toVersion=Assembly.GetAssembly(typeof(OpenDental.PrefL)).GetName().Version.ToString();
			//MessageBox.Show(Application.ProductVersion+" - "+
			if(!PrefL.ConvertDB(true,toVersion)) {
				throw new Exception("Wrong version.");
			}
			ProcedureCodes.TcodesClear();
			AutoCodes.SetToDefaultCanada();
			Prefs.UpdateDateT(PrefName.BackupReminderLastDateRun,DateTime.MaxValue.AddYears(-1));//We do not need backup reminders while testing.
			return "Fresh database loaded from sql dump.\r\n";
		}

		public static string ClearDb() {
			string command=@"
				DELETE FROM carrier;
				DELETE FROM claim;
				DELETE FROM claimproc;
				DELETE FROM fee;
				DELETE FROM feesched WHERE FeeSchedNum !=53; /*because this is the default fee schedule for providers*/
				DELETE FROM insplan;
				DELETE FROM patient;
				DELETE FROM patplan;
				DELETE FROM procedurelog;
				DELETE FROM etrans;
				";
			DataCore.NonQ(command);
			ProcedureCodes.RefreshCache();
			ProcedureCode procCode;
			if(!ProcedureCodes.IsValidCode("99222")) {
				procCode=new ProcedureCode();
				procCode.ProcCode="99222";
				procCode.Descript="Lab2";
				procCode.AbbrDesc="Lab2";
				procCode.IsCanadianLab=true;
				procCode.ProcCat=256;
				procCode.ProcTime="/X/";
				procCode.TreatArea=TreatmentArea.Mouth;
				ProcedureCodes.Insert(procCode);
				ProcedureCodes.RefreshCache();
			}
			procCode=ProcedureCodes.GetProcCode("99111");
			procCode.IsCanadianLab=true;
			ProcedureCodes.Update(procCode);
			ProcedureCodes.RefreshCache();
			if(!ProcedureCodes.IsValidCode("27213")) {
				procCode=new ProcedureCode();
				procCode.ProcCode="27213";
				procCode.Descript="Crown";
				procCode.AbbrDesc="Crn";
				procCode.ProcCat=250;
				procCode.ProcTime="/X/";
				procCode.TreatArea=TreatmentArea.Tooth;
				procCode.PaintType=ToothPaintingType.CrownLight;
				ProcedureCodes.Insert(procCode);
				ProcedureCodes.RefreshCache();
			}
			procCode=ProcedureCodes.GetProcCode("67211");
			procCode.TreatArea=TreatmentArea.Quad;
			ProcedureCodes.Update(procCode);
			ProcedureCodes.RefreshCache();



			return "Database cleared of old data.\r\n";
		}
	}
}
