using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text; 
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using DataConnectionBase;

namespace OpenDental {

	///<summary></summary>
	public class Shared {

		///<summary></summary>
		public Shared(){
			
		}

		///<summary>Converts numbers to ordinals.  For example, 120 to 120th, 73 to 73rd.  Probably doesn't work too well with foreign language translations.  Used in the Birthday postcards.</summary>
		public static string NumberToOrdinal(int number){
			if(number==11) {
				return "11th";
			}
			if(number==12) {
				return "12th";
			}
			if(number==13) {
				return "13th";
			}
			string str=number.ToString();
			string last=str.Substring(str.Length-1);
			switch(last){
				case "0":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
					return str+"th";
				case "1":
					return str+"st";
				case "2":
					return str+"nd";
				case "3":
					return str+"rd";
			}
			return "";//will never happen
		}

		///<summary>Returns false if the backup, repair, or the optimze failed.
		///Set isSilent to true to suppress the failure message boxes.  However, progress windows will always be shown.</summary>
		public static bool BackupRepairAndOptimize(bool isSilent,BackupLocation backupLocation,bool isSecurityLogged=true) {
			if(!MakeABackup(isSilent,backupLocation,isSecurityLogged)) {
				return false;
			}
			return RepairAndOptimize(isSilent);
		}

		///<summary>Returns false if the repair or the optimze failed.
		///Set isSilent to true to suppress the failure message boxes.  However, progress windows will always be shown.</summary>
		public static bool RepairAndOptimize(bool isSilent) {
			try {
				UI.ProgressWin progressOD=new UI.ProgressWin();
				progressOD.ActionMain=()=>DatabaseMaintenances.RepairAndOptimize();
				progressOD.ShowDialog();
				if(progressOD.IsCancelled){
					return false;
				}
			}
			catch(Exception ex) {//MiscData.MakeABackup() could have thrown an exception.
				//Show the user that something what went wrong when not in silent mode.
				if(!isSilent) {
					if(ex.Message!="") {
						MessageBox.Show(ex.Message);
					}
					MsgBox.Show("FormDatabaseMaintenance","Optimize and Repair failed.");
				}
				return false;
			}
			return true;
		}

		///<summary>This is a wrapper method for MiscData.MakeABackup() that will show a progress window so that the user can see progress.
		///Returns false if making a backup failed.</summary>
		public static bool MakeABackup(BackupLocation backupLocation) {
			return MakeABackup(false,backupLocation);
		}

		///<summary>This is a wrapper method for MiscData.MakeABackup() that will show a progress window so that the user can see progress.
		///Set isSilent to true to suppress the failure message boxes.  However, the progress window will always be shown.
		///Returns false if making a backup failed.</summary>
		public static bool MakeABackup(bool isSilent,BackupLocation backupLocation,bool isSecurityLogged=true) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return false;//Because MiscData.MakeABackup() is not yet Oracle compatible.
			}
			if(ODBuild.IsDebug()) {
				switch(MessageBox.Show("Would you like to make a backup of the DB?","DEBUG ONLY",MessageBoxButtons.YesNoCancel)) {
					case DialogResult.Cancel:
						return false;
					case DialogResult.No:
						return true;
					case DialogResult.Yes:
					default:
						//do nothing, make backup like usual.
						break;
				}
			}
			UI.ProgressWin progressOD=new UI.ProgressWin();
			progressOD.ActionMain=()=>MiscData.MakeABackup();
			try{
				if(isSilent) {
					MiscData.MakeABackup();
				}
				else {
					progressOD.ShowDialog();
				}
			}
			catch(Exception ex){
				//Show the user that something what went wrong when not in silent mode.
				if(!isSilent) {
					if(ex.Message!="") {
						MessageBox.Show(ex.Message);
					}
					//Reusing translation in ClassConvertDatabase, since it is most likely the only place a translation would have been performed previously.
					MsgBox.Show("ClassConvertDatabase","Backup failed. Your database has not been altered.");
				}
				return false;
			}
			if(progressOD.IsCancelled){
				return false;
			}
			if(isSecurityLogged && PrefC.GetStringNoCache(PrefName.UpdateStreamLinePassword)!="abracadabra") {
				SecurityLogs.MakeLogEntryNoCache(EnumPermType.Backup,0,Lan.g("Backups","A backup was created when running the")+" "+backupLocation.ToString());
			}
			return true;
		}

	}

	///<summary>Used to log where a backup was initiated from.
	///These enum values are named in a way so that they sound good at the end of this sentence:
	///"A backup was created when running the [enumValHere]"
	///</summary>
	public enum BackupLocation {
		ConvertScript,
		DatabaseMaintenanceTool,
		OptimizeTool,
		InnoDbTool
	}

	///<summary>Displays any error messages in a MessageBox.</summary>
	public class ShowErrors:Logger.IWriteLine {
		private Control _parent;

		public ShowErrors() {
		}

		///<summary>Use this constructor to make sure that the Cursor is always Default when the MessageBox is shown.</summary>
		public ShowErrors(Control parent) {
			_parent=parent;
		}

		public LogLevel LogLevel  { get; set; }

		///<summary>Shows all Errors in a message box. BeginInvokes over to the main thread if necessary.</summary>
		public void WriteLine(string data,LogLevel logLevel,string subDirectory="") {
			if(logLevel!=LogLevel.Error) {
				return;
			}
			if(_parent!=null && _parent.InvokeRequired) {
				_parent.BeginInvoke(() => WriteLine(data,logLevel,subDirectory));
				return;
			}
			MessageBox.Show(data);
		}
	}


}