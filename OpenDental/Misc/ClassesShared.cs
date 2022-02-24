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
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=()=>DatabaseMaintenances.RepairAndOptimize();
				progressOD.ShowDialogProgress();
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
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ActionMain=()=>MiscData.MakeABackup();
			try{
				progressOD.ShowDialogProgress();
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
				SecurityLogs.MakeLogEntryNoCache(Permissions.Backup,0,Lan.g("Backups","A backup was created when running the")+" "+backupLocation.ToString());
			}
			return true;
		}

	}

/*=================================Class DataValid=========================================
===========================================================================================*/

	///<summary>Handles a global event to keep local data synchronized.</summary>
	public class DataValid{

		/*
		///<summary>Triggers an event that causes a signal to be sent to all other computers telling them what kind of locally stored data needs to be updated.  Either supply a set of flags for the types, or supply a date if the appointment screen needs to be refreshed.  Yes, this does immediately refresh the local data, too.  The AllLocal override does all types except appointment date for the local computer only, such as when starting up.</summary>
		public static void SetInvalid(List<int> itypes){
			OnBecameInvalid(new OpenDental.ValidEventArgs(DateTime.MinValue,itypes,false,0));
		}*/

		///<summary>Triggers an event that causes a signal to be sent to all other computers telling them what kind of locally stored data needs to be updated.  Either supply a set of flags for the types, or supply a date if the appointment screen needs to be refreshed.  Yes, this does immediately refresh the local data, too.  The AllLocal override does all types except appointment date for the local computer only, such as when starting up.</summary>
		public static void SetInvalid(params InvalidType[] arrayITypes) {
			FormOpenDental.S_DataValid_BecomeInvalid(new ValidEventArgs(DateTime.MinValue,arrayITypes,false,0));
		}

		///<summary>Triggers an event that causes a signal to be sent to all other computers telling them what kind of locally stored data needs to be updated.  Either supply a set of flags for the types, or supply a date if the appointment screen needs to be refreshed.  Yes, this does immediately refresh the local data, too.  The AllLocal override does all types except appointment date for the local computer only, such as when starting up.</summary>
		public static void SetInvalid(bool onlyLocal) {
			FormOpenDental.S_DataValid_BecomeInvalid(new ValidEventArgs(DateTime.MinValue,new[] { InvalidType.AllLocal },true,0));
		}

	}

	///<summary></summary>
	public delegate void ValidEventHandler(ValidEventArgs e);

	///<summary></summary>
	public class ValidEventArgs : System.EventArgs{
		private DateTime dateViewing;
		private InvalidType[] itypes;
		private bool onlyLocal;
		private long taskNum;
		
		///<summary></summary>
		public ValidEventArgs(DateTime dateViewing,InvalidType[] itypes,bool onlyLocal,long taskNum)
			: base() {
			this.dateViewing=dateViewing;
			this.itypes=itypes;
			this.onlyLocal=onlyLocal;
			this.taskNum=taskNum;
		}

		///<summary></summary>
		public DateTime DateViewing{
			get{return dateViewing;}
		}

		///<summary></summary>
		public InvalidType[] ITypes {
			get{return itypes;}
		}

		///<summary></summary>
		public bool OnlyLocal{
			get{return onlyLocal;}
		}

		///<summary></summary>
		public long TaskNum {
			get{return taskNum;}
		}

	}

	/*=================================Class GotoModule==================================================
	===========================================================================================*/

	///<summary>Used to trigger a global event to jump between modules and perform actions in other modules.  PatNum is optional.  If 0, then no effect.</summary>
	public class GotoModule {

		/*
		///<summary>This triggers a global event which the main form responds to by going directly to a module.</summary>
		public static void GoNow(DateTime dateSelected,Appointment pinAppt,int selectedAptNum,int iModule,int claimNum) {
			OnModuleSelected(new ModuleEventArgs(dateSelected,pinAppt,selectedAptNum,iModule,claimNum));
		}*/

		///<summary>Goes directly to an existing appointment.</summary>
		public static void GotoAppointment(DateTime dateSelected,long selectedAptNum) {
			OnModuleSelected(new ModuleEventArgs(dateSelected,new List<long>(),selectedAptNum,0,0,0,0));
		}

		///<summary>Goes directly to a claim in someone's Account.</summary>
		public static void GotoClaim(long claimNum) {
			OnModuleSelected(new ModuleEventArgs(DateTime.MinValue,new List<long>(),0,EnumModuleType.Account,claimNum,0,0));
		}

		///<summary>Goes directly to an Account.  Sometimes, patient is selected some other way instead of being passed in here, so OK to pass in a patNum of zero.</summary>
		public static void GotoAccount(long patNum) {
			OnModuleSelected(new ModuleEventArgs(DateTime.MinValue,new List<long>(),0,EnumModuleType.Account,0,patNum,0));
		}
		
		///<summary>Goes directly to Family module.  Sometimes, patient is selected some other way instead of being passed in here, so OK to pass in a patNum of zero.</summary>
		public static void GotoFamily(long patNum) {
			OnModuleSelected(new ModuleEventArgs(DateTime.MinValue,new List<long>(),0,EnumModuleType.Family,0,patNum,0));
		}

		///<summary>Goes directly to TP module.  Sometimes, patient is selected some other way instead of being passed in here, so OK to pass in a patNum of zero.</summary>
		public static void GotoTreatmentPlan(long patNum) {
			OnModuleSelected(new ModuleEventArgs(DateTime.MinValue,new List<long>(),0,EnumModuleType.TreatPlan,0,patNum,0));
		}

		public static void GotoChart(long patNum){
			OnModuleSelected(new ModuleEventArgs(DateTime.MinValue, new List<long>(), 0,EnumModuleType.Chart, 0, patNum, 0));
		}

		public static void GotoManage(long patNum){
			OnModuleSelected(new ModuleEventArgs(DateTime.MinValue, new List<long>(), 0,EnumModuleType.Manage, 0, patNum, 0));
		}

		///<summary>Puts appointments on pinboard, then jumps to Appointments module with today's date.
		///Sometimes, patient is selected some other way instead of being passed in here, so OK to pass in a patNum of zero.</summary>
		public static void PinToAppt(List<long> pinAptNums,long patNum) {
			OnModuleSelected(new ModuleEventArgs(DateTime.Today,pinAptNums,0,EnumModuleType.Appointments,0,patNum,0));
		}

		///<summary>Jumps to Images module and pulls up the specified image.</summary>
		public static void GotoImage(long patNum,long docNum) {
			OnModuleSelected(new ModuleEventArgs(DateTime.MinValue,new List<long>(),0,EnumModuleType.Imaging,0,patNum,docNum));
		}

		///<summary></summary>
		protected static void OnModuleSelected(ModuleEventArgs e){
			FormOpenDental.S_GotoModule_ModuleSelected(e);
		}
	}

	///<summary></summary>
	public class ModuleEventArgs : System.EventArgs {
		private DateTime dateSelected;
		private List<long> listPinApptNums;
		private long selectedAptNum;
		private EnumModuleType moduleType;
		private long claimNum;
		private long patNum;
		private long docNum;//image

		///<summary></summary>
		public ModuleEventArgs(DateTime dateSelected,List<long> listPinApptNums,long selectedAptNum,EnumModuleType moduleType,
			long claimNum,long patNum,long docNum)
			: base()
		{
			this.dateSelected=dateSelected;
			this.listPinApptNums=listPinApptNums;
			this.selectedAptNum=selectedAptNum;
			this.moduleType=moduleType;
			this.claimNum=claimNum;
			this.patNum=patNum;
			this.docNum=docNum;
		}

		///<summary>If going to the ApptModule, this lets you pick a date.</summary>
		public DateTime DateSelected{
			get{return dateSelected;}
		}

		///<summary>The aptNums of the appointments that we want to put on the pinboard of the Apt Module.</summary>
		public List<long> ListPinApptNums {
			get{return listPinApptNums;}
		}

		///<summary></summary>
		public long SelectedAptNum {
			get{return selectedAptNum;}
		}

		///<summary></summary>
		public EnumModuleType ModuleType{
			get{return moduleType;}
		}

		///<summary>If going to Account module, this lets you pick a claim.</summary>
		public long ClaimNum {
			get{return claimNum;}
		}

		///<summary></summary>
		public long PatNum {
			get { return patNum; }
		}

		///<summary>If going to Images module, this lets you pick which image.</summary>
		public long DocNum {
			get { return docNum; }
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